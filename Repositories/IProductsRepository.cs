using APItesteInside.Models.Domain;
using APItesteInside.Models.Entities;

namespace APItesteInside.Repositories
{
    public interface IProductsRepository
    {
        //buscar todos
        Task<List<Product>> GetAllProductsAsync(string? filterBy = null, string? prop = null, int page = 1, string? sortBy = null, bool isAscending = true);
        //buscar por id
        Task<Product?> GetByIdAsync(int id);
        //criar produto
        Task<Product> CreateProductAsync(Product product);
        //editar produto
        Task<Product?> EditProductAsync(int id, Product product);
        //deletar produto
        Task<Product> DeleteProductAsync(int id);
    }
}
