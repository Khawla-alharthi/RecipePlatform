using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipePlatform.Models.Models;

namespace RecipePlatform.Models.ViewModels
{
    internal class RecipeDetailsViewModel
    {
        public Recipe Recipe { get; set; }
        public bool CanEdit { get; set; }
        public bool CanRate { get; set; }
        public int? UserRating { get; set; }
        public bool IsOwner { get; set; }
    }
}
