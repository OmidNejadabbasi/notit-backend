using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/auth");
        authGroup.MapPost("/login", Login);
        authGroup.MapPost("/signup", SignUp);
        authGroup.MapGet("/me", GetMe);
    }

    public static IResult GetMe(IHttpContextAccessor accessor)
    {
        var email = accessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
        var roles = accessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(r => r.Value);
        var id = accessor.HttpContext?.User.FindFirst(ClaimTypes.Sid)?.Value;
        return Results.Ok(
            new
            {
                email,
                roles,
                id
            }
        );
    }

    public static async Task<IResult> Login(
        MyLoginRequest request,
        UserManager<User> userManager,
        IConfiguration configuration
    )
    {
        // Use UserManager to find the user by username
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            // User not found
            return Results.NotFound("Invalid username or password");
        }

        // Use UserManager to check if the password is correct
        var signInResult = await userManager.CheckPasswordAsync(user, request.Password);

        if (signInResult)
        {
            // Password is correct, generate a JWT token
            var token = GenerateJwtToken(user, configuration, userManager);

            // You can customize the success response as needed
            return Results.Ok(new { accessToken = token });
        }
        else
        {
            // Password is incorrect
            return Results.Unauthorized();
        }
    }

    public static async Task<IResult> SignUp(
        MySignUpRequest request,
        UserManager<User> userManager,
        IConfiguration configuration
    )
    {
        // Validate the request model
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            validationResults,
            true
        );

        if (!isValid)
        {
            // Model is not valid, return validation errors
            var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
            return Results.BadRequest(errorMessages);
        }

        // Check if the username is already taken
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Results.BadRequest("Username is already taken");
        }

        // You might want to add additional validation, e.g., password strength

        // Create a new user
        var newUser = new User { Email = request.Email, UserName = request.Email };
        var createResult = await userManager.CreateAsync(newUser, request.Password);

        if (createResult.Succeeded)
        {
            // User creation succeeded, generate a JWT token and return it
            var token = GenerateJwtToken(newUser, configuration, userManager);

            // You can customize the success response as needed
            return Results.Ok(new { accessToken = token });
        }
        else
        {
            foreach (var error in createResult.Errors)
            {
                // Log or inspect the error details
                Console.WriteLine($"Error: {error.Description}");
            }
            // User creation failed
            return Results.BadRequest("Error creating user");
        }
    }

    private static string GenerateJwtToken(
        User user,
        IConfiguration configuration,
        UserManager<User> userManager
    )
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])
        );
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claimsIdentity = new ClaimsIdentity();
        var roles = userManager.GetRolesAsync(user).Result;
        foreach (var role in roles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Sid, user.Id));
        var Sectoken = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Issuer"],
            claimsIdentity.Claims,
            expires: DateTime
                .UtcNow
                .AddHours(Convert.ToDouble(configuration["Jwt:ExpirationHours"])),
            signingCredentials: credentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
        return token;
    }
}

public class MyLoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class MySignUpRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    // Add any additional properties for user registration
}
