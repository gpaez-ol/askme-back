using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAPI.Models;
using AskMe.Data.DTO;
using AskMe.Repositories;
using AskMe.Logic;
using AsKMe.Pagination;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("profile")]
    public class ProfileController : ControllerBase
    {
        private readonly CreatorLogic _creatorLogic;
        public ProfileController(CreatorLogic creatorLogic)
        {
            _creatorLogic = creatorLogic;
        }
        /*ProfileDTO*/
        //Get controller
        // / <summary>
        // / This Get method brings the profile of a user
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult<ProfileDTO>> GetProfile(Guid userId)
        {
            var posts = await _creatorLogic.GetProfile(userId);
            return Ok(posts);
        }
        /*ProfileDTO*/
        //Get controller
        // / <summary>
        // / This Get method brings the profile of a user
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpGet("followers/{userId}/{creatorId}")]
        public async Task<ActionResult<ICollection<ProfileItemDTO>>> GetFollowers(Guid userId, Guid creatorId)
        {
            var followers = await _creatorLogic.GetFollowers(userId, creatorId);
            return Ok(followers);
        }
        /*ProfileDTO*/
        //Get controller
        // / <summary>
        // / This Get method brings the profile of a user
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpGet("following/{userId}/{creatorId}")]
        public async Task<ActionResult<ICollection<ProfileItemDTO>>> GetFollowing(Guid userId, Guid creatorId)
        {
            var followings = await _creatorLogic.GetFollowing(userId, creatorId);
            return Ok(followings);
        }
    }
}
