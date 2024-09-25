using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Enums;
using minimal_api.Infrasctructure.DB;
using minimal_api.Infrasctructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key))
    key = "12345";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Autorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insert the JWT token here"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<DbContextMinimal>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql")));
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Administrators

string GenerateJwtToken(Administrator administrator)
{
    if (string.IsNullOrEmpty(key))
        return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrator.Email),
        new Claim("Role", administrator.Role),
        new Claim(ClaimTypes.Role, administrator.Role)
        
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    var adm = administratorService.Login(loginDTO);

    if (adm != null)
    {
        string token = GenerateJwtToken(adm);
        return Results.Ok(new LoggedAdministrator
        {
            Email = adm.Email,
            Role = adm.Role,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous().WithTags("Administrators");

app.MapGet("/administrators/", ([FromQuery] int? pages, IAdministratorService administratorService) =>
{
    var adms = new List<AdministratorModelView>();
    var administrators = administratorService.GetAllAdministrators(pages);

    foreach (var adm in administrators)
    {
        adms.Add(new AdministratorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Role = adm.Role
        });
    }

    return Results.Ok(adms);

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administrators");

app.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
{
    var administrator = administratorService.GetAdministratorById(id);

    if (administrator == null)
        return Results.NotFound();

    return Results.Ok(new AdministratorModelView
    {
        Id = administrator.Id,
        Email = administrator.Email,
        Role = administrator.Role
    }
    );

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administrators");

app.MapPost("/administrators/", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
{
    var validation = new ValidationErrors
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(administratorDTO.Email))
        validation.Messages.Add("Email can't be empty.");

    if (string.IsNullOrEmpty(administratorDTO.Password))
        validation.Messages.Add("type a valid password.");

    if (administratorDTO.Role == null)
        validation.Messages.Add("We need the role field to identify you.");

    if (validation.Messages.Count > 0)
        return Results.BadRequest(validation);

    var administrator = new Administrator
    {
        Email = administratorDTO.Email,
        Password = administratorDTO.Password,
        Role = administratorDTO.Role.ToString() ?? Roles.Editor.ToString()
    };

    administratorService.Create(administrator);

    return Results.Created($"/administrators/{administrator.Id}", new AdministratorModelView
    {
        Id = administrator.Id,
        Email = administrator.Email,
        Role = administrator.Role
    });

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administrators");
#endregion

#region Vehicles
ValidationErrors ValidaDTO(VehicleDTO vehicleDTO)
{
    var validation = new ValidationErrors
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Name))
        validation.Messages.Add("The name can't be empty.");

    if (string.IsNullOrEmpty(vehicleDTO.Brand))
        validation.Messages.Add("The brand can't be blank.");

    if (vehicleDTO.Year < 1950)
        validation.Messages.Add("Vehicle too old, we accept only vehicle higher than 1950.");

    return validation;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var validation = ValidaDTO(vehicleDTO);
    if (validation.Messages.Count > 0)
        return Results.BadRequest(validation);

    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year
    };

    vehicleService.CreateVehicle(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" }).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetAllVehicles(page);

    return Results.Ok(vehicles);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);

    if (vehicle == null)
        return Results.NotFound();

    return Results.Ok(vehicle);

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);
    if (vehicle == null)
        return Results.NotFound();

    var validation = ValidaDTO(vehicleDTO);
    if (validation.Messages.Count > 0)
        return Results.BadRequest(validation);

    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.UpdateVehicle(vehicle);

    return Results.Ok(vehicle);

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);

    if (vehicle == null)
        return Results.NotFound();

    vehicleService.DeleteVehicleById(vehicle);

    return Results.NoContent();

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Vehicles");
#endregion

#region app
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion
