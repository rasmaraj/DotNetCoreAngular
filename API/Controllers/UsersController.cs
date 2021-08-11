using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;     
using Microsoft.AspNetCore.Authorization;                 
namespace API.Controllers
{
                                               
    public class UsersController:BaseApiController
    {
        private readonly DataContext context;

        public UsersController(DataContext context) 
        {
            this.context = context;
        }
        [HttpGet]  
        [AllowAnonymous]      
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await context.Users.ToListAsync();
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return await context.Users.FindAsync(id);
        }

    }
}