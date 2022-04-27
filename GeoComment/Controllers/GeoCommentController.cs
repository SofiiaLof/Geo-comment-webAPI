
using System.ComponentModel.DataAnnotations;
using GeoComment.Models;
using GeoComment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    [Route("api/geo-comments")]
    [ApiController]
    public class GeoCommentController : ControllerBase
    {
        private readonly GeoCommentHandler _geoCommentHandler;
        public GeoCommentController(GeoCommentHandler geoCommentHandler)
        {
            _geoCommentHandler = geoCommentHandler;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> PostComment(CommentDTO comment)
        {
            
           var postComment = await _geoCommentHandler.PostComment(comment.Author,comment.Message, comment.Longitude, comment.Latitude);
          
               return Created("", new CommentDTO
               {
                   Id = postComment.Id,
                   Message = postComment.Message,
                   Author = postComment.User.First_name,
                   Longitude = postComment.maxLon,
                   Latitude = postComment.maxLat

               });
           


        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<CommentDTO>> GetComment(int id)
        {
            var comment = await _geoCommentHandler.GetComment(id);
            if (comment != null)
            {
                var commentDto = new CommentDTO()
                {
                    Id = comment.Id,
                    Author = comment.User.First_name,
                    Message = comment.Message,
                    Latitude = comment.maxLat,
                    Longitude = comment.maxLon

                };

                return  Ok(commentDto);
               
            }

            return NotFound();
        }

        [HttpGet]
        
        public async Task<ActionResult<Comment[]>> GetCommentsInRange([Required]int minLon, [Required] int maxLon, [Required] int minLat, [Required] int maxLat)
        {
            var comments = await _geoCommentHandler.GetCommentsInRange(minLon, maxLon, minLat, maxLat);

            var commentArray = new DtoArray();

            commentArray.commentsArray = new List<CommentDTO>();

            if (comments != null)
            {
                foreach (var comment in comments)
                {
                    var commentDto = new CommentDTO()
                    {
                        Id = comment.Id,
                        Author = comment.User.First_name,
                        Message = comment.Message,
                        Latitude = comment.maxLat,
                        Longitude = comment.maxLon,
                    };

                  
                    commentArray.commentsArray.Add(commentDto);
                }
          
                return Ok(commentArray.commentsArray);
            }


            return BadRequest();

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

    public class DtoArray
    {
        public List<CommentDTO> commentsArray  {get;set;}
    }
}
