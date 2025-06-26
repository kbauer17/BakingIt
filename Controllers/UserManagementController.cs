using BakingIt.Models;
using BakingIt.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BakingIt.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Roles
        /// <summary>
        ///     List view page for Roles
        /// </summary>
        /// <returns></returns>
        public IActionResult ViewRoles()
        {
            var roles = _roleManager.Roles;

            return View("Role/ViewRoles", roles);
        }

        /// <summary>
        ///     Create a new Role - Getter
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateRole()
        {
            return View("Role/CreateRole");
        }

        /// <summary>
        ///     Create a new Role - Setter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(UserManagementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate any dropdown lists if returning due to ModelState errors
                // The filter AutoPopulateModelErrorsAttribute is in play here as well
                return View("Role/CreateRole", model);
            }

            var identityRole = new IdentityRole
            {
                Name = model.IdentityRole.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var result = await _roleManager.CreateAsync(identityRole);

            if (result.Succeeded)
                return RedirectToAction("ViewRoles");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            // the filter AutoPopulateModelErrorsAttribute is in play here also

            return View("Role/CreateRole", model);
        }

        /// <summary>
        ///     Edit an existing Role - Getter
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with ID = {id} cannot be found";
                return View("NotFound");
            }

            return View("Role/EditRole", new UserManagementViewModel(role));
        }

        /// <summary>
        ///     Edit an existing Role - Setter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(UserManagementViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.IdentityRole.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with ID = {model.IdentityRole.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.IdentityRole.Name;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                    return RedirectToAction("ViewRoles");

                // Filter AutoPopulateModelErrorsAttribute is working here
                return View("Role/EditRole", model);
            }
        }

        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                TempData["Error"] = "Role not found";
                return RedirectToAction("ViewRoles");
            }

            try
            {
                await _roleManager.DeleteAsync(role);
                TempData["Message"] = "Role successfully deleted";
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to delete Role";
            }

            return RedirectToAction("ViewRoles");
        }

        #endregion
    }
}