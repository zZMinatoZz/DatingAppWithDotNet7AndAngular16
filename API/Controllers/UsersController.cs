using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUnitOfWork _uow;

        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;


        public UsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
        {
            _uow = uow;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            // var users = await _uow.UserRepository.GetUsersAsync();
            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            // return Ok(usersToReturn);

            // get username from jwt token 
            var currentUser = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUserName());
            userParams.CurrentUsername = currentUser.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                // if current user is man, only return woman
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            }

            var users = await _uow.UserRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

            return Ok(users);
        }

        [HttpGet("{username}")] // /api/users/2
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            // var user = await _uow.UserRepository.GetUserByUsernameAsync(username);
            // var userToReturn = _mapper.Map<MemberDto>(user);
            // return Ok(userToReturn);
            return await _uow.UserRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // get usuername from token
            // var username = User.GetUserName();
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUserName());
            if (user == null) return NotFound();
            _mapper.Map(memberUpdateDto, user);
            if (await _uow.Complete()) return NoContent();
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUserName());
            if (user == null) return NotFound();
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _uow.Complete())
            {
                return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("this photo is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(photo => photo.IsMain);

            if (currentMain != null) currentMain.IsMain = false;

            photo.IsMain = true;

            if (await _uow.Complete()) return NoContent();

            return BadRequest("Problem setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            var photoToDelete = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photoToDelete == null) return NotFound();

            if (photoToDelete.IsMain) return BadRequest("You cannot delete your main photo");

            if (photoToDelete.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photoToDelete.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photoToDelete);

            if (await _uow.Complete()) return Ok();

            return BadRequest("Problem deleting photo");
        }
    }
}