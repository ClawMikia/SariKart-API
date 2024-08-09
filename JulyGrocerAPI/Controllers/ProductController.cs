using JulyGrocerAPI.Entities;
using JulyGrocerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JulyGrocerAPI.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : Controller
    {
        [HttpGet("{categoryId}/{productKeyword}")]
        public Result GetProducts(int categoryId, string productKeyword)
        {
            var result = new Result();

            try
            {
                var products = new List<Product>();

                using (var db = new JulyGrocerContext())
                {
                    if (categoryId > 0)
                    {

                        if (productKeyword == "all")
                        {
                            products = db.Products.Where(x => x.CategoryId == categoryId).ToList();
                        }

                        else
                        {
                            products = db.Products.Where(x => x.CategoryId == categoryId && x.Product1.Contains(productKeyword)).ToList();
                        }
                    }
                    
                    else 
                    {
                        if (productKeyword == "all")
                        {
                            products = db.Products.ToList();
                        }

                        else
                        {
                            products = db.Products.Where(x => x.Product1.Contains(productKeyword)).ToList();
                        }
                    }

                    result.JsonResultObject = products;
                    result.Message = "You get all products";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get all products";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpGet("{id}")]
        public Result GetProduct(int id)
        {
            var result = new Result();

            try
            {
                var product = new Product();

                using (var db = new JulyGrocerContext())
                {
                    product = db.Products.Where(x => x.Id == id).FirstOrDefault();
                }

                result.JsonResultObject = product;
                result.Message = "You can get product details";
                result.IsSuccess = true;

                return result;
            }

            catch
            {
                result.Message = "Unable to get product details";
                result.IsSuccess = false;

                return result;
            }

        }

        [HttpPost("add")]
        public Result AddNewProduct([FromBody] ProductDataInput productDataInput)
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (productDataInput.Product.Length == 0 || productDataInput.CategoryId <= 0 || productDataInput.Price <= 0 || productDataInput.Picture.Length == 0 || productDataInput.Unit.Length == 0)
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                // If all fields are entered, add this new product to the list
                using (var db = new JulyGrocerContext())
                {
                    Product product = new Product();

                    product.Product1 = productDataInput.Product;
                    product.CategoryId = productDataInput.CategoryId;
                    product.Price = productDataInput.Price;
                    product.Picture = productDataInput.Picture;
                    product.Unit = productDataInput.Unit;

                    db.Add(product);
                    db.SaveChanges();
                }

                result.Message = "New product added successfully";
                result.IsSuccess = true;

                return result;
            }

            catch
            {
                result.Message = "Unable to add new product";
                result.IsSuccess = false;

                return result;
            }
        }
    }
}
