using EventManagmentTask.Data;
using EventManagmentTask.DTOs;
using EventManagmentTask.Interfaces;
using EventManagmentTask.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace EventManagmentTask.Services
{
    public class CategoryService : ICategoryRepository
    {
        private readonly EventManagmentDbContext _context; 

        public CategoryService(EventManagmentDbContext context) : base()
        {
          _context = context;
        }

        public async Task<ResponseDto> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            if (categories.Count == 0)
            {
                return new ResponseDto
                {
                    Message = "No categories found.",
                    IsSucceeded = false,
                    StatusCode = 404
                };
            }

            var categoryDtos = categories.Adapt<List<CategoryDto>>();

            return new ResponseDto
            {
                Message = "Categories retrieved successfully.",
                IsSucceeded = true,
                StatusCode = 200,
                Data = categoryDtos
            };
        }

        public async Task<ResponseDto> GetCategoryById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return new ResponseDto
                {
                    Message = "Category not found.",
                    IsSucceeded = false,
                    StatusCode = 404
                };
            }

            var categoryDto = category.Adapt<CategoryDto>();

            return new ResponseDto
            {
                Message = "Category retrieved successfully.",
                IsSucceeded = true,
                StatusCode = 200,
                Data = categoryDto
            };
        }
    

       

        public async Task<ResponseDto> AddCategoryAsync(CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return new ResponseDto
                {
                    Message = "Category data is required.",
                    IsSucceeded = false,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            var exists = await _context.Categories.AnyAsync(c => c.Name == categoryDto.Name);
            if (exists)
            {
                return new ResponseDto
                {
                    Message = "Category already exists!",
                    IsSucceeded = false,
                    StatusCode = 409
                };
            }

            var category = categoryDto.Adapt<Category>();
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var resultDto = category.Adapt<CategoryDto>();

            return new ResponseDto
            {
                Message = "Category added successfully!",
                IsSucceeded = true,
                StatusCode = 201,
                Data = resultDto
            };
        }

        public async Task<ResponseDto> UpdateCategoryAsync(int id, CategoryDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return new ResponseDto
                {
                    Message = "Category not found.",
                    IsSucceeded = false,
                    StatusCode = 404
                };
            }

            bool exists = await _context.Categories.AnyAsync(c => c.Name == categoryDto.Name && c.Id != id);
            if (exists)
            {
                return new ResponseDto
                {
                    Message = "Category name already in use.",
                    IsSucceeded = false,
                    StatusCode = 409
                };
            }

            categoryDto.Adapt(category);

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            var updatedDto = category.Adapt<CategoryDto>();

            return new ResponseDto
            {
                Message = "Category updated successfully.",
                IsSucceeded = true,
                StatusCode = 200,
                Data = updatedDto
            };
        }

        public async Task<ResponseDto> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return new ResponseDto
                {
                    Message = "Category not found.",
                    IsSucceeded = false,
                    StatusCode = 404
                };
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            var deletedDto = category.Adapt<CategoryDto>();

            return new ResponseDto
            {
                Message = "Category deleted successfully.",
                IsSucceeded = true,
                StatusCode = 200,
                Data = deletedDto
            };
        }
    }
}
