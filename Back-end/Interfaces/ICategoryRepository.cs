using EventManagmentTask.DTOs;

namespace EventManagmentTask.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ResponseDto> GetAllCategories();
        Task<ResponseDto> GetCategoryById(int id);
        Task<ResponseDto> AddCategoryAsync(CategoryDto categoryDto);

        Task<ResponseDto> UpdateCategoryAsync(int id, CategoryDto categoryDto);
        Task<ResponseDto> DeleteCategoryAsync(int id);
    }
}
