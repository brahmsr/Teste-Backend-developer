﻿using APItesteInside.Data;
using APItesteInside.DTOs;
using APItesteInside.Models.Domain;
using APItesteInside.Models.Entities;
using APItesteInside.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using static APItesteInside.DTOs.OrderAddDTO;

namespace APItesteInside.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DatabaseContext dbContext;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrdersController(DatabaseContext dbContext, IOrderRepository orderRepository,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }


        //obter todos os pedidos de forma não paginada
        [HttpGet]
        [ActionName("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders([FromQuery] string? filterBy, [FromQuery] string? prop, [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int page = 1, [FromQuery] int? status = null)
        {
            //obtendo dados do DB
            var ordersDomain = await _orderRepository.GetAllAsync(filterBy, prop, sortBy, isAscending ?? true, page, status);

            //retorna a ordem
            return Ok(_mapper.Map<List<OrdersDTO>>(ordersDomain));
        }

        //obter apenas um pedido pelo id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneOrder(int id)
        {
            //procura no db
            var getOneOrder = await _orderRepository.GetByIdAsync(id);
            //verificando se o pedido existe
            if (getOneOrder == null)
            {
                return NotFound();
            }

            //retorna o pedido em questão
            return Ok(_mapper.Map<OrdersDTO>(getOneOrder));
        }

        //criação do pedido
        [HttpPost]
        public async Task<IActionResult> RegisterOrder([FromBody] OrderAddDTO addOrderDTO)
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
                    return BadRequest($"O produto com a Id {orderProdDTO.ProductId} possui apenas {product.Quantity} Unidades.");
                }

                //tudo certo então diminui a quantidade do produto de acordo com a ordem
                product.Quantity -= orderProdDTO.Quantity;
                // dá o preço de acordo com a quantidade de produtos e preço setado anteriormente
                totalOrderPrice += product.Price * orderProdDTO.Quantity;

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
            await dbContext.SaveChangesAsync();
            transaction.Commit();

            return Ok(order);
        }

        // Editar informações do pedido
        [HttpPut("{id}")]
        public async Task<IActionResult> EditOrder (int id, [FromBody] OrderEditDTO editOrderDTO)
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

            //salva no banco e commita
            await dbContext.SaveChangesAsync();
            transaction.Commit();

            return Ok(order);
        }

        //adiciona um item ao pedido
        [HttpPut("add-item/{id}")]
        public async Task<IActionResult> AddItemInOrder(int id, [FromBody] ProductAddInOrderDTO.OrderAddProdDTO addProductInOrderDTO)
        {
            //retorna se o modelo está valido
            if (!ModelState.IsValid)
            {
                return BadRequest($"Erro, o estado do modelo é inválido.");
            }

            //busca a order na tabela
            var order = await dbContext.Orders
                .Include(op => op.OrderProducts)
                .ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

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
                if (productToAdd.Quantity < productsAddDTO.Quantity)
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
            await dbContext.SaveChangesAsync();
            transaction.Commit();

            return Ok(order);
        }

        [HttpPut("remove-item/{id}")]
        public async Task<IActionResult> RemoveItemFromOrder(int id, [FromBody] OrderDeleteProductDTO.RemoveItemInOrderDTO deleteProductInOrderDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var order = await dbContext.Orders
                .Include(op => op.OrderProducts)
                .ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
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
                if (productInDb is null)
                {
                    return NotFound("Produto não encontrado na base."); ;
                }

                // retorna ao estoque o produto que foi deletado da ordem
                productInDb.Quantity += productToDeletingDTO.Quantity;

                //retira da order o preço já que o produto foi removido
                order.Price -= productToDeletingDTO.Quantity * productInDb.Price;

                //remover o produto em questão se for todo removido
                if (productToDeletingDTO.Quantity == orderProduct.Quantity)
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

            await dbContext.SaveChangesAsync();
            transaction.Commit();

            return Ok(order);
        }

        [HttpPut("change-status/{id}")]
        public async Task<IActionResult> ChangeStatus(int id, OrderChangeStatusDTO changeStatusOrderDTO)
        {
            var order = await dbContext.Orders
                .Include(op => op.OrderProducts)
                .ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("ordem não encontrada");
            }
            foreach(var orderChangeStatus in order.OrderProducts)
            {
                if (orderChangeStatus.OrderId == null)
                {
                    return BadRequest($"Pedido numero {order.Id} não pode ser alterado porque não tem itens");
                }
            }

            if (order.OrderProducts.Count == 0)
            {
                return BadRequest($"O pedido n° {order.Id} não pode ter seu status mudado, pois não tem itens");
            }

            order.status = changeStatusOrderDTO.status;
            order.UpdatedAt = changeStatusOrderDTO.UpdatedAt;

            await dbContext.SaveChangesAsync();
            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _orderRepository.DeleteOrderAsync(id);
            if (order == null) { return NotFound($"O pedido n°{id} não existe"); }

            var orderDTO = new OrdersDTO
            {
                Id = order.Id,
                OrderName = order.OrderName,
                Price = order.Price,
                ClientName = order.ClientName,
                Phone = order.Phone,
                Email = order.Email,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };

            //remove o pedido
            dbContext.Orders.Remove(order);
            await dbContext.SaveChangesAsync();

            //resposta
            return Ok($"a ordem {order.OrderName} - #{order.Id} foi removida com sucesso!");
        }
    }
}
