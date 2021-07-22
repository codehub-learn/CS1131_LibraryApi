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

        /// <summary>
        /// Retrieves a list of all books.
        /// </summary>
        /// <returns>Retrurns a list of BookDtos.</returns>
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

        /// <summary>
        /// Retrieves the details of a specific book selected by its id.
        /// </summary>
        /// <param name="id">The if of the book to retrieve.</param>
        /// <returns>Returns a BookDto object.</returns>
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

        /// <summary>
        /// Create a new book.
        /// </summary>
        /// <param name="dto">BookDto with book characteristics.
        /// The author must already exist and a Name for the book is required.</param>
        /// <returns>Returns the newly created book.
        /// If the author cannot be found the book is not created and null is returned instead.</returns>
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

        /// <summary>
        /// Search for books by specified characteristics. Any combination can be used.
        /// The method is case insensitive.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="publisher"></param>
        /// <param name="authorFirst"></param>
        /// <param name="authorLast"></param>
        /// <returns>Returns a list of books with specified characteristics.</returns>
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

        /// <summary>
        /// Check if a book is rented, and if it is, get details of the rental.
        /// </summary>
        /// <param name="bookId">Id of the book to check for rental.</param>
        /// <returns>Returns a BookRentalDto containing both the member and the book details if the book is rented, or null otherwise.</returns>
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

            if (book.Author is null)
                throw new NotFoundException
                    ("An author must be specified. To update only specific properties of a book, consider using the PATCH method");

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

        /// <summary>
        /// Deletes a book.
        /// </summary>
        /// <param name="id">Id of book</param>
        /// <returns>True if the book is deleted</returns>
        public async Task<bool> Delete(int id)
        {
            Book book = await _context.Books.SingleOrDefaultAsync(b => b.Id == id);
            if (book is null) return false;

            _context.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}

