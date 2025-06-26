using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipePlatform.Models.Models.Enums;

namespace RecipePlatform.Models.Models
{
    public class Recipe : BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public int PrepTimeMinutes { get; set; }
        public int CookTimeMinutes { get; set; }
        public int Servings { get; set; }
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Easy;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
        public int RatingCount { get; set; }  // Total number of ratings


        // Computed properties
        [NotMapped]
        public double? AverageRating => Ratings?.Any() == true ? Ratings.Average(r => r.Stars) : null;


        public int CategoryId { get; set; } // fk
        public string UserId { get; set; } // fk
        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // Ratings
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
