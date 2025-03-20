using ApiElecateProspectsForm.Repositories;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;
using ApiElecateProspectsForm.Services.FormComponentsGenerators;
using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Utils;
using ApiElecateProspectsForm.Interfaces.Repositories;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Services.DbContextFactory;
using ApiElecateProspectsForm.Middlewares;

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

// Register the specific factories
builder.Services.AddSingleton<SqlServerDbContextOptionsFactory>();
builder.Services.AddSingleton<PostgreSqlDbContextOptionsFactory>();

// Register DbContextFactory
builder.Services.AddSingleton<DbContextFactory>();
builder.Services.AddScoped<IDbContextFactory, DbContextFactory>();

// Register the repository
builder.Services.AddScoped<IMaritalStatusRepository, MaritalStatusRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IFormFieldsRepository, FormFieldsRepository>();
builder.Services.AddScoped<ISecretsDbRepository, SecretsDbRepository>();
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

app.UseMiddleware<ExceptionMiddleware>();

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