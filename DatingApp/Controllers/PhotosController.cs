using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Extensions;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DatingApp.Controllers
{
    [Authorize]
    public class PhotosController : BaseApiController
    {
        private readonly IPhotoService _photoService;
        private readonly IUserRepository _userRepository;

        public PhotosController(IPhotoService photoService,IUserRepository userRepository)
        {
            _photoService = photoService;
            _userRepository = userRepository;
        }

        [HttpGet("{fileName}")]
        public IActionResult GetPhoto(string fileName)
        {
            return PhysicalFile(_photoService.GetPhoto(fileName), "image/jpeg");
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeletePhoto(string fileName)
        {
            if(!_photoService.DeletePhoto(fileName)) return BadRequest("File not found");

            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Url.Contains(fileName));

            user.Photos.Remove(photo);

            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem Remove photo");
        }
    }
}
