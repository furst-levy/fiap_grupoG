using Fiap.GrupoG.Mongo.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Fiap.GrupoG.Mongo.Service
{
    public class UserRepository
    {
        public IMongoCollection<UserEntity> UserCollection { get; set; }

        public UserRepository(IConfiguration configurationRoot)
        {
            var client = new MongoClient(configurationRoot.GetSection("Conn").Value);
            var database = client.GetDatabase(configurationRoot.GetSection("DatabaseName").Value);
            UserCollection =
                database.GetCollection<UserEntity>(configurationRoot.GetSection("UserCollectionName").Value);
        }

        public async Task SaveTweetAsync(UserEntity userEntity)
        {
            await UserCollection.InsertOneAsync(userEntity);
        }
    }
}
