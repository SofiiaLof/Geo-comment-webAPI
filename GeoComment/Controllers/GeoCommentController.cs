using GeoComment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    [Route("api")]
    [ApiController]
    public class GeoCommentController : ControllerBase
    {
        private readonly GeoCommentHandler _geoCommentHandler;
        public GeoCommentController(GeoCommentHandler geoCommentHandler)
        {
            _geoCommentHandler = geoCommentHandler;
        }

        [HttpPost]
        [Route("geo-comments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> PostComment(CommentDTO comment)
        {
            
           var postComment = await _geoCommentHandler.PostComment(comment.Author,comment.Message, comment.Longitude, comment.Latitude);

           
                return Created("",new CommentDTO
                {
                    Id = postComment.Id,
                    Message = postComment.Message,
                    Author = postComment.User.First_name,
                    Longitude = postComment.maxLon,
                    Latitude = postComment.maxLat

                });
            

        }
    }

    public class CommentDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Author { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }
    }

}
