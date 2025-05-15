using EventManagmentTask.Repositories;
using EventManagmentTask.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventManagmentTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly ICloudinaryRepository _cloudinaryService;
        private readonly ILogger<MediaController> _logger;
        public MediaController(ICloudinaryRepository cloudinaryService, ILogger<MediaController> logger)
        {
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        { 
            try
            {
                _logger.LogInformation($"Uploading file: {file.FileName}, Size: {file.Length} bytes");
                var imageUrl = await _cloudinaryService.UploadImageAsync(file);

                // Log successful upload
                _logger.LogInformation($"File uploaded successfully: {imageUrl}");
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Error occurred during file upload");
                return StatusCode(500, new
                {
                    message = "An error occurred while uploading the file",
                    details = ex.Message
                });
            }
        }
    }
}
