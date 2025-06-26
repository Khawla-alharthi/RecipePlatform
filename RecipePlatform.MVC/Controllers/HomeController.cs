using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RecipePlatform.BLL.Interfaces;
using RecipePlatform.MVC.Models;

namespace RecipePlatform.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRecipeService _recipeService;

        public HomeController(ILogger<HomeController> logger, IRecipeService recipeService)
        {
            _logger = logger;
            _recipeService = recipeService;
        }

        public async Task<IActionResult> Index()
        {
            var latestRecipes = await _recipeService.GetAllRecipes();
            var topRatedRecipes = await _recipeService.GetTopRatedRecipes(4);

            ViewBag.LatestRecipes = latestRecipes.Take(6);
            ViewBag.TopRatedRecipes = topRatedRecipes;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
