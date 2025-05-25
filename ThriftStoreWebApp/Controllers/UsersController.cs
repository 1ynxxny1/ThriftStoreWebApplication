using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ThriftStoreWebApp.Models;

namespace ThriftStoreWebApp.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly int _pageSize = 5;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index(int? pageIndex)
        {
            int currentPage = pageIndex.GetValueOrDefault(1);
            if (currentPage < 1) currentPage = 1;

            var query = _userManager.Users.OrderByDescending(u => u.CreatedAt);

            int totalUsers = query.Count();
            int totalPages = (int)Math.Ceiling(totalUsers / (double)_pageSize);

            var users = query
                .Skip((currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            ViewBag.PageIndex = currentPage;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction(nameof(Index));

            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
                return RedirectToAction(nameof(Index));

            var roles = await _userManager.GetRolesAsync(appUser);
            ViewBag.Roles = roles;

            var availableRoles = _roleManager.Roles.ToList();
            var selectItems = new List<SelectListItem>();

            foreach (var role in availableRoles)
            {
                selectItems.Add(new SelectListItem
                {
                    Text = role.NormalizedName,
                    Value = role.Name,
                    Selected = await _userManager.IsInRoleAsync(appUser, role.Name!)
                });
            }

            ViewBag.SelectItems = selectItems;

            return View(appUser);
        }

        public async Task<IActionResult> EditRole(string? id, string? newRole)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(newRole))
                return RedirectToAction(nameof(Index));

            var roleExists = await _roleManager.RoleExistsAsync(newRole);
            var appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null || !roleExists)
                return RedirectToAction(nameof(Index));

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == appUser.Id)
            {
                TempData["ErrorMessage"] = "You cannot update your own role!";
                return RedirectToAction(nameof(Details), new { id });
            }

            var userRoles = await _userManager.GetRolesAsync(appUser);
            await _userManager.RemoveFromRolesAsync(appUser, userRoles);
            await _userManager.AddToRoleAsync(appUser, newRole);

            TempData["SuccessMessage"] = "User role updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> DeleteAccount(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction(nameof(Index));

            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
                return RedirectToAction(nameof(Index));

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == appUser.Id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account!";
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await _userManager.DeleteAsync(appUser);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Unable to delete this account: " + result.Errors.First().Description;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = "User account deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}