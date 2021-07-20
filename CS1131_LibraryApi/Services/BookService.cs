using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS1131_LibraryApi.Data;
using CS1131_LibraryApi.Domain;
using CS1131_LibraryApi.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;


namespace CS1131_LibraryApi.Services
{
    public class BookService : IBookService
    {
        private readonly LibContext _context;


        public BookService(LibContext context)
        {
            _context = context;
        }

        public async Task<List<BookDto>> GetAllBooks()
        {
            return await _context.Books
                .Include(a => a.Author)
                .Select(b => new BookDto()
                {
                    Id = b.Id,
                    Name = b.Name,
                    Publisher = b.Publisher,
                    Author = new()
                    {
                        FirstName = b.Author.FirstName,
                        LastName = b.Author.LastName
                    }
                })
                .ToListAsync();
        }


        public async Task<BookDto> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .SingleOrDefaultAsync(b => b.Id == id);
            
            if (book == null) return null;

            return Convert(book);

            // return new BookDto()
            // {
            //     Id = book.Id,
            //     Name = book.Name,
            //     Publisher = book.Publisher,
            //     Author = new()
            //     {
            //         FirstName = book.Author.FirstName,
            //         LastName = book.Author.LastName
            //     }
            // };
        }

        
        public async Task<BookDto> AddBook(BookDto dto)
        {
            Author bookAuthor = await _context.Authors.SingleOrDefaultAsync(a => a.Id == dto.Author.Id);
            if (bookAuthor == null) return null;

            Book book = new Book()
            {
                Name = dto.Name,
                Publisher = dto.Publisher,
                Author = bookAuthor,
            };

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            return Convert(book);

            // return new BookDto()
            // {
            //     Id = book.Id,
            //     Name = book.Name,
            //     Publisher = book.Publisher,
            //     Author = new()
            //     {
            //         FirstName = book.Author.FirstName,
            //         LastName = book.Author.LastName
            //     }
            // };
        }

        public BookDto Convert(Book book)
        {
            return new BookDto()
            {
                Id = book.Id,
                Name = book.Name,
                Publisher = book.Publisher,
                Author = new()
                {
                    FirstName = book.Author.FirstName,
                    LastName = book.Author.LastName
                }
            };
        }

    }

}

