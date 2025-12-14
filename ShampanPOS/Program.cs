using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShampanPOS.Repository;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.Persistence;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthContext")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add this to your existing ConfigureServices method
//builder.Services.AddScoped<IRecentDateRepository, RecentDateRepository>();
//builder.Services.AddScoped<IRecentDateService, RecentDateService>();
// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                // Handle unauthorized access due to invalid or missing token
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";
                var response = new { message = "Unauthorized access. Please use a valid token." };
                return context.Response.WriteAsJsonAsync(response);
            }
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    //options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    //options.RoutePrefix = String.Empty;
});

app.UseHttpsRedirection();
app.UseDefaultFiles();  // 👈 Looks for index.html automatically
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
