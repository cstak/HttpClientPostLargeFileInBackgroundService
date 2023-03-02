using HttpClientPostLargeFileInBackgroundService.Interfaces;
namespace HttpClientPostLargeFileInBackgroundService;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IFileWatcherService _fileWatcherService;
    public Worker(ILogger<Worker> logger, IFileWatcherService fileWatcherService)
    {
        _logger = logger;
        _fileWatcherService = fileWatcherService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _fileWatcherService.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(Timeout.Infinite, stoppingToken);
                GC.KeepAlive(_fileWatcherService);
            }
        }
        catch (Exception ex) { _logger.LogError(ex.Message); }
    }
}
