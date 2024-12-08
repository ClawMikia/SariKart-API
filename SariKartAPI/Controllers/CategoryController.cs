using SariKartAPI.Entities;
using SariKartAPI.Models;
using Microsoft.AspNetCore.Mvc;

/*
    This controller performs different functions for category 
*/

namespace SariKartAPI.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : Controller
    {
        [HttpGet]
        public Result GetCategories() // This route gets all the categories
        {
            var result = new Result();

            try
            {
                var categories = new List<Category>();

                using (var db = new SariKartContext())
                {
                    categories = db.Categories.ToList();

                    result.JsonResultObject = categories;
                    result.Message = "You get all categories";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get all categories";
                result.IsSuccess = false;

                return result;
            }

        }

        [HttpGet("list")]
        public Result GetCategoriesList() // This route gets all the categories list
        {
            var result = new Result();

            try
            {
                var categories = new List<Category>();

                using (var db = new SariKartContext())
                {
                    categories = db.Categories.Select(X => new Category
                    {
                        Id = X.Id,
                        Category1 = X.Category1
                    }).ToList();

                    result.JsonResultObject = categories;
                    result.Message = "You get all categories";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get all categories";
                result.IsSuccess = false;

                return result;
            }

        }

        [HttpPost("add")]
        public Result AddNewCategory([FromBody] CategoryDataInput categoryDataInput) // This route allows users to add new category
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (categoryDataInput.Category.Length == 0 || categoryDataInput.Icon.Length == 0)
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                // If all fields are entered, add this new category to the list
                using (var db = new SariKartContext())
                {
                    Category category = new Category();

                    category.Category1 = categoryDataInput.Category;
                    category.Icon = categoryDataInput.Icon;

                    db.Add(category);
                    db.SaveChanges();
                }

                result.Message = "New category added successfully";
                result.IsSuccess = true;

                return result;
            }

            catch
            {
                result.Message = "Unable to add new category";
                result.IsSuccess = false;

                return result;
            }
        }
    }
}
