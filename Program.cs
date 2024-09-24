using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Enums;
using minimal_api.Infrasctructure.DB;
using minimal_api.Infrasctructure.Services;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.Arm;

var builder = WebApplication.CreateBuilder(args);

#region Builder
builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContextMinimal>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administrators
app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
    {
        return Results.Ok("Login authorized.");
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithTags("Administrators");

app.MapGet("/administrators/", ([FromQuery] int? pages, IAdministratorService administratorService) =>
{
    var adms = new List<AdministratorModelView>();
    var administrators = administratorService.GetAllAdministrators(pages);
    foreach(var adm in administrators)
    {
        adms.Add(new AdministratorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Role = adm.Role
        });
    }

    return Results.Ok(adms);

}).WithTags("Administrators");

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

}).WithTags("Administrators");

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

}).WithTags("Administrators");
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
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetAllVehicles(page);

    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);

    if(vehicle == null)
        return Results.NotFound();

    return Results.Ok(vehicle);

}).WithTags("Vehicles");

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

}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);

    if (vehicle == null)
        return Results.NotFound();

    vehicleService.DeleteVehicleById(vehicle);

    return Results.NoContent();

}).WithTags("Vehicles");
#endregion

#region app
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
