using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AskMe.Logic;
using AsKMe.Pagination;
using AskMe.Data.DTO;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly PostLogic _postLogic;
        public DashboardController(PostLogic postLogic)
        {
            _postLogic = postLogic;
        }
        //Get controller
        // / <summary>
        // / This Get method gets all the posts related to the users following
        // / </summary>
        // / <returns>Ok(post)</returns>
        [HttpGet("post/{userId}")]
        public async Task<ActionResult<PaginationResult<PostItemDTO>>> GetDashboardPosts(Guid userId, [FromQuery] int page = 0, [FromQuery] int pageSize = 10)
        {
            var posts = await _postLogic.GetDashboardPosts(userId, page, pageSize);
            return Ok(posts);
        }

    }
}
