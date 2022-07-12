using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;

public class ExceptionMessage
{
    [JsonIgnore]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Text { get; set; }
    public string Stacktrace { get; set; }
    public string ServiceId { get; set; }
    public bool IsCritical { get; set; }
    public DateTime DateTime { get; set; }
}