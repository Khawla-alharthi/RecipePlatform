using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecipePlatform.BLL.Interfaces;
using RecipePlatform.BLL.Repositories;
using RecipePlatform.Models.Models;

namespace RecipePlatform.BLL.Services
{
    public class RatingService : IRatingService
    {
        private readonly IGenaricRepository<Rating> _ratingRepository;
        private readonly IRecipeService _recipeService;

        public RatingService(IGenaricRepository<Rating> ratingRepository, IRecipeService recipeService)
        {
            _ratingRepository = ratingRepository;
            _recipeService = recipeService;
        }

        public async Task<Rating> AddRating(int recipeId, string userId, int stars, string feedback = null)
        {
            var existingRating = await GetUserRatingForRecipe(recipeId, userId);
            if (existingRating != null)
            {
                throw new InvalidOperationException("User has already rated this recipe");
            }

            var rating = new Rating
            {
                RecipeId = recipeId,
                UserId = userId,
                Stars = stars,
                Feedback = feedback,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _ratingRepository.Add(rating);
            await _recipeService.UpdateRecipeRating(recipeId);

            return result;
        }

        public async Task<Rating> UpdateRating(int recipeId, string userId, int stars, string feedback = null)
        {
            var existingRating = await GetUserRatingForRecipe(recipeId, userId);
            if (existingRating == null)
            {
                throw new InvalidOperationException("Rating not found");
            }

            existingRating.Stars = stars;
            existingRating.Feedback = feedback;

            var result = await _ratingRepository.Update(existingRating);
            await _recipeService.UpdateRecipeRating(recipeId);

            return result;
        }

        public async Task<Rating> GetUserRatingForRecipe(int recipeId, string userId)
        {
            return await _ratingRepository.GetQueryable()
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId);
        }

        public async Task<IEnumerable<Rating>> GetRatingsForRecipe(int recipeId)
        {
            return await _ratingRepository.GetQueryable()
                .Include(r => r.User)
                .Where(r => r.RecipeId == recipeId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }
    }
}
