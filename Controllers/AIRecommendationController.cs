using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessCenter.Web.Services;
using FitnessCenter.Web.ViewModels;

namespace FitnessCenter.Web.Controllers
{
    [Authorize]
    public class AIRecommendationController : Controller
    {
        private readonly AIService _aiService;

        public AIRecommendationController(AIService aiService)
        {
            _aiService = aiService;
        }

        // GET: AIRecommendation
        public IActionResult Index()
        {
            return View(new AIRecommendationViewModel());
        }

        // POST: AIRecommendation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AIRecommendationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                if (viewModel.Photo != null && viewModel.Photo.Length > 0)
                {
                    var (exerciseRecs, dietRecs) = await _aiService.GetRecommendationsFromImageAsync(viewModel.Photo);
                    viewModel.ExerciseRecommendations = exerciseRecs;
                    viewModel.DietSuggestions = dietRecs;
                }
                else if (viewModel.Height.HasValue || viewModel.Weight.HasValue || !string.IsNullOrEmpty(viewModel.BodyType))
                {
                    var (exerciseRecs, dietRecs) = await _aiService.GetRecommendationsAsync(
                        viewModel.Height, 
                        viewModel.Weight, 
                        viewModel.BodyType, 
                        viewModel.FitnessGoals);
                    viewModel.ExerciseRecommendations = exerciseRecs;
                    viewModel.DietSuggestions = dietRecs;
                }
                else
                {
                    viewModel.ErrorMessage = "Lütfen bir fotoğraf yükleyin veya vücut ölçülerinizi (boy, kilo, vücut tipi) girin.";
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                viewModel.ErrorMessage = $"Bir hata oluştu: {ex.Message}";
            }

            return View(viewModel);
        }
    }
}

