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
    internal class RecipeConfig : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Description)
                .HasMaxLength(500);

            builder.Property(r => r.Ingredients)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(r => r.Instructions)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(r => r.PrepTimeMinutes)
                .IsRequired();

            builder.Property(r => r.CookTimeMinutes)
                .IsRequired();

            builder.Property(r => r.Servings)
                .IsRequired();

            builder.Property(r => r.Difficulty)
                .IsRequired()
                .HasConversion<string>(); // Store as string in the database

            builder.Property(r => r.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time

            builder.Property(r => r.ModifiedDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time

            builder.Property(r => r.RatingCount)
                .IsRequired()
                .HasDefaultValue(0); // Default to 0 ratings

            builder.HasOne(r => r.Category)
                .WithMany(r => r.Recipes)
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if category is deleted

            builder.HasOne(r => r.User)
                .WithMany(r => r.Recipes)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if user is deleted

            builder.HasMany(r => r.Ratings)
                .WithOne(r => r.Recipe)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if recipe is deleted
        }
    }
}
