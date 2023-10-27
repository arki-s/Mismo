using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mismo.Data;
using Mismo.Models;

namespace Mismo.Controllers
{
    public class MoodsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MoodsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return _context.Mood != null ?
                         View(await _context.Mood.ToListAsync()) :
                         Problem("Entity set 'ApplicationDbContext.Recipe'  is null.");

        }


        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
    }
}
