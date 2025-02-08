using SariKartAPI.Entities;
using SariKartAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
    This controller performs different functions for product 
*/

namespace SariKartAPI.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : Controller
    {
        [HttpGet("getProductImage/{id}/{filename}")]
        public async Task<IActionResult> GetImage(int id, string fileName)
        {
            // Define the path where the image will be stored
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\Products\\" + id.ToString() + "\\" + fileName);

            if (System.IO.File.Exists(uploadPath))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(uploadPath);
                var outputFile = File(fileBytes, "image/png");
                return outputFile;
            }

            else
            {
                return NotFound("Image not found");
            }
        }

        [HttpGet("{categoryId}/{productKeyword}")]
        public Result GetProducts(int categoryId, string productKeyword) // This route gets products by search filter
        {
            var result = new Result();

            try
            {
                var products = new List<Product>();

                using (var db = new SariKartContext())
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
        public Result GetProduct(int id) // This route gets specific product
        {
            var result = new Result();

            try
            {
                var product = new Product();

                using (var db = new SariKartContext())
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
        public async Task<Result> AddNewProduct(IFormFile file, [FromForm] string product, [FromForm] int categoryId, [FromForm] decimal price, [FromForm] string unit, [FromForm] int stock) // This route add new product record
        {
            product = product.Replace("\"", "");
            unit = unit.Replace("\"", "");

            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (product.Length == 0 || categoryId <= 0 || price <= 0 || file == null || unit.Length == 0 || stock <= 0)
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                // If all fields are entered, add this new product to the list
                using (var db = new SariKartContext())
                {
                    Product currentProduct = new Product();

                    currentProduct.Product1 = product;
                    currentProduct.CategoryId = categoryId;
                    currentProduct.Price = price;
                    currentProduct.Picture = file.FileName;
                    currentProduct.Unit = unit;
                    currentProduct.Stock = stock;

                    db.Add(currentProduct);
                    db.SaveChanges();

                    // Define the path where the image will be stored
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\Products\\" + currentProduct.Id.ToString());

                    // Make folder of a category if not exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Get the file's full patht
                    var filePath = Path.Combine(uploadPath, file.FileName);

                    // Save the uploaded file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                result.Message = "New product added successfully";
                result.IsSuccess = true;

                return result;
            }

            catch(Exception e)
            {
                result.Message = "Unable to add new product";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPut("edit")]
        public async Task<Result> EditProduct(IFormFile? file, [FromForm] int id, [FromForm] string product, [FromForm] int categoryId, [FromForm] decimal price, [FromForm] string unit, [FromForm] int stock) // This route updates current product
        {
            product = product.Replace("\"", "");
            unit = unit.Replace("\"", "");

            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (product.Length == 0 || categoryId <= 0 || price <= 0 || unit.Length == 0 || stock <= 0)
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                // If all fields are entered, add this new product to the list
                using (var db = new SariKartContext())
                {
                    var currentProduct = db.Products.Where(x => x.Id == id).FirstOrDefault();

                    currentProduct.Product1 = product;
                    currentProduct.CategoryId = categoryId;
                    currentProduct.Price = price;
                    currentProduct.Unit = unit;
                    currentProduct.Stock = stock;

                    if (file != null)
                    {
                        currentProduct.Picture = file.FileName;
                    }

                    db.SaveChanges();

                    // If there are changes to the product file image
                    if (file != null)
                    {
                        // Define the path where the image will be stored
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\Products\\" + id.ToString());

                        Directory.Delete(uploadPath, true); // Delete existing folder
                        Directory.CreateDirectory(uploadPath); // Create new one as replacement

                        // Get the file's full path
                        var filePath = Path.Combine(uploadPath, file.FileName);

                        // Save the uploaded file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                }

                result.Message = "Current product updated successfully";
                result.IsSuccess = true;

                return result;
            }

            catch
            {
                result.Message = "Unable to update current product";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpGet("productHistory/{year}/{month}")]
        public Result getProductsHistoryCount(int year, int month) // This route gets every product with no. of times purchased for each
        {
            var result = new Result();

            try
            {
                var products = new List<ProductHistoryDataOutput>();

                using (var db = new SariKartContext())
                {
                    products = db.OrderLines
                        .Include(o => o.Order)
                        .Include(p => p.Product)
                        .Where(o => o.Order.CreateDate.Month == month && o.Order.CreateDate.Year == year)
                        .Select(p => new ProductHistoryDataOutput
                        {
                            ProductName = p.Product.Product1,
                            Quantity = p.Quantity
                        })
                        .GroupBy(p => p.ProductName)
                        .Select(p => new ProductHistoryDataOutput
                        {
                            ProductName = p.Key,
                            Quantity = p.Sum(x => x.Quantity )
                        })
                        .OrderByDescending(p => p.Quantity)
                        .ToList();

                    result.JsonResultObject = products;
                    result.Message = "You get all your sellable products history for this date";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get all your sellable products history for this date";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpGet("productHistory/{branchId}/{year}/{month}")]
        public Result getBranchProductsHistoryCount(int branchId, int year, int month) // This route gets every branch with no. of times purchased for each product
        {
            var result = new Result();

            try
            {
                using (var db = new SariKartContext())
                {
                    var query = from d in db.Deliveries
                                join o in db.ShopOrders on d.OrderId equals o.Id
                                join ol in db.OrderLines on o.Id equals ol.OrderId
                                join p in db.Products on ol.ProductId equals p.Id
                                where o.CreateDate.Month == month
                                where o.CreateDate.Year == year
                                where d.StoreId == branchId
                                select new ProductHistoryDataOutput
                                {
                                    ProductName = p.Product1,
                                    Quantity = ol.Quantity
                                };

                    var productHistoryBranchList = new List<ProductHistoryDataOutput>();

                    foreach (var item in query)
                    {
                        productHistoryBranchList.Add(item);
                    }

                    productHistoryBranchList = productHistoryBranchList
                        .GroupBy(x => new {x.ProductName})
                        .Select(p => new ProductHistoryDataOutput
                        {
                            ProductName = p.Key.ProductName,
                            Quantity = p.Sum(x => x.Quantity)
                        }).ToList();

                    result.JsonResultObject = productHistoryBranchList;
                    result.Message = "You get all your sellable products history for this date for this branch";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;

                return result;
            }
        }
    }
}
