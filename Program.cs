using Microsoft.EntityFrameworkCore;
using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Repositories;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Services.FormComponentsGenerators;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework and the database connection
builder.Services.AddDbContext<ElecateDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") + 
    ";Encrypt=False"));

// Register the repository
builder.Services.AddScoped<IMaritalStatusRepository, MaritalStatusRepository>();
builder.Services.AddScoped<IServiceReository, ServiceRepository>();

// Register the Form Fields Factory
builder.Services.AddSingleton<TextFieldGenerator>();
builder.Services.AddSingleton<SelectFieldGenerator>();
builder.Services.AddSingleton<CheckboxFieldGenerator>();
builder.Services.AddScoped<RadioFieldGenerator>();
builder.Services.AddScoped<FieldGeneratorFactory>();

// Register HttpClient
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();
app.MapControllers();

app.Run();
