using Microsoft.EntityFrameworkCore;
using PatientManagementApp.Data;
using PatientManagementApp.DTOs;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using PatientManagementApp.Repositories;
using PatientManagementApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Register FluentValidation validators
builder.Services.AddTransient<IValidator<CreatePatientDto>, CreatePatientDtoValidator>();
builder.Services.AddTransient<IValidator<UpdatePatientDto>, UpdatePatientDtoValidator>();

// Add services to the container.
builder.Services.AddControllers();

// Add FluentValidation services to the ASP.NET Core pipeline
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();


// Read connection string from environment variable
var shard1MasterConnection = Environment.GetEnvironmentVariable("SHARD1_MASTER_CONNECTION");
var shard2MasterConnection = Environment.GetEnvironmentVariable("SHARD2_MASTER_CONNECTION");
var shard1ReplicaConnection = Environment.GetEnvironmentVariable("SHARD1_REPLICA_CONNECTION");
var shard2ReplicaConnection = Environment.GetEnvironmentVariable("SHARD2_REPLICA_CONNECTION");

if (string.IsNullOrEmpty(shard1MasterConnection) || string.IsNullOrEmpty(shard2MasterConnection) ||
    string.IsNullOrEmpty(shard1ReplicaConnection) || string.IsNullOrEmpty(shard2ReplicaConnection))
{
    throw new InvalidOperationException("Connection strings not found in environment variables.");
}

builder.Services.AddDbContext<Shard1Context>(options =>
    options.UseMySql(shard1MasterConnection, ServerVersion.AutoDetect(shard1MasterConnection)), ServiceLifetime.Transient);

builder.Services.AddDbContext<Shard2Context>(options =>
    options.UseMySql(shard2MasterConnection, ServerVersion.AutoDetect(shard2MasterConnection)), ServiceLifetime.Transient);

builder.Services.AddDbContext<Shard1Context>(options =>
    options.UseMySql(shard1ReplicaConnection, ServerVersion.AutoDetect(shard1ReplicaConnection)), ServiceLifetime.Transient);

builder.Services.AddDbContext<Shard2Context>(options =>
    options.UseMySql(shard2ReplicaConnection, ServerVersion.AutoDetect(shard2ReplicaConnection)), ServiceLifetime.Transient);

builder.Services.AddSingleton<IGeneratorIdService, GeneratorIdService>();

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAllOrigins");
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
