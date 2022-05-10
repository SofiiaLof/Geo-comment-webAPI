using System.Text;
using GeoComment.Data;
using GeoComment.Models;
using GeoComment.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<DatabaseHandler>();
builder.Services.AddScoped<JwtPrinter>();
builder.Services.AddScoped<GeoCommentHandler>();
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContext<GeoCommentDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("GeoCommentDb")));

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<GeoCommentDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuerSigningKey = true,

            ValidateLifetime = true,
            RequireExpirationTime = true,

            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(0, 1);
    options.AssumeDefaultVersionWhenUnspecified = true;

    options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'api-version'VV";
});

builder.Services.AddSwaggerGen(options =>
{
   options.AddSecurityDefinition("BearerToken", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
    });

    options.SwaggerDoc("api-version0.1", new OpenApiInfo()
    {
        Title = "Geo Comments API",
        Version ="0.1"
    });
    options.SwaggerDoc("api-version0.2", new OpenApiInfo()
    {
        Title = "Geo Comments API",
        Version ="0.2"
    });

   
        options.OperationFilter<SecurityRequirementsOperationFilter>();
   

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/api-version0.1/swagger.json","0.1");
        options.SwaggerEndpoint("/swagger/api-version0.2/swagger.json", "0.2");
    });
}



app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
