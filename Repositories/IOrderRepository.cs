using APItesteInside.Models.Domain;
using APItesteInside.Models.Entities;

namespace APItesteInside.Repositories
{
    public interface IOrderRepository
    {
        //busca todas as ordens
        Task<List<Order>> GetAllAsync(string? filterBy = null, string? prop = null, string? sortBy = null, bool isAscending = true,
            int page = 1, int? status = null);
        //busca uma ordem
        Task<Order?> GetByIdAsync(int id);
        //cria uma ordem
        Task<Order> CreateOrderAsync(Order order, List<OrderProduct> orderProducts);
        //edita uma ordem
        Task<Order?> UpdateOrderAsync(int id, Order order, OrderProduct orderProduct);
        //deleta uma ordem
        Task<Order> DeleteOrderAsync(int id);
    }
}
