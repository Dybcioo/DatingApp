using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.DTO;
using DatingApp.Entities;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DevExpress.Data.ODataLinq.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(user => user.Photos)
                .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(user => user.Photos)
                .SingleOrDefaultAsync(user => user.Id == id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users
                .Include(user => user.Photos)
                .SingleOrDefaultAsync(user => user.UserName == userName.ToLower());
        }

        public async Task<PageList<MemberDto>> GetAllMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            query = query.Where(user => user.UserName != userParams.CurrentUsername);
            query = query.Where(user => user.Gender == userParams.Gender); 

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(user => user.DateOfBirth >= minDob && user.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActivated)
            };

            return await PageList<MemberDto>.CreateAsync(query
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking(),
                userParams.PageNumber, userParams.PageSize);
        }

        public async Task<MemberDto> GetMemberByUserNameAsync(string userName)
        {
            return await _context.Users
                .Where(user => user.UserName == userName)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

    }
}