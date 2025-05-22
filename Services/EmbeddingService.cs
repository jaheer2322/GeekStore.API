using GeekStore.API.Service;
using Pgvector;

namespace GeekStore.API.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        public async Task<Vector> GenerateEmbeddingAsync(string text)
        {
            float[] arr = new float[768];
            arr[0] = 0.1f;
            return new Vector(arr);
        }
    }
}
