using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using API.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundTasksSample.Services
{
    public class RefreshCacheHostedService : IHostedService, IDisposable
    {
        private readonly IAgileEnginePhotosService agileEnginePhotosService;
        private readonly IConfiguration configuration;
        private readonly ILogger<RefreshCacheHostedService> logger;
        private readonly IMemoryCache memoryCache;
        private Timer _timer;
        private static Semaphore semaphore;
        private static Stopwatch stopwatch;

        public RefreshCacheHostedService(IAgileEnginePhotosService agileEnginePhotosService, IConfiguration configuration, ILogger<RefreshCacheHostedService> logger, IMemoryCache memoryCache)
        {
            this.agileEnginePhotosService = agileEnginePhotosService;
            this.configuration = configuration;
            this.logger = logger;
            this.memoryCache = memoryCache;
            semaphore = new Semaphore(1, 1);
            stopwatch = new Stopwatch();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Refresh Cache Hosted Service running.");

            _timer = new Timer(RefreshCache, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(double.Parse(configuration["RefreshCachePeriodInMinutes"])));

            return Task.CompletedTask;
        }

        // async void is terribly bad for everything except event handlers, like this one
        private async void RefreshCache(object state)
        {
            semaphore.WaitOne(); // We don't want to overload the API with multiple executions at the same time 
            try
            {
                stopwatch.Restart();
                logger.LogInformation("Refreshing cache");
                var photos = await agileEnginePhotosService.GetPicturesMetadata();
                var fullData = await agileEnginePhotosService.GetPicturesFullData(photos);
                memoryCache.Set("Photos", fullData);
                stopwatch.Stop();
                logger.LogInformation($"Cache refreshed successfully. Elapsed: {stopwatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error has occurred wihle trying to refresh the cache. Details: {ex.Message}");
            }
            finally
            {
                semaphore.Release();
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Refresh Cache Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}