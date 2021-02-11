using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BS.API.Data;
using BS.API.Dtos;
using BS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace BS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const string BadRequestUserExist = "Username already exist";

        private IAuthRepository repo;
        private IConfiguration config;
        private readonly IMapper mapper;

        public AuthController(
            IAuthRepository repo,
            IConfiguration config,
            IMapper mapper)
        {
            this.config = config;
            this.mapper = mapper;
            this.repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Regietr(UserForRegisterDto userForRegisterDto)
        {
            //validate result

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await this.repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest(BadRequestUserExist);
            }

            var userToCreate = this.mapper.Map<User>(userForRegisterDto);

            var createdUser = await this.repo.Register(userToCreate, userForRegisterDto.Password);

            var userToReturn = this.mapper.Map<UserForDetailedDto>(createdUser);

            return CreatedAtAction(nameof(UsersController.GetUser), new { controller="Users", id = createdUser.Id }, userToReturn);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await this.repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(this.config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = this.mapper.Map<UserForListDto>(userFromRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}