using Microsoft.AspNetCore.Mvc;
using RecipePlatform.BLL.Interfaces;
using RecipePlatform.Models.Models;

namespace RecipePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : Controller
    {
        private static List<Recipe> _recipes = new List<Recipe>
        {
            new Recipe { Id = 1, Title = "Pasta Carbonara", Description = "Classic Italian pasta dish.", Ingredients = "Pasta, Eggs, Pancetta, Pecorino Romano", Instructions = "Cook pasta. Fry pancetta. Mix eggs and cheese. Combine all.", PrepTimeMinutes = 15, CookTimeMinutes = 20, Servings = 4, Difficulty = RecipePlatform.Models.Models.Enums.DifficultyLevel.Medium },
            new Recipe { Id = 2, Title = "Chicken Curry", Description = "Spicy and flavorful chicken curry.", Ingredients = "Chicken, Curry Powder, Coconut Milk, Vegetables", Instructions = "Cook chicken. Add spices and coconut milk. Simmer with vegetables.", PrepTimeMinutes = 20, CookTimeMinutes = 30, Servings = 4, Difficulty = RecipePlatform.Models.Models.Enums.DifficultyLevel.Medium }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Recipe>> GetRecipes()
        {
            return Ok(_recipes);
        }

        [HttpGet("{id}")]
        public ActionResult<Recipe> GetRecipe(int id)
        {
            var recipe = _recipes.FirstOrDefault(r => r.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }
            return Ok(recipe);
        }

        [HttpPost]
        public ActionResult<Recipe> CreateRecipe(Recipe recipe)
        {
            recipe.Id = _recipes.Any() ? _recipes.Max(r => r.Id) + 1 : 1;
            _recipes.Add(recipe);
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRecipe(int id, Recipe updatedRecipe)
        {
            var recipe = _recipes.FirstOrDefault(r => r.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            recipe.Title = updatedRecipe.Title;
            recipe.Description = updatedRecipe.Description;
            recipe.Ingredients = updatedRecipe.Ingredients;
            recipe.Instructions = updatedRecipe.Instructions;
            recipe.PrepTimeMinutes = updatedRecipe.PrepTimeMinutes;
            recipe.CookTimeMinutes = updatedRecipe.CookTimeMinutes;
            recipe.Servings = updatedRecipe.Servings;
            recipe.Difficulty = updatedRecipe.Difficulty;
            recipe.ModifiedDate = System.DateTime.UtcNow;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRecipe(int id)
        {
            var recipe = _recipes.FirstOrDefault(r => r.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            _recipes.Remove(recipe);
            return NoContent();
        }
    }
}
