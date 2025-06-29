using Microsoft.AspNetCore.Mvc;
using RecipePlatform.Models.Models.Enums;
using RecipePlatform.Models.Models;

namespace RecipePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        // This would typically come from a service or repository
        private static List<Recipe> _recipes = new List<Recipe>
        {
            new Recipe { Id = 1, Title = "Pasta Carbonara", Description = "Classic Italian pasta dish.", Ingredients = "Pasta, Eggs, Pancetta, Pecorino Romano", Instructions = "Cook pasta. Fry pancetta. Mix eggs and cheese. Combine all.", PrepTimeMinutes = 15, CookTimeMinutes = 20, Servings = 4, Difficulty = DifficultyLevel.Medium, CategoryId = 1 },
            new Recipe { Id = 2, Title = "Chicken Curry", Description = "Spicy and flavorful chicken curry.", Ingredients = "Chicken, Curry Powder, Coconut Milk, Vegetables", Instructions = "Cook chicken. Add spices and coconut milk. Simmer with vegetables.", PrepTimeMinutes = 20, CookTimeMinutes = 30, Servings = 4, Difficulty = DifficultyLevel.Medium, CategoryId = 2 },
            new Recipe { Id = 3, Title = "Chocolate Cake", Description = "Rich and moist chocolate cake.", Ingredients = "Flour, Cocoa, Sugar, Eggs, Butter", Instructions = "Mix dry ingredients. Add wet ingredients. Bake at 350F.", PrepTimeMinutes = 30, CookTimeMinutes = 45, Servings = 8, Difficulty = DifficultyLevel.Hard, CategoryId = 3 },
            new Recipe { Id = 4, Title = "Vegetable Stir Fry", Description = "Quick and healthy vegetable dish.", Ingredients = "Mixed Vegetables, Soy Sauce, Garlic, Ginger", Instructions = "Heat oil. Add vegetables. Stir fry with sauce.", PrepTimeMinutes = 10, CookTimeMinutes = 15, Servings = 2, Difficulty = DifficultyLevel.Easy, CategoryId = 4 }
        };

        [HttpGet("recipes")]
        public ActionResult<IEnumerable<Recipe>> SearchRecipes(
            [FromQuery] string? title = null,
            [FromQuery] string? ingredients = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] DifficultyLevel? difficulty = null,
            [FromQuery] int? maxPrepTime = null,
            [FromQuery] int? maxCookTime = null,
            [FromQuery] int? minServings = null,
            [FromQuery] int? maxServings = null)
        {
            var filteredRecipes = _recipes.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                filteredRecipes = filteredRecipes.Where(r =>
                    r.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(ingredients))
            {
                filteredRecipes = filteredRecipes.Where(r =>
                    r.Ingredients.Contains(ingredients, StringComparison.OrdinalIgnoreCase));
            }

            if (categoryId.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => r.CategoryId == categoryId.Value);
            }

            if (difficulty.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => r.Difficulty == difficulty.Value);
            }

            if (maxPrepTime.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => r.PrepTimeMinutes <= maxPrepTime.Value);
            }

            if (maxCookTime.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => r.CookTimeMinutes <= maxCookTime.Value);
            }

            if (minServings.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => r.Servings >= minServings.Value);
            }

            if (maxServings.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => r.Servings <= maxServings.Value);
            }

            return Ok(filteredRecipes.ToList());
        }

        [HttpGet("recipes/popular")]
        public ActionResult<IEnumerable<Recipe>> GetPopularRecipes([FromQuery] int limit = 10)
        {
            // In a real application, this would be based on actual rating data
            var popularRecipes = _recipes
                .OrderByDescending(r => r.RatingCount)
                .Take(limit)
                .ToList();

            return Ok(popularRecipes);
        }

        [HttpGet("recipes/recent")]
        public ActionResult<IEnumerable<Recipe>> GetRecentRecipes([FromQuery] int limit = 10)
        {
            var recentRecipes = _recipes
                .OrderByDescending(r => r.CreatedDate)
                .Take(limit)
                .ToList();

            return Ok(recentRecipes);
        }

        [HttpGet("recipes/by-category/{categoryId}")]
        public ActionResult<IEnumerable<Recipe>> GetRecipesByCategory(int categoryId)
        {
            var categoryRecipes = _recipes.Where(r => r.CategoryId == categoryId).ToList();
            return Ok(categoryRecipes);
        }

        [HttpGet("recipes/quick")]
        public ActionResult<IEnumerable<Recipe>> GetQuickRecipes([FromQuery] int maxTotalTime = 30)
        {
            var quickRecipes = _recipes
                .Where(r => (r.PrepTimeMinutes + r.CookTimeMinutes) <= maxTotalTime)
                .OrderBy(r => r.PrepTimeMinutes + r.CookTimeMinutes)
                .ToList();

            return Ok(quickRecipes);
        }
    }
}
