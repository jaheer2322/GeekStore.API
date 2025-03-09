using GeekStore.API.Models.Domains;

namespace GeekStore.API.Repositories
{
    public interface IImageRepository
    {
        Task<Image> UploadAsync(Image image);
    }
}
