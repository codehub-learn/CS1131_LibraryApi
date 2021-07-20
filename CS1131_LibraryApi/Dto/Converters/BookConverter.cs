using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS1131_LibraryApi.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace CS1131_LibraryApi.Dto.Converters
{
    public class BookConverter : IConverter<Book, BookDto>
    {

        public BookDto Convert(Book book)
        {
            return new BookDto()
            {
                Id = book.Id,
                Name = book.Name,
                Publisher = book.Publisher,
                RenterId = book.RentedTo?.Id,
                Author = new AuthorDto()
                {
                    FirstName = book.Author.FirstName,
                    LastName = book.Author.LastName
                }
            };
        }

        public Book Convert(BookDto dto)
        {
            return new Book()
            {
                Id = dto.Id,
                Name = dto.Name,
                Publisher = dto.Publisher
            };

        }
    }
}
