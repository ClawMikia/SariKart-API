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
                if (productDataInput.Product.Length == 0 || productDataInput.CategoryId <= 0 || productDataInput.Price <= 0 || productDataInput.Picture.Length == 0 || productDataInput.Unit.Length == 0 || productDataInput.Stock <= 0)
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
                    product.Stock = productDataInput.Stock;

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

        [HttpPut("edit")]
        public Result EditProduct([FromBody] ProductDataInput productDataInput)
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
                    var product = db.Products.Where(x => x.Id == productDataInput.Id).FirstOrDefault();

                    product.Product1 = productDataInput.Product;
                    product.CategoryId = productDataInput.CategoryId;
                    product.Price = productDataInput.Price;
                    product.Picture = productDataInput.Picture;
                    product.Unit = productDataInput.Unit;
                    product.Stock = productDataInput.Stock;

                    db.SaveChanges();
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
        public Result getProductsHistoryCount(int year, int month)
        {
            var result = new Result();

            try
            {
                var products = new List<ProductHistoryDataOutput>();

                using (var db = new JulyGrocerContext())
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
        public Result getBranchProductsHistoryCount(int branchId, int year, int month)
        {
            var result = new Result();

            try
            {
                using (var db = new JulyGrocerContext())
                {
                    //var productHistoryBranchList = db.Deliveries
                    //    .Where(d => d.StoreId == branchId)
                    //    .Include(o => o.Order)
                    //    .ThenInclude(o => o.OrderLines)
                    //    .ThenInclude(p => p.Product)
                    //    .Where(o => o.Order.CreateDate.Month == month && o.Order.CreateDate.Year == year)
                    //    .Select(p => new ProductHistoryDataOutput
                    //    {
                    //        ProductName = p.Order.OrderLines.Select(x => x.Product.Product1).First(),
                    //        Quantity = p.Order.OrderLines.Select(x => x.Quantity).First()
                    //    })
                    //    .GroupBy(p => p.ProductName)
                    //    .ToList();

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
