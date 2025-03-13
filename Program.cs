using ApiElecateProspectsForm.Repositories;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;
using ApiElecateProspectsForm.Services.FormComponentsGenerators;
using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Utils;
using ApiElecateProspectsForm.Controllers;
using ApiElecateProspectsForm.Interfaces.Repositories;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.DTOs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure the loading of configuration files
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework and the database connection
builder.Services.AddSingleton<DbContextFactory>();
builder.Services.AddScoped<IDbContextFactory, DbContextFactory>();

// Register the repository
builder.Services.AddScoped<IMaritalStatusRepository, MaritalStatusRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IFormFieldsRepository, FormFieldsRepository>();
builder.Services.AddScoped<IMaskFormatter, MaskFormatter>();

// Register the Form Fields Factory
builder.Services.AddSingleton<TextFieldGenerator>();
builder.Services.AddSingleton<SelectFieldGenerator>();
builder.Services.AddSingleton<CheckboxFieldGenerator>();
builder.Services.AddScoped<RadioFieldGenerator>();
builder.Services.AddScoped<FieldGeneratorFactory>();

// Register HttpClient
builder.Services.AddHttpClient();

// Register IResponseHandler
builder.Services.AddScoped<IResponseHandler, ResponseHandler>();

// Register other necessary services
builder.Services.AddScoped<IValidateFields, ValidateFields>();
builder.Services.AddScoped<IProspectMapper, ProspectMapper>();

// Register FieldNamesConfigDTO
builder.Services.Configure<FieldNamesConfigDTO>(builder.Configuration.GetSection("FieldNames"));

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

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();