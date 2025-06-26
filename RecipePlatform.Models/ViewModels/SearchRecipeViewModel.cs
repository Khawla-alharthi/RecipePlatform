using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipePlatform.Models.Models.Enums;
using RecipePlatform.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecipePlatform.Models.ViewModels
{
    public class SearchRecipeViewModel
    {
        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public DifficultyLevel? Difficulty { get; set; }
        public int? MaxPrepTime { get; set; }
        public int? MaxCookTime { get; set; }
        public decimal? MinRating { get; set; }

        public List<Recipe> Results { get; set; } = new List<Recipe>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
