using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipePlatform.Models.Models;

namespace RecipePlatform.BLL.Interfaces
{
    public interface IRatingService
    {
        Task<Rating> AddRating(int recipeId, string userId, int stars, string feedback = null);
        Task<Rating> UpdateRating(int recipeId, string userId, int stars, string feedback = null);
        Task<Rating> GetUserRatingForRecipe(int recipeId, string userId);
        Task<IEnumerable<Rating>> GetRatingsForRecipe(int recipeId);
    }
}
