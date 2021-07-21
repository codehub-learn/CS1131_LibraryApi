using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CS1131_LibraryApi.Data;
using CS1131_LibraryApi.Domain;
using CS1131_LibraryApi.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                .Select(b => b.Convert())
                // .Select(b => new BookDto()
                // {
                //     Id = b.Id,
                //     Name = b.Name,
                //     Publisher = b.Publisher,
                //     Author = new()
                //     {
                //         Id = b.Author.Id,
                //         FirstName = b.Author.FirstName,
                //         LastName = b.Author.LastName
                //     }
                // })
                .ToListAsync();
        }


        public async Task<BookDto> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .SingleOrDefaultAsync(b => b.Id == id);
            
            if (book == null) return null;

            return book.Convert();

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

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return book.Convert();
        }

        public async Task<List<BookDto>> Search
            ([FromQuery] string name, [FromQuery] string publisher, [FromQuery] string authorFirst, [FromQuery] string authorLast)
        {
            var results = _context.Books.Include(a => a.Author).Select(b => b);

            if (name != null)
            {
                results = results.Where(b => b.Name.ToLower().Contains(name.ToLower()));
            }

            if (publisher != null)
            {
                results = results.Where(b => b.Publisher.ToLower().Contains(publisher.ToLower()));
            }

            if (authorFirst != null)
            {
                results = results.Where(b => b.Author.FirstName.ToLower().Contains(authorFirst.ToLower()));
            }

            if (authorLast != null)
            {
                results = results.Where(b => b.Author.LastName.ToLower().Contains(authorLast.ToLower()));
            }

            var resultsList = await results.ToListAsync();

            if (resultsList == null) return null;

            List<BookDto> response = new();
            foreach (var b in resultsList)
            {
                response.Add(b.Convert());
            }

            return response;
        }

        public async Task<BookRentalDto> GetRental(int bookId)
        {
            var result = await _context.Books
                .Include(a => a.Author)
                .Include(b => b.RentedTo)
                .SingleOrDefaultAsync(b => b.Id == bookId);

            if (result == null) throw new NotFoundException("The book id is invalid or the book has been removed."); 
            if (result.RentedTo == null) return null;
            return result.ConvertRental();
        }

        /// <summary>
        /// Updates a book by replacing only the given properties. 
        /// </summary>
        /// <param name="bookId">Id of the book to replace.</param>
        /// <param name="dto">(Partial) representation of the updated book.</param>
        /// <returns>DTO of the complete updated book.</returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<BookDto> Update(int bookId, BookDto dto)
        {
            Book book = await _context.Books
                .Include(b => b.Author)
                .SingleOrDefaultAsync(b => b.Id == bookId);

            if (book is null) throw new NotFoundException("The book id is invalid or the book has been removed.");

            if (dto.Name is not null) book.Name = dto.Name;
            if (dto.Publisher is not null) book.Publisher = dto.Publisher;
            if (dto.Author is not null)
            {
                var author = _context.Authors.SingleOrDefault(a => a.Id == dto.Author.Id);
                if (author is not null) book.Author = author;
            }

            await _context.SaveChangesAsync();

            return book.Convert();
        }

        /// <summary>
        /// Accepts a complete representation of a book and replaces the existing one.
        /// </summary>
        /// <param name="bookId">Id of the book to replace.</param>
        /// <param name="dto">Representation of the updated book.</param>
        /// <returns>DTO of the updated book</returns>
        /// <exception cref="NotFoundException">Occurs if the specified book or its author does not exist.</exception>
        public async Task<BookDto> Replace(int bookId, BookDto dto)
        {
            Book book = await _context.Books
                .SingleOrDefaultAsync(b => b.Id == bookId);

            if (book is null)
                throw new NotFoundException("The book id is invalid or has been removed.");

            Author author = await _context.Authors
                .SingleOrDefaultAsync(a => a.Id == dto.Author.Id);

            if (author is null)
                throw new NotFoundException("The author id is invalid or has been removed.");

            //If a property does not exist in the request body, it will be updated to null
            book.Name = dto.Name;
            book.Publisher = dto.Publisher;
            book.Author = author;

            await _context.SaveChangesAsync();

            return book.Convert();
        }



    }

}

