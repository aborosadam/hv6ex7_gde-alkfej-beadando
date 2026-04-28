using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MoviesApi.Models;

namespace MoviesApi.Services;

public class MoviesService
{
    private readonly IMongoCollection<Movie> _moviesCollection;

    public MoviesService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _moviesCollection = mongoDatabase.GetCollection<Movie>(mongoDbSettings.Value.MoviesCollectionName);
    }

    public async Task<List<Movie>> GetAsync(int page = 1, int pageSize = 10)
    {
        return await _moviesCollection.Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<long> GetCountAsync() =>
        await _moviesCollection.CountDocumentsAsync(_ => true);

    public async Task<Movie?> GetAsync(string id) =>
        await _moviesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Movie newMovie) =>
        await _moviesCollection.InsertOneAsync(newMovie);

    public async Task UpdateAsync(string id, Movie updatedMovie) =>
        await _moviesCollection.ReplaceOneAsync(x => x.Id == id, updatedMovie);

    public async Task RemoveAsync(string id) =>
        await _moviesCollection.DeleteOneAsync(x => x.Id == id);
}