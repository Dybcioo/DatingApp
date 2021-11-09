using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.DTO;
using DatingApp.Entities;
using DatingApp.Helpers;

namespace DatingApp.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetAllUsersAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByUserNameAsync(string userName);

        Task<PageList<MemberDto>> GetAllMembersAsync(UserParams userParams);

        Task<MemberDto> GetMemberByUserNameAsync(string userName);
    }
}