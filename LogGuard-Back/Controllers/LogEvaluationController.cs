using Microsoft.AspNetCore.Mvc;
using LogGuard.Services;
using System.Text.Json.Serialization;

namespace LogGuard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogEvaluationController : ControllerBase
    {
        private readonly ILogEvaluationService _logService;

        public LogEvaluationController(ILogEvaluationService logService)
        {
            _logService = logService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessLogs([FromBody] LogRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Logs))
                return BadRequest("Le champ 'logs' est vide ou manquant.");

            try
            {
                var result = await _logService.EvaluateStreamAsync(request.Logs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class LogRequest
    {
        // CORRECTIF : accepte "logs" (minuscule, envoyé par Angular)
        // ET "Logs" (majuscule, convention C#)
        [JsonPropertyName("logs")]
        public string Logs { get; set; } = string.Empty;
    }
}