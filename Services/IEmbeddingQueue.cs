namespace GeekStore.API.Services
{
    public interface IEmbeddingQueue
    {
        void Enqueue(Guid productId, string embeddingText);
    }
}
