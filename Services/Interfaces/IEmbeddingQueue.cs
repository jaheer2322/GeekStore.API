namespace GeekStore.API.Services.Interfaces
{
    public interface IEmbeddingQueue
    {
        void Enqueue(Guid productId, string embeddingText);
    }
}
