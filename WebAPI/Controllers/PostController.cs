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
    [Route("post")]
    public class PostController : ControllerBase
    {
        private readonly PostLogic _postLogic;
        public PostController(PostLogic postLogic)
        {
            _postLogic = postLogic;
        }

        //POST controller
        // / <summary>
        // / This POST method creates a Post
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpPost("{userId}")]
        public async Task<ActionResult<PostDTO>> CreatePost([FromBody] PostCreateDTO post, Guid userId)
        {
            var newPost = await _postLogic.CreatePost(post, userId);
            return Ok(newPost);
        }
        //PUT controller
        // / <summary>
        // / This PUT method updates a Post
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpPut("{postId}")]
        public async Task<ActionResult<PostDTO>> UpdatePost([FromBody] PostCreateDTO post, Guid postId)
        {
            var newPost = await _postLogic.UpdatePost(post, postId);
            return Ok(newPost);
        }
        /*PostItemDTO*/
        //Get controller
        // / <summary>
        // / This Get method brings the details of a post
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpGet("details/{postId}")]
        public async Task<PostDTO> GetPost(Guid postId)
        {
            var post = await _postLogic.GetPost(postId);
            return post;
        }
        /*PostItemDTO*/
        //Get controller
        // / <summary>
        // / This Get method brings a list of posts
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpGet("{userId}")]
        public ActionResult<PaginationResult<PostItemDTO>> GetPosts(Guid userId, [FromQuery] int page = 0, [FromQuery] int pageSize = 10)
        {
            var posts = _postLogic.GetPosts(userId, page, pageSize);
            return Ok(posts);
        }

        //PUT controller
        // / <summary>
        // / This PUT method likes a post
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpPut("like/{postId}/{userId}")]
        public async Task<ActionResult> LikePost(Guid postId, Guid userId)
        {
            await _postLogic.LikePost(postId, userId);
            return Ok();
        }



    }
}
