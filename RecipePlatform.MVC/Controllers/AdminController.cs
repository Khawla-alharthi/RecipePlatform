using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipePlatform.BLL.Interfaces;
using RecipePlatform.Models.Models;
using RecipePlatform.DAL.Context; // Added for ApplicationDbContext
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecipePlatform.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Only users with \'Admin\' role can access this controller
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAdminService _adminService;
        private readonly IRecipeService _recipeService;
        private readonly ApplicationDbContext _context; // Inject ApplicationDbContext

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IAdminService adminService, IRecipeService recipeService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _adminService = adminService;
            _recipeService = recipeService;
            _context = context; // Assign injected context
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _adminService.GetAllUsers();
            return View(users);
        }

        // GET: Admin/UserDetails/5
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _adminService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = await _userManager.GetRolesAsync(user);
            return View(user);
        }

        // GET: Admin/EditUser/5
        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _adminService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            // Pre-select the user\'s current role in the dropdown
            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name", (await _userManager.GetRolesAsync(user)).FirstOrDefault());
            return View(user);
        }

        // POST: Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, [Bind("Id,FirstName,LastName,Email,UserName")] ApplicationUser userModel, string selectedRole)
        {
            if (id != userModel.Id)
            {
                return NotFound();
            }

            // Remove UserName from ModelState to prevent validation errors if it\'s read-only
            // and automatically set by Identity based on Email.
            ModelState.Remove("UserName");

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the user from UserManager to ensure it\'s tracked by Identity
                    var user = await _userManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Update user properties
                    user.FirstName = userModel.FirstName;
                    user.LastName = userModel.LastName;
                    user.Email = userModel.Email;
                    user.UserName = userModel.Email; // Keep UserName same as Email for Identity

                    // Update the user using UserManager
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        // Repopulate roles for the view if update fails
                        ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name", selectedRole);
                        return View(userModel);
                    }

                    // Update user roles
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Failed to remove existing roles.");
                        ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name", selectedRole);
                        return View(userModel);
                    }

                    if (!string.IsNullOrEmpty(selectedRole))
                    {
                        var addResult = await _userManager.AddToRoleAsync(user, selectedRole);
                        if (!addResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Failed to add new role.");
                            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name", selectedRole);
                            return View(userModel);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Re-throw if it\'s a genuine concurrency issue
                    }
                }
                return RedirectToAction(nameof(Users));
            }
            // If ModelState is not valid, repopulate roles and return view
            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name", selectedRole);
            return View(userModel);
        }

        // GET: Admin/DeleteUser/5
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _adminService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = await _userManager.GetRolesAsync(user);
            return View(user);
        }

        // POST: Admin/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            await _adminService.DeleteUser(id);
            return RedirectToAction(nameof(Users));
        }

        // GET: Admin/Roles
        public IActionResult Roles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // GET: Admin/CreateRole
        public IActionResult CreateRole()
        {
            return View();
        }

        // POST: Admin/CreateRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Roles));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(roleName);
        }

        // GET: Admin/DeleteRole/5
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Admin/DeleteRole/5
        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoleConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to delete role.");
                }
            }
            return RedirectToAction(nameof(Roles));
        }

        // GET: Admin/Recipes
        public async Task<IActionResult> Recipes()
        {
            var recipes = await _recipeService.GetAllRecipes();
            return View(recipes);
        }

        // GET: Admin/DeleteRecipe/5
        public async Task<IActionResult> DeleteRecipe(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _recipeService.GetRecipeById(id.Value);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Admin/DeleteRecipe/5
        [HttpPost, ActionName("DeleteRecipe")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRecipeConfirmed(int id)
        {
            await _recipeService.DeleteRecipe(id);
            return RedirectToAction(nameof(Recipes));
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // GET: Admin/CreateCategory
        public IActionResult CreateCategory()
        {
            return View();
        }

        // POST: Admin/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        // GET: Admin/EditCategory/5
        public async Task<IActionResult> EditCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/EditCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        // GET: Admin/DeleteCategory/5
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/DeleteCategory/5
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Categories));
        }

        private bool UserExists(string id)
        {
            return _userManager.Users.Any(e => e.Id == id);
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
