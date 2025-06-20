using Ardalis.ApiEndpoints;
using BlazingTrails.Api.Persistence;
using BlazingTrails.Api.Persistence.Entities;
using BlazingTrails.Shared.Features.ManageTrails.AddTrail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace BlazingTrails.Api.Features.ManageTrails;

public class AddTrailEndpoint : EndpointBaseAsync.WithRequest<AddTrailRequest>.WithActionResult<int>
{
    private readonly BlazingTrailsContext _database;
    private readonly ILogger<AddTrailEndpoint> _logger;

    public AddTrailEndpoint(BlazingTrailsContext database, ILogger<AddTrailEndpoint> logger)
    {   
        _database = database;
        _logger = logger;
    }
    [Authorize]
    [HttpPost(AddTrailRequest.RouteTemplate)]
    public override async Task<ActionResult<int>> HandleAsync(AddTrailRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User.Identity.Name: {Name}", HttpContext.User.Identity?.Name);

        foreach (var claim in HttpContext.User.Claims)
        {
            _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
        }
        
        var trail = new BlazingTrails.Api.Persistence.Entities.Trail
        {
            Name = request.Trail.Name,
            Description = request.Trail.Description,
            Location = request.Trail.Location,
            TimeInMinutes = request.Trail.TimeInMinutes,
            Length = request.Trail.Length,
            Owner = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                    ?? HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value
                    ?? throw new Exception("Email not found in user claims"),
            Waypoints = request.Trail.Waypoints.Select(wp => new Waypoint
            {
                Latitude = wp.Latitude,
                Longitude = wp.Longitude
                
            }).ToList()
        };

        await _database.Trails.AddAsync(trail, cancellationToken);

        // var routeInstructions = request.Trail.Route.Select(x => new Persistence.Entities.RouteInstruction
        // {
        //     Stage = x.Stage,
        //     Description = x.Description,
        //     Trail = trail
        // });
        //
        // await _database.RouteInstructions.AddRangeAsync(routeInstructions, cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);

        return Ok(trail.Id);
    }
}
