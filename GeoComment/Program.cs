using GeoComment.Data;
using GeoComment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<DatabaseHandler>();
builder.Services.AddScoped<GeoCommentHandler>();
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'api-version'VV";
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("api-version0.1", new OpenApiInfo()
    {
        Title = "Geo Comments API",
        Version ="api-version0.1"
    });
    options.SwaggerDoc("api-version0.2", new OpenApiInfo()
    {
        Title = "Geo Comments API",
        Version ="api-version0.2"
    });
});
builder.Services.AddDbContext<GeoCommentDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("GeoCommentDb")));

builder.Services.AddApiVersioning(options=>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(0, 1);
    options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/api-version0.1/swagger.json","api-version 0.1");
        options.SwaggerEndpoint("/swagger/api-version0.2/swagger.json", "api-version 0.2");
    });
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
