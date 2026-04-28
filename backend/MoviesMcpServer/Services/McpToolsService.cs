using MoviesMcpServer.Models;
using System.Text.Json;

namespace MoviesMcpServer.Services;

public class McpToolsService
{
    private readonly MoviesApiClient _apiClient;

    public McpToolsService(MoviesApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public List<McpTool> GetAvailableTools()
    {
        return new List<McpTool>
        {
            new McpTool
            {
                Name = "search_movies",
                Description = "Search movies by title or genre. Returns matching movies.",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        query = new { type = "string", description = "Search query (title or genre keyword)" }
                    },
                    required = new[] { "query" }
                }
            },
            new McpTool
            {
                Name = "get_top_rated_movies",
                Description = "Get the top N rated movies based on average user reviews.",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        limit = new { type = "integer", description = "How many movies to return (default 5)" }
                    }
                }
            },
            new McpTool
            {
                Name = "get_movie_details",
                Description = "Get detailed information about a specific movie including all its reviews.",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        movie_id = new { type = "string", description = "The movie ID (MongoDB ObjectId, 24 chars)" }
                    },
                    required = new[] { "movie_id" }
                }
            },
            new McpTool
            {
                Name = "recommend_similar_movies",
                Description = "Recommend movies similar to a given movie based on genre.",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        movie_id = new { type = "string", description = "The movie ID to find similar movies for" }
                    },
                    required = new[] { "movie_id" }
                }
            }
        };
    }

    public async Task<string> ExecuteToolAsync(string toolName, JsonElement arguments)
    {
        return toolName switch
        {
            "search_movies" => await SearchMovies(arguments),
            "get_top_rated_movies" => await GetTopRatedMovies(arguments),
            "get_movie_details" => await GetMovieDetails(arguments),
            "recommend_similar_movies" => await RecommendSimilarMovies(arguments),
            _ => $"Unknown tool: {toolName}"
        };
    }

    private async Task<string> SearchMovies(JsonElement args)
    {
        var query = args.GetProperty("query").GetString()?.ToLower() ?? "";
        var movies = await _apiClient.GetAllMoviesAsync();

        var matches = movies.Where(m =>
            m.Title.ToLower().Contains(query) ||
            m.Genre.ToLower().Contains(query) ||
            m.Director.ToLower().Contains(query)
        ).ToList();

        if (matches.Count == 0)
            return $"No movies found matching '{query}'.";

        return string.Join("\n\n", matches.Select(m =>
            $"**{m.Title}** ({m.Year}) — {m.Genre}\n" +
            $"Directed by {m.Director}\n" +
            $"ID: {m.Id}\n" +
            $"{m.Description}"
        ));
    }

    private async Task<string> GetTopRatedMovies(JsonElement args)
    {
        int limit = 5;
        if (args.TryGetProperty("limit", out var limitProp))
            limit = limitProp.GetInt32();

        var movies = await _apiClient.GetAllMoviesAsync();

        var ratedMovies = new List<(Movie Movie, double AvgRating, int ReviewCount)>();
        foreach (var movie in movies)
        {
            if (string.IsNullOrEmpty(movie.Id)) continue;
            var reviews = await _apiClient.GetReviewsForMovieAsync(movie.Id);
            if (reviews.Count == 0) continue;
            var avg = reviews.Average(r => r.Rating);
            ratedMovies.Add((movie, avg, reviews.Count));
        }

        var top = ratedMovies
            .OrderByDescending(x => x.AvgRating)
            .ThenByDescending(x => x.ReviewCount)
            .Take(limit)
            .ToList();

        if (top.Count == 0)
            return "No rated movies in the database yet.";

        return $"Top {top.Count} rated movies:\n\n" +
            string.Join("\n", top.Select((x, i) =>
                $"{i + 1}. **{x.Movie.Title}** ({x.Movie.Year}) — " +
                $"⭐ {x.AvgRating:F1}/5 ({x.ReviewCount} reviews)"
            ));
    }

    private async Task<string> GetMovieDetails(JsonElement args)
    {
        var movieId = args.GetProperty("movie_id").GetString() ?? "";
        var movie = await _apiClient.GetMovieByIdAsync(movieId);

        if (movie is null)
            return $"Movie with ID '{movieId}' not found.";

        var reviews = await _apiClient.GetReviewsForMovieAsync(movieId);
        var avgRating = reviews.Count > 0 ? reviews.Average(r => r.Rating) : 0;

        var result = $"**{movie.Title}** ({movie.Year})\n" +
                     $"Director: {movie.Director}\n" +
                     $"Genre: {movie.Genre}\n" +
                     $"Description: {movie.Description}\n\n";

        if (reviews.Count > 0)
        {
            result += $"⭐ Average rating: {avgRating:F1}/5 ({reviews.Count} reviews)\n\n";
            result += "Reviews:\n";
            result += string.Join("\n", reviews.Select(r =>
                $"- **{r.UserName}** ({r.Rating}/5): {r.Comment}"
            ));
        }
        else
        {
            result += "No reviews yet.";
        }

        return result;
    }

    private async Task<string> RecommendSimilarMovies(JsonElement args)
    {
        var movieId = args.GetProperty("movie_id").GetString() ?? "";
        var sourceMovie = await _apiClient.GetMovieByIdAsync(movieId);

        if (sourceMovie is null)
            return $"Source movie with ID '{movieId}' not found.";

        var allMovies = await _apiClient.GetAllMoviesAsync();
        var similar = allMovies
            .Where(m => m.Id != sourceMovie.Id &&
                        m.Genre.Equals(sourceMovie.Genre, StringComparison.OrdinalIgnoreCase))
            .Take(5)
            .ToList();

        if (similar.Count == 0)
            return $"No other movies found in the genre '{sourceMovie.Genre}'.";

        return $"Movies similar to **{sourceMovie.Title}** (genre: {sourceMovie.Genre}):\n\n" +
            string.Join("\n", similar.Select(m =>
                $"- **{m.Title}** ({m.Year}) directed by {m.Director}"
            ));
    }
}