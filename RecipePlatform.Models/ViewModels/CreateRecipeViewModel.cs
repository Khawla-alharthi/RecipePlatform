using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using RecipePlatform.Models.Models.Enums;

namespace RecipePlatform.Models.ViewModels
{
    public class CreateRecipeViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Instructions { get; set; }

        [Required]
        [Range(1, 1440)]
        public int PrepTimeMinutes { get; set; }

        [Required]
        [Range(1, 1440)]
        public int CookTimeMinutes { get; set; }

        [Required]
        [Range(1, 50)]
        public int Servings { get; set; }

        [Required]
        public DifficultyLevel Difficulty { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
