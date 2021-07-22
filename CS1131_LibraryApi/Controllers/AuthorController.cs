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
        public async Task<ActionResult<List<AuthorDto>>> Get(bool includeBooks = false)
        {
            var response = await _service.GetAllAuthors(includeBooks);
            return response;
        }

        [HttpGet, Route("{id}")]
        public async Task<ActionResult<AuthorDto>> Get(int id, bool includeBooks = false)
        {
            var response = await _service.GetAuthor(id, includeBooks);
            return response;
        }

        [HttpGet, Route("Search")]
        public async Task<ActionResult<List<AuthorDto>>> Search(string firstName, string lastName, bool includeBooks = false)
        {
            var response = await _service.Search(firstName, lastName, includeBooks);
            if (response == null) return NotFound("Could not find any author that matches the specified criteria.");
            return response;
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> Post(AuthorDto dto)
        {

            AuthorDto result = await _service.AddAuthor(dto);
            return Ok(result);
        }

        [HttpPatch, Route("{id}")]
        public async Task<ActionResult<AuthorDto>> Patch([FromRoute] int id, [FromBody] AuthorDto dto)
        {
            try
            {
                var response = await _service.Update(id, dto);
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
        public async Task<ActionResult<AuthorDto>> Put([FromRoute] int id, [FromBody] AuthorDto dto)
        {
            try
            {
                var response = await _service.Replace(id, dto);
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
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return await _service.Delete(id);
        }
    }
}
