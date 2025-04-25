using AutoMapper;
using GeekStore.API.CustomActionFilters;
using GeekStore.API.Models.Domains;
using GeekStore.API.Models.DTOs;
using GeekStore.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekStore.API.Controllers
{
    // https://localhost/api/products
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        // List of allowed query parameters for getAll request
        private readonly List<string> allowedParameters = new List<string> { "filterOn", "queryFilter", "sortBy", "isAscending", "pageNumber", "pageSize" };

        public ProductsController(IProductRepository productRepository, IMapper mapper) {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        // POST: https://localhost/api/products
        [HttpPost]
        [ValidateModelAttribute]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddProductRequestDto createProductDto)
        {
            // Create new product with the given params
            var product = _mapper.Map<Product>(createProductDto);

            // Add and save to db
            product = await _productRepository.CreateAsync(product);
            if(product == null)
            {
                return BadRequest("Product creation failed");
            }

            // Return the created product
            var createdProductDto = _mapper.Map<ProductDto>(product);
            return CreatedAtAction(nameof(GetById), new { id = createdProductDto.Id }, createdProductDto);
        }
        
        // GET: https://localhost/api/products?filterOn=Name&queryFilter=CPU&sortBy=Name&isAscending=true&pageNumber=1&paeSize=3
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? queryFilter, 
            [FromQuery] string? sortBy, [FromQuery] bool isAscending = true, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            // Check if any extra query parameters are passed
            var extraParameters = HttpContext.Request.Query.Keys.Except(allowedParameters, StringComparer.OrdinalIgnoreCase).ToList();

            if (extraParameters.Any())
            {
                return BadRequest($"Invalid query parameters: {string.Join(", ", extraParameters)}. Allowed parameters are: {string.Join(", ", allowedParameters)}");
            }

            // Validating filterOn query
            List<string> queryableColumns = new List<string> { 
                "name",
                "tier",
                "category"
            };
            if(string.IsNullOrWhiteSpace(filterOn) == false && !queryableColumns.Contains(filterOn.ToLower()))
            {
                return BadRequest($"Invalid column to filter {filterOn}. Allowed values are: {string.Join(", ", queryableColumns)}");
            }

            List<string> sortableColumns = new List<string> {
            // Validating sortBy query
                "name",
                "tier",
                "category"
            };
            if (string.IsNullOrWhiteSpace(sortBy) == false && !sortableColumns.Contains(sortBy.ToLower()))
            {
                return BadRequest($"Invalid column to sortBy {filterOn}. Allowed values are: {string.Join(", ", sortableColumns)}");
            }

            // Get relevant domain models
            var products = await _productRepository.GetAllAsync(filterOn, queryFilter, sortBy, isAscending, pageNumber, pageSize);

            // Map domain model to DTO
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            // Return fetched products
            return Ok(productDtos);
        }
        
        // GET: https://localhost/api/products/{id}
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Getting relevant domain models
            var domainProduct = await _productRepository.GetByIdAsync(id);

            if (domainProduct == null)
                return NotFound();

            // Map domain model to dto and return the fetched product
            var productDto = _mapper.Map<ProductDto>(domainProduct);
            return Ok(productDto);
        }
        
        // PUT: https://localhost/api/products/{id}
        [HttpPut]
        [Route("{id}")]
        [ValidateModelAttribute]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductRequestDto updateProductRequestDto)
        {
            var updatedProduct = _mapper.Map<Product>(updateProductRequestDto);

            var product = await _productRepository.UpdateAsync(id, updatedProduct);

            if(product == null)
                return NotFound();

            var updatedProductDto = _mapper.Map<ProductDto>(product);
            return Ok(updatedProductDto);
        }

        // DELETE: https://localhost/api/products/{id}
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedProduct = await _productRepository.DeleteAsync(id);
            
            if (deletedProduct == null)
                return NotFound();

            var deletedProductDto = _mapper.Map<ProductDto>(deletedProduct);
            return Ok(deletedProductDto);
        }
    }
}
