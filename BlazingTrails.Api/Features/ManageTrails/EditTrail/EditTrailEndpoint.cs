using Ardalis.ApiEndpoints;
using BlazingTrails.Api.Persistence;
using BlazingTrails.Api.Persistence.Entities;
using BlazingTrails.Shared.Features.ManageTrails.EditTrail;
using BlazingTrails.Shared.Features.ManageTrails.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BlazingTrails.Api.Features.ManageTrails.EditTrail;

public class EditTrailEndpoint : EndpointBaseAsync.WithRequest<EditTrailRequest>.WithActionResult<bool>
{
    private readonly BlazingTrailsContext _database;

    public EditTrailEndpoint(BlazingTrailsContext database)
    {
        _database = database;
    }
    [Authorize]
    [HttpPut(EditTrailRequest.RouteTemplate)]
    public override async Task<ActionResult<bool>> HandleAsync(EditTrailRequest request, CancellationToken cancellationToken = default)
    {
        var trail = await _database.Trails.Include(x => x.Waypoints)
            .SingleOrDefaultAsync(x => x.Id == request.Trail.Id, cancellationToken: cancellationToken);

        if (trail is null)
        {
            return BadRequest("Trail could not be found.");
        }
        var email = 
            HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value
            ?? HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value
            ?? throw new Exception("Email not found in user claims");

        
        var isAdmin = HttpContext.User.IsInRole("Administrator");

        if (!trail.Owner.Equals(email, StringComparison.OrdinalIgnoreCase) && !isAdmin)
        {
            return Unauthorized();
        }

        trail.Name = request.Trail.Name;
        trail.Description = request.Trail.Description;
        trail.Location = request.Trail.Location;
        trail.TimeInMinutes = request.Trail.TimeInMinutes;
        trail.Length = request.Trail.Length;
        trail.Waypoints = request.Trail.Waypoints.Select(wp => new Waypoint
        {
            Latitude = wp.Latitude,
            Longitude = wp.Longitude
        }).ToList();

        if (request.Trail.ImageAction == ImageAction.Remove)
        {
            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Images", trail.Image!));
            trail.Image = null;
        }

        await _database.SaveChangesAsync(cancellationToken);

        return Ok(true);
    }
}