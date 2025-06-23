using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipePlatform.BLL.Interfaces;
using RecipePlatform.BLL.Repositories;
using RecipePlatform.DAL.Context;
using RecipePlatform.Models.Models;

namespace RecipePlatform.BLL.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenaricRepository<Recipe> _recipeRepository;
        private readonly IGenaricRepository<Rating> _ratingRepository;
        private readonly ApplicationDbContext _context;

        public AdminService(UserManager<ApplicationUser> userManager,
                           IGenaricRepository<Recipe> recipeRepository,
                           IGenaricRepository<Rating> ratingRepository,
                           ApplicationDbContext context)
        {
            _userManager = userManager;
            _recipeRepository = recipeRepository;
            _ratingRepository = ratingRepository;
            _context = context;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            return await _userManager.Users
                .Include(u => u.Recipes)
                .ToListAsync();
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _userManager.Users
                .Include(u => u.Recipes)
                .Include(u => u.Ratings)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesForAdmin()
        {
            return await _recipeRepository.GetQueryable()
                .Include(r => r.User)
                .Include(r => r.Category)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task DeleteRecipe(int recipeId)
        {
            await _recipeRepository.Delete(recipeId);
        }

        public async Task<Dictionary<string, object>> GetDashboardStats()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var totalRecipes = await _recipeRepository.GetQueryable().CountAsync();
            var totalRatings = await _ratingRepository.GetQueryable().CountAsync();
            var avgRating = await _ratingRepository.GetQueryable().AverageAsync(r => (double?)r.Stars) ?? 0;

            return new Dictionary<string, object>
            {
                { "TotalUsers", totalUsers },
                { "TotalRecipes", totalRecipes },
                { "TotalRatings", totalRatings },
                { "AverageRating", Math.Round(avgRating, 2) }
            };
        }
    }
}
