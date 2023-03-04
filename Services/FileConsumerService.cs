using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpClientPostLargeFileInBackgroundService.Errors;
using HttpClientPostLargeFileInBackgroundService.Interfaces;

namespace HttpClientPostLargeFileInBackgroundService.Services
{
    public class FileConsumerService : IFileConsumerService
    {
        private ILogger<FileConsumerService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public FileConsumerService(ILogger<FileConsumerService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task ConsumeFile(string pathToFile)
        {
            if (!File.Exists(pathToFile))
                return;

            _logger.LogInformation($"The file is starting to be sent: {pathToFile}");
            try
            {
                await UploadFilesAsync(pathToFile);
                File.Delete(pathToFile);
                _logger.LogInformation($"The file has been sent: {pathToFile}");
            }
            catch (Exception? ex)
            {
                Console.WriteLine($"Request failed. Error Code: {ex.InnerException?.Message}");
            }
        }
        public async Task UploadFilesAsync(string filePath)
        {
            string boundary = $"--{DateTime.Now.Ticks:x}";
            using var content = new MultipartFormDataContent(boundary);
            content.Headers.Add("ContentType", $"multipart/form-data, boundary={boundary}");

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileName = Path.GetFileNameWithoutExtension(filePath) + Path.GetExtension(filePath);
            content.Add(new StreamContent(stream), "files", fileName);

            using HttpRequestMessage request = new(HttpMethod.Post, new Uri("https://your-api-url")) { Content = content };

            try
            {
                var clientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    {
                        return true;
                    }
                };
                var httpClient = _httpClientFactory.CreateClient("Default");
                //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var result = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                result.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpEx)
            {
                if (httpEx is HttpErrorStatusCodeException httpErrorStatusEx)
                {
                    Console.WriteLine($"Request failed. Error Code: {httpErrorStatusEx.ErrorStatusCode}");
                }
                else
                {
                    _logger.LogError("Error: " + httpEx.Message);
                }
            }
        }
    }
}