using ApiElecateProspectsForm.Repositories;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;
using ApiElecateProspectsForm.Services.FormComponentsGenerators;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configurar la carga de archivos de configuración
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
//builder.Services.AddDbContext<ElecateDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
//    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

builder.Services.AddSingleton<DbContextFactory>();

// Register the repository
builder.Services.AddScoped<IMaritalStatusRepository, MaritalStatusRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IFormFieldsRepository, FormFieldsRepository>();

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
app.MapControllers();

app.Run();