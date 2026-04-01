using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ws.Models;

[BsonIgnoreExtraElements]
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonElement("firstname")]
    public string Firstname { get; set; } = default!;

    [BsonElement("lastname")]
    public string Lastname { get; set; } = default!;

    [BsonElement("address")]
    public string Address { get; set; } = default!;

    [BsonElement("dateOfBirth")]
    public DateTime DateOfBirth { get; set; } = default!;

    [BsonElement("age")]
    public int Age { get; set; } = default!;

}
