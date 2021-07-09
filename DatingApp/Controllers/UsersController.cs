using DatingApp.Data;
using DatingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
 
    public class UsersController : BaseApiController 
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> getUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AppUser>> getUser(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}
