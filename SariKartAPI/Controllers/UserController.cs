using SariKartAPI.Entities;
using SariKartAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
    This controller performs different functions for user 
*/

namespace SariKartAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        [HttpPost("login")]
        public Result Login([FromBody] UserLoginDataInput userLoginDataInput) // API Controller Method for user login. If no data found, the input username and password in incorrect
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(userLoginDataInput.Username) || String.IsNullOrEmpty(userLoginDataInput.Password))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                var user = new AppUser();

                using (var db = new SariKartContext())
                {
                    user = db.AppUsers.Where(x => x.Username == userLoginDataInput.Username && x.Password == userLoginDataInput.Password).FirstOrDefault();
                }

                // Check if the specific user exists
                if (user != null)
                {
                    result.JsonResultObject = user;
                    result.Message = "User login successful";
                    result.IsSuccess = true;

                    return result;
                }

                else
                {
                    result.Message = "Incorrect username or password";
                    result.IsSuccess = false;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to login user";
                result.IsSuccess = false;

                return result;
            }

        }

        [HttpGet("{id}")]
        public Result GetUser(int id) // This route gets a specific user
        {
            var result = new Result();

            try
            {
                var user = new AppUser();

                using (var db = new SariKartContext())
                {
                    user = db.AppUsers.Where(x => x.Id == id).Include(x => x.UserStores).FirstOrDefault();
                }

                result.JsonResultObject = user;
                result.Message = "You can get your user account details";
                result.IsSuccess = true;

                return result;
            }

            catch
            {
                result.Message = "Unable to get user account details";
                result.IsSuccess = false;

                return result;
            }

        }

        [HttpPost("add")]
        public Result InsertUser([FromBody] UserDataInput userDataInput) // User sign up registration if the user has no account yet
        {
            var result = new Result();

            try
            {
                // Check if the account has already existed. if it is, the user will be asked to input different username
                using (var db = new SariKartContext())
                {
                    var user = db.AppUsers.Where(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id).FirstOrDefault();

                    // Check if the specific user exists
                    if (user != null)
                    {
                        result.Message = "This user account " + userDataInput.Username + " already exists";
                        result.IsSuccess = false;

                        return result;
                    }
                }

                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber) || String.IsNullOrEmpty(userDataInput.Password) || String.IsNullOrEmpty(userDataInput.ConfirmPassword) || String.IsNullOrEmpty(userDataInput.UserStore.Address))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                // Validate that the password and confirmed password should match
                else if (userDataInput.Password != userDataInput.ConfirmPassword)
                {
                    result.Message = "The new password should match with the confirm password";
                    result.IsSuccess = false;

                    return result;
                }

                else
                {
                    using (var db = new SariKartContext())
                    {
                        var user = new AppUser();

                        user.Username = userDataInput.Username;
                        user.FirstName = userDataInput.Firstname;
                        user.LastName = userDataInput.Lastname;
                        user.Password = userDataInput.Password;
                        user.ContactNumber = userDataInput.ContactNumber;
                        user.UserTypeId = 1;
                        user.EditableContactPerson = userDataInput.Firstname + " " + userDataInput.Lastname;
                        user.EditableContactNumber = userDataInput.ContactNumber;
                        user.EditableContactAddress = userDataInput.UserStore.Address;

                        db.Add(user);
                        db.SaveChanges();

                        var userStore = new UserStore();

                        userStore.UserId = user.Id;
                        userStore.Address = userDataInput.UserStore.Address;

                        db.Add(userStore);
                        db.SaveChanges();

                        result.Message = "New user record is successfully registered";
                        result.IsSuccess = true;

                        return result;
                    }
                }
            }

            catch
            {
                result.Message = "Unable to register your user account";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPut("edit")]
        public Result UpdateUser([FromBody] UserDataInput userDataInput) // This route updates current user
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber) || String.IsNullOrEmpty(userDataInput.UserStore.Address))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                else
                {
                    // Check if the account has already existed. if it is, the user will be asked to input different username
                    using (var db = new SariKartContext())
                    {
                        var user = db.AppUsers.Where(x => x.Id == userDataInput.Id).FirstOrDefault();
                        var userStore = db.UserStores.Where(x => x.UserId == userDataInput.Id).FirstOrDefault();
                        var usernameExists = db.AppUsers.Where(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id).FirstOrDefault();

                        // Check if the specific user exists
                        if (user == null)
                        {
                            result.Message = "No user data to update";
                            result.IsSuccess = false;

                            return result;
                        }

                        else
                        {
                            // Check if the specific username exists
                            if (usernameExists != null)
                            {
                                result.Message = "This user account " + userDataInput.Username + " already exists";
                                result.IsSuccess = false;

                                return result;
                            }

                            user.Username = userDataInput.Username;
                            user.FirstName = userDataInput.Firstname;
                            user.LastName = userDataInput.Lastname;

                            db.SaveChanges();

                            userStore.Address = userDataInput.UserStore.Address;

                            db.SaveChanges();

                            result.Message = "Current user record is successfully updated";
                            result.IsSuccess = true;

                            return result;
                        }
                    }
                }
            }
               
            catch
            {
                result.Message = "Unable to update current user account";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPut("editAdminUser")]
        public Result UpdateAdminUser([FromBody] UserDataInput userDataInput) // This route updates current admin user
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                else
                {
                    // Check if the account has already existed. if it is, the user will be asked to input different username
                    using (var db = new SariKartContext())
                    {
                        var user = db.AppUsers.Where(x => x.Id == userDataInput.Id).FirstOrDefault();
                        var usernameExists = db.AppUsers.Where(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id).FirstOrDefault();

                        // Check if the specific user exists
                        if (user == null)
                        {
                            result.Message = "No user data to update";
                            result.IsSuccess = false;

                            return result;
                        }

                        else
                        {
                            // Check if the specific username exists
                            if (usernameExists != null)
                            {
                                result.Message = "This user account " + userDataInput.Username + " already exists";
                                result.IsSuccess = false;

                                return result;
                            }

                            user.Username = userDataInput.Username;
                            user.FirstName = userDataInput.Firstname;
                            user.LastName = userDataInput.Lastname;

                            db.SaveChanges();

                            result.Message = "Current user record is successfully updated";
                            result.IsSuccess = true;

                            return result;
                        }
                    }
                }
            }

            catch
            {
                result.Message = "Unable to update current user account";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPut("editContact")]
        public Result UpdateUserContact([FromBody] UserDataInput userDataInput) // This route updates current user contact details
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(userDataInput.EditableContactPerson) || String.IsNullOrEmpty(userDataInput.EditableContactNumber) || String.IsNullOrEmpty(userDataInput.EditableContactAddress))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                else
                {
                    // Check if the account has already existed. if it is, the user will be asked to input different username
                    using (var db = new SariKartContext())
                    {
                        var user = db.AppUsers.Where(x => x.Id == userDataInput.Id).FirstOrDefault();
                        
                        // Check if the specific user exists
                        if (user == null)
                        {
                            result.Message = "No user data to update";
                            result.IsSuccess = false;

                            return result;
                        }

                        else
                        {
                            user.EditableContactPerson = userDataInput.EditableContactPerson;
                            user.EditableContactNumber = userDataInput.EditableContactNumber;
                            user.EditableContactAddress = userDataInput.EditableContactAddress;

                            db.SaveChanges();

                            result.Message = "Current user contact is successfully updated";
                            result.IsSuccess = true;

                            return result;
                        }
                    }
                }
            }

            catch
            {
                result.Message = "Unable to update current user contact";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPut("changePassword")]
        public Result UpdateUserPassword([FromBody] UserDataInput userDataInput) // The user can change password for their account
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (userDataInput.Password.Length == 0 || userDataInput.ConfirmPassword.Length == 0)
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                // Validate that the password and confirmed password should match
                if (userDataInput.Password != userDataInput.ConfirmPassword)
                {
                    result.Message = "The new password should match with the confirm password";
                    result.IsSuccess = false;

                    return result;
                }

                else
                {
                    using (var db = new SariKartContext())
                    {
                        var user = db.AppUsers.Where(x => x.Id == userDataInput.Id).FirstOrDefault();

                        // Check if the specific user exists
                        if (user == null)
                        {
                            result.Message = "No user data for password update";
                            result.IsSuccess = false;

                            return result;
                        }

                        else
                        {
                            user.Password = userDataInput.Password;

                            db.SaveChanges();

                            result.Message = "User password is successfully updated";
                            result.IsSuccess = true;

                            return result;
                        }
                    }
                }
            }

            catch
            {
                result.Message = "Unable to update your password";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpGet("adminUser/{id}/{name}")] // This route gets admin users by search filter
        public Result GetAdminUsers(int id, string name)
        {
            var result = new Result();

            try
            {
                var users = new List<AppUser>();

                using (var db = new SariKartContext())
                {
                    if (name == "all")
                    {
                        users = db.AppUsers.Where(x => x.UserTypeId == 2 && x.Id != id).Include(x => x.UserStores).ToList();
                    }

                    else
                    {
                        users = db.AppUsers.Where(x => x.FirstName.Contains(name) || x.LastName.Contains(name) || x.Username.Contains(name))
                        .Where(x => x.UserTypeId == 2 && x.Id != id).ToList();
                    }
                }

                result.JsonResultObject = users;
                result.Message = "You can get the admin users";
                result.IsSuccess = true;

                return result;
            }

            catch
            {
                result.Message = "Unable to get the admin users";
                result.IsSuccess = false;

                return result;
            }

        }

        [HttpGet("list/riders")]
        public Result GetRidersList() // This route gets riders list
        {
            var result = new Result();

            try
            {
                var riders = new List<AppUser>();

                using (var db = new SariKartContext())
                {
                    riders = db.AppUsers.Where(x => x.UserTypeId == 3).Select(X => new AppUser
                    {
                        Id = X.Id,
                        FirstName = X.FirstName,
                        LastName = X.LastName
                    }).ToList();

                    result.JsonResultObject = riders;
                    result.Message = "You get all riders list";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get all riders list";
                result.IsSuccess = false;

                return result;
            }

        }

        [HttpGet("rider/{name}")]
        public Result GetRiders(string name) // This route gets riders by search filter
        {
            var result = new Result();

            try
            {
                var users = new List<AppUser>();

                using (var db = new SariKartContext())
                {
                    if (name == "all")
                    {
                        users = db.AppUsers.Where(x => x.UserTypeId == 3).Include(x => x.UserStores).ToList();
                    }

                    else
                    {
                        users = db.AppUsers.Where(x => x.FirstName.Contains(name) || x.LastName.Contains(name) || x.Username.Contains(name))
                        .Where(x => x.UserTypeId == 3).ToList();
                    }
                }

                result.JsonResultObject = users;
                result.Message = "You can get the riders";
                result.IsSuccess = true;

                return result;
            }

            catch
            {
                result.Message = "Unable to get the riders";
                result.IsSuccess = false;

                return result;
            }

        }

        [HttpGet("cashier/{name}")]
        public Result GetCashiers(string name) // This route gets cashiers by search filter
        {
            var result = new Result();

            try
            {
                var users = new List<AppUser>();

                using (var db = new SariKartContext())
                {
                    if (name == "all")
                    {
                        users = db.AppUsers.Where(x => x.UserTypeId == 1003).ToList();
                    }

                    else
                    {
                        users = db.AppUsers.Where(x => x.FirstName.Contains(name) || x.LastName.Contains(name) || x.Username.Contains(name))
                        .Where(x => x.UserTypeId == 1003).ToList();
                    }
                }

                result.JsonResultObject = users;
                result.Message = "You can get the cashiers";
                result.IsSuccess = true;

                return result;
            }

            catch
            {
                result.Message = "Unable to get the cashiers";
                result.IsSuccess = false;

                return result;
            }

        }

        [HttpPost("adminUser/add")] // This route adds new admin user record
        public Result AddAdminUser([FromBody] UserDataInput userDataInput)
        {
            var result = new Result();

            try
            {
                // Check if the account has already existed. if it is, the user will be asked to input different username
                using (var db = new SariKartContext())
                {
                    var user = db.AppUsers.Where(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id).FirstOrDefault();

                    // Check if the specific username exists
                    if (user != null)
                    {
                        result.Message = "This user account " + userDataInput.Username + " already exists";
                        result.IsSuccess = false;

                        return result;
                    }
                }

                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber) || String.IsNullOrEmpty(userDataInput.Password) || String.IsNullOrEmpty(userDataInput.ConfirmPassword))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                // Validate that the password and confirmed password should match
                else if (userDataInput.Password != userDataInput.ConfirmPassword)
                {
                    result.Message = "The new password should match with the confirm password";
                    result.IsSuccess = false;

                    return result;
                }

                else
                {
                    using (var db = new SariKartContext())
                    {
                        var user = new AppUser();

                        user.Username = userDataInput.Username;
                        user.FirstName = userDataInput.Firstname;
                        user.LastName = userDataInput.Lastname;
                        user.Password = userDataInput.Password;
                        user.UserTypeId = 2;
                        user.ContactNumber = userDataInput.ContactNumber;

                        db.Add(user);
                        db.SaveChanges();

                        result.Message = "New user record is successfully registered";
                        result.IsSuccess = true;

                        return result;
                    }
                }
            }

            catch
            {
                result.Message = "Unable to register new user account";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPost("rider/add")]
        public Result AddRider([FromBody] UserDataInput userDataInput) //This route adds new rider record
        {
            var result = new Result();

            try
            {
                // Check if the account has already existed. if it is, the user will be asked to input different username
                using (var db = new SariKartContext())
                {
                    var user = db.AppUsers.Where(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id).FirstOrDefault();

                    // Check if the specific username exists
                    if (user != null)
                    {
                        result.Message = "This user account " + userDataInput.Username + " already exists";
                        result.IsSuccess = false;

                        return result;
                    }
                }

                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber) || String.IsNullOrEmpty(userDataInput.Password) || String.IsNullOrEmpty(userDataInput.ConfirmPassword))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                // Validate that the password and confirmed password should match
                else if (userDataInput.Password != userDataInput.ConfirmPassword)
                {
                    result.Message = "The new password should match with the confirm password";
                    result.IsSuccess = false;

                    return result;
                }

                else
                {
                    using (var db = new SariKartContext())
                    {
                        var user = new AppUser();

                        user.Username = userDataInput.Username;
                        user.FirstName = userDataInput.Firstname;
                        user.LastName = userDataInput.Lastname;
                        user.Password = userDataInput.Password;
                        user.UserTypeId = 3;
                        user.ContactNumber = userDataInput.ContactNumber;

                        db.Add(user);
                        db.SaveChanges();

                        result.Message = "New user record is successfully registered";
                        result.IsSuccess = true;

                        return result;
                    }
                }
            }

            catch
            {
                result.Message = "Unable to register new user account";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPost("cashier/add")]
        public Result AddCashier([FromBody] UserDataInput userDataInput) //This route adds new cashier record
        {
            var result = new Result();

            try
            {
                // Check if the account has already existed. if it is, the user will be asked to input different username
                using (var db = new SariKartContext())
                {
                    var user = db.AppUsers.Where(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id).FirstOrDefault();

                    // Check if the specific user exists
                    if (user != null)
                    {
                        result.Message = "This user account " + userDataInput.Username + " already exists";
                        result.IsSuccess = false;

                        return result;
                    }
                }

                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber) || String.IsNullOrEmpty(userDataInput.Password) || String.IsNullOrEmpty(userDataInput.ConfirmPassword))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                // Validate that the password and confirmed password should match
                else if (userDataInput.Password != userDataInput.ConfirmPassword)
                {
                    result.Message = "The new password should match with the confirm password";
                    result.IsSuccess = false;

                    return result;
                }

                else
                {
                    using (var db = new SariKartContext())
                    {
                        var user = new AppUser();

                        user.Username = userDataInput.Username;
                        user.FirstName = userDataInput.Firstname;
                        user.LastName = userDataInput.Lastname;
                        user.Password = userDataInput.Password;
                        user.UserTypeId = 1003;
                        user.ContactNumber = userDataInput.ContactNumber;

                        db.Add(user);
                        db.SaveChanges();

                        result.Message = "New user record is successfully registered";
                        result.IsSuccess = true;

                        return result;
                    }
                }
            }

            catch
            {
                result.Message = "Unable to register new user account";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPut("adminUser/edit")]
        public Result UpdateOtAdminUser([FromBody] UserDataInput userDataInput) // This route updates current admin user
        {
            var result = new Result();

            try
            {
                // Check if the account has already existed. if it is, the user will be asked to input different username
                using (var db = new SariKartContext())
                {
                    var user = db.AppUsers.Where(x => x.Username == userDataInput.Username && x.Id != userDataInput.Id).FirstOrDefault();

                    // Check if the specific user exists
                    if (user != null)
                    {
                        result.Message = "This user account " + userDataInput.Username + " already exists";
                        result.IsSuccess = false;

                        return result;
                    }
                }

                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.ContactNumber) || String.IsNullOrEmpty(userDataInput.Password) || String.IsNullOrEmpty(userDataInput.ConfirmPassword))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                // Validate that the password and confirmed password should match
                else if (userDataInput.Password != userDataInput.ConfirmPassword)
                {
                    result.Message = "The new password should match with the confirm password";
                    result.IsSuccess = false;

                    return result;
                }

                else
                {
                    using (var db = new SariKartContext())
                    {
                        var user = db.AppUsers.Where(x => x.Id == userDataInput.Id).FirstOrDefault();

                        user.Username = userDataInput.Username;
                        user.FirstName = userDataInput.Firstname;
                        user.LastName = userDataInput.Lastname;
                        user.Password = userDataInput.Password;
                        user.ContactNumber = userDataInput.ContactNumber;

                        db.SaveChanges();

                        result.Message = "Current user record is successfully updated";
                        result.IsSuccess = true;

                        return result;
                    }
                }
            }

            catch
            {
                result.Message = "Unable to update current user account";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpGet("adminUser/get/{id}")]
        public Result GetAdminUser(int id) // This route gets a specific admin user
        {
            var result = new Result();

            try
            {
                var user = new AppUser();

                using (var db = new SariKartContext())
                {
                    user = db.AppUsers.Where(x => x.Id == id).FirstOrDefault();
                }

                result.JsonResultObject = user;
                result.Message = "You can get current user account details";
                result.IsSuccess = true;

                return result;
            }

            catch
            {
                result.Message = "Unable to get current user account details";
                result.IsSuccess = false;

                return result;
            }

        }
        
        [HttpDelete("adminUser/delete/{id}")]
        public Result DeleteAdminUser(int id) // This route deletes current admin user
        {
            var result = new Result();

            try
            {
                using (var db = new SariKartContext())
                {
                    var adminUser = db.AppUsers.Where(x => x.Id == id).FirstOrDefault();

                    // Check if the specific user exists
                    if (adminUser == null)
                    {
                        result.Message = "No admin user to delete";
                        result.IsSuccess = false;

                        return result;
                    }

                    else
                    {
                        db.AppUsers.Remove(adminUser);
                        db.SaveChanges();

                        result.Message = "Admin user is successfully deleted";
                        result.IsSuccess = true;

                        return result;
                    }
                }
            }

            catch
            {
                result.Message = "Unable to delete an admin user";
                result.IsSuccess = false;

                return result;
            }
        }
    }
}
