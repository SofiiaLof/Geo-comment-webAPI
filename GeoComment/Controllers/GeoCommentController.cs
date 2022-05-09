
using System.ComponentModel.DataAnnotations;
using GeoComment.Models;
using GeoComment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Controllers
{
    
    [Route("api/geo-comments")]
    [ApiController]
    public class GeoCommentController : ControllerBase
    {
        private readonly GeoCommentHandler _geoCommentHandler;
        private readonly UserManager<User> _userManager;
        public GeoCommentController(GeoCommentHandler geoCommentHandler, UserManager<User> userManager)
        {
            _geoCommentHandler = geoCommentHandler;
            _userManager = userManager;
        }

       
        [HttpPost]
        [ApiVersion("0.1")]
        
        public async Task<ActionResult<CommentDTO>> PostComment(CommentDtoInput comment )
        {
            
           var postComment = await _geoCommentHandler.PostComment(comment.Author,comment.Message, comment.Longitude, comment.Latitude);
          
               return Created("", new CommentDTO
               {
                   Id = postComment.Id,
                   Message = postComment.Message,
                   Author =comment.Author,
                   Longitude = postComment.maxLon,
                   Latitude = postComment.maxLat

               });
           


        }

        [HttpGet]
        [Route("{id:int}")]
        [ApiVersion("0.1")]
        [ApiVersion("0.2")]
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
        [ApiVersion("0.1")]
        [ApiVersion("0.2")]
        public async Task<ActionResult<DtoArray>> GetCommentsInRange([Required]int minLon, [Required] int maxLon, [Required] int minLat, [Required] int maxLat)
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

        [HttpPost]
        [ApiVersion("0.2")] 
        [Authorize]
        public async Task<ActionResult<CommentDtoOutputV_2>> PostCommentV_2( CommentDtoInputV_2 input)
        {
            var user = await _userManager.GetUserAsync(User);
         
           var postComment = await _geoCommentHandler.PostCommentV_2( input.body.Title,input.body.Message, input.Longitude, input.Latitude, user);

            
            if (user != null)
            {
                return Created("", new CommentDtoOutputV_2
                {
                    Id =postComment.Id,
                   body = new BodyOutput
                    {
                        Title = postComment.Title,
                        Message = postComment.Message,
                        Author = user.First_name,
                    },
                    Longitude = postComment.maxLon,
                    Latitude = postComment.maxLat
                });
            }

            return Unauthorized(user);

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
   

  

    public class CommentDtoInput
    {
        public string Message { get; set; }
        public string Author { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }
       

    }

    
    public class CommentDtoOutputV_2
    {
        public int Id { get; set; }
        public BodyOutput body { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }


    }
   
    public class CommentDtoInputV_2
    {
        public Body body { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
    }

    public class Body
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class BodyOutput
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Author { get; set; }
    }
    public class DtoArray
    {
        public List<CommentDTO> commentsArray  {get;set;}
    }
}
