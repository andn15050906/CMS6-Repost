using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using CMSnet6.Models;
using CMSnet6.Controllers.DTOs.Comment;
using CMSnet6.Models.DTOs.Post;
using CMSnet6.Helpers;

namespace CMSnet6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly UnitOfWork _uow;

        public CommentsController(UnitOfWork uow)
        {
            _uow = uow;
        }



        [HttpGet]
        public IActionResult Get([FromQuery] CommentQueryDTO query)
        {
            List<CommentDTO>? result = _uow.CommentRepo.Get(query);

            if (result == null || result.Count == 0)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            CommentDTO? result = _uow.CommentRepo.GetById(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromForm] CommentPostDTO dto)
        {
            (CommentDTO?, StatusMessage) result = _uow.CommentRepo.Create(dto, CookieParser.GetEmail(HttpContext));
            //called Save()

            return result.Item2 switch
            {
                StatusMessage.Created => Ok(result.Item1),
                StatusMessage.Unauthorized => Unauthorized(),
                StatusMessage.Forbidden => Forbid(),
                _ => BadRequest()
            };
        }

        [HttpPut]
        [Authorize]
        public IActionResult Update([FromForm] CommentPutDTO dto)
        {
            (CommentDTO?, StatusMessage) result = _uow.CommentRepo.Update(dto, CookieParser.GetEmail(HttpContext));

            return result.Item2 switch
            {
                StatusMessage.Ok => Ok(result.Item1),
                StatusMessage.Unauthorized => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            StatusMessage result = _uow.CommentRepo.Delete(id, CookieParser.GetEmail(HttpContext));
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
