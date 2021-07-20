using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS1131_LibraryApi.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace CS1131_LibraryApi.Dto.Converters
{
    public class AuthorConverter : IConverter<Author, AuthorDto>
    {
        public  AuthorDto Convert(Author author)
        {
            return new AuthorDto()
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
            };
        }

        public Author Convert(AuthorDto dto)
        {
            return new Author()
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
        }
    }
}
