using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipePlatform.Models.Models.Enums;
using RecipePlatform.Models.Models;

namespace RecipePlatform.BLL.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetAllRecipes();
        Task<Recipe> GetRecipeById(int id);
        Task<Recipe> CreateRecipe(Recipe recipe);
        Task<Recipe> UpdateRecipe(Recipe recipe);
        Task DeleteRecipe(int id);
        Task<IEnumerable<Recipe>> SearchRecipes(string searchTerm, int? categoryId = null, DifficultyLevel? difficulty = null);
        Task<IEnumerable<Recipe>> GetRecipesByUser(string userId);
        Task<IEnumerable<Recipe>> GetTopRatedRecipes(int count = 10);
        Task UpdateRecipeRating(int recipeId);
    }
}
