using SariKartAPIV2.Entities;
using SariKartAPIV2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
    This controller performs different functions for vehicle
 */

namespace SariKartAPIV2.Controllers
{
    [ApiController]
    [Route("api/vehicle")]
    public class VehicleController : Controller
    {
        private readonly SariKartContext _db;

        public VehicleController(SariKartContext db)
        {
            _db = db;
        }

        [HttpGet("list")]
        public Result GetVehiclesList()
        {
            var result = new Result();

            try
            {
                var vehicles = _db.Vehicles.AsNoTracking()
                    .Select(X => new Vehicle
                    {
                        Id = X.Id,
                        Type = X.Type
                    }).ToList();

                result.JsonResultObject = vehicles;
                result.Message = "You get all vehicles list";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get all vehicles list";
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
