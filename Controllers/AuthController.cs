using GeekStore.API.Models.DTOs;
using GeekStore.API.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GeekStore.API.Controllers
{   // https://localhost/api/auth
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
           _userManager = userManager;
            _tokenRepository = tokenRepository;
        }
        // POST: https://localhost/api/register
        [HttpPost]
        [Route("/Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.UserName
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);
            
            if (identityResult.Succeeded)
            {
                if (registerRequestDto.Roles.Any())
                {
                    identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                }
                if (identityResult.Succeeded) {
                    return Ok("User registration successful! Please login");
                }
            }

            return BadRequest("Something went wrong");

        }
        // POST: https://localhost/api/login
        [HttpPost]
        [Route("/Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Username);
            if(user != null)
            {
                var passwordValidation = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (passwordValidation)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    // Token Generation
                    var jwtToken = _tokenRepository.CreateJwtToken(user, roles.ToList());
                    var response = new LoginResponseDto { JwtToken = jwtToken };

                    return Ok(response);
                }
                return BadRequest("Incorrect password!");
            }
            return BadRequest("Incorrect username");
        }
    }
}
