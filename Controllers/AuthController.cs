using APItesteInside.DTOs;
using APItesteInside.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APItesteInside.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> identityUser, ITokenRepository tokenRepository)
        {
            this.userManager = identityUser;
            this.tokenRepository = tokenRepository;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerDTO.Username,
                Email = registerDTO.Username, 
            };
            var identityResult = await userManager.CreateAsync(identityUser, registerDTO.Password);

            if (identityResult.Succeeded)
            {
                //Add role para esse user
                if (registerDTO.Roles != null && registerDTO.Roles.Any())
                {
                    foreach (var role in registerDTO.Roles)
                    {
                        identityResult = await userManager.AddToRoleAsync(identityUser, role);
                        if (!identityResult.Succeeded)
                        {
                            return BadRequest(identityResult.Errors);
                        }
                    }

                    if (identityResult.Succeeded)
                    {
                        return Ok("usuário cadastrado");
                    }
                }
            }
            // se algo der errado:
            return BadRequest();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Username);

            //se o user não for nulo checa o password
            if(user != null)
            {
                var checkPass = await userManager.CheckPasswordAsync(user, loginDTO.Password);

                if (checkPass)
                {
                    //obtem os papeis desse usuario
                    var roles = await userManager.GetRolesAsync(user);

                    if(roles != null)
                    {
                        //cria o token
                        var tokenJwt = tokenRepository.CreateJwtToken(user, roles.ToList());

                        var response = new LoginResponseDTO
                        {
                            JwtToken = tokenJwt
                        };

                        return Ok(response);
                    }
                    

                    return Ok();
                }
            }
            return BadRequest("Login ou senha inválidos!");
        }
    }
}
