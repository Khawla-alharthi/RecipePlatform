using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipePlatform.Models.Models
{
    public class Rating : BaseEntity
    {
        public int Stars { get; set; }
        public string? Feedback { get; set; } 
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } // fk
        public int RecipeId { get; set; } // fk

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("RecipeId")]
        public virtual Recipe Recipe { get; set; }
    }
}
