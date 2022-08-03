using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

public class ActivationCode
{
    [JsonIgnore]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public DateTime CreationDateTime { get; set; }
    public string UserId { get; set; }
    public string Value { get; set; }
}