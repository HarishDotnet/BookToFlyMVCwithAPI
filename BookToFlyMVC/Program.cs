using BookToFlyMVC.Configurations;
using BookToFlyMVC.Filters;
using BookToFlyMVC.Handlers;
using BookToFlyMVC.MappingDTO;  // Add this namespace for GlobalExceptionFilter

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Register the Global Exception Filter
    options.Filters.Add<GlobalExceptionFilter>();
});

// Other configurations remain the same
builder.Services.ConfigureAuthentication();
builder.Services.ConfigureHttpClient();
builder.Services.ConfigureSession();
builder.Services.ConfigureLogging();
builder.Services.AddAutoMapper(typeof(FlightDTOMapping));
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TokenHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
