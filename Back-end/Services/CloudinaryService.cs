﻿using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using EventManagmentTask.Repositories;
using Microsoft.Extensions.Logging;

namespace EventManagmentTask.Services
{
    public class CloudinaryService : ICloudinaryRepository
    {
        #region Dependencies
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger; 
        #endregion

        #region Constructor
        public CloudinaryService(Cloudinary cloudinary, ILogger<CloudinaryService> logger)
        {
            _cloudinary = cloudinary;
            _logger = logger;
        }

        #endregion

        #region Services implementation
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentNullException("No file provided");

            var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                throw new Exception("Unsupported file type. Only JPEG, PNG, and GIF are allowed.");

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Width(500).Height(500).Crop("fill")
            };

            //var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            //if (uploadResult.Error != null)
            //    throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
            {
                _logger.LogError($"Cloudinary error: {uploadResult.Error.Message}, StatusCode: {uploadResult.StatusCode}");
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}, StatusCode: {uploadResult.StatusCode}");
            }

            _logger.LogInformation($"File uploaded successfully. Public URL: {uploadResult.SecureUrl}");
            return uploadResult.SecureUrl.AbsoluteUri;
        } 
        #endregion
    }
}
