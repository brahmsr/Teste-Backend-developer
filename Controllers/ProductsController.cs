using APItesteInside.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using APItesteInside.DTOs;
using APItesteInside.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace APItesteInside.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductsRepository productsRepository,
            IMapper mapper)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] string? filterBy, [FromQuery] string? prop,
            [FromQuery] int page = 1, [FromQuery] bool isAscending = true, [FromQuery] string? sortBy = null)
        {
            //obtem os dados do DB
            var ProductsDomain = await _productsRepository.GetAllProductsAsync(filterBy, prop, page);

            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var productDTO = _mapper.Map<List<ProductsDTO>>(ProductsDomain);

            return Ok(productDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneProduct([FromRoute] int id)
        {
            //recupera do banco de dados
            var productDomain = await _productsRepository.GetByIdAsync(id);

            //verifica se existe
            if (productDomain == null)
            {
                return NotFound($"O produto de número: {id} - Não existe no banco de dados");
            }

            return Ok(_mapper.Map<ProductsDTO>(productDomain));
        }

        [HttpPost]
        public async Task<IActionResult> RegisterProduct([FromBody] ProductAddDTO addProductDTO)
        {
            //adiciona o produto
            var productDomainModel = _mapper.Map<Product>(addProductDTO); //utiliza o automapper para receber o model

            productDomainModel = await _productsRepository.CreateProductAsync(productDomainModel); //usando a interface

            var productDTO = _mapper.Map<ProductsDTO>(productDomainModel);

            return CreatedAtAction(nameof(GetOneProduct), new {Id = productDTO.Id}, productDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id,[FromBody] ProductEditDTO updateProductDTO)
        {
            //mapeando o DTO para domain model
            var product = _mapper.Map<Product>(updateProductDTO);

            product = await _productsRepository.EditProductAsync(id, product);

            //checa se o produto existe
            if (product is null)
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<ProductEditDTO>(product));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            //verifica se existe no db
            var product = await _productsRepository.DeleteProductAsync(id);
            if (product is null)
            { return NotFound($"produto {id} não encontrado"); }

            //retorna o produto deletado

            return Ok(_mapper.Map<ProductsDTO>(product));
        }

    }
}
