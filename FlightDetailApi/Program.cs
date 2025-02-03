using FlightDetailApi.Configurations;
using FlightDetailApi.Data;
using FlightDetailApi.Controllers.HelperMethods;
using FlightDetailApi.MappingDTO;
using FlightDetailApi.Models;
using FlightDetailApi.Repositories;
using FlightDetailApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add configurations
builder.Services.AddSwaggerConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAutoMapper(typeof(FlightMapper)); // AutoMapper profile registration
builder.Services.AddDatabaseContexts(builder.Configuration); // Ensure this method correctly configures your DbContext
builder.Services.AddScoped<JWTTokenService>();
builder.Services.AddCorsPolicy();
builder.Host.ConfigureLogging();
builder.Services.AddScoped<IFlightHelper, FlightHelper>();

// Registering IRepository and other services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<IGenericRepository<AdminModel>, GenericRepository<AdminModel>>();

// Ensure you have registered FlightHelper (if used in controllers)
builder.Services.AddScoped<FlightHelper>(); // Add this line to register FlightHelper if used

var app = builder.Build();

// Enable CORS
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware Order
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
