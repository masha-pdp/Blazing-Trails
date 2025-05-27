using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using MediatR;
using BlazingTrails.Shared.Features.Home.Shared;
namespace BlazingTrails.Client.Features.Home;




public partial class HomePage
{
    // [Inject] private HttpClient Http { get; set; } = default!;
    private Trail? _selectedTrail;

    private IEnumerable<Trail> _trails; 

    protected override async Task OnInitializedAsync()
    {
        try
        {
            // _trails = await Http.GetFromJsonAsync<IEnumerable<Trail>>("trails/trail-data.json");
            var response = await Mediator.Send(new GetTrailsRequest());
            _trails = response.Trails.Select(x => new Trail
            {
                Id = x.Id,
                Name = x.Name,
                Image = x.Image,
                Description = x.Description,
                Location = x.Location,
                Length = x.Length,
                TimeInMinutes = x.TimeInMinutes,
                Waypoints = x.Waypoints.Select(wp => new BlazingTrails.ComponentLibrary.Map.LatLong (wp.Latitude, wp.Longitude)).ToList()
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was a problem loading trail data:{ex.Message}");
            
        }
    }
    
    private void HandleTrailSelected(Trail trail)
        => _selectedTrail = trail;
    
}