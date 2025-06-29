using Microsoft.AspNetCore.Mvc;
using RecipePlatform.Models.Models;

namespace RecipePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private static List<ApplicationUser> _users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = "user1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                UserName = "johndoe",
                DateOfBirth = new DateTime(1990, 5, 15)
            },
            new ApplicationUser
            {
                Id = "user2",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                UserName = "janesmith",
                DateOfBirth = new DateTime(1985, 8, 22)
            }
        };

        [HttpGet]
        public ActionResult<IEnumerable<object>> GetUsers()
        {
            // Return only public information, not sensitive data
            var publicUsers = _users.Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.UserName,
                u.Email
            });
            return Ok(publicUsers);
        }

        [HttpGet("{id}")]
        public ActionResult<object> GetUser(string id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // Return only public information
            var publicUser = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.UserName,
                user.Email,
                user.DateOfBirth
            };

            return Ok(publicUser);
        }

        [HttpPost]
        public ActionResult<object> CreateUser(ApplicationUser user)
        {
            // In a real application, this would involve proper user registration with password hashing
            user.Id = Guid.NewGuid().ToString();
            _users.Add(user);

            var publicUser = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.UserName,
                user.Email
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, publicUser);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, ApplicationUser updatedUser)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.DateOfBirth = updatedUser.DateOfBirth;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _users.Remove(user);
            return NoContent();
        }

        [HttpGet("{id}/recipes")]
        public ActionResult<IEnumerable<object>> GetUserRecipes(string id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // This would typically query the recipes from the database
            // For now, return a placeholder response
            return Ok(new { message = $"Recipes for user {user.FirstName} {user.LastName}" });
        }

        [HttpGet("{id}/ratings")]
        public ActionResult<IEnumerable<object>> GetUserRatings(string id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // This would typically query the ratings from the database
            // For now, return a placeholder response
            return Ok(new { message = $"Ratings by user {user.FirstName} {user.LastName}" });
        }
    }
}
