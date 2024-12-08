using JulyGrocerAPI.Entities;
using JulyGrocerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

/*
    This controller performs different functions for store branch 
*/

namespace JulyGrocerAPI.Controllers
{
    [ApiController]
    [Route("api/branch")]
    public class StoreBranchController : Controller
    {
        [HttpGet]
        public Result GetBranches() // This route gets all store branches
        {
            var result = new Result();

            try
            {
                var branches = new List<StoreBranch>();

                using (var db = new JulyGrocerContext())
                {
                    branches = db.StoreBranches.ToList();
                }

                result.JsonResultObject = branches;
                result.Message = "You can get your branches";
                result.IsSuccess = true;

                return result;
            }

            catch
            {
                result.Message = "Unable to get your branches";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpGet("list")]
        public Result GetBranchList() // This route gets store branches list
        {
            var result = new Result();

            try
            {
                var branches = new List<StoreBranch>();

                using (var db = new JulyGrocerContext())
                {
                    branches = db.StoreBranches.Select(X => new StoreBranch
                    {
                        Id = X.Id,
                        Branch = X.Branch
                    }).ToList();

                    result.JsonResultObject = branches;
                    result.Message = "You get all branch list";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get all branch list";
                result.IsSuccess = false;

                return result;
            }

        }

        [HttpGet("{id}")]
        public Result GetBranch(int id) // This route gets a specific branch
        {
            var result = new Result();

            try
            {
                var branch = new StoreBranch();

                using (var db = new JulyGrocerContext())
                {
                    branch = db.StoreBranches.Where(x => x.Id == id).FirstOrDefault();
                }

                result.JsonResultObject = branch;
                result.Message = "You can get your current branch";
                result.IsSuccess = true;

                return result;
            }

            catch
            {
                result.Message = "Unable to get your current branch";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPost("add")]
        public Result InsertNewBranch([FromBody] StoreBranchDataInput storeBranchDataInput) // This route adds new store branch record
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(storeBranchDataInput.Branch) || String.IsNullOrEmpty(storeBranchDataInput.Street) || String.IsNullOrEmpty(storeBranchDataInput.City) || String.IsNullOrEmpty(storeBranchDataInput.Province))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                else
                {
                    using (var db = new JulyGrocerContext())
                    {
                        var storeBranch = new StoreBranch();

                        storeBranch.Branch = storeBranchDataInput.Branch;
                        storeBranch.Street = storeBranchDataInput.Street;
                        storeBranch.City = storeBranchDataInput.City;
                        storeBranch.Province = storeBranchDataInput.Province;

                        db.Add(storeBranch);
                        db.SaveChanges();

                        result.Message = "New store branch is successfully registered";
                        result.IsSuccess = true;

                        return result;
                    }
                }
            }

            catch
            {
                result.Message = "Unable to register your new store branch";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPut("edit")]
        public Result UpdateNewBranch([FromBody] StoreBranchDataInput storeBranchDataInput) // This route updates current store branch
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(storeBranchDataInput.Branch) || String.IsNullOrEmpty(storeBranchDataInput.Street) || String.IsNullOrEmpty(storeBranchDataInput.City) || String.IsNullOrEmpty(storeBranchDataInput.Province))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                else
                {
                    // Check if the account has already existed. if it is, the user will be asked to input different username
                    using (var db = new JulyGrocerContext())
                    {
                        var storeBranch = db.StoreBranches.Where(x => x.Id == storeBranchDataInput.Id).FirstOrDefault();
                        
                        // Check if the specific user exists
                        if (storeBranch == null)
                        {
                            result.Message = "No branch data to update";
                            result.IsSuccess = false;

                            return result;
                        }

                        else
                        {
                            storeBranch.Branch = storeBranchDataInput.Branch;
                            storeBranch.Street = storeBranchDataInput.Street;
                            storeBranch.City = storeBranchDataInput.City;
                            storeBranch.Province = storeBranchDataInput.Province;

                            db.SaveChanges();

                            result.Message = "Current store branch is successfully updated";
                            result.IsSuccess = true;

                            return result;
                        }
                    }
                }
            }
               
            catch
            {
                result.Message = "Unable to update current store branch";
                result.IsSuccess = false;

                return result;
            }
        }

        // The user account will be deleted
        [HttpDelete("delete/{id}")]
        public Result DeleteBranch(int id) // This route deletes current store branch
        {
            var result = new Result();

            try
            {
                using (var db = new JulyGrocerContext())
                {
                    var storeBranch = db.StoreBranches.Where(x => x.Id == id).FirstOrDefault();

                    // Check if the specific user exists
                    if (storeBranch == null)
                    {
                        result.Message = "No store branch to delete";
                        result.IsSuccess = false;

                        return result;
                    }

                    else
                    {
                        db.StoreBranches.Remove(storeBranch);
                        db.SaveChanges();

                        result.Message = "Store branch is successfully deleted";
                        result.IsSuccess = true;

                        return result;
                    }
                }
            }

            catch
            {
                result.Message = "Unable to delete your store branch";
                result.IsSuccess = false;

                return result;
            }
        }
    }
}
