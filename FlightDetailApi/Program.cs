using FlightDetailApi.Configurations;
using FlightDetailApi.MappingDTO;
using FlightDetailApi.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add configurations
builder.Services.AddSwaggerConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAutoMapper(typeof(FlightMapper)); // AutoMapper profile registration
builder.Services.AddDatabaseContexts(builder.Configuration);
builder.Services.AddScoped<JWTTokenService>();
builder.Services.AddCorsPolicy();

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
