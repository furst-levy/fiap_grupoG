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

        public class TweetSearchItem
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("text")]
            public string Text { get; set; }

            public TweetEnriry ConverterParaTabela(UserEntity userEntity)
            {
                return new TweetEnriry
                {
                    User = new TweetEnriry.UserEntity
                    {
                        UserId = userEntity.UserId,
                        Name = userEntity.Name,
                        UserName = userEntity.UserName
                    }
                };
            }
        }
    }
}
