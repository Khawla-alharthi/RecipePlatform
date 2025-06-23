using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipePlatform.Models.Models;

namespace RecipePlatform.DAL.Configurations
{
    public class RatingConfig : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.Property(r => r.Stars)
                .IsRequired()
                .HasDefaultValue(0); // Default rating value

            builder.Property(r => r.Feedback)
                .HasMaxLength(1000); // Limit feedback length

            builder.Property(r => r.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time

            // Configure the many-to-one relationship with Recipe
            builder.HasOne(r => r.Recipe)
                .WithMany(recipe => recipe.Ratings)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the many-to-one relationship with User
            builder.HasOne(r => r.User)
                .WithMany(recipe => recipe.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

        }
    }
}
