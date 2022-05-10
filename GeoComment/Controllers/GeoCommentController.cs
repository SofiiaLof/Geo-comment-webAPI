
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
                   Author = comment.Author,
                   Longitude = postComment.maxLon,
                   Latitude = postComment.maxLat

               });
           


        }

        [HttpGet]
        [Route("{id:int}")]
        [ApiVersion("0.1")]
      
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


        [HttpGet]
        [Route("{id:int}")]
        [ApiVersion("0.2")]
        public async Task<ActionResult<CommentDtoOutputV_2>> GetCommentV_2(int id)
        {
            var comment = await _geoCommentHandler.GetComment(id);

            if (comment != null)
            {
                var commentDto = new CommentDtoOutputV_2
                {
                    Id = comment.Id,
                    body = new BodyOutput
                    {
                        Author = comment.User.First_name,
                        Title = comment.Title,
                        Message = comment.Message,
                    },
                       
                    Latitude = comment.maxLat,
                    Longitude = comment.maxLon

                };

                return Ok(commentDto);

            }

            return NotFound();
        }


        [HttpGet]
        [Route("{username}")]
        [ApiVersion("0.2")]

        public async Task<ActionResult<List<CommentDtoOutputV_2>>> GetCommentsByUsername(string username)
        {
            var comments = await _geoCommentHandler.GetCommentsFoUser(username);

            var commentsList = new List<CommentDtoOutputV_2>();

            if (comments != null)
            {
                foreach (var comment in comments)
                {
                    var commentV_2 = new CommentDtoOutputV_2
                    {
                        Id = comment.Id,
                        body =new BodyOutput
                        {
                            Title = comment.Title,
                        },
                        
                        Latitude = comment.maxLat,
                        Longitude = comment.maxLon,
                    };

                   
                    commentsList.Add(commentV_2);

                    
                }
                return Ok(commentsList);
            }
            return NotFound ();

        }

        [HttpGet]
        [ApiVersion("0.2")]

        public async Task<ActionResult<CommentDtoOutputV_2>> GetCommentsInRangeV_2([Required] int minLon, [Required] int maxLon, [Required] int minLat, [Required] int maxLat)
        {
            var comments = await _geoCommentHandler.GetCommentsInRange(minLon, maxLon, minLat, maxLat);

            var commentsList = new List<CommentDtoOutputV_2>();

            if (comments != null)
            {
                foreach (var comment in comments)
                {
                    var commentsInRange = new CommentDtoOutputV_2
                    {
                        Id = comment.Id,
                        body =new BodyOutput
                        {
                            Author = comment.User.First_name,
                            Message = comment.Message,
                            Title = comment.Title
                        },
                       
                     
                        Latitude = comment.maxLat,
                        Longitude = comment.maxLon,
                    };


                    commentsList.Add(commentsInRange);
                }

                return Ok(commentsList);
            }


            return BadRequest();

        }

        [Authorize]
        [HttpDelete]
        [Route("{id:int}")]
        [ApiVersion("0.2")]

        public async Task<ActionResult<CommentDtoOutputV_2>> DeleteComment(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var commentToDelete = await _geoCommentHandler.GetComment(id);

            if (user == null)
            {
                return Unauthorized();
            }

            if (commentToDelete == null)
            {
                return NotFound();
            }

            var deletedComment = await _geoCommentHandler.DeleteComment(id);

            if (deletedComment != null)
            {
                if(deletedComment.User.Id !=user.Id)
                {
                    return Unauthorized();
                }
                return Ok(new CommentDtoOutputV_2
                {
                    Id = deletedComment.Id,
                    body = new BodyOutput(){
                        Author =deletedComment.User.UserName,
                        Title = deletedComment.Title,
                        Message = deletedComment.Message,
                    },
                    Latitude = deletedComment.maxLat,
                    Longitude = deletedComment.maxLon

                });
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
