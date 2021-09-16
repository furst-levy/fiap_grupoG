﻿using Fiap.GrupoG.Mongo.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Fiap.GrupoG.Mongo.Service
{
    public class TweetRepository
    {
        public IMongoCollection<TweetEnriry> TweetCollection { get; set; }

        public TweetRepository(IConfiguration configurationRoot)
        {
            var client = new MongoClient(configurationRoot.GetSection("Conn").Value);
            var database = client.GetDatabase(configurationRoot.GetSection("DatabaseName").Value);
            TweetCollection =
                database.GetCollection<TweetEnriry>(configurationRoot.GetSection("TweetCollectionName").Value);
        }

        public async Task SaveTweetAsync(TweetEnriry tweetEnriry)
        {
            await TweetCollection.InsertOneAsync(tweetEnriry);
        }
    }
}
