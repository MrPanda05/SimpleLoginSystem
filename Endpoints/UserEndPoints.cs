using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SimpleLoginSystem.Objects;
using SimpleLoginSystem.Objects.Requests;
using SimpleLoginSystem.Objects.Responses;
using SimpleLoginSystem.Services;

namespace SimpleLoginSystem.Endpoints
{
    public static class UserUserEndPoints
    {
        public static void MapUserEndpoints(RouteGroupBuilder route)
        {
            route.MapGet("/{id}", GetUser);
            route.MapGet("/all", GetAllUser).RequireAuthorization("admin");
            route.MapPost("/register", Register);
            route.MapPost("/login", Login);
            route.MapPost("/logout", Logout).RequireAuthorization();
            route.MapPut("/edit", EditPassword).RequireAuthorization();


        }

        static async Task<Results<Ok<UserDTO>, BadRequest<string>>> Register(RegisterRequest req, EcommerceDB db)
        {
            if (string.IsNullOrEmpty(req.Username) || string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.Password))
            {
                return TypedResults.BadRequest("Username, Email, and Password are required.");
            }

            var tempEmail = await db.Users.SingleOrDefaultAsync(u => u.Email == req.Email);

            if (tempEmail != null)
            {
                return TypedResults.BadRequest("Email already in use.");
            }
            User newUser = new User
            {
                Username = req.Username,
                Email = req.Email,
                Password = req.Password//this would be encrypted in a real application
            };
            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            UserDTO userDto = new UserDTO
            {
                Username = newUser.Username,
                Email = newUser.Email
            };
            return TypedResults.Ok(userDto);
        }

        static async Task<Results<Ok<UserDTO>, NotFound>> GetUser(int id, EcommerceDB db)
        {
            User? user = await db.Users.FindAsync(id);
            if (user is null)
            {
                return TypedResults.NotFound();
            }
            UserDTO userDto = new UserDTO
            {
                Username = user.Username,
                Email = user.Email,
                IsLoggedIn = user.IsLoggedIn
            };

            return TypedResults.Ok(userDto);
        }

        static async Task<Results<Ok<List<UserDTO>>, NotFound>> GetAllUser(EcommerceDB db)
        {
            var users = await db.Users.Select(u => new UserDTO
            {
                Username = u.Username,
                Email = u.Email,
                IsLoggedIn = u.IsLoggedIn
            }).ToListAsync();

            if (users == null || users.Count == 0)
            {
                return TypedResults.NotFound();
            }
            
            return TypedResults.Ok(users);
        }

        static async Task<Results<Ok<LoginResponse>, NotFound, BadRequest<string>>> Login(RegisterRequest req, EcommerceDB db, TokenService service)
        {
            if (string.IsNullOrEmpty(req.Username) || string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.Password))
            {
                return TypedResults.BadRequest("Username, Email, and Password are required.");
            }

            var user = await db.Users.SingleOrDefaultAsync(u => u.Email == req.Email);

            if (user == null)
            {
                return TypedResults.BadRequest("Invalid email or password");
            }

            if (user.Password != req.Password)
            {
                return TypedResults.BadRequest("Invalid password");
            }

            user.IsLoggedIn = true;
            user.Token = service.GenerateToken(user);
            await db.SaveChangesAsync();
            LoginResponse response = new LoginResponse
            {
                UserDTO = new UserDTO
                {
                    Username = user.Username,
                    Email = user.Email,
                    IsLoggedIn = user.IsLoggedIn
                },
                Token = user.Token
            };
            return TypedResults.Ok(response);
        }
        static async Task<Results<Ok<LoginResponse>, NotFound, BadRequest<string>>> Logout(LogoutRequest req, EcommerceDB db)
        {
            if (string.IsNullOrEmpty(req.Username) || string.IsNullOrEmpty(req.Email))
            {
                return TypedResults.BadRequest("Username, Email required.");
            }

            if(string.IsNullOrEmpty(req.Token))
            {
                return TypedResults.BadRequest("Token is required for logout.");
            }

            var user = await db.Users.SingleOrDefaultAsync(u => u.Email == req.Email);

            if (user == null)
            {
                return TypedResults.BadRequest("Invalid email or password");
            }

            if(user.Token != req.Token)
            {
                return TypedResults.BadRequest("Invalid token");
            }

            user.IsLoggedIn = false;
            user.Token = null; // Clear the token on logout
            await db.SaveChangesAsync();
            LoginResponse response = new LoginResponse
            {
                UserDTO = new UserDTO
                {
                    Username = user.Username,
                    Email = user.Email,
                    IsLoggedIn = user.IsLoggedIn
                },
                Token = user.Token // Return the token in the response
            };
            return TypedResults.Ok(response);
        }

        static async Task<Results<Ok, NotFound, BadRequest<string>>> EditPassword(EditPassRequest req, EcommerceDB db)
        {
            if(string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.OldPassword) || string.IsNullOrEmpty(req.NewPassword))
            {
                return TypedResults.BadRequest("Email, Old Password, and New Password are required.");
            }
            if(string.IsNullOrEmpty(req.Token))
            {
                return TypedResults.BadRequest("Invalid token");
            }
            var user = await db.Users.SingleOrDefaultAsync(u => u.Email == req.Email);

            if (user == null)
            {
                return TypedResults.NotFound();
            }
            if (user.Token != req.Token)
            {
                return TypedResults.BadRequest("Invalid token");
            }

            if(user.Password != req.OldPassword)
            {
                return TypedResults.BadRequest("Old password is incorrect.");
            }

            if(user.Password == req.NewPassword)
            {
                return TypedResults.BadRequest("New password cannot be the same as the old password.");
            }

            user.Password = req.NewPassword; // Update the password
            await db.SaveChangesAsync();
            return TypedResults.Ok();
        }
    }
}
