using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Fiap.GrupoG.Mongo.Entities
{
    [BsonIgnoreExtraElements(true)]
    public class UserEntity
    {
        [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }
        [BsonElement("userId")] public long UserId { get; set; }
        [BsonElement("name")] public string Name { get; set; }
        [BsonElement("userName")] public string UserName { get; set; }
    }
}
