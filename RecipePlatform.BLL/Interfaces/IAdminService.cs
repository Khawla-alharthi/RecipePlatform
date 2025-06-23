using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipePlatform.Models.Models;

namespace RecipePlatform.BLL.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsers();
        Task<ApplicationUser> GetUserById(string userId);
        Task DeleteUser(string userId);
        Task<IEnumerable<Recipe>> GetAllRecipesForAdmin();
        Task DeleteRecipe(int recipeId);
        Task<Dictionary<string, object>> GetDashboardStats();
    }
}
