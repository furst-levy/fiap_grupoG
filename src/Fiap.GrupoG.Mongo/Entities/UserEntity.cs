using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Fiap.GrupoG.Mongo.Entities
{
    [BsonIgnoreExtraElements(true)]
    public class UserEntity
    {
        [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }
        [BsonElement("userId")] public Guid UserId { get; set; }
        [BsonElement("name")] public Guid Name { get; set; }
        [BsonElement("userName")] public Guid UserName { get; set; }
    }
}
