using APItesteInside.Models.Entities;

namespace APItesteInside.Repositories
{
    public interface IProductsRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetOneProductAsync();
    }
}
