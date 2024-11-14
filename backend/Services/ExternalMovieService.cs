using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

public class ExternalMovieService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;

    public ExternalMovieService(HttpClient httpClient, AppDbContext context, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _context = context;

        var baseUrl = configuration["ExternalApi:BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new ArgumentNullException("ExternalApi:BaseUrl", "External API base URL configuration is missing");
        }
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public async Task<List<Models.Movie>> GetExternalMoviesAsync()
    {
        var response = await _httpClient.GetAsync("/MyMovies");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var externalMovies = JsonConvert.DeserializeObject<List<Models.Movie>>(content);

        return externalMovies ?? new List<Models.Movie>([]);
    }

    public async Task SaveNewMoviesAsync(List<Models.Movie> externalMovies)
    {
        if (externalMovies.Count == 0)
        {
            return;
        }

        var existingMovies = await _context.Movies.ToListAsync();
        var newMovies = externalMovies
            .Where(em => !existingMovies.Any(m => 
                m.ExternalApiId == em.Id || 
                (m.Title == em.Title && 
                 m.Year == em.Year && 
                 m.Director == em.Director)))
            .Select(m => new Models.Movie
            {
                Title = m.Title,
                Year = m.Year,
                Director = m.Director,
                Rate = m.Rate,
                ExternalApiId = m.Id
            })
            .ToList();

        if (newMovies.Any())
        {
            _context.Movies.AddRange(newMovies);
            await _context.SaveChangesAsync();
        }
    }
}
