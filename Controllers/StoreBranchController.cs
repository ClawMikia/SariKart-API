using SariKartAPI.Entities;
using SariKartAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
    This controller performs different functions for store branch
 */

namespace SariKartAPI.Controllers
{
    [ApiController]
    [Route("api/branch")]
    public class StoreBranchController : Controller
    {
        private readonly SariKartContext _db;

        public StoreBranchController(SariKartContext db)
        {
            _db = db;
        }

        [HttpGet]
        public Result GetBranches()
        {
            var result = new Result();

            try
            {
                var branches = _db.StoreBranches.AsNoTracking().ToList();

                result.JsonResultObject = branches;
                result.Message = "You can get your branches";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get your branches";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("list")]
        public Result GetBranchList()
        {
            var result = new Result();

            try
            {
                var branches = _db.StoreBranches.AsNoTracking()
                    .Select(X => new StoreBranch
                    {
                        Id = X.Id,
                        Branch = X.Branch
                    }).ToList();

                result.JsonResultObject = branches;
                result.Message = "You get all branch list";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get all branch list";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("{id}")]
        public Result GetBranch(int id)
        {
            var result = new Result();

            try
            {
                var branch = _db.StoreBranches.AsNoTracking().FirstOrDefault(x => x.Id == id);

                result.JsonResultObject = branch;
                result.Message = "You can get your current branch";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get your current branch";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPost("add")]
        public Result InsertNewBranch([FromBody] StoreBranchDataInput storeBranchDataInput)
        {
            var result = new Result();

            try
            {
                if (String.IsNullOrEmpty(storeBranchDataInput.Branch) || String.IsNullOrEmpty(storeBranchDataInput.Street) || String.IsNullOrEmpty(storeBranchDataInput.City) || String.IsNullOrEmpty(storeBranchDataInput.Province))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                _db.StoreBranches.Add(new StoreBranch
                {
                    Branch = storeBranchDataInput.Branch,
                    Street = storeBranchDataInput.Street,
                    City = storeBranchDataInput.City,
                    Province = storeBranchDataInput.Province
                });

                _db.SaveChanges();

                result.Message = "New store branch is successfully registered";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to register your new store branch";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPut("edit")]
        public Result UpdateNewBranch([FromBody] StoreBranchDataInput storeBranchDataInput)
        {
            var result = new Result();

            try
            {
                if (String.IsNullOrEmpty(storeBranchDataInput.Branch) || String.IsNullOrEmpty(storeBranchDataInput.Street) || String.IsNullOrEmpty(storeBranchDataInput.City) || String.IsNullOrEmpty(storeBranchDataInput.Province))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                var storeBranch = _db.StoreBranches.FirstOrDefault(x => x.Id == storeBranchDataInput.Id);

                if (storeBranch == null)
                {
                    result.Message = "No branch data to update";
                    result.IsSuccess = false;

                    return result;
                }

                storeBranch.Branch = storeBranchDataInput.Branch;
                storeBranch.Street = storeBranchDataInput.Street;
                storeBranch.City = storeBranchDataInput.City;
                storeBranch.Province = storeBranchDataInput.Province;

                _db.SaveChanges();

                result.Message = "Current store branch is successfully updated";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to update current store branch";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpDelete("delete/{id}")]
        public Result DeleteBranch(int id)
        {
            var result = new Result();

            try
            {
                var storeBranch = _db.StoreBranches.FirstOrDefault(x => x.Id == id);

                if (storeBranch == null)
                {
                    result.Message = "No store branch to delete";
                    result.IsSuccess = false;

                    return result;
                }

                _db.StoreBranches.Remove(storeBranch);
                _db.SaveChanges();

                result.Message = "Store branch is successfully deleted";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to delete your store branch";
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
