using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Fiap.GrupoG.Mongo.Entities
{
    [BsonIgnoreExtraElements(true)]
    public class TweetEnriry
    {
        [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }
        [BsonElement("user")] public UserEntity User { get; set; }
        [BsonElement("comprehend")] public ComprehendEntity Comprehend { get; set; }

        public class UserEntity
        {
            [BsonElement("userId")] public long UserId { get; set; }
            [BsonElement("name")] public string Name { get; set; }
            [BsonElement("userName")] public string UserName { get; set; }
        }

        public class ComprehendEntity
        {
            [BsonElement("detectEntities")] public DetectEntitiesDb DetectEntities { get; set; }
            [BsonElement("detectKeyPhrases")] public DetectKeyPhrasesDb DetectKeyPhrases { get; set; }
            [BsonElement("detectSentimentDb")] public DetectSentimentDb DetectSentiment { get; set; }

            public class DetectEntitiesDb
            {
                [BsonElement("text")] public string Text { get; set; }
                [BsonElement("type")] public string Type { get; set; }
                [BsonElement("score")] public float Score { get; set; }
            }

            public class DetectKeyPhrasesDb
            {
                [BsonElement("text")] public string Text { get; set; }
                [BsonElement("score")] public float Score { get; set; }
            }

            public class DetectSentimentDb
            {
                [BsonElement("sentiment")] public string Sentiment { get; set; }
                [BsonElement("scoreMixed")] public float ScoreMixed { get; set; }
                [BsonElement("scoreNeutral")] public float ScoreNeutral { get; set; }
                [BsonElement("scorePositive")] public float ScorePositive { get; set; }
                [BsonElement("scoreNegative")] public float ScoreNegative { get; set; }
            }
        }
    }
}
