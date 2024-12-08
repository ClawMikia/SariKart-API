using JulyGrocerAPI.Entities;
using JulyGrocerAPI.Models;
using Microsoft.AspNetCore.Mvc;

/*
    This controller performs different functions for vehicle 
*/

namespace JulyGrocerAPI.Controllers
{
    [ApiController]
    [Route("api/vehicle")]
    public class VehicleController : Controller
    {
        [HttpGet("list")]
        public Result GetVehiclesList() // This route gets vehicles list
        {
            var result = new Result();

            try
            {
                var vehicles = new List<Vehicle>();

                using (var db = new JulyGrocerContext())
                {
                    vehicles = db.Vehicles.Select(X => new Vehicle
                    {
                        Id = X.Id,
                        Type = X.Type
                    }).ToList();

                    result.JsonResultObject = vehicles;
                    result.Message = "You get all vehicles list";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get all vehicles list";
                result.IsSuccess = false;

                return result;
            }

        }
    }
}
