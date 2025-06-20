using Ardalis.ApiEndpoints;
using BlazingTrails.Api.Persistence;
using BlazingTrails.Shared.Features.ManageTrails.EditTrail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace BlazingTrails.Api.Features.ManageTrails.EditTrail;

public class GetTrailEndpoint : EndpointBaseAsync.WithRequest<int>.WithActionResult<GetTrailRequest.Response>
{
    private readonly BlazingTrailsContext _context;

    public GetTrailEndpoint(BlazingTrailsContext context)
    {
        _context = context;
    }
    [Authorize]
    [HttpGet(GetTrailRequest.RouteTemplate)]
    public override async Task<ActionResult<GetTrailRequest.Response>> HandleAsync(int trailId, CancellationToken cancellationToken = default)
    {
        var trail = await _context.Trails.Include(x => x.Waypoints)
            .SingleOrDefaultAsync(x => x.Id == trailId, cancellationToken: cancellationToken);

        if (trail is null)
        {
            return BadRequest("Trail could not be found.");
        }
        
        var email = 
            HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value
            ?? HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value
            ?? throw new Exception("Email not found in user claims");
        
        var role = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            ?.Value;

        if (!trail.Owner.Equals(email, StringComparison.OrdinalIgnoreCase) && role != "Administrator")
        {
            return Unauthorized();
        }

        var response = new GetTrailRequest.Response(new GetTrailRequest.Trail(trail.Id,
            trail.Name,
            trail.Location,
            trail.Image,
            trail.TimeInMinutes,
            trail.Length,
            trail.Description,
            trail.Waypoints.Select(wp => new GetTrailRequest.Waypoint(wp.Latitude,
                wp.Longitude))));

        return Ok(response);
    }
}