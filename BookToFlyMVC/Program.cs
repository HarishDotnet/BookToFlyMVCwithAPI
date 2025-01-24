using BookToFlyMVC.Data;
using BookToFlyMVC.MappingDTO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Configuring DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
#endregion

#region Configuring CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
#endregion

#region Configure AutoMapper
builder.Services.AddAutoMapper(typeof(FlightDTOMapping));
#endregion

#region Configuring Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";  // Ensure this is pointing to UserController
        options.LogoutPath = "/User/Logout"; // Ensure this is pointing to UserController
    });
#endregion

#region Configuring HttpClient
// Register a named HttpClient for the Flight API
builder.Services.AddHttpClient("FlightClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5087/api/"); // Your API base URL
});
#endregion

// Add controllers with views
builder.Services.AddControllersWithViews();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication and Authorization Middleware
app.UseAuthentication();  // Authentication middleware before Authorization
app.UseAuthorization();

// CORS Middleware
app.UseCors("AllowAll");

// Map routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
