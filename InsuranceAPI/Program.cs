
using InsuranceAPI.Data;
using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;

//register services and configure the database context
var builder = WebApplication.CreateBuilder(args);
//services
builder.Services.AddControllers();
// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token"
    });
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    
});

builder.Services.AddDbContext<InsuranceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("InsuranceConnection")));

var jwtSettings = builder.Configuration.GetSection("Jwt");

// The signing key is a secret, so it is never stored in appsettings.json.
// Locally it comes from user secrets; in Azure it comes from the Jwt__Key app setting.
var jwtKey = jwtSettings["Key"] ?? string.Empty;
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key is missing or shorter than 32 bytes. Set it with " +
        "'dotnet user-secrets set \"Jwt:Key\" \"<secret>\"' locally, " +
        "or the 'Jwt__Key' app setting in Azure.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,//Confirms the token came from our API
            ValidateAudience = true,//Confirms the token is intended for our application
            ValidateLifetime = true,//Rejects expired tokens
            ValidateIssuerSigningKey = true,//Validates the token's signature

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.Zero//Removes the default expiration grace period
        };
    });

    builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200","https://mango-grass-094671400.6.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization();//Enables authorization policies and roles

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure middleware order
app.UseHttpsRedirection();
app.UseCors("AngularPolicy");
app.UseAuthentication();//Adds authentication middleware to the request pipeline
app.UseAuthorization();//Adds authorization middleware to the request pipeline
app.MapControllers();//Maps attribute-routed controller actions to HTTP endpoints

app.Run();


