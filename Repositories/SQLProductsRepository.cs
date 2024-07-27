using APItesteInside.Data;
using APItesteInside.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace APItesteInside.Repositories
{
    public class SQLProductsRepository : IProductsRepository
    {
        private readonly DatabaseContext _dbContext;

        public SQLProductsRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public Task<List<Product>> GetOneProductAsync()
        {
            throw new NotImplementedException();
        }
    }
}
