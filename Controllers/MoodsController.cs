using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [Authorize]
        public async Task<IActionResult> Edit(int? id) {

            if (id == null || _context.Mood == null)
            {
                return NotFound();
            }

            var mood = await _context.Mood.FindAsync(id);
            if (mood == null)
            {
                return NotFound();
            }

            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (mood.UserId != loginUserId)
            {
                TempData["AlertMoodError"] = "アクセス権がありません。";
                return Redirect("/Moods/Index");
            }
 
            if (!(mood.UserId.Equals(loginUserId)))
            {
                TempData["AlertMoodError"] = "アクセス権がありません。";
                return Redirect("/Moods/Index");
            }

            return View(mood);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[]values)
        {
            Mood mood = new Mood();
            mood.MoodId = id;
            mood.Date = DateTime.Parse(values[0]);
            mood.Rating = int.Parse(values[1]);
            mood.Comment = values[2];
            mood.UserId = values[3];

            if (id != mood.MoodId)
            {
                return NotFound();
            }

            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mood);
                    await _context.SaveChangesAsync();
                    TempData["AlertMood"] = "気分を編集しました。";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MoodExists(mood.MoodId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(mood);
        }

        private bool MoodExists(int moodId)
        {
            throw new NotImplementedException();
        }
    }
}
