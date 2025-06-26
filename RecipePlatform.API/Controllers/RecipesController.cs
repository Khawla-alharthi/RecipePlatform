using Microsoft.AspNetCore.Mvc;
using RecipePlatform.BLL.Interfaces;

namespace RecipePlatform.API.Controllers
{
    [ApiController]

    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IRatingService _ratingService;

        public RecipesController(IRecipeService recipeService, IRatingService ratingService)
        {
            _recipeService = recipeService;
            _ratingService = ratingService;
        }
    }
}
