using AspNetCore8MinimalApis.Endpoints;
using AspNetCore8MinimalApis.Identity;
using AspNetCore8MinimalApis.Mapping;
using AspNetCore8MinimalApis.Mapping.Interfaces;
using BLL.Services; 
using Domain.DTOs;
using Domain.Repositories;
using Domain.Services;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserProfile, UserProfile>();
// Add controllers and JSON Patch support
builder.Services.AddControllers().AddNewtonsoftJson();

// Api versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AspNetCore8MinimalApis", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
    // Ensure controllers are included
    c.DocInclusionPredicate((name, api) => true);
    // Include all discovered endpoints in the generated docs (helps minimal APIs + versioning)
    c.DocInclusionPredicate((docName, apiDesc) => true);
    // Group actions by group name if present, otherwise use default
    c.TagActionsBy(apiDesc => new[] { apiDesc.GroupName ?? "v1" });
});

// Database and app services
builder.Services.AddDbContext<DemoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\MSSQLLocalDB;Database=DemoDb;Trusted_Connection=True;MultipleActiveResultSets=true;"));
builder.Services.AddScoped<Domain.Repositories.ICountryRepository, Infrastructure.SQL.Repositories.CountryRepository>();
builder.Services.AddScoped<Domain.Services.ICountryService, BLL.Services.CountryService>();
builder.Services.AddScoped<AspNetCore8MinimalApis.Mapping.Interfaces.ICountryMapper, AspNetCore8MinimalApis.Mapping.CountryMapper>();
var dbConnection1 = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DemoContext>(options => options.UseSqlServer(dbConnection1));

builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryMapper, CountryMapper>();
builder.Services.AddScoped<ICountryService, CountryService>();

var app = builder.Build();

// Configure Swagger UI with versioning provider if available, otherwise use default Swagger UI
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }
});

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapGet("/countries", CountryEndpoints.GetCountries);

app.Run();