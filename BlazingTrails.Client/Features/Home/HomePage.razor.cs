using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazingTrails.Client.Features.Home;

public partial class HomePage
{
    [Inject] private HttpClient Http { get; set; } = default!;
    private Trail? _selectedTrail;

    private IEnumerable<Trail> _trails; 

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _trails = await Http.GetFromJsonAsync<IEnumerable<Trail>>("trails/trail-data.json");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was a problem loading trail data:{ex.Message}");
        }
    }
    
    private void HandleTrailSelected(Trail trail)
        => _selectedTrail = trail;
    
}