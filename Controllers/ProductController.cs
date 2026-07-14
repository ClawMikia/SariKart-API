using SariKartAPIV2.Entities;
using SariKartAPIV2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
    This controller performs different functions for product
 */

namespace SariKartAPIV2.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : Controller
    {
        private readonly SariKartContext _db;

        public ProductController(SariKartContext db)
        {
            _db = db;
        }

        [HttpGet("getProductImage/{id}/{filename}")]
        public async Task<IActionResult> GetImage(int id, string fileName)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\Products\\" + id.ToString() + "\\" + fileName);

            if (System.IO.File.Exists(uploadPath))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(uploadPath);
                return File(fileBytes, "image/png");
            }

            return NotFound("Image not found");
        }

        [HttpGet("{categoryId:int}/{productKeyword}")]
        public Result GetProducts(int categoryId, string productKeyword)
        {
            var result = new Result();

            try
            {
                var query = _db.Products.AsNoTracking().AsQueryable();

                if (categoryId > 0)
                {
                    query = query.Where(x => x.CategoryId == categoryId);
                }

                if (!string.IsNullOrEmpty(productKeyword) && productKeyword != "all")
                {
                    query = query.Where(x => x.Product1.Contains(productKeyword));
                }

                result.JsonResultObject = query.Select(x => new Product
                {
                    Id = x.Id,
                    Product1 = x.Product1,
                    CategoryId = x.CategoryId,
                    Price = x.Price,
                    Unit = x.Unit,
                    Stock = x.Stock,
                    Picture = string.IsNullOrEmpty(x.Picture) ? null : $"{Request.Scheme}://{Request.Host}/api/product/getProductImage/{x.Id}/{x.Picture}",
                    Category = x.Category
                }).ToList();
                result.Message = "You get all products";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get all products";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("{id:int}")]
        public Result GetProduct(int id)
        {
            var result = new Result();

            try
            {
                var product = _db.Products.AsNoTracking().FirstOrDefault(x => x.Id == id);

                if (product != null && !string.IsNullOrEmpty(product.Picture))
                {
                    product.Picture = $"{Request.Scheme}://{Request.Host}/api/product/getProductImage/{product.Id}/{product.Picture}";
                }

                result.JsonResultObject = product;
                result.Message = "You can get product details";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get product details";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPost("add")]
        public async Task<Result> AddNewProduct(IFormFile file, [FromForm] string product, [FromForm] int categoryId, [FromForm] decimal price, [FromForm] string unit, [FromForm] int stock)
        {
            product = product.Replace("\"", "");
            unit = unit.Replace("\"", "");

            var result = new Result();

            try
            {
                if (product.Length == 0 || categoryId <= 0 || price <= 0 || file == null || unit.Length == 0 || stock <= 0)
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                var currentProduct = new Product
                {
                    Product1 = product,
                    CategoryId = categoryId,
                    Price = price,
                    Picture = file.FileName,
                    Unit = unit,
                    Stock = stock
                };

                _db.Products.Add(currentProduct);
                _db.SaveChanges();

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\Products\\" + currentProduct.Id.ToString());

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                result.Message = "New product added successfully";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to add new product";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPut("edit")]
        public async Task<Result> EditProduct(IFormFile? file, [FromForm] int id, [FromForm] string product, [FromForm] int categoryId, [FromForm] decimal price, [FromForm] string unit, [FromForm] int stock)
        {
            product = product.Replace("\"", "");
            unit = unit.Replace("\"", "");

            var result = new Result();

            try
            {
                if (product.Length == 0 || categoryId <= 0 || price <= 0 || unit.Length == 0 || stock <= 0)
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                var currentProduct = _db.Products.FirstOrDefault(x => x.Id == id);

                if (currentProduct == null)
                {
                    result.Message = "Product not found";
                    result.IsSuccess = false;

                    return result;
                }

                currentProduct.Product1 = product;
                currentProduct.CategoryId = categoryId;
                currentProduct.Price = price;
                currentProduct.Unit = unit;
                currentProduct.Stock = stock;

                if (file != null)
                {
                    currentProduct.Picture = file.FileName;
                }

                _db.SaveChanges();

                if (file != null)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\Products\\" + id.ToString());

                    Directory.Delete(uploadPath, true);
                    Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                result.Message = "Current product updated successfully";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to update current product";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("productHistory/{year}/{month}")]
        public Result getProductsHistoryCount(int year, int month)
        {
            var result = new Result();

            try
            {
                var products = _db.OrderLines.AsNoTracking()
                    .Where(o => o.Order.CreateDate.Month == month && o.Order.CreateDate.Year == year)
                    .GroupBy(p => p.Product.Product1)
                    .Select(g => new ProductHistoryDataOutput
                    {
                        ProductName = g.Key,
                        Quantity = g.Sum(x => x.Quantity)
                    })
                    .OrderByDescending(p => p.Quantity)
                    .ToList();

                result.JsonResultObject = products;
                result.Message = "You get all your sellable products history for this date";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get all your sellable products history for this date";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("productHistory/{branchId}/{year}/{month}")]
        public Result getBranchProductsHistoryCount(int branchId, int year, int month)
        {
            var result = new Result();

            try
            {
                var productHistoryBranchList = _db.Deliveries.AsNoTracking()
                    .Where(d => d.StoreId == branchId
                                && d.Order.CreateDate.Month == month
                                && d.Order.CreateDate.Year == year)
                    .SelectMany(d => d.Order.OrderLines.Select(ol => new ProductHistoryDataOutput
                    {
                        ProductName = ol.Product.Product1,
                        Quantity = ol.Quantity
                    }))
                    .ToList()
                    .GroupBy(x => x.ProductName)
                    .Select(g => new ProductHistoryDataOutput
                    {
                        ProductName = g.Key,
                        Quantity = g.Sum(x => x.Quantity)
                    })
                    .ToList();

                result.JsonResultObject = productHistoryBranchList;
                result.Message = "You get all your sellable products history for this date for this branch";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get all your sellable products history for this date for this branch";
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
