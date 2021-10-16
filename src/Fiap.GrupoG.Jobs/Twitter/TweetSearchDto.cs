using System;
using Fiap.GrupoG.Mongo.Entities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fiap.GrupoG.Jobs.Twitter
{
    public class TweetSearchDto
    {
        public TweetSearchDto()
        {
            Data = new List<TweetSearchItem>();
        }

        [JsonPropertyName("data")]
        public IEnumerable<TweetSearchItem> Data { get; set; }

        public virtual UserEntity User { get; set; }

        public class TweetSearchItem
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("text")]
            public string Text { get; set; }

            [JsonPropertyName("created_at")]
            public DateTime CreatedAt { get; set; }
        }
    }
}
