using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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
        public async Task<ActionResult<List<MemberDto>>> Get(bool includeBooks = false)
        {
            return await _service.GetAllMembers();
        }

        [HttpGet, Route("{id}")]
        public async Task<ActionResult<MemberDto>> Get(int id)
        {
            return await _service.GetMember(id);
        }

        [HttpPost, Route("{id}/rent")]
        public async Task<ActionResult<bool>> Rent(int id, [Required] int bookId)
        {
            try
            {
                return await _service.Rent(id, bookId);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost, Route("return"), Route("{id}/return")]
        public async Task<ActionResult<bool>> Rent([Required] int bookId)
        {
            try
            {
                return await _service.Return(bookId);
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
        public async Task<ActionResult<MemberDto>> Post(MemberDto dto)
        {
           return await _service.AddMember(dto);
        }

        [HttpPatch, Route("{id}")]
        public async Task<ActionResult<MemberDto>> Patch(int id, MemberDto dto)
        {
            try
            {
                return await _service.Update(id, dto);
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
        public async Task<ActionResult<MemberDto>> Put(int id, MemberDto dto)
        {
            try
            {
                return await _service.Replace(id, dto);
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
