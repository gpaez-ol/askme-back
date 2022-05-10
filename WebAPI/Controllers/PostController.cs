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
        public async Task<PostDTO> CreatePost([FromBody] PostCreateDTO post, Guid userId)
        {
            var newPost = await _postLogic.CreatePost(post, userId);
            return newPost;
        }
        //PUT controller
        // / <summary>
        // / This PUT method updates a Post
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpPut("{postId}")]
        public async Task<PostDTO> UpdatePost([FromBody] PostCreateDTO post, Guid postId)
        {
            var newPost = await _postLogic.UpdatePost(post, postId);
            return newPost;
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
        public PaginationResult<PostItemDTO> GetPosts(Guid userId, [FromQuery] int page = 0, [FromQuery] int pageSize = 10)
        {
            var posts = _postLogic.GetPosts(userId, page, pageSize);
            return posts;
        }


    }
}
