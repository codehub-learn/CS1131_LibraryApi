using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CS1131_LibraryApi.Dto;
using CS1131_LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CS1131_LibraryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemberController : ControllerBase
    {
        private IMemberService _service;

        public MemberController(IMemberService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<MemberDto>> Get(bool includeBooks = false)
        {
            return _service.GetAllMembers().Result;
        }

        [HttpGet, Route("{id}")]
        public ActionResult<MemberDto> Get(int id)
        {
            return _service.GetMember(id).Result;
        }

        [HttpPost, Route("{id}/rent")]
        public ActionResult<bool> Rent(int id, [Required] int bookId)
        {
            try
            {
                return _service.Rent(id, bookId).Result;
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost, Route("return"), Route("{id}/return")]
        public ActionResult<bool> Rent([Required] int bookId)
        {
            try
            {
                return _service.Return(bookId).Result;
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

        [HttpPost]
        public ActionResult<MemberDto> Post(MemberDto dto)
        {
           return _service.AddMember(dto).Result;
        }

        [HttpPatch, Route("{id}")]
        public ActionResult<MemberDto> Patch(int id, MemberDto dto)
        {
            try
            {
                return _service.Update(id, dto).Result;
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
        public ActionResult<MemberDto> Put(int id, MemberDto dto)
        {
            try
            {
                return _service.Replace(id, dto).Result;
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
