using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CS1131_LibraryApi.Dto;

namespace CS1131_LibraryApi.Services
{
    public interface IBookService
    {
        public Task<BookDto> GetBook(int id);
        public Task<List<BookDto>> GetAllBooks();
        public Task<BookDto> AddBook(BookDto dto);
        public Task<List<BookDto>> Search
            (string name, string publisher, string authorFirst, string authorLast); 
        public Task<BookRentalDto> GetRental(int bookId);
        public Task<BookDto> Update(int bookId, BookDto dto);
        public Task<BookDto> Replace(int bookId, BookDto dto);
        public Task<bool> Delete(int bookId);
    }
}
