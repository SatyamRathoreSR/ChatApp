using MongoDB.Bson.Serialization.Attributes;

namespace ChatApp.Models;

public class Chats
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? Id { get; set; } = null!;
    public string? From { get; set; } = null!;
    public string? To { get; set; } = null!;
    public string? Message { get; set; } = null!;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
