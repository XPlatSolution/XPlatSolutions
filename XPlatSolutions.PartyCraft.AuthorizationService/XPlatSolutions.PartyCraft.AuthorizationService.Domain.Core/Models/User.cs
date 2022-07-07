using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

public class User
{
    [JsonIgnore]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string? SecondName { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public string RoleId { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; }
    public string? Phone { get; set; }
    public string Email { get; set; }
    public bool EmailVerified { get; set; }

    [BsonIgnore]
    public Role Role { get; set; }

    [BsonIgnore]
    public List<RefreshToken> Tokens { get; set; }
}