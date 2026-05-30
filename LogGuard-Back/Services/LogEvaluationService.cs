using System.Text;
using System.Text.Json;

namespace LogGuard.Services
{
    public interface ILogEvaluationService
    {
        Task<string> EvaluateLogsAsync(string logs);
        Task<string> EvaluateStreamAsync(string logs);
    }

    public class LogEvaluationService : ILogEvaluationService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LogEvaluationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> EvaluateLogsAsync(string logs) => await CallOllamaAsync(logs);
        public async Task<string> EvaluateStreamAsync(string logs) => await CallOllamaAsync(logs);

        private async Task<string> CallOllamaAsync(string logs)
        {
            var requestBody = new
            {
                model = "gemma2:2b",
                prompt = $"Tu es un expert en cybersecurite. Analyse ces logs et identifie toutes les anomalies et comportements suspects. Reponds UNIQUEMENT en francais :\n{logs}",
                stream = false,
                keep_alive = "10m"
            };

            var jsonPayload = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(10);

            try
            {
                var response = await client.PostAsync("http://localhost:11434/api/generate", content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return $"Erreur HTTP Ollama ({(int)response.StatusCode}) : {errorBody}";
                }
                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(jsonResponse);
                if (document.RootElement.TryGetProperty("response", out var resp))
                    return resp.GetString() ?? "Reponse vide.";
                return $"Format inconnu : {jsonResponse}";
            }
            catch (TaskCanceledException)
            {
                return "Timeout.";
            }
            catch (HttpRequestException ex)
            {
                return $"Impossible de joindre Ollama : {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Erreur : {ex.Message}";
            }
        }
    }
}
