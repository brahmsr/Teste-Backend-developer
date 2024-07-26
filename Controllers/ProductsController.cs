using APItesteInside.Data;
using APItesteInside.Models.DTOs;
using APItesteInside.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace APItesteInside.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DatabaseContext dbContext;

        public ProductsController(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            try
            {
                var AllProducts = dbContext.Products
                    .Include(op => op.OrderProducts)
                    .ThenInclude(o => o.Order)
                    .ToList();

                return Ok(AllProducts);
            }
            catch (Exception e)
            {
                return BadRequest($"erro ocorrido durante a procura dos produtos: {e.Message}.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetOneProduct(Guid id)
        {
            try
            {
                var GetOneClient = dbContext.Products
                    .Include(op => op.OrderProducts)
                    .ThenInclude(o => o.Order)
                    .FirstOrDefault(p => p.Id == id);

                if (GetOneClient == null)
                {
                    return NotFound();
                }

                return Ok(GetOneClient);
            }
            catch (Exception e)
            {
                return BadRequest($"algo deu errado: {e.Message}");
            }
        }

        [HttpPost]
        public IActionResult RegisterProduct(ProductAddDTO addProductDTO)
        {
            try
            {
                var productEntity = new Product()
                {
                    Name = addProductDTO.Name,
                    Description = addProductDTO.Description,
                    Category = addProductDTO.Category,
                    Quantity = addProductDTO.Quantity,
                    Price = addProductDTO.Price
                };
                dbContext.Products.Add(productEntity);
                dbContext.SaveChanges();

                return Ok(productEntity);
            }
            catch (Exception e)
            {
                return BadRequest($"Algo deu errado: {e.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(Guid id, UpdateProductDTO updateProductDTO)
        {
            try
            {
                var product = dbContext.Products.Find(id);
                if (product is null)
                {
                    return NotFound();
                }
                product.Name = updateProductDTO.Name;
                product.Description = updateProductDTO.Description;
                product.Category = updateProductDTO.Category;
                product.Quantity = updateProductDTO.Quantity;
                product.Price = updateProductDTO.Price;
                product.UpdatedAt = updateProductDTO.UpdatedAt;

                dbContext.SaveChanges();
                return Ok(product);
            }
            catch (Exception e)
            {
                return BadRequest($"Algo de errado aconteceu: {e.Message}");
            }
        }

    }
}
