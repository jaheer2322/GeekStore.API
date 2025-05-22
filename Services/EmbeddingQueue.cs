
using System.Threading.Channels;
using GeekStore.API.Repositories;
using GeekStore.API.Service;

namespace GeekStore.API.Services
{
    public class EmbeddingQueue : BackgroundService, IEmbeddingQueue
    {
        private readonly Channel<(Guid productId, string embeddingText)> _queue = Channel.CreateUnbounded<(Guid, string)>();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmbeddingQueue> _logger;

        public EmbeddingQueue(IServiceProvider serviceProvider, ILogger<EmbeddingQueue> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public void Enqueue(Guid productId, string embeddingText)
        {
            _queue.Writer.TryWrite((productId, embeddingText));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Background service started"); // Should appear in console
            await foreach (var (productId, text) in _queue.Reader.ReadAllAsync(stoppingToken))
            { 
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                    var embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();

                    var embedding = await embeddingService.GenerateEmbeddingAsync(text);
                    await productRepository.SaveEmbeddingAsync(productId, embedding);
                }
                catch (Exception ex)
                {
                    // Log manually since background services are not covered by HTTP pipeline
                    _logger.LogError(ex, "Error embedding product {ProductId}", productId);
                }
            }
        }
    }
}
