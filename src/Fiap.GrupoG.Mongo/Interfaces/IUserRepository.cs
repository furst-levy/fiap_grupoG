using System.Collections.Generic;
using Fiap.GrupoG.Mongo.Entities;
using System.Threading.Tasks;

namespace Fiap.GrupoG.Mongo.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> ListUsersAsync();
        Task SaveUserAsync(UserEntity userEntity);
    }
}
