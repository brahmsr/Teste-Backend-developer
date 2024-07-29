using APItesteInside.Data;
using APItesteInside.Models.Domain;
using APItesteInside.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace APItesteInside.Repositories
{
    public class SQLOrdersRepository : IOrderRepository
    {
        private readonly DatabaseContext _dbContext;
        //conexão com banco
        public SQLOrdersRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        //criar ordem
        public async Task<Order> CreateOrderAsync(Order order, List<OrderProduct> orderProducts)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            foreach (var orderProduct in orderProducts)
            {
                orderProduct.OrderId = order.Id;

                await _dbContext.OrderProducts.AddAsync(orderProduct);
            }
            await _dbContext.SaveChangesAsync();
            return order;
        }

        //deleta ordem
        public async Task<Order> DeleteOrderAsync(int id)
        {
            var existingOrder = await _dbContext.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (existingOrder == null) { return null; }

            var existingOrderProduct = await _dbContext.OrderProducts.FirstOrDefaultAsync(op => op.OrderId == id);
            if(existingOrderProduct == null) { return null; }

            _dbContext.Orders.Remove(existingOrder);
            _dbContext.OrderProducts.Remove(existingOrderProduct);
            await _dbContext.SaveChangesAsync();

            return existingOrder;
        }

        //Retorna todas as ordens
        public async Task<List<Order>> GetAllAsync(string? filterBy = null, string? prop = null, string? sortBy = null, bool isAscending = true, int page = 1, int? status = null)
        {
            var proprieties = _dbContext.Orders.Include(o => o.OrderProducts).AsQueryable();

            //filtros
            if (string.IsNullOrWhiteSpace(filterBy) == false && string.IsNullOrWhiteSpace(prop) == false)
            {
                //nome do pedido
                if (filterBy.Equals("OrderName", StringComparison.OrdinalIgnoreCase))
                {
                    proprieties = proprieties.Where(o => o.OrderName.Contains(prop));
                }
                //nome do cliente
                else if (filterBy.Equals("clientName", StringComparison.OrdinalIgnoreCase))
                {
                    proprieties = proprieties.Where(o => o.ClientName.Contains(prop));
                }
            }

            //status
            if(status.HasValue)
            {
                proprieties = proprieties.Where(o => o.status == status.Value);
            }

            //ordenação
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                //ordenar por nome
                if(sortBy.Equals("OrderName", StringComparison.OrdinalIgnoreCase))
                {
                    proprieties = isAscending ? proprieties.OrderBy(o => o.OrderName): proprieties.OrderByDescending(o => o.OrderName);
                }
                // ordenar por preço
                else if (sortBy.Equals("price", StringComparison.OrdinalIgnoreCase))
                {
                    proprieties = isAscending ? proprieties.OrderBy(o => o.Price) : proprieties.OrderByDescending(o => o.Price);
                }
                //ordenar por data de criação
                else if (sortBy.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
                {
                    proprieties = isAscending ? proprieties.OrderBy(o => o.CreatedAt) : proprieties.OrderByDescending(o => o.CreatedAt);
                }
            }

            //paginação
            int pageSize = 5; //resultados por página
            var skipResults = (page - 1) * pageSize;

            return await proprieties.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        // recupera pelo ID
        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _dbContext.Orders
                .Include (o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        //edita ordem
        public async Task<Order?> UpdateOrderAsync(int id, Order order, OrderProduct orderProduct)
        {
            var existingOrder = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (existingOrder == null) { return null; }

            existingOrder.OrderName = order.OrderName;
            existingOrder.Price = order.Price;
            existingOrder.ClientName = order.ClientName;
            existingOrder.Phone = order.Phone;
            existingOrder.Email = order.Email;
            existingOrder.status = order.status;

            var existingOrderProduct = await _dbContext.OrderProducts.FirstOrDefaultAsync(op => op.OrderId == id);
            if (existingOrderProduct == null) { return null; }

            existingOrderProduct.ProductId = orderProduct.ProductId;
            existingOrderProduct.Quantity = orderProduct.Quantity;

            await _dbContext.SaveChangesAsync();

            return existingOrder;
        }
    }
}
