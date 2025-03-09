using AutoMapper;
using GeekStore.API.CustomActionFilters;
using GeekStore.API.Models.Domains;
using GeekStore.API.Models.DTOs;
using GeekStore.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekStore.API.Controllers
{   // https://localhost/api/categories
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IMapper mapper, ICategoryRepository categoryRepository, ILogger<CategoriesController> logger)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }
        //CRUD
        // POST: https://localhost/categories/
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddCategoryRequestDto addCategoryRequestDto)
        {
            var category = _mapper.Map<Category>(addCategoryRequestDto);

            category = await _categoryRepository.CreateAsync(category);
            if(category == null)
            {
                return BadRequest("Category creation failed");
            }
            
            var createdCategoryDto = _mapper.Map<CategoryDto>(category);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, createdCategoryDto);
        }
        // GET: https://localhost/categories/
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetAllAsync();

            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return Ok(categoryDtos);
        }
        // GET: https://localhost/categories/{id}
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return NotFound();
            
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }
        // PUT: https://localhost/categories/{id}
        [HttpPut]
        [Route("{id}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCategoryRequestDto updateCategoryRequestDto)
        {
            var updatedCategory = _mapper.Map<Category>(updateCategoryRequestDto);
            var category = await _categoryRepository.UpdateAsync(id, updatedCategory);
            
            if (category == null)
                return NotFound();
            
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }
        // DELETE: https://localhost/categories/{id}
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var category = await _categoryRepository.DeleteAsync(id);

            if (category == null)
                return NotFound();

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }
    }
}
