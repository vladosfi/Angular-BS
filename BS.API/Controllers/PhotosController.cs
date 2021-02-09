using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BS.API.Data;
using BS.API.Dtos;
using BS.API.Helpers;
using BS.API.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BS.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IBSRepository repo;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private Cloudinary cloudinary;

        public PhotosController(
            IBSRepository repo,
            IMapper mapper,
            IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this.repo = repo;
            this.mapper = mapper;
            this.cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                this.cloudinaryConfig.Value.CloudName,
                this.cloudinaryConfig.Value.ApiKey,
                this.cloudinaryConfig.Value.ApiSecret
            );

            this.cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = nameof(GetPhoto))]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await this.repo.GetPhoto(id);

            var photo = this.mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await this.repo.GetUser(userId);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                            .Width(500).Height(500).Crop("fill").Gravity("face"),
                        Folder = "bs-api",
                    };

                    uploadResult = this.cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.PublicId = uploadResult.PublicId;
            var photo = this.mapper.Map<Photo>(photoForCreationDto);

            photoForCreationDto.Url = uploadResult.Url.ToString();

            if (!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if (await this.repo.SaveAll())
            {
                var photoToReturn = this.mapper.Map<PhotoForReturnDto>(photo);
                //return CreatedAtRoute(nameof(this.GetPhoto), new { id = photo.Id }, photoToReturn);
                //return CreatedAtRoute("GetPhoto", new { controller = "Photos", id = photo.Id }, photoToReturn);
                //return RedirectToAction(nameof(this.GetPhoto), new { id = photo.Id });
                return this.RedirectToAction(nameof(this.GetPhoto), new { id = photo.Id });
            }

            return BadRequest("Coud not add the photo!");
        }

    }
}