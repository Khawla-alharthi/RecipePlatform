using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipePlatform.Models.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }


        // Navigation properties
        public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    }
}
