using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MoviesApi.Models;

namespace MoviesApi.Services;

public class ReviewsService
{
    private readonly IMongoCollection<Review> _reviewsCollection;

    public ReviewsService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _reviewsCollection = mongoDatabase.GetCollection<Review>(mongoDbSettings.Value.ReviewsCollectionName);
    }

    public async Task<List<Review>> GetAsync() =>
        await _reviewsCollection.Find(_ => true).ToListAsync();

    public async Task<List<Review>> GetByMovieIdAsync(string movieId) =>
        await _reviewsCollection.Find(x => x.MovieId == movieId).ToListAsync();

    public async Task<Review?> GetAsync(string id) =>
        await _reviewsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Review newReview) =>
        await _reviewsCollection.InsertOneAsync(newReview);

    public async Task UpdateAsync(string id, Review updatedReview) =>
        await _reviewsCollection.ReplaceOneAsync(x => x.Id == id, updatedReview);

    public async Task RemoveAsync(string id) =>
        await _reviewsCollection.DeleteOneAsync(x => x.Id == id);
}