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

        [Authorize(Roles = "Member, Manager")]
        public async Task<IActionResult> Index(string? id)
        {
            if (_context.Mood == null) {
                Problem("Entity set 'ApplicationDbContext.Mood'  is null.");
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            UserMoods userMoods = new UserMoods();
            userMoods.User = user;
            userMoods.MoodList = new List<Mood>();

            userMoods.MoodList = _context.Mood.Where(x => x.UserId.Equals(id)).ToList();

            return View(userMoods);

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
                return Redirect($"/Moods/Index/{values[2]}");
            }

            return View(mood);
        }

        [Authorize(Roles ="Member")]
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
 
            if (!(mood.UserId.Equals(loginUserId)))
            {
                TempData["AlertError"] = "アクセス権がありません。";
                return Redirect("/");
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
                return Redirect($"/Moods/Index/{values[3]}");
            }
            return View(mood);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Mood == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Mood'  is null.");
            }
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var mood = await _context.Mood.FindAsync(id);
            if (mood != null && mood.UserId.Equals(loginUserId))
            {
                _context.Mood.Remove(mood);
                TempData["AlertMood"] = "気分を削除しました。";
            }

            await _context.SaveChangesAsync();
            return Redirect($"/Moods/Index/{loginUserId}");
            //return RedirectToAction(nameof(Index));
        }

        private bool MoodExists(int moodId)
        {
            throw new NotImplementedException();
        }
    }
}
