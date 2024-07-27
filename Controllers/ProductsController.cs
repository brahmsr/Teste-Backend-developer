using APItesteInside.Data;
using APItesteInside.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using APItesteInside.DTOs;

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
        public async Task<IActionResult> GetAllProducts([FromQuery] string? filterBy = null)
        {
            try
            {
                var AllProducts = await dbContext.Products
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Category,
                        p.Price,
                        p.CreatedAt,
                        p.UpdatedAt,
                        Ordens = p.OrderProducts.Select(op => new
                        {
                            op.OrderId,
                            op.Quantity
                        }).ToList()
                    }).ToListAsync();

                return Ok(AllProducts);
            }
            catch (Exception e)
            {
                return BadRequest($"erro ocorrido durante a procura dos produtos: {e.Message}.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneProduct([FromRoute] int id)
        {
            try
            {
                var GetOneClient = await dbContext.Products
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Category,
                        p.Price,
                        p.CreatedAt,
                        p.UpdatedAt,
                        Ordens = p.OrderProducts.Select(op => new
                        {
                            op.OrderId,
                            op.Order,
                            op.Quantity
                        })
                    })
                    .FirstOrDefaultAsync(p => p.Id == id);

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
        public async Task<IActionResult> RegisterProduct([FromBody] ProductAddDTO addProductDTO)
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
                await dbContext.Products.AddAsync(productEntity);
                await dbContext.SaveChangesAsync();

                return Ok(productEntity);
            }
            catch (Exception e)
            {
                return BadRequest($"Algo deu errado: {e.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id,[FromBody] UpdateProductDTO updateProductDTO)
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

                await dbContext.SaveChangesAsync();
                return Ok(product);
            }
            catch (Exception e)
            {
                return BadRequest($"Algo de errado aconteceu: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            try
            {
                var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product is null)
                {
                    return NotFound($"produto {id} não encontrado");
                }
                dbContext.Products.Remove(product);
                await dbContext.SaveChangesAsync();

                return Ok(product);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro: {e.Message}");
            }
        }

    }
}
