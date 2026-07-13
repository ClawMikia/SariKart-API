using SariKartAPI.Entities;
using SariKartAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

/*
    This controller performs different functions for category
 */

namespace SariKartAPI.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : Controller
    {
        private readonly SariKartContext _db;

        public CategoryController(SariKartContext db)
        {
            _db = db;
        }

        [HttpGet("getCategoryImage/{id}/{filename}")]
        public async Task<IActionResult> GetImage(int id, string fileName)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\Categories\\" + id.ToString() + "\\" + fileName);

            if (System.IO.File.Exists(uploadPath))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(uploadPath);
                return File(fileBytes, "image/png");
            }

            return NotFound("Image not found");
        }

        [HttpGet]
        public Result GetCategories()
        {
            var result = new Result();

            try
            {
                var categories = _db.Categories.AsNoTracking().ToList();

                result.JsonResultObject = categories;
                result.Message = "You get all categories";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get all categories";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("list")]
        public Result GetCategoriesList()
        {
            var result = new Result();

            try
            {
                var categories = _db.Categories.AsNoTracking()
                    .Select(X => new Category
                    {
                        Id = X.Id,
                        Category1 = X.Category1
                    }).ToList();

                result.JsonResultObject = categories;
                result.Message = "You get all categories";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get all categories";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPost("add")]
        public async Task<Result> AddNewCategory(CategoryDataInput categoryDataInput)
        {
            var result = new Result();

            try
            {
                if (categoryDataInput.Category?.Length == 0 || categoryDataInput.Icon == null)
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                var category = new Category
                {
                    Category1 = categoryDataInput.Category!,
                    Icon = categoryDataInput.Icon!.FileName
                };

                _db.Categories.Add(category);
                _db.SaveChanges();

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\Categories\\" + category.Id.ToString());

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, categoryDataInput.Icon.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await categoryDataInput.Icon.CopyToAsync(stream);
                }

                result.Message = "New category added successfully";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to add new category";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPut("edit")]
        public async Task<Result> EditCategory(CategoryDataInput categoryDataInput)
        {
            var result = new Result();

            try
            {
                if (categoryDataInput.Category?.Length == 0 || categoryDataInput.Icon == null)
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                var category = _db.Categories.FirstOrDefault(x => x.Id == categoryDataInput.Id);

                if (category == null)
                {
                    result.Message = "Category not found";
                    result.IsSuccess = false;

                    return result;
                }

                category.Category1 = categoryDataInput.Category!;
                category.Icon = categoryDataInput.Icon!.FileName;

                _db.SaveChanges();

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\Categories\\" + category.Id.ToString());

                Directory.Delete(uploadPath, true);
                Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, categoryDataInput.Icon.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await categoryDataInput.Icon.CopyToAsync(stream);
                }

                result.Message = "Current category updated successfully";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to update current category";
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
