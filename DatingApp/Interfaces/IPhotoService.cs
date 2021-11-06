using System.Drawing;
using System.Threading.Tasks;
using DatingApp.Helpers.HO;
using Microsoft.AspNetCore.Http;

namespace DatingApp.Interfaces
{
    public interface IPhotoService
    {
        Task<PhotoResultHO> AddPhotoAsync(IFormFile photo, string host);

        string GetPhoto(string photoPath);

        bool DeletePhoto(string path);
    }
}
