using SariKartAPIV2.Entities;
using SariKartAPIV2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
    This controller performs different functions for user
 */

namespace SariKartAPIV2.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly SariKartContext _db;

        public UserController(SariKartContext db)
        {
            _db = db;
        }

        [HttpPost("login")]
        public Result Login([FromBody] UserLoginDataInput userLoginDataInput)
        {
            var result = new Result();

            try
            {
                if (String.IsNullOrEmpty(userLoginDataInput.Username) || String.IsNullOrEmpty(userLoginDataInput.Password))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                var user = _db.AppUsers.AsNoTracking()
                    .FirstOrDefault(x => x.Username == userLoginDataInput.Username && x.Password == userLoginDataInput.Password);

                if (user != null)
                {
                    result.JsonResultObject = user;
                    result.Message = "User login successful";
                    result.IsSuccess = true;

                    return result;
                }

                result.Message = "Incorrect username or password";
                result.IsSuccess = false;
            }
            catch
            {
                result.Message = "Unable to login user";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("{id:int}")]
        public Result GetUser(int id)
        {
            var result = new Result();

            try
            {
                var user = _db.AppUsers.AsNoTracking()
                    .Include(x => x.UserStores)
                    .FirstOrDefault(x => x.Id == id);

                result.JsonResultObject = user;
                result.Message = "You can get your user account details";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get user account details";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPost("add")]
        public Result InsertUser([FromBody] UserDataInput userDataInput)
        {
            var result = new Result();

            try
            {
                var existing = _db.AppUsers.AsNoTracking()
                    .FirstOrDefault(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id);

                if (existing != null)
                {
                    result.Message = "This user account " + userDataInput.Username + " already exists";
                    result.IsSuccess = false;

                    return result;
                }

                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber) || String.IsNullOrEmpty(userDataInput.Password) || String.IsNullOrEmpty(userDataInput.ConfirmPassword) || String.IsNullOrEmpty(userDataInput.UserStore?.Address))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                if (userDataInput.Password != userDataInput.ConfirmPassword)
                {
                    result.Message = "The new password should match with the confirm password";
                    result.IsSuccess = false;

                    return result;
                }

                var user = new AppUser
                {
                    Username = userDataInput.Username,
                    FirstName = userDataInput.Firstname,
                    LastName = userDataInput.Lastname,
                    Password = userDataInput.Password,
                    ContactNumber = userDataInput.ContactNumber,
                    UserTypeId = 1,
                    EditableContactPerson = userDataInput.Firstname + " " + userDataInput.Lastname,
                    EditableContactNumber = userDataInput.ContactNumber,
                    EditableContactAddress = userDataInput.UserStore!.Address
                };

                _db.AppUsers.Add(user);
                _db.SaveChanges();

                _db.UserStores.Add(new UserStore
                {
                    UserId = user.Id,
                    Address = userDataInput.UserStore.Address
                });

                _db.SaveChanges();

                result.Message = "New user record is successfully registered";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to register your user account";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPut("edit")]
        public Result UpdateUser([FromBody] UserDataInput userDataInput)
        {
            var result = new Result();

            try
            {
                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber) || String.IsNullOrEmpty(userDataInput.UserStore?.Address))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                var user = _db.AppUsers.FirstOrDefault(x => x.Id == userDataInput.Id);
                var userStore = _db.UserStores.FirstOrDefault(x => x.UserId == userDataInput.Id);
                var usernameExists = _db.AppUsers.AsNoTracking()
                    .FirstOrDefault(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id);

                if (user == null)
                {
                    result.Message = "No user data to update";
                    result.IsSuccess = false;

                    return result;
                }

                if (usernameExists != null)
                {
                    result.Message = "This user account " + userDataInput.Username + " already exists";
                    result.IsSuccess = false;

                    return result;
                }

                user.Username = userDataInput.Username;
                user.FirstName = userDataInput.Firstname;
                user.LastName = userDataInput.Lastname;

                if (userStore != null)
                {
                    userStore.Address = userDataInput.UserStore!.Address;
                }

                _db.SaveChanges();

                result.Message = "Current user record is successfully updated";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to update current user account";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPut("editAdminUser")]
        public Result UpdateAdminUser([FromBody] UserDataInput userDataInput)
        {
            var result = new Result();

            try
            {
                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                var user = _db.AppUsers.FirstOrDefault(x => x.Id == userDataInput.Id);
                var usernameExists = _db.AppUsers.AsNoTracking()
                    .FirstOrDefault(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id);

                if (user == null)
                {
                    result.Message = "No user data to update";
                    result.IsSuccess = false;

                    return result;
                }

                if (usernameExists != null)
                {
                    result.Message = "This user account " + userDataInput.Username + " already exists";
                    result.IsSuccess = false;

                    return result;
                }

                user.Username = userDataInput.Username;
                user.FirstName = userDataInput.Firstname;
                user.LastName = userDataInput.Lastname;

                _db.SaveChanges();

                result.Message = "Current user record is successfully updated";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to update current user account";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPut("editContact")]
        public Result UpdateUserContact([FromBody] UserDataInput userDataInput)
        {
            var result = new Result();

            try
            {
                if (String.IsNullOrEmpty(userDataInput.EditableContactPerson) || String.IsNullOrEmpty(userDataInput.EditableContactNumber) || String.IsNullOrEmpty(userDataInput.EditableContactAddress))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                var user = _db.AppUsers.FirstOrDefault(x => x.Id == userDataInput.Id);

                if (user == null)
                {
                    result.Message = "No user data to update";
                    result.IsSuccess = false;

                    return result;
                }

                user.EditableContactPerson = userDataInput.EditableContactPerson;
                user.EditableContactNumber = userDataInput.EditableContactNumber;
                user.EditableContactAddress = userDataInput.EditableContactAddress;

                _db.SaveChanges();

                result.Message = "Current user contact is successfully updated";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to update current user contact";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPut("changePassword")]
        public Result UpdateUserPassword([FromBody] UserDataInput userDataInput)
        {
            var result = new Result();

            try
            {
                if (userDataInput.Password?.Length == 0 || userDataInput.ConfirmPassword?.Length == 0)
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                if (userDataInput.Password != userDataInput.ConfirmPassword)
                {
                    result.Message = "The new password should match with the confirm password";
                    result.IsSuccess = false;

                    return result;
                }

                var user = _db.AppUsers.FirstOrDefault(x => x.Id == userDataInput.Id);

                if (user == null)
                {
                    result.Message = "No user data for password update";
                    result.IsSuccess = false;

                    return result;
                }

                user.Password = userDataInput.Password;

                _db.SaveChanges();

                result.Message = "User password is successfully updated";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to update your password";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("adminUser/{id}/{name}")]
        public Result GetAdminUsers(int id, string name)
        {
            var result = new Result();

            try
            {
                var users = name == "all"
                    ? _db.AppUsers.AsNoTracking().Where(x => x.UserTypeId == 2 && x.Id != id).Include(x => x.UserStores).ToList()
                    : _db.AppUsers.AsNoTracking()
                        .Where(x => x.UserTypeId == 2 && x.Id != id &&
                                    (x.FirstName.Contains(name) || x.LastName.Contains(name) || x.Username.Contains(name)))
                        .ToList();

                result.JsonResultObject = users;
                result.Message = "You can get the admin users";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get the admin users";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("list/riders")]
        public Result GetRidersList()
        {
            var result = new Result();

            try
            {
                var riders = _db.AppUsers.AsNoTracking()
                    .Where(x => x.UserTypeId == 3)
                    .Select(X => new AppUser
                    {
                        Id = X.Id,
                        FirstName = X.FirstName,
                        LastName = X.LastName
                    }).ToList();

                result.JsonResultObject = riders;
                result.Message = "You get all riders list";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get all riders list";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("rider/{name}")]
        public Result GetRiders(string name)
        {
            var result = new Result();

            try
            {
                var users = name == "all"
                    ? _db.AppUsers.AsNoTracking().Where(x => x.UserTypeId == 3).Include(x => x.UserStores).ToList()
                    : _db.AppUsers.AsNoTracking()
                        .Where(x => x.UserTypeId == 3 &&
                                    (x.FirstName.Contains(name) || x.LastName.Contains(name) || x.Username.Contains(name)))
                        .ToList();

                result.JsonResultObject = users;
                result.Message = "You can get the riders";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get the riders";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("cashier/{name}")]
        public Result GetCashiers(string name)
        {
            var result = new Result();

            try
            {
                var users = name == "all"
                    ? _db.AppUsers.AsNoTracking().Where(x => x.UserTypeId == 1003).ToList()
                    : _db.AppUsers.AsNoTracking()
                        .Where(x => x.UserTypeId == 1003 &&
                                    (x.FirstName.Contains(name) || x.LastName.Contains(name) || x.Username.Contains(name)))
                        .ToList();

                result.JsonResultObject = users;
                result.Message = "You can get the cashiers";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get the cashiers";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPost("adminUser/add")]
        public Result AddAdminUser([FromBody] UserDataInput userDataInput)
        {
            return AddUserByType(userDataInput, 2, "Unable to register new user account");
        }

        [HttpPost("rider/add")]
        public Result AddRider([FromBody] UserDataInput userDataInput)
        {
            return AddUserByType(userDataInput, 3, "Unable to register new user account");
        }

        [HttpPost("cashier/add")]
        public Result AddCashier([FromBody] UserDataInput userDataInput)
        {
            return AddUserByType(userDataInput, 1003, "Unable to register new user account");
        }

        private Result AddUserByType(UserDataInput userDataInput, int userTypeId, string failureMessage)
        {
            var result = new Result();

            try
            {
                var existing = _db.AppUsers.AsNoTracking()
                    .FirstOrDefault(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id);

                if (existing != null)
                {
                    result.Message = "This user account " + userDataInput.Username + " already exists";
                    result.IsSuccess = false;

                    return result;
                }

                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber) || String.IsNullOrEmpty(userDataInput.Password) || String.IsNullOrEmpty(userDataInput.ConfirmPassword))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                if (userDataInput.Password != userDataInput.ConfirmPassword)
                {
                    result.Message = "The new password should match with the confirm password";
                    result.IsSuccess = false;

                    return result;
                }

                _db.AppUsers.Add(new AppUser
                {
                    Username = userDataInput.Username,
                    FirstName = userDataInput.Firstname,
                    LastName = userDataInput.Lastname,
                    Password = userDataInput.Password,
                    UserTypeId = userTypeId,
                    ContactNumber = userDataInput.ContactNumber
                });

                _db.SaveChanges();

                result.Message = "New user record is successfully registered";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = failureMessage;
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPut("adminUser/edit")]
        public Result UpdateOtAdminUser([FromBody] UserDataInput userDataInput)
        {
            var result = new Result();

            try
            {
                var user = _db.AppUsers.AsNoTracking()
                    .FirstOrDefault(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id);

                if (user != null)
                {
                    result.Message = "This user account " + userDataInput.Username + " already exists";
                    result.IsSuccess = false;

                    return result;
                }

                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber) || String.IsNullOrEmpty(userDataInput.Password) || String.IsNullOrEmpty(userDataInput.ConfirmPassword))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                if (userDataInput.Password != userDataInput.ConfirmPassword)
                {
                    result.Message = "The new password should match with the confirm password";
                    result.IsSuccess = false;

                    return result;
                }

                var existing = _db.AppUsers.FirstOrDefault(x => x.Id == userDataInput.Id);

                if (existing == null)
                {
                    result.Message = "No user data to update";
                    result.IsSuccess = false;

                    return result;
                }

                existing.Username = userDataInput.Username;
                existing.FirstName = userDataInput.Firstname;
                existing.LastName = userDataInput.Lastname;
                existing.Password = userDataInput.Password;
                existing.ContactNumber = userDataInput.ContactNumber;

                _db.SaveChanges();

                result.Message = "Current user record is successfully updated";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to update current user account";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("adminUser/get/{id:int}")]
        public Result GetAdminUser(int id)
        {
            var result = new Result();

            try
            {
                var user = _db.AppUsers.AsNoTracking().FirstOrDefault(x => x.Id == id);

                result.JsonResultObject = user;
                result.Message = "You can get current user account details";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get current user account details";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpDelete("adminUser/delete/{id}")]
        public Result DeleteAdminUser(int id)
        {
            var result = new Result();

            try
            {
                var adminUser = _db.AppUsers.FirstOrDefault(x => x.Id == id);

                if (adminUser == null)
                {
                    result.Message = "No admin user to delete";
                    result.IsSuccess = false;

                    return result;
                }

                _db.AppUsers.Remove(adminUser);
                _db.SaveChanges();

                result.Message = "Admin user is successfully deleted";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to delete an admin user";
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
