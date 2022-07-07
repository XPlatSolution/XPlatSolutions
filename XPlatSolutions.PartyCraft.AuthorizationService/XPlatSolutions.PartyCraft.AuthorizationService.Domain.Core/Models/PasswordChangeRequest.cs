using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

public class PasswordChangeRequest
{
    [JsonIgnore]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public DateTime RequestDateTime { get; set; }
    public string UserId { get; set; }

}