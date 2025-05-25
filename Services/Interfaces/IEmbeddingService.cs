using Pgvector;
namespace GeekStore.API.Services.Interfaces
{
    public interface IEmbeddingService
    {
        Task<Vector> GenerateEmbeddingAsync(string text);
    }
}
