using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.DTO;
using DatingApp.Entities;

namespace DatingApp.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetAllUsersAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByUserNameAsync(string userName);

        Task<IEnumerable<MemberDto>> GetAllMembersAsync();

        Task<MemberDto> GetMemberByUserNameAsync(string userName);
    }
}