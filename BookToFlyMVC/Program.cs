using BookToFlyMVC.MappingDTO;
using BookToFlyMVC.Handlers;
using BookToFlyMVC.Configurations;
var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureCors();
builder.Services.ConfigureAuthentication();
builder.Services.ConfigureHttpClient();
builder.Services.ConfigureSession();
builder.Services.ConfigureLogging();
builder.Services.AddAutoMapper(typeof(FlightDTOMapping));
builder.Services.AddControllersWithViews();

// Add HttpContextAccessor and TokenHandler
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TokenHandler>();

var app = builder.Build();

// Use middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");
app.ConfigureMiddleware(); // Use the custom middleware here
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

// Map controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
