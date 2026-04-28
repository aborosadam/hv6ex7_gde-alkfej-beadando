using MoviesMcpServer.Models;
using System.Net.Http.Json;

namespace MoviesMcpServer.Services;

public class MoviesApiClient
{
    private readonly HttpClient _http;

    public MoviesApiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        var baseUrl = config["MoviesApi:BaseUrl"] ?? "http://localhost:5062";
        _http.BaseAddress = new Uri(baseUrl);
    }

    public async Task<List<Movie>> GetAllMoviesAsync()
    {
        var allMovies = new List<Movie>();
        int page = 1;
        const int pageSize = 100;

        while (true)
        {
            var response = await _http.GetFromJsonAsync<MoviesPage>(
                $"/api/Movies?page={page}&pageSize={pageSize}");

            if (response is null || response.Items.Count == 0) break;

            allMovies.AddRange(response.Items);

            if (allMovies.Count >= response.TotalCount) break;
            page++;
        }

        return allMovies;
    }

    public async Task<Movie?> GetMovieByIdAsync(string id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Movie>($"/api/Movies/{id}");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<List<Review>> GetReviewsForMovieAsync(string movieId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Review>>(
                $"/api/Reviews?movieId={movieId}") ?? new List<Review>();
        }
        catch (HttpRequestException)
        {
            return new List<Review>();
        }
    }
}