using System.Text.Json.Serialization;

namespace MoviesMcpServer.Models;

public class Movie
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("director")]
    public string Director { get; set; } = string.Empty;

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("genre")]
    public string Genre { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("posterUrl")]
    public string PosterUrl { get; set; } = string.Empty;
}

public class MoviesPage
{
    [JsonPropertyName("items")]
    public List<Movie> Items { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public long TotalCount { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
}

public class Review
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("movieId")]
    public string MovieId { get; set; } = string.Empty;

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public int Rating { get; set; }

    [JsonPropertyName("comment")]
    public string Comment { get; set; } = string.Empty;
}