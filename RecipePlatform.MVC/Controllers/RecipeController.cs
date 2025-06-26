using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipePlatform.BLL.Interfaces;
using RecipePlatform.DAL.Context;
using RecipePlatform.Models.Models.Enums;
using RecipePlatform.Models.Models;
using RecipePlatform.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace RecipePlatform.MVC.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IRatingService _ratingService;
        private readonly ApplicationDbContext _context; // Inject ApplicationDbContext

        public RecipeController(IRecipeService recipeService, IRatingService ratingService, ApplicationDbContext context)
        {
            _recipeService = recipeService;
            _ratingService = ratingService;
            _context = context; // Assign the injected DbContext
        }

        // GET: Recipe
        public async Task<IActionResult> Index(int? categoryId)
        {
            var recipes = await _recipeService.GetAllRecipes(categoryId);
            ViewBag.Categories = await _context.Categories.ToListAsync(); // Pass all categories to the view
            ViewBag.SelectedCategoryId = categoryId; // Keep track of the selected category
            return View(recipes);
        }

        // GET: Recipe/Details/5
        public async Task<IActionResult> Details(int? id)
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRating = userId != null ? await _ratingService.GetUserRatingForRecipe(id.Value, userId) : null;

            var viewModel = new RecipeDetailsViewModel
            {
                Recipe = recipe,
                CanEdit = userId == recipe.UserId,
                CanRate = userId != null && userId != recipe.UserId,
                UserRating = userRating?.Stars,
                IsOwner = userId == recipe.UserId
            };

            return View(viewModel);
        }

        // GET: Recipe/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateRecipeViewModel();
            await PopulateCategories(viewModel); // Use the updated PopulateCategories
            return View(viewModel);
        }

        // POST: Recipe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateRecipeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var recipe = new Recipe
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Ingredients = viewModel.Ingredients,
                    Instructions = viewModel.Instructions,
                    PrepTimeMinutes = viewModel.PrepTimeMinutes,
                    CookTimeMinutes = viewModel.CookTimeMinutes,
                    Servings = viewModel.Servings,
                    Difficulty = viewModel.Difficulty,
                    CategoryId = viewModel.CategoryId,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                await _recipeService.CreateRecipe(recipe);
                return RedirectToAction(nameof(Index));
            }
            await PopulateCategories(viewModel);
            return View(viewModel);
        }

        // GET: Recipe/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (recipe.UserId != userId)
            {
                return Forbid();
            }

            var viewModel = new CreateRecipeViewModel
            {
                Title = recipe.Title,
                Description = recipe.Description,
                Ingredients = recipe.Ingredients,
                Instructions = recipe.Instructions,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                CookTimeMinutes = recipe.CookTimeMinutes,
                Servings = recipe.Servings,
                Difficulty = recipe.Difficulty,
                CategoryId = recipe.CategoryId
            };

            await PopulateCategories(viewModel);
            return View(viewModel);
        }

        // POST: Recipe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, CreateRecipeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var recipeToUpdate = await _recipeService.GetRecipeById(id);
                    if (recipeToUpdate == null)
                    {
                        return NotFound();
                    }

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (recipeToUpdate.UserId != userId)
                    {
                        return Forbid();
                    }

                    recipeToUpdate.Title = viewModel.Title;
                    recipeToUpdate.Description = viewModel.Description;
                    recipeToUpdate.Ingredients = viewModel.Ingredients;
                    recipeToUpdate.Instructions = viewModel.Instructions;
                    recipeToUpdate.PrepTimeMinutes = viewModel.PrepTimeMinutes;
                    recipeToUpdate.CookTimeMinutes = viewModel.CookTimeMinutes;
                    recipeToUpdate.Servings = viewModel.Servings;
                    recipeToUpdate.Difficulty = viewModel.Difficulty;
                    recipeToUpdate.CategoryId = viewModel.CategoryId;

                    await _recipeService.UpdateRecipe(recipeToUpdate);
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    await PopulateCategories(viewModel);
                    return View(viewModel);
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateCategories(viewModel);
            return View(viewModel);
        }

        // GET: Recipe/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (recipe.UserId != userId)
            {
                return Forbid();
            }

            return View(recipe);
        }

        // POST: Recipe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _recipeService.GetRecipeById(id);
            if (recipe == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (recipe.UserId != userId)
            {
                return Forbid();
            }

            await _recipeService.DeleteRecipe(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Recipe/Search
        public async Task<IActionResult> Search(string searchTerm, int? categoryId, DifficultyLevel? difficulty)
        {
            var recipes = await _recipeService.SearchRecipes(searchTerm, categoryId, difficulty);
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.Difficulty = difficulty;

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);

            return View(recipes);
        }

        // GET: Recipe/UserRecipes
        [Authorize]
        public async Task<IActionResult> UserRecipes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var recipes = await _recipeService.GetRecipesByUser(userId);
            return View(recipes);
        }

        // GET: Recipe/TopRated
        public async Task<IActionResult> TopRated()
        {
            var recipes = await _recipeService.GetTopRatedRecipes();
            return View(recipes);
        }

        private async Task PopulateCategories(CreateRecipeViewModel viewModel)
        {
            var categories = await _context.Categories.ToListAsync();
            viewModel.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRating(int recipeId)
        {
            try
            {
                await _recipeService.UpdateRecipeRating(recipeId);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the rating.");
            }
        }
    }
}