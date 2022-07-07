using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

public class RefreshToken
{
    [JsonIgnore]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Value { get; set; }
    public DateTime ExpireDateTime { get; set; }
    public DateTime CreationDateTime { get; set; }
    public string CreatorIp { get; set; }
    public DateTime? RevokeDateTime { get; set; }
    public string? RevokerIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }
    public string UserId { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpireDateTime;
    public bool IsRevoked => RevokeDateTime != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}