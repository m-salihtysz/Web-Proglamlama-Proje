using OpenAI;
using OpenAI.Chat;
using System.Text;

namespace FitnessCenter.Web.Services
{
    public class AIService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIService> _logger;

        public AIService(IConfiguration configuration, ILogger<AIService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(string ExerciseRecommendations, string DietSuggestions)> GetRecommendationsAsync(
            decimal? height, 
            decimal? weight, 
            string? bodyType, 
            string? fitnessGoals)
        {
            try
            {
                var apiKey = _configuration["OpenAI:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return ("Please configure OpenAI API key in appsettings.json", 
                           "Please configure OpenAI API key in appsettings.json");
                }

                var client = new OpenAIClient(apiKey);
                var chatClient = client.GetChatClient("gpt-4o-mini");

                var prompt = new StringBuilder();
                prompt.AppendLine("Sen bir fitness ve beslenme uzmanısın. Aşağıdaki bilgilere göre kişiselleştirilmiş öneriler sun. TÜM CEVAPLARINI TÜRKÇE VER:");
                
                if (height.HasValue)
                    prompt.AppendLine($"Boy: {height} cm");
                if (weight.HasValue)
                    prompt.AppendLine($"Kilo: {weight} kg");
                if (!string.IsNullOrEmpty(bodyType))
                    prompt.AppendLine($"Vücut Tipi: {bodyType}");
                if (!string.IsNullOrEmpty(fitnessGoals))
                    prompt.AppendLine($"Fitness Hedefleri: {fitnessGoals}");

                prompt.AppendLine("\nLütfen şunları sağla:");
                prompt.AppendLine("1. Egzersiz önerileri (spesifik egzersizler, set, tekrar, sıklık)");
                prompt.AppendLine("2. Diyet önerileri (yemek planları, makro besinler, kalori alımı)");
                prompt.AppendLine("3. Hangi egzersizleri yapınca nasıl görüneceğine dair açıklama");
                prompt.AppendLine("\nCevabını JSON formatında ver, iki alan içersin: 'exerciseRecommendations' ve 'dietSuggestions'. TÜM CEVAPLAR TÜRKÇE OLMALI.");

                var response = await chatClient.CompleteChatAsync(prompt.ToString());

                if (response?.Value?.Content != null)
                {
                    string content = response.Value.Content.ToString();
                    
                    // Try to parse JSON response
                    try
                    {
                        var jsonDoc = System.Text.Json.JsonDocument.Parse(content);
                        var exerciseRecs = jsonDoc.RootElement.TryGetProperty("exerciseRecommendations", out var ex) 
                            ? ex.GetString() ?? "No exercise recommendations available."
                            : "No exercise recommendations available.";
                        var dietRecs = jsonDoc.RootElement.TryGetProperty("dietSuggestions", out var diet) 
                            ? diet.GetString() ?? "No diet suggestions available."
                            : "No diet suggestions available.";
                        
                        return (exerciseRecs, dietRecs);
                    }
                    catch
                    {
                        // If not JSON, split the response
                        var parts = content.Split(new[] { "Diet", "diet", "DIET" }, StringSplitOptions.None);
                        var exerciseRecs = parts.Length > 0 ? parts[0].Trim() : content;
                        var dietRecs = parts.Length > 1 ? parts[1].Trim() : "No diet suggestions available.";
                        
                        return (exerciseRecs, dietRecs);
                    }
                }

                return ("Unable to generate recommendations. Please try again.", 
                       "Unable to generate recommendations. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI recommendations");
                return ($"Error: {ex.Message}", $"Error: {ex.Message}");
            }
        }

        public async Task<(string ExerciseRecommendations, string DietSuggestions)> GetRecommendationsFromImageAsync(
            IFormFile photo)
        {
            try
            {
                var apiKey = _configuration["OpenAI:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return ("Please configure OpenAI API key in appsettings.json", 
                           "Please configure OpenAI API key in appsettings.json");
                }

                // For image analysis, we would use vision API
                // For now, we'll use a text-based approach with image description
                using var memoryStream = new MemoryStream();
                await photo.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var base64Image = Convert.ToBase64String(imageBytes);

                // Use chat completion with image description
                var client = new OpenAIClient(apiKey);
                var chatClient = client.GetChatClient("gpt-4o-mini");

                var prompt = "Sen bir fitness ve beslenme uzmanısın. Yüklenen vücut fotoğrafını analiz et ve egzersiz ve diyet için kişiselleştirilmiş öneriler sun. " +
                            "Cevabını JSON formatında ver, iki alan içersin: 'exerciseRecommendations' ve 'dietSuggestions'. " +
                            "Ayrıca hangi egzersizleri yapınca nasıl görüneceğine dair açıklama da ekle. TÜM CEVAPLAR TÜRKÇE OLMALI.";

                var response = await chatClient.CompleteChatAsync(prompt);

                if (response?.Value?.Content != null)
                {
                    string content = response.Value.Content.ToString();
                    var parts = content.Split(new[] { "Diet", "diet", "DIET" }, StringSplitOptions.None);
                    var exerciseRecs = parts.Length > 0 ? parts[0].Trim() : content;
                    var dietRecs = parts.Length > 1 ? parts[1].Trim() : "No diet suggestions available.";
                    
                    return (exerciseRecs, dietRecs);
                }

                return ("Unable to analyze image. Please try again.", 
                       "Unable to analyze image. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing image");
                return ($"Error: {ex.Message}", $"Error: {ex.Message}");
            }
        }
    }
}

