using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mismo.Data;
using Mismo.Models;
using Mismo.ViewModel;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Mismo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Home()
        {
            return View();
        }
        public IActionResult Landing()
        {

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {
            Users users = new Users();
            users.Departments = new List<Department>();
            users.AllUsers = new List<ApplicationUser>();

            users.AllUsers = _userManager.Users.ToList();
            users.Departments = _context.Department.ToList();
        
            return View(users);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> MgrIndex()
        {
            var loginManagerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser loginManager = await _userManager.FindByIdAsync(loginManagerId);

            DepDetails depDetails = new DepDetails();
            depDetails.Managers = new List<ApplicationUser>();
            depDetails.Members = new List<ApplicationUser>();

            depDetails.Department = await _context.Department.FindAsync(loginManager.DepartmentId);
            depDetails.Managers = _userManager.Users.Where(x => x.Role.Equals("Manager") && x.DepartmentId == loginManager.DepartmentId).ToList();
            depDetails.Members = _userManager.Users.Where(x => x.Role.Equals("Member") && x.DepartmentId == loginManager.DepartmentId).ToList();

            return View(depDetails);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _userManager == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewData["Departments"] = new SelectList(_context.Department, "DepartmentId", "Name");

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string[] values)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.LastName = values[0];
            user.FirstName = values[1];
            user.Email = values[2];
            user.UserName = values[2];
            user.Role = values[3];


            if (user.LastName == null || user.FirstName == null || user.Email == null)
            {
                TempData["UserEditError"] = "姓、名、Emailは入力必須項目です。";
                ViewData["Departments"] = new SelectList(_context.Department, "DepartmentId", "Name");
                return View(user);
            }

            if (values[6].Equals(values[7]) && HasUpperCase(values[6]) == true && HasLowerCase(values[6]) == true && values[6].Any(char.IsDigit) == true && values[6].Any(IsSpecialCharacter) == true)
            {
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, values[6]);

            }
            else {
                TempData["UserEditError"] = "パスワードの入力情報に誤りがあります。";
                ViewData["Departments"] = new SelectList(_context.Department, "DepartmentId", "Name");
                return View(user);

            }

            var joinedDep = await _context.Department.FindAsync(user.DepartmentId);

            if (joinedDep != null && int.Parse(values[4]) != joinedDep.DepartmentId)
            {
                user.DepartmentId = int.Parse(values[4]);
            }

            if (!(values[5].Equals(user.Role)))
            {
                await _userManager.AddToRoleAsync(user, user.Role);
                await _userManager.RemoveFromRoleAsync(user, values[5]);
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {

                TempData["AlertUser"] = "ユーザーを編集しました。";
                return RedirectToAction(nameof(Users));
            }

            TempData["UserEditError"] = "入力情報に誤りがあります。";
            ViewData["Departments"] = new SelectList(_context.Department, "DepartmentId", "Name");
            return View(user);
        }


        [Authorize(Roles = "Member")]
        [HttpGet]
        public async Task<IActionResult> MemMain(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);

        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> MgrMain(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_userManager == null)
            {
                return Problem("Entity set 'UserManager'  is null.");
            }
            var user = await _userManager.FindByIdAsync(id);

            if (user.Email.Equals("admin@admin.com")) {
                TempData["AlertError"] = "管理者の削除はできません。";
                return Redirect("/Home/Users");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            TempData["AlertMessage"] = "ユーザーを削除しました。";
            return Redirect("/Home/Users");
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private bool HasUpperCase(string str)
        {
            return str.Any(char.IsUpper);
        }
        private bool HasLowerCase(string str)
        {
            return str.Any(char.IsLower);
        }
        private bool IsSpecialCharacter(char c)
        {
            var specialCharacters = new[] { '!', '@', '#', '$', '%', '^', '&', '*', ',', ';', ':' };
            return specialCharacters.Contains(c);
        }

    }
}