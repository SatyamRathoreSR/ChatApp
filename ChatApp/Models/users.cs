using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ChatApp.Models;

public class Users
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("Username")]
    public string Username { get; set; } = null!;

    [BsonElement("Password")]
    public string Password { get; set; } = null!;
}
