using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using CMSnet6.Controllers.DTOs.Post;
using CMSnet6.Models;
using CMSnet6.Models.DTOs.Post;
using CMSnet6.Helpers;

namespace CMSnet6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly UnitOfWork _uow;

        public PostsController(UnitOfWork uow)
        {
            _uow = uow;
        }



        [HttpGet]
        public IActionResult Get([FromQuery] PostQueryDTO query)
        {
            List<PostDTO>? result = _uow.PostRepo.Get(query);

            if (result == null || result.Count == 0)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            PostDTO? result = _uow.PostRepo.GetById(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromForm] PostPostDTO dto)
        {
            (PostDTO?, StatusMessage) result = _uow.PostRepo.Create(dto, CookieParser.GetEmail(HttpContext));
            //called Save()

            return result.Item2 switch
            {
                StatusMessage.Created => Ok(result.Item1),
                StatusMessage.Conflict => Conflict(),
                StatusMessage.Unauthorized => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpPut]
        [Authorize]
        public IActionResult Update([FromForm] PostPutDTO dto)
        {
            (PostDTO?, StatusMessage) result = _uow.PostRepo.Update(dto, CookieParser.GetEmail(HttpContext));
            //called Save()

            return result.Item2 switch
            {
                StatusMessage.Ok => Ok(result.Item1),
                StatusMessage.Conflict => Conflict(),
                StatusMessage.Unauthorized => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpPut("/api/[controller]/Read")]
        public void IncreaseReadCount(int postId)
        {
            _uow.PostRepo.IncreaseReadCount(postId);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            StatusMessage result = _uow.PostRepo.Delete(id, CookieParser.GetEmail(HttpContext));
            _uow.Save();

            return result switch
            {
                StatusMessage.Ok => Ok(),
                StatusMessage.Unauthorized => Unauthorized(),
                _ => BadRequest()
            };
        }
    }
}
