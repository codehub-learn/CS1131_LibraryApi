using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS1131_LibraryApi.Domain;
using CS1131_LibraryApi.Dto;

namespace CS1131_LibraryApi.Services
{
    public interface IBookService
    {
        public Task<BookDto> GetBook(int Id, bool includeAuthors);
        public Task<List<BookDto>> GetAllBooks(bool includeAuthors);
        public Task<BookDto> AddBook(BookDto dto);
    }
}
