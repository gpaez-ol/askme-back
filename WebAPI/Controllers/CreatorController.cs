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
    [Route("creator")]
    public class CreatorController : ControllerBase
    {
        private readonly CreatorLogic _creatorLogic;
        public CreatorController(CreatorLogic creatorLogic)
        {
            _creatorLogic = creatorLogic;
        }

        /*CreatorItemDTO*/
        //Get controller
        // / <summary>
        // / This Get method brings a list of creators
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult<PaginationResult<CreatorItemDTO>>> GetCreators(Guid userId, [FromQuery] int page = 0, [FromQuery] int pageSize = 10)
        {
            var posts = await _creatorLogic.GetCreators(userId, page, pageSize);
            return Ok(posts);
        }
        //PUT controller
        // / <summary>
        // / This PUT method follows a creator
        // / </summary>
        // / <returns>Ok()</returns>
        [HttpPut("follow/{creatorId}/{userId}")]
        public async Task<ActionResult> FollowCreator(Guid creatorId, Guid userId)
        {
            await _creatorLogic.FollowCreator(creatorId, userId);
            return Ok();
        }
        //PUT controller
        // / <summary>
        // / This PUT method unfollows a creator
        // / </summary>
        // / <returns>Ok()</returns>
        [HttpPut("unfollow/{creatorId}/{userId}")]
        public async Task<ActionResult> UnFollowCreator(Guid creatorId, Guid userId)
        {
            await _creatorLogic.UnFollowCreator(creatorId, userId);
            return Ok();
        }
        /*CreatorItemDTO*/
        //Get controller
        // / <summary>
        // / This Get method brings a list of creators
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpGet("posts/{creatorId}/{userId}")]
        public async Task<ActionResult<PaginationResult<PostItemDTO>>> GetPosts(Guid creatorId, Guid userId, [FromQuery] int page = 0, [FromQuery] int pageSize = 10)
        {
            var posts = await _creatorLogic.GetPosts(creatorId, userId, page, pageSize);
            return Ok(posts);
        }
    }
}
