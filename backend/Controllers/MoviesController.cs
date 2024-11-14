using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ExternalMovieService _externalMovieService;
    public MoviesController(AppDbContext context, ExternalMovieService externalMovieService)
    {
        _context = context;
        _externalMovieService = externalMovieService;
    }

    // GET: api/Movies - get all movies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Models.Movie>>> GetMovies()
    {
        return await _context.Movies.ToListAsync();
    }

    // GET: api/Movies/5 - get movie with id
    [HttpGet("{id}")]
    public async Task<ActionResult<Models.Movie>> GetMovie(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return movie;
    }

    // POST: api/Movies - add new movie
    [HttpPost]
    public async Task<ActionResult<Models.Movie>> PostMovie(Models.Movie movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
    }

    // PUT: api/Movies/5 - update movie with id
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMovie(int id, Models.Movie movie)
    {
        if (id != movie.Id)
        {
            return BadRequest();
        }

        _context.Entry(movie).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MovieExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Movies/5 - remove movie with id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // helper method to check if movie exists in database
    private bool MovieExists(int id)
    {
        return _context.Movies.Any(e => e.Id == id);
    }

    [HttpPost("fetch-external")]
    public async Task<IActionResult> FetchExternalMovies()
    {
        var externalMovies = await _externalMovieService.GetExternalMoviesAsync();
        var existingMovies = await _context.Movies.ToListAsync();
        
        var newMovies = externalMovies
            .Where(em => !existingMovies.Any(m => 
                m.ExternalApiId == em.Id || 
                (m.Title == em.Title &&     
                 m.Year == em.Year && 
                 m.Director == em.Director)))
            .ToList();
            
        await _externalMovieService.SaveNewMoviesAsync(externalMovies);

        return Ok(new { 
            message = "Movies fetched and saved.",
            newMoviesCount = newMovies.Count
        });
    }
}
