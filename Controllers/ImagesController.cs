using System.Text.Json;
using GeekStore.API.Models.Domains;
using GeekStore.API.Models.DTOs;
using GeekStore.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GeekStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IImageRepository imageRepository, ILogger<ImagesController> logger)
        {
            _imageRepository = imageRepository;
            _logger = logger;
        }
        // POST: api/images/upload
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto requestDto)
        {
            _logger.LogInformation("Ready to upload an image");

            ValidateImage(requestDto);
            if (ModelState.IsValid)
            {
                _logger.LogInformation("File validation succeeded");
                var image = new Image
                {
                    File = requestDto.File,
                    FileExtension = Path.GetExtension(requestDto.File.FileName),
                    FileName = requestDto.FileName,
                    FileSizeInBytes = requestDto.File.Length,
                    FileDescription = requestDto.FileDescription
                };
                // perform repo operation
                await _imageRepository.UploadAsync(image);

                _logger.LogInformation($"Uploaded image: {JsonSerializer.Serialize(image)}");

                return Ok(image);
            }
            return BadRequest(ModelState);
        }
        private void ValidateImage(ImageUploadRequestDto request)
        {
            var acceptedExtensions = new string[] { ".jpg", ".png", ".jpeg" };

            if (!acceptedExtensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension");
            }
            if(request.File.Length > 10 * 1024 * 1024)
            {
                ModelState.AddModelError("file size", "File size limit (10 MiB) exceeded");
            }
        }
    }
}
