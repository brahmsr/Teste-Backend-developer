using APItesteInside.Data;
using APItesteInside.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace APItesteInside.Repositories
{
    public class SQLProductsRepository : IProductsRepository
    {
        private readonly DatabaseContext _dbContext;

        public SQLProductsRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        //criar produto
        public async Task<Product> CreateProductAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        //deletar produto
        public async Task<Product> DeleteProductAsync(int id)
        {
            var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if(existingProduct == null) { return null; }

            _dbContext.Products.Remove(existingProduct);
            await _dbContext.SaveChangesAsync();

            return existingProduct;
        }

        //editar produto
        public async Task<Product> EditProductAsync(int id, Product product)
        {
            var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null) { return null; }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Quantity = product.Quantity;
            existingProduct.Category = product.Category;
            existingProduct.Description = product.Description;
            existingProduct.UpdatedAt = product.UpdatedAt;

            await _dbContext.SaveChangesAsync();

            return existingProduct;
        }

        //busca produtos
        public async Task<List<Product>> GetAllProductsAsync(string? filterBy = null, string? prop = null, int page = 1, string? sortBy = null, bool isAscending = true)
        {
            var names = _dbContext.Products.Include(o => o.OrderProducts).AsQueryable();

            //filtros
            if(string.IsNullOrWhiteSpace(filterBy) == false && string.IsNullOrWhiteSpace(prop) == false)
            {
                if (filterBy.Equals("Name", StringComparison.OrdinalIgnoreCase)) 
                { 
                    names = names.Where(p => p.Name.Contains(prop));
                }
            }

            //ordenação
            if(string.IsNullOrWhiteSpace(sortBy) == false)
            {
                //ordenar por nome
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    names = isAscending ? names.OrderBy(o => o.Name) : names.OrderByDescending(o => o.Name);
                }
                // ordenar por preço
                else if (sortBy.Equals("price", StringComparison.OrdinalIgnoreCase))
                {
                    names = isAscending ? names.OrderBy(o => o.Price) : names.OrderByDescending(o => o.Price);
                }
            }

            //paginação
            int pageSize = 5; //resultados por página
            var skipResults = (page - 1) * pageSize;

            return await names.Skip(skipResults).Take(pageSize).ToListAsync();

        }

        //busca produto por id
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _dbContext.Products
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
