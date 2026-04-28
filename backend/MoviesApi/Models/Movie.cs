using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoviesApi.Models;

public class Movie
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
}