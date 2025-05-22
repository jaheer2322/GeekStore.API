using Pgvector;
namespace GeekStore.API.Service
{
    public interface IEmbeddingService
    {
        Task<Vector> GenerateEmbeddingAsync(string text);
    }
}
