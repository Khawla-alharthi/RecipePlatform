using Microsoft.AspNetCore.Mvc;
using RecipePlatform.Models.Models;

namespace RecipePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : Controller
    {
        private static List<Rating> _ratings = new List<Rating>
        {
            new Rating { Id = 1, Stars = 5, Feedback = "Excellent recipe!", UserId = "user1", RecipeId = 1 },
            new Rating { Id = 2, Stars = 4, Feedback = "Very good, will make again", UserId = "user2", RecipeId = 1 },
            new Rating { Id = 3, Stars = 5, Feedback = "Perfect curry!", UserId = "user1", RecipeId = 2 }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Rating>> GetRatings()
        {
            return Ok(_ratings);
        }

        [HttpGet("{id}")]
        public ActionResult<Rating> GetRating(int id)
        {
            var rating = _ratings.FirstOrDefault(r => r.Id == id);
            if (rating == null)
            {
                return NotFound();
            }
            return Ok(rating);
        }

        [HttpGet("recipe/{recipeId}")]
        public ActionResult<IEnumerable<Rating>> GetRatingsByRecipe(int recipeId)
        {
            var recipeRatings = _ratings.Where(r => r.RecipeId == recipeId).ToList();
            return Ok(recipeRatings);
        }

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<Rating>> GetRatingsByUser(string userId)
        {
            var userRatings = _ratings.Where(r => r.UserId == userId).ToList();
            return Ok(userRatings);
        }

        [HttpPost]
        public ActionResult<Rating> CreateRating(Rating rating)
        {
            // Check if user has already rated this recipe
            var existingRating = _ratings.FirstOrDefault(r => r.UserId == rating.UserId && r.RecipeId == rating.RecipeId);
            if (existingRating != null)
            {
                return BadRequest("User has already rated this recipe. Use PUT to update the rating.");
            }

            rating.Id = _ratings.Any() ? _ratings.Max(r => r.Id) + 1 : 1;
            rating.CreatedDate = DateTime.UtcNow;
            _ratings.Add(rating);
            return CreatedAtAction(nameof(GetRating), new { id = rating.Id }, rating);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRating(int id, Rating updatedRating)
        {
            var rating = _ratings.FirstOrDefault(r => r.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            rating.Stars = updatedRating.Stars;
            rating.Feedback = updatedRating.Feedback;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRating(int id)
        {
            var rating = _ratings.FirstOrDefault(r => r.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            _ratings.Remove(rating);
            return NoContent();
        }
    }
}
