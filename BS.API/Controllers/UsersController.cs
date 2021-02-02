using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BS.API.Data;
using BS.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using mvc = Microsoft.AspNetCore.Mvc;

namespace BS.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IBSRepository repo;
        public IMapper mapper { get; set; }

        public UsersController(IBSRepository repo, IMapper mapper)
        {
            this.mapper = mapper;
            this.repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await this.repo.GetUsers();

            var usersToReturn = this.mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await this.repo.GetUser(id);

            var userToReturn = this.mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }
    }
}