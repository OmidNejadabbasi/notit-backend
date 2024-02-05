using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
ConfigAuth(builder);
ConfigCors(builder, [ "http://localhost:2342" ]);

builder
    .Services
    .AddDbContext<MyDBContext>(
        options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

// Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<NoteService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontEnd");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapAuthEndpoints();
app.MapNoteEndpoints();
app.Run();

void ConfigAuth(WebApplicationBuilder builder)
{
    var jwtIssuer = builder.Configuration["Jwt:Issuer"];
    var jwtKey = builder.Configuration["Jwt:Secret"];
    builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<MyDBContext>();
    builder
        .Services
        .AddAuthentication(x => {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            }
        );
    builder.Services.AddAuthorization();
    builder
        .Services
        .AddAuthorizationBuilder()
        .AddPolicy("admin", policy => policy.RequireRole("admin"))
        .AddDefaultPolicy("user", policy => policy.RequireAuthenticatedUser());
}

void ConfigCors(WebApplicationBuilder builder, string[] origins)
{
    builder
        .Services
        .AddCors(
            options =>
                options.AddPolicy(
                    "AllowFrontEnd",
                    builder =>
                        builder
                            .WithOrigins(origins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                )
        );
    Console.WriteLine("Cors cofigured : " + origins[0]);
}
