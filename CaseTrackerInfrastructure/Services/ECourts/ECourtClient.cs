using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.ECourts;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CaseTrackerInfrastructure.Services.ECourts
{
    public class ECourtClient : IECourtClient
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ECourtClient> _logger;
        private readonly string _apiKey;

        public ECourtClient(
            HttpClient httpClient,
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            ILogger<ECourtClient> logger)
        {
            _httpClient = httpClient;
            _scopeFactory = scopeFactory;
            _logger = logger;

            _apiKey = configuration["ECourts:ApiKey"]
                ?? configuration["ECOURTS_API_KEY"]
                ?? throw new InvalidOperationException("ECourts:ApiKey is not configured.");

            var baseUrl = configuration["ECourts:BaseUrl"] ?? "https://webapi.ecourtsindia.com";
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<ECourtsStateItem>> GetStatesAsync()
        {
            var response = await ExecuteWithRetryAsync<List<ECourtsStateItem>>(
                "api/partner/causelist/court-structure/states",
                HttpMethod.Get,
                creditsCost: 0);

            return response?.Data ?? new List<ECourtsStateItem>();
        }

        public async Task<List<ECourtsDistrictItem>> GetDistrictsAsync(string stateCode)
        {
            string url = $"api/partner/causelist/court-structure/states/{Uri.EscapeDataString(stateCode)}/districts";
            var response = await ExecuteWithRetryAsync<List<ECourtsDistrictItem>>(
                url,
                HttpMethod.Get,
                creditsCost: 0);

            return response?.Data ?? new List<ECourtsDistrictItem>();
        }

        public async Task<List<ECourtsComplexItem>> GetComplexesAsync(string stateCode, string districtCode)
        {
            string url = $"api/partner/causelist/court-structure/states/{Uri.EscapeDataString(stateCode)}/districts/{Uri.EscapeDataString(districtCode)}/complexes";
            var response = await ExecuteWithRetryAsync<List<ECourtsComplexItem>>(
                url,
                HttpMethod.Get,
                creditsCost: 0);

            return response?.Data ?? new List<ECourtsComplexItem>();
        }

        public async Task<List<ECourtsCourtItem>> GetCourtsAsync(string stateCode, string districtCode, string complexCode)
        {
            string url = $"api/partner/causelist/court-structure/states/{Uri.EscapeDataString(stateCode)}/districts/{Uri.EscapeDataString(districtCode)}/complexes/{Uri.EscapeDataString(complexCode)}/courts";
            var response = await ExecuteWithRetryAsync<List<ECourtsCourtItem>>(
                url,
                HttpMethod.Get,
                creditsCost: 0);

            return response?.Data ?? new List<ECourtsCourtItem>();
        }

        public async Task<ECourtsCaseDetailPayload?> GetCaseDetailAsync(string cnrNumber)
        {
            string url = $"api/partner/case/{Uri.EscapeDataString(cnrNumber)}";
            var response = await ExecuteWithRetryAsync<ECourtsCaseDetailPayload>(
                url,
                HttpMethod.Get,
                creditsCost: 1,
                cnrNumber: cnrNumber);

            return response?.Data;
        }

        private async Task<ECourtsResponse<T>?> ExecuteWithRetryAsync<T>(
            string endpoint,
            HttpMethod method,
            int creditsCost,
            string? cnrNumber = null,
            int maxRetries = 3)
        {
            int attempts = 0;
            var sw = Stopwatch.StartNew();
            DateTimeOffset requestedAt = DateTimeOffset.UtcNow;
            string? requestId = null;
            int statusCode = 0;
            bool isSuccess = false;
            string? errorMessage = null;

            while (attempts < maxRetries)
            {
                attempts++;
                try
                {
                    using var request = new HttpRequestMessage(method, endpoint);
                    using var response = await _httpClient.SendAsync(request);
                    statusCode = (int)response.StatusCode;

                    if (response.StatusCode == (HttpStatusCode)429)
                    {
                        var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(Math.Pow(2, attempts));
                        _logger.LogWarning("eCourts Rate limit reached for {Endpoint}. Retrying after {Seconds} seconds.", endpoint, retryAfter.TotalSeconds);
                        await Task.Delay(retryAfter);
                        continue;
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        errorMessage = $"HTTP {statusCode}: {content}";
                        _logger.LogError("eCourts request to {Endpoint} failed with status {Status}: {Error}", endpoint, statusCode, errorMessage);
                        break;
                    }

                    isSuccess = true;
                    var envelope = JsonSerializer.Deserialize<ECourtsResponse<T>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    requestId = envelope?.Meta?.RequestId;
                    return envelope;
                }
                catch (HttpRequestException ex)
                {
                    errorMessage = ex.Message;
                    _logger.LogWarning("Network exception during eCourts call to {Endpoint}: {Error}. Attempt {Attempt} of {Max}", endpoint, ex.Message, attempts, maxRetries);
                    if (attempts >= maxRetries) break;
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempts)));
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    _logger.LogError(ex, "Unexpected error calling eCourts API at {Endpoint}", endpoint);
                    break;
                }
            }

            sw.Stop();
            await LogApiRequestAsync(endpoint, method.Method, cnrNumber, statusCode, requestId, isSuccess ? creditsCost : 0, sw.ElapsedMilliseconds, isSuccess, errorMessage, requestedAt);

            return null;
        }

        private async Task LogApiRequestAsync(
            string endpoint,
            string httpMethod,
            string? cnrNumber,
            int statusCode,
            string? requestId,
            int creditsCharged,
            long durationMs,
            bool isSuccess,
            string? errorMessage,
            DateTimeOffset requestedAt)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var log = new ECourtApiLog
                {
                    LogId = Guid.NewGuid(),
                    Endpoint = endpoint.Length > 200 ? endpoint[..200] : endpoint,
                    HttpMethod = httpMethod,
                    CnrNumber = cnrNumber,
                    StatusCode = statusCode,
                    RequestId = requestId,
                    CreditsCharged = creditsCharged,
                    DurationMs = durationMs,
                    IsSuccess = isSuccess,
                    ErrorMessage = errorMessage,
                    RequestedAt = requestedAt
                };

                await dbContext.ECourtApiLogs.AddAsync(log);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to write eCourts API audit log: {Message}", ex.Message);
            }
        }
    }
}
