using AutoMapper;
using GeekStore.API.CustomActionFilters;
using GeekStore.API.Models.Domains;
using GeekStore.API.Models.DTOs;
using GeekStore.API.Repositories.Interfaces;
using GeekStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekStore.API.Controllers
{
    // https://localhost/api/products
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IRecommendationService _recommendationService;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;


        private readonly List<string> queryableColumns = new List<string> {
                "name",
                "tier",
                "category"
            };
        private readonly List<string> sortableColumns = new List<string> {
                "name",
                "tier",
                "price",
                "category"
            };

        // List of allowed query parameters for getAll request
        private readonly List<string> allowedParameters = new List<string> { "filterOn", "filterQuery", "sortBy", "isAscending", "pageNumber", "pageSize" };

        public ProductsController(IProductService productService, IProductRepository productRepository, IRecommendationService recommendationService, IMapper mapper) {
            _productService = productService;
            _productRepository = productRepository;
            _recommendationService = recommendationService;
            _mapper = mapper;
        }

        // POST: https://localhost/api/products
        [HttpPost]
        [ValidateModelAttribute]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Create([FromBody] AddProductRequestDto createProductDto)
        {
            // Create new product with the given params
            var product = _mapper.Map<Product>(createProductDto);

            // Call the service to save the product
            product = await _productService.CreateAsync(product);

            if(product == null)
            {
                return BadRequest("Product creation failed, the given product already exists");
            }

            // Return the created product
            var createdProductDto = _mapper.Map<ProductDto>(product);
            return CreatedAtAction(nameof(GetById), new { id = createdProductDto.Id }, createdProductDto);
        }

        // POST: https://localhost/api/products/bulk
        [HttpPost("bulk")]
        [ValidateModelAttribute]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> CreateMultiple([FromBody] List<AddProductRequestDto> createProductDtos)
        {
            if (createProductDtos == null || !createProductDtos.Any())
            {
                return BadRequest("Product list cannot be empty.");
            }

            if(createProductDtos.Count() > 200)
            {
                return BadRequest("Product addition count exceeded! Atmost 200 products are allowed at once.");
            }

            // Map DTOs to domain models
            var products = _mapper.Map<List<Product>>(createProductDtos);

            // Save to DB using repository
            var createdProducts = await _productService.CreateMultipleAsync(products);

            if (createdProducts == null)
            {
                return BadRequest("Product creation failed!");
            }

            if(createdProducts.Count() != createProductDtos.Count())
            {
                return BadRequest($"Duplicate found for product: {createProductDtos[createdProducts.Count()].Name}");
            }

            // Map domain models to response DTOs
            var createdProductDtos = _mapper.Map<List<ProductDto>>(createdProducts);

            return Ok(createdProductDtos); // You could use CreatedAtAction per product if needed
        }

        // GET: https://localhost/api/products?filterOn=Name&filterQuery=CPU&sortBy=Name&isAscending=true&pageNumber=1&paeSize=3
        [HttpGet]
        [Authorize(Roles = "Reader,Writer,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery, 
            [FromQuery] string? sortBy, [FromQuery] bool isAscending = true, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            // Check if any extra query parameters are passed
            var extraParameters = HttpContext.Request.Query.Keys.Except(allowedParameters, StringComparer.OrdinalIgnoreCase).ToList();

            if (extraParameters.Any())
            {
                return BadRequest($"Invalid query parameters: {string.Join(", ", extraParameters)}. Allowed parameters are: {string.Join(", ", allowedParameters)}");
            }

            if (string.IsNullOrWhiteSpace(filterOn) == false && !queryableColumns.Contains(filterOn.ToLower()))
            {
                return BadRequest($"Invalid column to filter {filterOn}. Allowed values are: {string.Join(", ", queryableColumns)}");
            }

            if (string.IsNullOrWhiteSpace(sortBy) == false && !sortableColumns.Contains(sortBy.ToLower()))
            {
                return BadRequest($"Invalid column to sortBy {filterOn}. Allowed values are: {string.Join(", ", sortableColumns)}");
            }

            // Get relevant domain models
            var products = await _productRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);

            // Map domain model to DTO
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            // Return fetched products
            return Ok(productDtos);
        }
        
        // GET: https://localhost/api/products/{id}
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "Reader,Writer,Admin")]
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

        // POST: https://localhost/api/products/recommend
        [HttpPost("recommend")]
        [Authorize(Roles = "Reader,Writer,Admin")]
        public async Task<IActionResult> GetRecommendation([FromBody] RecommendationQueryDto queryDTO)
        {
            var recommendations = await _recommendationService.GetRecommendationAsync(queryDTO.Query);

            return Ok(recommendations);
        }

        // PUT: https://localhost/api/products/{id}
        [HttpPut]
        [Route("{id}")]
        [ValidateModelAttribute]
        [Authorize(Roles = "Reader,Writer,Admin")]
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
        [Authorize(Roles = "Writer,Admin")]
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
