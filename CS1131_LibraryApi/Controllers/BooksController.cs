using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CS1131_LibraryApi.Domain;
using CS1131_LibraryApi.Dto;
using CS1131_LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CS1131_LibraryApi.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("1.1")]
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private IBookService _service;

        public BooksController(IBookService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<List<BookDto>> Get()
        {
            return await _service.GetAllBooks();
        }

        [HttpGet, Route("{id}")]
        public async Task<ActionResult<BookDto>> Get(int id)
        {
            var dto = await _service.GetBook(id);
            if (dto == null) return NotFound("The book Id is invalid or the book has been removed.");
            return Ok(dto);
        
        }

        [ApiVersion("1.1")]
        [HttpGet, Route("Search")]
        public ActionResult<List<BookDto>> Search(string name, string publisher, string authorFirst, string authorLast)
        {
            var response = _service.Search(name, publisher, authorFirst, authorLast);
            if (response.Result == null) return NotFound("Could not find a book that matches the specified criteria.");
            return response.Result;
        }

        [HttpGet, Route("{id}/rental")]
        public async ActionResult<BookRentalDto> GetRental([FromRoute] int id)
        {
            try
            {
                var response = await _service.GetRental(id);
                if (response == null)
                    return NotFound("Could not find a rental for the book.");
                return response;
            }
            catch (AggregateException e)
            {
                foreach (var exception in e.InnerExceptions)
                {
                    if (exception is NotFoundException)
                        return BadRequest("Could not find the book. Ensure the book id is correct and try again.");
                }
            }

            return StatusCode(500);
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> Post(BookDto dto)
        {

            BookDto result = await _service.AddBook(dto);
            if (result == null) return NotFound("The specified author Id is invalid or the author has been removed. Could not create book.");
            return Ok(result);
        }

        [HttpPatch, Route("{id}")]
        public async Task<ActionResult<BookDto>> Patch([FromRoute] int id, [FromBody] BookDto dto)
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
                        return BadRequest("Could not find the book. Ensure the book id is correct and try again.");
                }
            }

            return StatusCode(500);
        }

        [HttpPut, Route("{id}")]
        public async Task<ActionResult<BookDto>> Put([FromRoute] int id, [FromBody] BookDto dto)
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

