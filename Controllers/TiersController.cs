using Microsoft.AspNetCore.Mvc;
using GeekStore.API.Models.DTOs;
using GeekStore.API.Models.Domains;
using GeekStore.API.Repositories;
using AutoMapper;
using GeekStore.API.CustomActionFilters;
using Microsoft.AspNetCore.Authorization;

namespace GeekStore.API.Controllers
{
    // https://localhost/api/tiers
    [ApiController]
    [Route("api/[controller]")]
    public class TiersController : ControllerBase
    {
        private ITierRepository _tierRepository;
        private IMapper _mapper;

        public TiersController(ITierRepository tierRepository, IMapper mapper)
        {
            _tierRepository = tierRepository;
            this._mapper = mapper;
        }
        // CRUD
        // POST: https://localhost/api/tiers
        [HttpPost]
        [ValidateModelAttribute]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddTierRequestDto createTierDto)
        {
            var tier = _mapper.Map<Tier>(createTierDto);

            tier = await _tierRepository.CreateAsync(tier);
            if (tier == null)
            {
                return BadRequest("Tier creation failed");
            }

            var tierDto = _mapper.Map<TierDto>(tier);
            return CreatedAtAction(nameof(GetById), new { id = tierDto.Id }, tierDto);
        }

        // GET: https://localhost/api/tiers
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAll()
        {
            var tiersDomain = await _tierRepository.GetAllAsync();

            var tiersDtos = _mapper.Map<List<TierDto>>(tiersDomain);

            return Ok(tiersDtos);
        }
        // GET: https://localhost/api/tiers/{id}
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var tier = await _tierRepository.GetByIDAsync(id);

            if (tier == null)
                return NotFound();

            var tierDto = _mapper.Map<TierDto>(tier);
            return Ok(tierDto);
        }
        // PUT: https://localhost/api/tiers/{id}
        [HttpPut]
        [Route("{id}")]
        [ValidateModelAttribute]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTierRequestDto updateTierRequestDto)
        {
            var tier = _mapper.Map<Tier>(updateTierRequestDto);

            var updatedTier = await _tierRepository.UpdateAsync(id, tier);
            
            if (updatedTier == null)
                return NotFound();

            var tierDto = _mapper.Map<TierDto>(updatedTier);
            return Ok(tierDto);
        }
        // DELETE: https://localhost/api/tiers/{id}
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedTier = await _tierRepository.DeleteAsync(id);
            
            if (deletedTier == null) 
                return NotFound();

            var deletedTierDto = _mapper.Map<TierDto>(deletedTier);
            return Ok(deletedTierDto);
        }
    }
}
