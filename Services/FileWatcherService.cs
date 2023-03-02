using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpClientPostLargeFileInBackgroundService.Interfaces;

namespace HttpClientPostLargeFileInBackgroundService.Services
{
    public class FileWatcherService : IFileWatcherService
    {

        private readonly FileSystemWatcher? _fileSystemWatcher;
        private readonly ILogger<FileWatcherService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        //You can define a filter for file type. exmpl:"*.mp4"
        private string _fileFilter = "*";
        public FileWatcherService(ILogger<FileWatcherService> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;


        }
        private string _consumedDirectoryPath => _configuration["DirectoryPaths:MyDirectory"];
        
        public void Start()
        {
            if (!Directory.Exists(_consumedDirectoryPath))
            {
                Directory.CreateDirectory(_consumedDirectoryPath);
            }
            _fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;

            //_fileSystemWatcher.Changed += _fileSystemWatcher_Changed;
            _fileSystemWatcher.Created += _fileSystemWatcher_Created;
            //_fileSystemWatcher.Deleted += _fileSystemWatcher_Deleted;
            //_fileSystemWatcher.Renamed += _fileSystemWatcher_Renamed;
            _fileSystemWatcher.Error += _fileSystemWatcher_Error;

            _fileSystemWatcher.EnableRaisingEvents = true;
            _fileSystemWatcher.IncludeSubdirectories = true;

            _logger.LogInformation($"{_consumedDirectoryPath} dizininde video dosyası gözlemlemesi başladı");
        }
        private void _fileSystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            _logger.LogInformation($"Dosya hatası {e.GetException().Message}");
        }
        /*      private void _fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
                {
                    _logger.LogInformation($"File rename event for file {e.FullPath}");
                }

                private void _fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
                {
                    _logger.LogInformation($"File deleted event for file {e.FullPath}");
                }

                private void _fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
                {
                }
        */

        private void _fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(5000);
            using (var scope = _serviceProvider.CreateScope())
            {
                var consumerService = scope.ServiceProvider.GetRequiredService<IFileConsumerService>();
                if (IsFileClosed(e.FullPath))
                {
                    Task.Run(() => consumerService.ConsumeFile(e.FullPath));
                }
                else
                {
                    _logger.LogInformation($"File is not ready for Uploading: {e.FullPath}");
                }
            }
        }
    }
}