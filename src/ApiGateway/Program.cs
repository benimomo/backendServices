using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var reverseProxyConfig = builder.Configuration.GetSection("ReverseProxy");

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(reverseProxyConfig);

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("authenticatedUser", policy =>
        policy.RequireAuthenticatedUser());

    options.AddPolicy("adminOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Admin"));
});

var app = builder.Build();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/auth/v1/swagger.json", "Auth Service");
    options.SwaggerEndpoint("/swagger/books/v1/swagger.json", "Book Service");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();