using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ws.Dto;
using ws.Models;

namespace ws.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _collection = mongoDatabase.GetCollection<User>(mongoDBSettings.Value.UserCollectionName);
    }

    public async Task<(List<User> items, long total)> GetPaginatedAsync(RequestPaginatedUsersDto dto)
    {
        var filterBuilder = Builders<User>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrWhiteSpace(dto.Keyword))
        {
            var fnameFilter = filterBuilder.Regex(x => x.Firstname, new MongoDB.Bson.BsonRegularExpression(dto.Keyword, "i"));
            var lnameFilter = filterBuilder.Regex(x => x.Lastname, new MongoDB.Bson.BsonRegularExpression(dto.Keyword, "i"));
            var searchFilter = filterBuilder.Or(fnameFilter, lnameFilter);
            filter = filterBuilder.And(filter, searchFilter);
        }

        var total = await _collection.CountDocumentsAsync(filter);
        var skipAmount = Math.Max(0, (dto.Page > 0 ? dto.Page - 1 : 0) * dto.PageSize);

        var items = await _collection.Find(filter)
            .SortBy(x => x.CreatedAt)
            .Skip(skipAmount)
            .Limit(dto.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<User?> GetByUserIdAsync(string userId)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserId, userId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<string> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _collection.InsertOneAsync(user);
        return user.UserId;
    }

    public async Task UpdateAsync(string userId, User incomingUser)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserId, userId);
        var existing = await _collection.Find(filter).FirstOrDefaultAsync();
        if (existing == null) throw new KeyNotFoundException("User not found");

        var updateBuilder = Builders<User>.Update
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        if (!string.IsNullOrEmpty(incomingUser.Firstname)) updateBuilder = updateBuilder.Set(x => x.Firstname, incomingUser.Firstname);
        if (!string.IsNullOrEmpty(incomingUser.Lastname)) updateBuilder = updateBuilder.Set(x => x.Lastname, incomingUser.Lastname);
        if (!string.IsNullOrEmpty(incomingUser.Address)) updateBuilder = updateBuilder.Set(x => x.Address, incomingUser.Address);
        if (incomingUser.DateOfBirth != default) updateBuilder = updateBuilder.Set(x => x.DateOfBirth, incomingUser.DateOfBirth);
        if (incomingUser.Age != default) updateBuilder = updateBuilder.Set(x => x.Age, incomingUser.Age);

        await _collection.UpdateOneAsync(filter, updateBuilder);
    }

    public async Task DeleteUserAsync(string userId)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserId, userId);
        await _collection.DeleteOneAsync(filter);
    }
}
