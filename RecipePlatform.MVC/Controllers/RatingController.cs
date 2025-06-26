using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecipePlatform.BLL.Interfaces;
using RecipePlatform.DAL.Context;
using RecipePlatform.Models.Models;

namespace RecipePlatform.MVC.Controllers
{
    [Authorize]
    public class RatingController : Controller
    {
        private readonly IRatingService _ratingService;
        private readonly IRecipeService _recipeService;
        private readonly ApplicationDbContext _context;

        public RatingController(IRatingService ratingService, IRecipeService recipeService, ApplicationDbContext context)
        {
            _ratingService = ratingService;
            _recipeService = recipeService;
            _context = context;
        }

        // GET: Rating
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ratings.Include(r => r.Recipe).Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Rating/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Recipe)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // GET: Rating/Create
        public async Task<IActionResult> Create(int recipeId)
        {
            var recipe = await _recipeService.GetRecipeById(recipeId);
            if (recipe == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingRating = await _ratingService.GetUserRatingForRecipe(recipeId, userId);

            if (existingRating != null)
            {
                return RedirectToAction("Edit", new { id = existingRating.Id });
            }

            ViewBag.Recipe = recipe;
            return View();
        }

        // POST: Rating/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int recipeId, int stars, string feedback = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _ratingService.AddRating(recipeId, userId, stars, feedback);
                return RedirectToAction("Details", "Recipe", new { id = recipeId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var recipe = await _recipeService.GetRecipeById(recipeId);
                ViewBag.Recipe = recipe;
                return View();
            }
        }

        // GET: Rating/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Recipe)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rating == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (rating.UserId != userId)
            {
                return Forbid();
            }

            return View(rating);
        }

        // POST: Rating/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int stars, string feedback = null)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (rating.UserId != userId)
            {
                return Forbid();
            }

            try
            {
                await _ratingService.UpdateRating(rating.RecipeId, userId, stars, feedback);
                return RedirectToAction("Details", "Recipe", new { id = rating.RecipeId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(rating);
            }
        }

        // GET: Rating/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Recipe)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rating == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (rating.UserId != userId)
            {
                return Forbid();
            }

            return View(rating);
        }

        // POST: Rating/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (rating.UserId != userId)
                {
                    return Forbid();
                }

                var recipeId = rating.RecipeId;
                _context.Ratings.Remove(rating);
                await _context.SaveChangesAsync();

                // Update recipe rating after deletion
                await _recipeService.UpdateRecipeRating(recipeId);

                return RedirectToAction("Details", "Recipe", new { id = recipeId });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }
    }
}
