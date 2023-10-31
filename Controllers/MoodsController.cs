using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mismo.Data;
using Mismo.Models;
using Mismo.ViewModel;
using System.Security.Claims;

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
            if (_context.Mood == null) {
                Problem("Entity set 'ApplicationDbContext.Mood'  is null.");
            }

            var moodList = new List<Mood>();
            var users = new List<ApplicationUser>();

            foreach (var item in _context.Mood)
            {
                moodList.Add(new Mood
                {
                    MoodId = item.MoodId,
                    Date = item.Date,
                    Rating = item.Rating,
                    Comment = item.Comment,
                    UserId = item.UserId
                });

                var mooduser = await _userManager.FindByIdAsync(item.UserId);
                users.Add(new ApplicationUser
                {
                    Email = mooduser.Email,
                    UserName = mooduser.UserName,
                    Department = mooduser.Department,
                    FirstName = mooduser.FirstName,
                    LastName = mooduser.LastName,
                    Moods = mooduser.Moods,
                });
            }

            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(loginUserId);

            var usermoods = new UserMoods()
            {
                User = user,
                MoodList = moodList,
                Users = users,
            };

            return View(usermoods);

            //return _context.Mood != null ?
            //             View(await _context.Mood.ToListAsync()) :
            //             Problem("Entity set 'ApplicationDbContext.Mood'  is null.");

        }


        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[] values) {

            var today = DateTime.Now;

            var mood = new Mood()
            {
                Date = today,
                Rating = int.Parse(values[0]),
                Comment = values[1],
                UserId = values[2],
            };

            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                _context.Add(mood);
                await _context.SaveChangesAsync();
                TempData["AlertMood"] = "新しい今の気分を登録しました。";
                return Redirect("/Moods/Index");
            }

            return View(mood);
        }


    }
}
