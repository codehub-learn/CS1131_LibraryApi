using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS1131_LibraryApi.Data;
using CS1131_LibraryApi.Domain;
using CS1131_LibraryApi.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CS1131_LibraryApi.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly LibContext _context;


        public AuthorService(LibContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns an author's basic details
        /// </summary>
        /// <param name="id">Id of the author</param>
        /// <param name="includeBooks">Optionally, can also include the books of the author</param>
        /// <returns>AuthorDto</returns>
        public async Task<AuthorDto> GetAuthor(int id, bool includeBooks)
        {
            var authorQuery = _context.Authors.Where(a => a.Id == id);
            if (includeBooks) 
            {
                authorQuery = authorQuery.Include(a => a.Books);
            }

            var author = await authorQuery.SingleOrDefaultAsync();
            return author.Convert();
        }

        /// <summary>
        /// Returns all authors with their basic details
        /// </summary>
        /// <param name="includeBooks">Optionally, can also include the books of the author</param>
        /// <returns>List of AuhtorDto</returns>
        public async Task<List<AuthorDto>> GetAllAuthors(bool includeBooks)
        {
            IQueryable<Author> authorsQuery = _context.Authors;
            if (includeBooks)
            {
                 authorsQuery = _context.Authors.Include(a => a.Books);
            }

            return await authorsQuery.Select(a => a.Convert()).ToListAsync();
        }

        /// <summary>
        /// Adds a new author
        /// </summary>
        /// <param name="dto">Request body with details of new author</param>
        /// <returns>Dto representation of the new author</returns>
        [HttpPost]
        public async Task<AuthorDto> AddAuthor(AuthorDto dto)
        {
            Author author = new Author()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return author.Convert();
        }

        /// <summary>
        /// Returns authors that specify criteria (non-case sensitive)
        /// </summary>
        /// <param name="firstName">Author First Name (Optional)</param>
        /// <param name="lastName">Author Last Name (Optional)</param>
        /// <param name="includeBooks">Optionally, also include books of retruned authors</param>
        /// <returns>List of AuthorDto</returns>
        public async Task<List<AuthorDto>> Search(string firstName, string lastName, bool includeBooks)
        {
            IQueryable<Author> results = _context.Authors;

            if (firstName != null)
            {
                results = results.Where(b => b.FirstName.ToLower().Contains(firstName.ToLower()));
            }

            if (lastName != null)
            {
                results = results.Where(b => b.LastName.ToLower().Contains(lastName.ToLower()));
            }

            if (includeBooks)
            {
                results = results.Include(b => b.Books);
            }

            return await results.Select(a => a.Convert()).ToListAsync();
        }

        /// <summary>
        /// Update an author's specified basic properties
        /// </summary>
        /// <param name="authorId">Id of author</param>
        /// <param name="dto">Body with author properties to update</param>
        /// <returns>AuthorDto representation of updated author</returns>
        /// <exception cref="NotFoundException">NotFoundException thrown if author does not exist</exception>
        public async Task<AuthorDto> Update(int authorId, AuthorDto dto)
        {
            Author author = await _context.Authors
                .SingleOrDefaultAsync(a => a.Id == authorId);

            if (author is null) throw new NotFoundException("The author id is invalid or has been removed.");

            if (dto.FirstName is not null) author.FirstName = dto.FirstName;
            if (dto.LastName is not null) author.LastName = dto.LastName;

            await _context.SaveChangesAsync();

            return author.Convert();
        }

        /// <summary>
        /// Update all basic properties of an author.
        /// If not specified in the request body, properties are set to null.
        /// </summary>
        /// <param name="authorId">Id of author</param>
        /// <param name="dto">Body with updated author properties</param>
        /// <returns>AuthorDto representation of updated author</returns>
        /// <exception cref="NotFoundException">NotFoundException thrown if author does not exist</exception>
        public async Task<AuthorDto> Replace(int authorId, AuthorDto dto)
        {
            Author author = await _context.Authors
                .SingleOrDefaultAsync(a => a.Id == authorId);

            if (author is null) throw new NotFoundException("The author id is invalid or has been removed.");

            //If a property does not exist in the request body, it will be updated to null
            author.FirstName = dto.FirstName;
            author.LastName = dto.LastName;

            await _context.SaveChangesAsync();

            return author.Convert();
        }

        /// <summary>
        /// Deletes an author.
        /// (WARNING!) Deleting an author also cascade deletes all of their books.
        /// </summary>
        /// <param name="id">Id of the author to delete</param>
        /// <returns>True after deletion, false if the author does not exist.</returns>
        public async Task<bool> Delete(int id)
        {
            Author author = await _context.Authors.SingleOrDefaultAsync(b => b.Id == id);
            if (author is null) return false;

            _context.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
