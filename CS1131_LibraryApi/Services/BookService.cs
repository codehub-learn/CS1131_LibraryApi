using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS1131_LibraryApi.Data;
using CS1131_LibraryApi.Domain;
using CS1131_LibraryApi.Dto;
using CS1131_LibraryApi.Dto.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CS1131_LibraryApi.Services
{
    public class BookService : IBookService
    {
        private LibContext _context;
        private IConverter<Book, BookDto> _converter;
        private IConverter<Author, AuthorDto> _authorConverter;

        public BookService(LibContext context, IConverter<Book, BookDto> converter, IConverter<Author, AuthorDto> authorConverter)
        {
            _context = context;
            _converter = converter; 
            _authorConverter = authorConverter;
        }

        public async Task<List<BookDto>> GetAllBooks(bool includeAuthors)
        {
            if(!includeAuthors) return await _context.Books.Select(b => _converter.Convert(b)).ToListAsync();
            else return await _context.Books.Include(b => b.Author).Select(b => _converter.Convert(b)).ToListAsync();
        }

        public async Task<BookDto> GetBook(int Id, bool includeAuthors)
        {
            var book = await _context.Books.Include(b => b.Author).SingleOrDefaultAsync(b => b.Id == Id);
            if (book == null) return null;

            BookDto dto = _converter.Convert(book);

            if (!includeAuthors) return dto;
            
            dto.Author = _authorConverter.Convert(book.Author);
            return dto;
        }

        public async Task<BookDto> AddBook(BookDto dto)
        {
            if (dto.Author == null) return null; //Creating a book without an author is not allowed

            Author bookAuthor = await _context.Authors.SingleOrDefaultAsync(b => b.Id == dto.Author.Id);
            if (bookAuthor == null) return null; //If the author does not exist the book cannot be added

            Book book = _converter.Convert(dto);
            book.Author = bookAuthor;

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return _converter.Convert(await _context.Books.SingleAsync(b => b.Id == dto.Id));
        }



    }
}
