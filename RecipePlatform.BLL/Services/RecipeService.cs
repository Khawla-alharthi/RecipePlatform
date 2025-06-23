using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecipePlatform.BLL.Interfaces;
using RecipePlatform.BLL.Repositories;
using RecipePlatform.DAL.Context;
using RecipePlatform.Models.Models;
using RecipePlatform.Models.Models.Enums;

namespace RecipePlatform.BLL.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IGenaricRepository<Recipe> _recipeRepository;
        private readonly IGenaricRepository<Rating> _ratingRepository;
        private readonly ApplicationDbContext _context;

        public RecipeService(IGenaricRepository<Recipe> recipeRepository, IGenaricRepository<Rating> ratingRepository, ApplicationDbContext context)
        {
            _recipeRepository = recipeRepository;
            _ratingRepository = ratingRepository;
            _context = context;
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipes()
        {
            return await _recipeRepository.GetQueryable()
            .Include(r => r.User)
            .Include(r => r.Category)
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync();
        }

        public async Task<Recipe> GetRecipeById(int id)
        {
            return await _recipeRepository.GetQueryable()
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.Ratings)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recipe> CreateRecipe(Recipe recipe)
        {
            recipe.CreatedDate = DateTime.UtcNow;
            recipe.ModifiedDate = DateTime.UtcNow;
            return await _recipeRepository.Add(recipe);
        }

        public async Task<Recipe> UpdateRecipe(Recipe recipe)
        {
            var existingRecipe = await _recipeRepository.GetById(recipe.Id);
            if (existingRecipe == null)
            {
                throw new KeyNotFoundException("Recipe not found");
            }
            existingRecipe.Title = recipe.Title;
            existingRecipe.Description = recipe.Description;
            existingRecipe.Ingredients = recipe.Ingredients;
            existingRecipe.Instructions = recipe.Instructions;
            existingRecipe.PrepTimeMinutes = recipe.PrepTimeMinutes;
            existingRecipe.CookTimeMinutes = recipe.CookTimeMinutes;
            existingRecipe.Servings = recipe.Servings;
            existingRecipe.Difficulty = recipe.Difficulty;
            existingRecipe.CategoryId = recipe.CategoryId;
            existingRecipe.ModifiedDate = DateTime.UtcNow;
            _context.Entry(existingRecipe).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return existingRecipe;
        }

        public async Task DeleteRecipe(int id)
        {
            await _recipeRepository.Delete(id);
        }

        public async Task<IEnumerable<Recipe>> SearchRecipes(string searchTerm, int? categoryId = null, DifficultyLevel? difficulty = null)
        {
            var query = _recipeRepository.GetQueryable()
                .Include(r => r.User)
                .Include(r => r.Category)
                .AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Title.Contains(searchTerm) || r.Description.Contains(searchTerm));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }
            if (difficulty.HasValue)
            {
                query = query.Where(r => r.Difficulty == difficulty.Value);
            }
            return await query.OrderByDescending(r => r.CreatedDate).ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByUser(string userId)
        {
            return await _recipeRepository.GetQueryable()
                .Include(r => r.User)
                .Include(r => r.Category)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetTopRatedRecipes(int count = 10)
        {
            return await _recipeRepository.GetQueryable()
                .Include(r => r.User)
                .Include(r => r.Category)
                .OrderByDescending(r => r.Ratings.Average(r => r.Stars))
                .Take(count)
                .ToListAsync();
        }

        public async Task UpdateRecipeRating(int recipeId)
        {
            var recipe = await _recipeRepository.GetQueryable()
                .Include(r => r.Ratings)
                .FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe == null)
            {
                throw new KeyNotFoundException("Recipe not found");
            }

            recipe.RatingCount = recipe.Ratings.Count;
            recipe.ModifiedDate = DateTime.UtcNow;

            _context.Entry(recipe).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
