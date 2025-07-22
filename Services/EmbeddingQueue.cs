
using System.Threading.Channels;
using GeekStore.API.Repositories.Interfaces;
using GeekStore.API.Services.Interfaces;

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
            _logger.LogInformation("Enqueuing embedding for product {ProductId}", productId);
            _queue.Writer.TryWrite((productId, embeddingText));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var (productId, text) in _queue.Reader.ReadAllAsync(stoppingToken))
            { 
                var pythonEngine = _serviceProvider.GetRequiredService<PythonEngineSingleton>();

                // Ensure Python engine is initialized before processing
                while (!pythonEngine.isReady)
                {
                    _logger.LogInformation("Waiting for Python engine to be ready...");
                    await Task.Delay(1000, stoppingToken);
                }

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
