using JulyGrocerAPI.Entities;
using JulyGrocerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

/*
 * The users can register and login to account
 * The user can update user account, change password, and delete account
 */

namespace JulyGrocerAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        // API Controller Method for user login. If no data found, the input username and password in incorrect
        [HttpPost("login")]
        public Result Login([FromBody] UserLoginDataInput userLoginDataInput)
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

                using (var db = new JulyGrocerContext())
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
        public Result GetUser(int id)
        {
            var result = new Result();

            try
            {
                var user = new AppUser();

                using (var db = new JulyGrocerContext())
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

        // User sign up registration if the user has no account yet
        [HttpPost("add")]
        public Result InsertUser([FromBody] UserDataInput userDataInput)
        {
            var result = new Result();

            try
            {
                // Check if the account has already existed. if it is, the user will be asked to input different username
                using (var db = new JulyGrocerContext())
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
                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.Password) || String.IsNullOrEmpty(userDataInput.ConfirmPassword) || String.IsNullOrEmpty(userDataInput.UserStore.Address))
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
                    using (var db = new JulyGrocerContext())
                    {
                        var user = new AppUser();

                        user.Username = userDataInput.Username;
                        user.FirstName = userDataInput.Firstname;
                        user.LastName = userDataInput.Lastname;
                        user.Password = userDataInput.Password;
                        user.ContactNumber = userDataInput.ContactNumber;

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
        public Result UpdateUser([FromBody] UserDataInput userDataInput)
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (String.IsNullOrEmpty(userDataInput.Username) || String.IsNullOrEmpty(userDataInput.Firstname) || String.IsNullOrEmpty(userDataInput.Lastname) || String.IsNullOrEmpty(userDataInput.UserStore.Address))
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
                            // Check if the specific user exists
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

        // The user can change password for their account
        [HttpPut("changePassword")]
        public Result UpdateUserPassword([FromBody] UserDataInput userDataInput)
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
                    using (var db = new JulyGrocerContext())
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
    }
}
