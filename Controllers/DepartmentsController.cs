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
    public class DepartmentsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepartmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Index()
        {
            if (_context.Department == null)
            {
                Problem("Entity set 'ApplicationDbContext.Department'  is null.");
            }

            var applicationDbContext = _context.Department;
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Department department)
        {
            ModelState.Remove("Users");
            if (ModelState.IsValid)
            {
                var checkDup = _context.Department.Where(x => x.Name.Equals(department.Name)).ToList();
                if (checkDup.Count() == 0)
                {
                    _context.Add(department);
                    await _context.SaveChangesAsync();
                    TempData["AlertDepartment"] = "新しい部門を追加しました。";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["AlertDepartmentError"] = "既に同じ部門が存在しています。";
                    return View(department);
                }


            }
            TempData["AlertDepartmentError"] = "新しい部門を追加できませんでした。";
            return View(department);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Department == null)
            {
                return NotFound();
            }

            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentId,Name")] Department department)
        {
            if (id != department.DepartmentId)
            {
                return NotFound();
            }

            ModelState.Remove("Users");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    TempData["AlertDepartment"] = "部門を編集しました。";
                    await _context.SaveChangesAsync();
                }
                catch (/*DbUpdateConcurrency*/Exception)
                {
                    if (!DepartmentExists(department.DepartmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["AlertDepartmentError"] = "既に同じ部門が存在しています。";
                        return View(department);
                        /*throw;*/
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            DepDetails depDetails = new DepDetails();
            depDetails.Managers = new List<ApplicationUser>();
            depDetails.Members = new List<ApplicationUser>();

            depDetails.Department = await _context.Department.FindAsync(id);
            depDetails.Managers = _userManager.Users.Where(x => x.Role.Equals("Manager") && x.DepartmentId == id).ToList();
            depDetails.Members = _userManager.Users.Where(x => x.Role.Equals("Member") && x.DepartmentId == id).ToList();

            return View(depDetails);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Department == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Department'  is null.");
            }
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var department = await _context.Department.FindAsync(id);

            _context.Department.Remove(department);
            TempData["AlertDepartment"] = "部門を削除しました。";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool DepartmentExists(int id)
        {
            return (_context.Department?.Any(e => e.DepartmentId == id)).GetValueOrDefault();
        }





    }

    
}
