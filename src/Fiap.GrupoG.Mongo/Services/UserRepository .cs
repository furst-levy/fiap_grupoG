using Fiap.GrupoG.Mongo.Entities;
using Fiap.GrupoG.Mongo.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fiap.GrupoG.Mongo.Services
{
    public class UserRepository : IUserRepository
    {
        public IMongoCollection<UserEntity> UserCollection { get; set; }

        public UserRepository(IConfiguration configurationRoot)
        {
            var client = new MongoClient(configurationRoot.GetSection("Conn").Value);
            var database = client.GetDatabase(configurationRoot.GetSection("DatabaseName").Value);
            UserCollection =
                database.GetCollection<UserEntity>(configurationRoot.GetSection("UserCollectionName").Value);
        }

        public async Task<IEnumerable<UserEntity>> ListUsersAsync()
        {
            return await UserCollection.AsQueryable().ToListAsync();
        }

        public async Task SaveUserAsync(UserEntity userEntity)
        {
            await UserCollection.InsertOneAsync(userEntity);
        }
    }
}
