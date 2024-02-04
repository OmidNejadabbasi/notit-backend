using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
ConfigAuth(builder);
ConfigCors(builder, [ "localhost:2342" ]);

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
    builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<MyDBContext>();
    builder.Services.AddAuthentication().AddJwtBearer();
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
