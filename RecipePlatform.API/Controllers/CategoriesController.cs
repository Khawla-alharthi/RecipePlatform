using Microsoft.AspNetCore.Mvc;
using RecipePlatform.Models.Models;

namespace RecipePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private static List<Category> _categories = new List<Category>
        {
            new Category { Id = 1, Name = "Italian", Description = "Traditional Italian cuisine" },
            new Category { Id = 2, Name = "Asian", Description = "Asian-inspired dishes" },
            new Category { Id = 3, Name = "Desserts", Description = "Sweet treats and desserts" },
            new Category { Id = 4, Name = "Vegetarian", Description = "Plant-based recipes" },
            new Category { Id = 5, Name = "Quick Meals", Description = "Fast and easy recipes" }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            return Ok(_categories);
        }

        [HttpGet("{id}")]
        public ActionResult<Category> GetCategory(int id)
        {
            var category = _categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public ActionResult<Category> CreateCategory(Category category)
        {
            category.Id = _categories.Any() ? _categories.Max(c => c.Id) + 1 : 1;
            _categories.Add(category);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, Category updatedCategory)
        {
            var category = _categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = updatedCategory.Name;
            category.Description = updatedCategory.Description;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _categories.Remove(category);
            return NoContent();
        }
    }
}
