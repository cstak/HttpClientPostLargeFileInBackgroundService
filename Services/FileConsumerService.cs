using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public Task ConsumeFile(string pathToFile)
        {
            throw new NotImplementedException();
        }
    }
}