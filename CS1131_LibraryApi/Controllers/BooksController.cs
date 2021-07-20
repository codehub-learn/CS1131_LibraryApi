using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        public ActionResult<BookDto> Post(BookDto dto)
        {
            BookDto result = _service.AddBook(dto).Result;
            if (result == null) return BadRequest("Invalid Book Description!");
            else return Ok(result);
        }
    }
}
