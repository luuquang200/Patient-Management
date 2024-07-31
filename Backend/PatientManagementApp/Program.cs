using Microsoft.EntityFrameworkCore;
using PatientManagementApp.Data;
using PatientManagementApp.Repositories;
using PatientManagementApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenAnyIP(8080); // HTTP port
});

// Add services to the container.
builder.Services.AddControllers();

// Read connection string from environment variable
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string not found in environment variables.");
}
builder.Services.AddDbContext<PatientContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
