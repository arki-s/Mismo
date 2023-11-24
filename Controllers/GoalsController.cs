using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mismo.Data;
using Mismo.Models;
using Mismo.ViewModel;
using System.Security.Claims;

namespace Mismo.Controllers
{
    public class GoalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GoalsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (_context.Goal == null)
            {
                Problem("Entity set 'ApplicationDbContext.Goal'  is null.");
            }

            var goalList = new List<Goal>();
            var users = new List<ApplicationUser>();

            foreach (var item in _context.Goal)
            {
                goalList.Add(new Goal
                {
                    GoalId = item.GoalId,
                    Name = item.Name,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    Description = item.Description,
                    Achievement = item.Achievement,
                    UserId = item.UserId,
                });

                var goaluser = await _userManager.FindByIdAsync(item.UserId);
                users.Add(new ApplicationUser
                {
                    Email = goaluser.Email,
                    UserName = goaluser.UserName,
                    Department = goaluser.Department,
                    FirstName = goaluser.FirstName,
                    LastName = goaluser.LastName,
                    Goals = goaluser.Goals,
                });
            }

            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(loginUserId);

            var usergoals = new UserGoals()
            {
                User = user,
                GoalList = goalList,
                Users = users,
            };

            return View(usergoals);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(loginUserId);

            var createUser = new ApplicationUser()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Department = user.Department,
            };

            var users = _userManager.Users.ToList();
            var usersCreate = new List<ApplicationUser>();

            foreach (var u in users)
            {
                usersCreate.Add(
                    user = new ApplicationUser()
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Department = u.Department
                    }
                    );
            }

            UserCreate userCreate = new UserCreate()
            {
                User = user,
                Users = usersCreate,
                Goal = new Goal()
            };

            var userss = usersCreate.Where(u => u.Department.Equals(createUser.Department));

            var members = userss.Select(user => new SelectListItem
            {
                Value = user.Id,
                Text = $"{user.LastName} {user.FirstName}"
            });

            ViewBag.Members = new SelectList(members, "Value", "Text");
            return View(userCreate);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[] values)
        {
            //var goal = new Goal()
            //{
            //    Date = today,
            //    Rating = int.Parse(values[0]),
            //    Comment = values[1],
            //    UserId = values[2],
            //};

            //ModelState.Remove("User");
            //if (ModelState.IsValid)
            //{
            //    _context.Add(goal);
            //    await _context.SaveChangesAsync();
            //    TempData["AlertGoal"] = "新しい目標を登録しました。";
            //    return Redirect("/Goals/Index");
            //}

            //return View(goal);
            return View();
        }


    }
}
