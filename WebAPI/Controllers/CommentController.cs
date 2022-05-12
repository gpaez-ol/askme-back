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
    [Route("comment")]
    public class CommentController : ControllerBase
    {
        private readonly CommentLogic _commentLogic;
        public CommentController(CommentLogic commentLogic)
        {
            _commentLogic = commentLogic;
        }

        //POST controller
        // / <summary>
        // / This POST method creates a Post
        // / </summary>
        // / <returns>Ok()</returns>
        [HttpPost("comment/{commentId}/{userId}")]
        public async Task<ActionResult<PostDTO>> CommentOnComment([FromBody] CommentCreateDTO comment, Guid commentId, Guid userId)
        {
            await _commentLogic.CommentComment(comment, commentId, userId);
            return Ok();
        }
        //DELETE controller
        // / <summary>
        // / This DELETE method deletes a Comment
        // / </summary>
        // / <returns>Ok()</returns>
        [HttpDelete("{commentId}")]
        public async Task<ActionResult> DeleteComment(Guid commentId)
        {
            await _commentLogic.DeleteComment(commentId);
            return Ok();
        }
        /*PostItemDTO*/
        //Get controller
        // / <summary>
        // / This Get method brings a list of comments on a post
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpGet("post/{postId}")]
        public ActionResult<PaginationResult<PostItemDTO>> GetPostComments(Guid postId, [FromQuery] int page = 0, [FromQuery] int pageSize = 10)
        {
            var posts = _commentLogic.GetCommentsOnPost(postId, page, pageSize);
            return Ok(posts);
        }
        /*PostItemDTO*/
        //Get controller
        // / <summary>
        // / This Get method brings a list of comments on a comment
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpGet("comment/{commentId}")]
        public ActionResult<PaginationResult<CommentItemDTO>> GetCommentComments(Guid commentId, [FromQuery] int page = 0, [FromQuery] int pageSize = 10)
        {
            var posts = _commentLogic.GetCommentsOnComment(commentId, page, pageSize);
            return Ok(posts);
        }

        //PUT controller
        // / <summary>
        // / This PUT method likes a post
        // / </summary>
        // / <returns>Ok()</returns>
        [HttpPut("like/{commentId}/{userId}")]
        public async Task<ActionResult> LikeComment(Guid commentId, Guid userId)
        {
            await _commentLogic.LikeComment(commentId, userId);
            return Ok();
        }
    }
}
