using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CS1131_LibraryApi.Dto;
using CS1131_LibraryApi.Services;

namespace CS1131_LibraryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : ControllerBase
    {
        private IAuthorService _service;

        public AuthorController(IAuthorService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<AuthorDto>> Get(bool includeBooks = false)
        {
            return _service.GetAllAuthors(includeBooks).Result;
        }

        [HttpGet, Route("{id}")]
        public ActionResult<AuthorDto> Get(int id, bool includeBooks = false)
        {
            return _service.GetAuthor(id, includeBooks).Result;
        }

        [HttpGet, Route("Search")]
        public ActionResult<List<AuthorDto>> Search(string firstName, string lastName, bool includeBooks = false)
        {
            var response = _service.Search(firstName, lastName, includeBooks);
            if (response.Result == null) return NotFound("Could not find any author that matches the specified criteria.");
            return response.Result;
        }

        [HttpPost]
        public ActionResult<AuthorDto> Post(AuthorDto dto)
        {

            AuthorDto result = _service.AddAuthor(dto).Result;
            return Ok(result);
        }

        [HttpPatch, Route("{id}")]
        public ActionResult<AuthorDto> Patch([FromRoute] int id, [FromBody] AuthorDto dto)
        {
            try
            {
                var response = _service.Update(id, dto).Result;
                return Ok(response);
            }

            catch (AggregateException e)
            {
                foreach (var exception in e.InnerExceptions)
                {
                    if (exception is NotFoundException)
                        return BadRequest(e.Message);
                }
            }

            return StatusCode(500);
        }

        [HttpPut, Route("{id}")]
        public ActionResult<AuthorDto> Put([FromRoute] int id, [FromBody] AuthorDto dto)
        {
            try
            {
                var response = _service.Replace(id, dto).Result;
                return Ok(response);
            }

            catch (AggregateException e)
            {
                foreach (var exception in e.InnerExceptions)
                {
                    if (exception is NotFoundException)
                        return BadRequest(e.Message);
                }
            }

            return StatusCode(500);
        }

        [HttpDelete, Route("{id}")]
        public ActionResult<bool> Delete(int id)
        {
            return _service.Delete(id).Result;
        }
    }
}
