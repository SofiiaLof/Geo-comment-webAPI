using GeoComment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    [Route("test")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly DatabaseHandler _databaseHandler;

        public DatabaseController(DatabaseHandler databaseHandler)
        {
            _databaseHandler = databaseHandler;
        }


        [ApiVersion("0.1")]
        [ApiVersion("0.2")]
        [HttpGet]
        [Route("reset-db")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<ActionResult> ResetDatabase()
        {
           var databaseReset =  await _databaseHandler.ResetDatabase();

           if (databaseReset)
           {
               return Ok();
           }

           return NoContent();
        }


    }
}
