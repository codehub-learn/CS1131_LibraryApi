using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS1131_LibraryApi.Dto;

namespace CS1131_LibraryApi.Services
{
    public interface IAuthorService
    {
        public Task<AuthorDto> GetAuthor(int id, bool includeBooks);
        public Task<List<AuthorDto>> GetAllAuthors(bool includeBooks);
        public Task<AuthorDto> AddAuthor(AuthorDto dto);
        public Task<List<AuthorDto>> Search (string firstName, string lastName, bool includeBooks);
        public Task<AuthorDto> Update(int authorId, AuthorDto dto);
        public Task<AuthorDto> Replace(int bookId, AuthorDto dto);
        public Task<bool> Delete(int id);
    }
}
