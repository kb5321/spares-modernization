using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpaRes.Api.Data;
using SpaRes.Api.Models;
using SpaRes.Api.Services;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SpaResDbContext>(options =>
    options.UseSqlite("Data Source=spares.db"));


/*
 * 
 is basically saying:

    - Register SpaResDbContext as a service that DI knows how to construct.
    - When you create its options, configure EF Core to use SQLite and this database file
 *
 **/


// Add services to the container.
builder.Services.AddScoped<ReservationAuditService>();

builder.Services.AddControllers();
builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "SpaRes.Api",
            ValidAudience = "SpaRes.Web",

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "SpaRes-Development-Key-Change-Me-2026-123456789"
                )
            )

        };
        options.IncludeErrorDetails = true;
    
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(
                    $"JWT AUTH FAILED: {context.Exception.Message}"
                );

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaResWeb", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
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

app.UseCors("SpaResWeb");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
