using APItesteInside.Data;
using APItesteInside.Models.Domain;
using APItesteInside.Models.DTOs;
using APItesteInside.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using static APItesteInside.Models.DTOs.OrderAddDTO;

namespace APItesteInside.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DatabaseContext dbContext;

        public OrdersController(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }


        //obter todos os pedidos de forma não paginada
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            try
            {
                var getAllOrders = dbContext.Orders
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                    .ToList();

                return Ok(getAllOrders);
            }
            catch (Exception e)
            {
                return BadRequest($"Algo deu errado. Erro: {e.Message}");
            }
        }

        //obter apenas um pedido pelo id
        [HttpGet("{id}")]
        public IActionResult GetOneOrder(Guid id)
        {
            var getOneOrder = dbContext.Orders
                .Include(op => op.OrderProducts)
                    .FirstOrDefault(o => o.Id == id);

            if (getOneOrder == null)
            {
                return NotFound();
            }

            return Ok(getOneOrder);
        }

        //criação do pedido
        [HttpPost]
        public IActionResult RegisterOrder([FromBody] CreateOrderDTO addOrderDTO)
        {
            //verificação da integridade do modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // se houver falha, é revertido
            using var transaction = dbContext.Database.BeginTransaction();

            // utilizado para armazenar o valor do preço 
            decimal totalOrderPrice = 0;

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderName = addOrderDTO.OrderName,
                ClientName = addOrderDTO.ClientName,
                Phone = addOrderDTO.Phone,
                Email = addOrderDTO.Email,
                status = addOrderDTO.status,
                CreatedAt = DateTime.Now,
                OrderProducts = new List<OrderProduct>()
            };

            foreach( var orderProdDTO in addOrderDTO.OrderProducts)
            {
                //Executando as verificações para adicionar ou não o produto ao pedido
                var product = dbContext.Products.Find(orderProdDTO.ProductId);

                if (product is null)
                {
                    return NotFound($"Produto com a id {orderProdDTO.ProductId} não encontrado");
                }

                if (product.Quantity < orderProdDTO.Quantity)
                {
                    return BadRequest($"O produto com a Id {orderProdDTO.ProductId} não possui essas unidades, ele possui apenas {product.Quantity} Unidades.");
                }

                //tudo certo então diminui a quantidade do produto de acordo com a ordem
                product.Quantity -= orderProdDTO.Quantity;
                // dá o preço de acordo com a quantidade de produtos e preço setado anteriormente
                totalOrderPrice += product.Price * product.Quantity;

                var orderProducts = new OrderProduct
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = orderProdDTO.Quantity
                };

                order.OrderProducts.Add(orderProducts);
            }

            order.Price = totalOrderPrice;

            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
            transaction.Commit();

            return Ok(order);
        }

        // Editar informações do pedido
        [HttpPut("{id}")]
        public IActionResult EditOrder (Guid id, [FromBody] OrderEditDTO.EditOrdersDTO editOrderDTO)
        {
            //testar o modelo para ver se é válido!
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //recebe a order do banco de dados
            var order = dbContext.Orders
                .Include(op => op.OrderProducts)
                .FirstOrDefault(o => o.Id == id);

            //se a ordem for nula
            if (order is null)
            {
                return NotFound($"O pedido Id: {order.Id} não foi encontrado");
            }

            //usa a transaction para proteger o registro em caso de algum erro
            using var transaction = dbContext.Database.BeginTransaction();

            //se o status da ordem for diferente de zero
            if (order.status != 0)
            {
                return BadRequest($"O pedido {order.Id} não pode ser mais editado.");
            }

            //começa a receber as informações para atualizar a ordem
            order.OrderName = editOrderDTO.OrderName;
            order.ClientName = editOrderDTO.ClientName;
            order.Phone = editOrderDTO.Phone;
            order.Email = editOrderDTO.Email;
            order.status = order.status;
            order.UpdatedAt = editOrderDTO.UpdatedAt ?? DateTime.UtcNow;

            //retorna os produtos anteriores para os respectivos estoques do produto
            foreach (var returnProducts in order.OrderProducts)
            {
                var product = dbContext.Products.Find(returnProducts.ProductId);
                if (product != null)
                {
                    product.Quantity += returnProducts.Quantity;
                }
            }

            //limpando os produtos para não ter erros
            order.OrderProducts.Clear();

            //edição dos produtos da ordem
            foreach (var orderProdDTO in editOrderDTO.OrderProducts)
            {
                //encontra os produtos inclusos na ordem do pedido
                var productInOrder = dbContext.Products.Find(orderProdDTO.ProductId);

                //caso não tenham produtos na ordem
                if(productInOrder is null)
                {
                    return NotFound($"Produtos da ordem {order} não encontrado!");
                }

                //se a quantidade adicionada for menor que a indicada no estoque
                if(productInOrder.Quantity < orderProdDTO.Quantity)
                {
                    return BadRequest($"Quantidade disponível no estoque de {productInOrder.Name} é de apenas {productInOrder.Quantity}" );
                }
                
                //edita a quantidade de itens do estoque
                productInOrder.Quantity -= orderProdDTO.Quantity;

                //realiza então um novo registro
                var orderProduct = new OrderProduct
                {
                    OrderId = order.Id,
                    ProductId = productInOrder.Id,
                    Quantity = orderProdDTO.Quantity
                };

                //atualiza o preço da ordem de pedido
                order.Price = orderProdDTO.Quantity * productInOrder.Price;

                //adiciona os produtos ao pedido
                order.OrderProducts.Add(orderProduct);
            }

            //salva no banco e commita
            dbContext.SaveChanges();
            transaction.Commit();

            return Ok(order);
        }

        //adiciona um item ao pedido
        [HttpPut("add-item/{id}")]
        public IActionResult AddItemInOrder(Guid id, [FromBody] ProductAddInOrderDTO.OrderAddProdDTO addProductInOrderDTO)
        {
            //retorna se o modelo está valido
            if(!ModelState.IsValid)
            {
                return BadRequest($"Erro, o estado do modelo é inválido.");
            }

            //busca a order na tabela
            var order = dbContext.Orders
                .Include(op => op.OrderProducts)
                .ThenInclude(o => o.Product)
                .FirstOrDefault(o => o.Id == id);
            
            //verifica se a order pode ser editada
            if (addProductInOrderDTO.status != 0)
            {
                return BadRequest($"o pedido {order.OrderName} Está impossibilitado de ser editado");
            };

            //começo do commit
            using var transaction = dbContext.Database.BeginTransaction();

            //adiciona uma nova data ao updatedAt
            order.UpdatedAt = addProductInOrderDTO.UpdatedAt;


            //entra na collection OrderProducts
            foreach (var productsAddDTO in addProductInOrderDTO.OrderProducts)
            {
                //procura o id do item que será adicionado
                var productToAdd = dbContext.Products.Find(productsAddDTO.ProductId);

                //valida se o produto existe
                if (productToAdd is null)
                {
                    return NotFound($"o produto de id {productsAddDTO.ProductId} não existe");
                };

                //verifica se o produto tem quantidade suficiente
                if(productToAdd.Quantity < productsAddDTO.Quantity)
                {
                    return BadRequest($"o produto {productToAdd.Name} tem apenas {productToAdd.Quantity} Unidades");
                }

                // cria um novo elemento de produto no pedido
                var orderProduct = new OrderProduct
                {
                    OrderId = order.Id,
                    ProductId = productToAdd.Id,
                    Quantity = productsAddDTO.Quantity
                };

                //retira a quantidade do produto
                productToAdd.Quantity -= productsAddDTO.Quantity;

                //atualiza o preço
                var priceToAdd = productsAddDTO.Quantity * productToAdd.Price;
                order.Price += priceToAdd;

                //salva as alterações
                order.OrderProducts.Add(orderProduct);
            }

            //salvar no Db e commita
            dbContext.SaveChanges();
            transaction.Commit();

            return Ok(order);
        }

        [HttpPut("remove-item/{id}")]
        public IActionResult RemoveItemFromOrder(Guid id, [FromBody] OrderDeleteProductDTO.RemoveItemInOrderDTO deleteProductInOrderDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"O pedido de remoção do item {id} não é válido");
            }
            var order = dbContext.Orders
                .Include(op => op.OrderProducts)
                .ThenInclude(o => o.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null) {
                return NotFound($"ordem {id} não encontrado no banco de dados");
            }

            using var transaction = dbContext.Database.BeginTransaction();

            order.UpdatedAt = DateTime.UtcNow;

            foreach (var productToDeletingDTO in deleteProductInOrderDTO.OrderProducts)
            {
                //busca a relação de ordem e produto
                var orderProduct = dbContext.OrderProducts.FirstOrDefault(op => op.ProductId == productToDeletingDTO.ProductId);

                //verifica se o produto existe no pedido
                if (productToDeletingDTO is null)
                {
                    return NotFound($"Não existem produtos no pedido {order.OrderName}");
                }

                //busca o id do produto no banco
                var productInDb = dbContext.Products.Find(productToDeletingDTO.ProductId);

                //verifica se o produto existe
                if(productInDb is null)
                {
                    return NotFound("Produto não encontrado na base."); ;
                }

                // retorna ao estoque o produto que foi deletado da ordem
                productInDb.Quantity += productToDeletingDTO.Quantity;

                //retira da order o preço já que o produto foi removido
                order.Price -= productToDeletingDTO.Quantity * productInDb.Price;

                //remover o produto em questão se for todo removido
                if(productToDeletingDTO.Quantity == orderProduct.Quantity)
                {
                    order.OrderProducts.Remove(orderProduct);
                }
                // se não for todo o produto, apenas retira a quantidade do produto
                else if (productToDeletingDTO.Quantity < orderProduct.Quantity)
                {
                    orderProduct.Quantity -= productToDeletingDTO.Quantity;
                }
                // não é menos nem igual, então retorna erro
                else
                {
                    return BadRequest($"Quantidade a ser removida de {productToDeletingDTO.Quantity} é maior do que a quantidade no pedido de {orderProduct.Quantity}");
                }
            }

            dbContext.SaveChanges();
            transaction.Commit();

            return Ok(order);
        }
        [HttpPut("change-status/{id}")]
        public IActionResult ChangeStatus(Guid id, OrderChangeStatusDTO changeStatusOrderDTO)
        {
            var order = dbContext.Orders
                .Include(op => op.OrderProducts)
                .ThenInclude(o => o.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound("ordem não encontrada");
            }

            order.status = changeStatusOrderDTO.status;
            order.UpdatedAt = changeStatusOrderDTO.UpdatedAt;

            dbContext.SaveChanges();
            return Ok(order);
        }
    }
}
