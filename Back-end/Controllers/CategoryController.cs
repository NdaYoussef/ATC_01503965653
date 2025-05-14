using EventManagmentTask.DTOs;
using EventManagmentTask.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventManagmentTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        #region Constructor
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        #endregion

        #region EndPoints



        [HttpGet("Categories")]
        public async Task<IActionResult> GetAllCategories()
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _categoryRepository.GetAllCategories();
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }

        [Authorize(Policy = "Admin")]

        [HttpGet("CategoryById/{id}")]
        public async Task<IActionResult> CategoryById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _categoryRepository.GetCategoryById(id);
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }



        [Authorize(Policy = "Admin and Organizer")]

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _categoryRepository.AddCategoryAsync(categoryDto);
            if (response.IsSucceeded)
                return Ok(response);

            return StatusCode(response.StatusCode, new { response.Message });
        }


        [Authorize(Policy = "Admin and Organizer")]

        [HttpPut("EditCategory/{id}")]
        [Authorize(Policy = "Only Admin")]
        public async Task<IActionResult> EditCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _categoryRepository.UpdateCategoryAsync(id, categoryDto);
            if (response.IsSucceeded)
                return Ok(response);

            return StatusCode(response.StatusCode, new { response.Message });
        }


        [Authorize(Policy = "Admin and Organizer")]

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoryRepository.DeleteCategoryAsync(id);
            if (response.IsSucceeded)
                return Ok(response);

            return StatusCode(response.StatusCode, new { response.Message });
        }

        #endregion

    }
}
