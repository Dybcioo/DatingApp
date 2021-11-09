using DatingApp.Data;
using DatingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.DTO;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace DatingApp.Controllers
{
 
    [Authorize]
    public class UsersController : BaseApiController 
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            userParams.CurrentUsername = user.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            var users = await _userRepository.GetAllMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalPages, users.TotalCount);

            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberByUserNameAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile photo)
        {
           var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

           var host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api";

           var photoResult = await _photoService.AddPhotoAsync(photo, host);

           if (photoResult.Error != null) return BadRequest(photoResult.Error.Message);

           var createPhoto = new Photo()
           {
               Url = photoResult.PhotoPath
           };

           if (user.Photos.Count == 0)
           {
               createPhoto.IsMain = true;
           }

           user.Photos.Add(createPhoto);

           if (await _userRepository.SaveAllAsync())
           {
               return CreatedAtRoute("GetUser", new {username = user.UserName}, _mapper.Map<PhotoDto>(createPhoto));
           }

           return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo"); 

            var mainPhoto = user.Photos.FirstOrDefault(x => x.IsMain);

            if(mainPhoto != null) mainPhoto.IsMain = false;

            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem change main photo");
        }
    }
}
