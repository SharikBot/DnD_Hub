using System.Net;
using System.Text;
using System.Text.Json;
using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DnDCharacterManager.Infrastructure.Services;

public class GeminiAiService : IGeminiAiService
{
    private const int MaxRetryAttempts = 3;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiAiService> _logger;

    public GeminiAiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiAiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AiGenerationResponseDto> GenerateAsync(
        AiGenerationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return new AiGenerationResponseDto
            {
                IsSuccess = false,
                ErrorMessage = "Gemini API key is not configured."
            };
        }

        var model = _configuration["Gemini:Model"] ?? "gemini-2.0-flash";
        var baseUrl = _configuration["Gemini:BaseUrl"]
            ?? "https://generativelanguage.googleapis.com/v1beta/models/";

        var prompt = BuildPrompt(request);
        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[] { new { text = prompt } }
                }
            },
            generationConfig = new
            {
                maxOutputTokens = request.MaxTokens > 0 ? request.MaxTokens : 1024
            }
        };

        var url = $"{baseUrl.TrimEnd('/')}/{model}:generateContent?key={apiKey}";
        var json = JsonSerializer.Serialize(payload);

        for (var attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            try
            {
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync(url, content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    var generatedText = ExtractGeneratedText(responseBody);
                    return new AiGenerationResponseDto
                    {
                        IsSuccess = true,
                        GeneratedText = generatedText,
                        TokensUsed = EstimateTokens(generatedText)
                    };
                }

                if (!IsRetryableStatusCode(response.StatusCode) || attempt == MaxRetryAttempts)
                {
                    var error = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("Gemini API request failed with status {StatusCode}: {Error}", response.StatusCode, error);
                    return new AiGenerationResponseDto
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Gemini API error: {response.StatusCode}"
                    };
                }

                _logger.LogWarning("Gemini API attempt {Attempt} failed with {StatusCode}, retrying...", attempt, response.StatusCode);
            }
            catch (Exception ex) when (attempt < MaxRetryAttempts)
            {
                _logger.LogWarning(ex, "Gemini API attempt {Attempt} threw an exception, retrying...", attempt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini API request failed after {Attempts} attempts", MaxRetryAttempts);
                return new AiGenerationResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }

            await Task.Delay(RetryDelay * attempt, cancellationToken);
        }

        return new AiGenerationResponseDto
        {
            IsSuccess = false,
            ErrorMessage = "Gemini API request failed after retries."
        };
    }

    private static string BuildPrompt(AiGenerationRequestDto request)
    {
        var contentType = string.IsNullOrWhiteSpace(request.ContentType) ? "general" : request.ContentType.ToLowerInvariant();
        var characterContext = request.CharacterId.HasValue
            ? $" ID персонажа: {request.CharacterId.Value}."
            : string.Empty;

        var instruction = contentType switch
        {
            "backstory" => "Напиши биографию персонажа D&D 5e на русском (2–4 абзаца).",
            "name" => "Предложи 5 подходящих имён для персонажа D&D 5e. По одному на строку, с кратким значением.",
            "personality" => "Опиши черты характера, идеалы, привязанности и слабости персонажа D&D на русском.",
            "build" => "Предложи распределение характеристик и выбор черт для описанного билда.",
            "class-recommendation" => "Порекомендуй 2–3 класса D&D 5e с обоснованием для описанной концепции.",
            "spell-recommendation" => "Порекомендуй заклинания для указанного класса и уровня с краткими тактическими заметками.",
            "npc" => "Создай описание NPC для D&D 5e: роль, характер, тактика и заметки для мастера.",
            "encounter" => "Придумай сцену (бой или социальную) для D&D 5e: завязка, участники, награды.",
            "quest" => "Составь квест: цель, осложнения и награды для кампании D&D.",
            _ => "Ответь по правилам D&D 5e на русском языке."
        };

        return $"""
            Задание: {instruction}
            Тип: {contentType}.{characterContext}
            Запрос пользователя: {request.Prompt}
            Ответ — связный текст на русском, в стиле настольной ролевой игры.
            """;
    }

    private static string ExtractGeneratedText(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);
        if (!document.RootElement.TryGetProperty("candidates", out var candidates) ||
            candidates.GetArrayLength() == 0)
        {
            return string.Empty;
        }

        var firstCandidate = candidates[0];
        if (!firstCandidate.TryGetProperty("content", out var content) ||
            !content.TryGetProperty("parts", out var parts) ||
            parts.GetArrayLength() == 0)
        {
            return string.Empty;
        }

        return parts[0].TryGetProperty("text", out var text)
            ? text.GetString() ?? string.Empty
            : string.Empty;
    }

    private static int EstimateTokens(string text) =>
        string.IsNullOrWhiteSpace(text) ? 0 : text.Length / 4;

    private static bool IsRetryableStatusCode(HttpStatusCode statusCode) =>
        statusCode is HttpStatusCode.TooManyRequests
            or HttpStatusCode.RequestTimeout
            or HttpStatusCode.BadGateway
            or HttpStatusCode.ServiceUnavailable
            or HttpStatusCode.GatewayTimeout;
}
