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
        public List<BookDto> Get()
        {
            return _service.GetAllBooks().Result;
        }

        [HttpGet, Route("{id}")]
        public ActionResult<BookDto> Get(int id)
        {
            var dto = _service.GetBook(id).Result;
            if (dto == null) return NotFound("The book Id is invalid or the book has been removed.");
            return Ok(dto);
        
        }

        [HttpGet, Route("Search")]
        public ActionResult<List<BookDto>> Search(string name, string publisher, string authorFirst, string authorLast)
        {
            var response = _service.Search(name, publisher, authorFirst, authorLast);
            if (response.Result == null) return NotFound("Could not find a book that matches the specified criteria.");
            return response.Result;
        }

        [HttpGet, Route("{id}/rental")]
        public ActionResult<BookRentalDto> GetRental([FromRoute] int id)
        {
            try
            {
                var response = _service.GetRental(id).Result;
                if (response == null)
                    return NotFound("Could not find a rental for the book or the book does not exist.");
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
        public ActionResult<BookDto> Post(BookDto dto)
        {

            BookDto result = _service.AddBook(dto).Result;
            if (result == null) return NotFound("The specified author Id is invalid or the author has been removed. Could not create book.");
            return Ok(result);
        }

        [HttpPatch, Route("{id}")]
        public ActionResult<BookDto> Patch([FromRoute] int id, [FromBody] BookDto dto)
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
                        return BadRequest("Could not find the book. Ensure the book id is correct and try again.");
                }
            }

            return StatusCode(500);
        }

        [HttpPut, Route("{id}")]
        public ActionResult<BookDto> Put([FromRoute] int id, [FromBody] BookDto dto)
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
    }
}

