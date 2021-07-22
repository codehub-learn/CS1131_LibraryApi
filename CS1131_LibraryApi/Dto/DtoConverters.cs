using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS1131_LibraryApi.Domain;

namespace CS1131_LibraryApi.Dto
{
    public static class DtoConverters
    {
        public static BookDto Convert(this Book book)
        {
            return new BookDto()
            {
                Id = book.Id,
                Name = book.Name,
                Publisher = book.Publisher,
                Author = new()
                {
                    Id = book.Author.Id,
                    FirstName = book.Author.FirstName,
                    LastName = book.Author.LastName
                }
            };
        }

        public static BookRentalDto ConvertRental(this Book book)
        {
            return new BookRentalDto()
            {
                Id = book.Id,
                Name = book.Name,
                Publisher = book.Publisher,
                Author = new()
                {
                    FirstName = book.Author.FirstName,
                    LastName = book.Author.LastName
                },
                RentedTo = new MemberDto()
                {
                    Id = book.RentedTo.Id,
                    FirstName = book.RentedTo.FirstName,
                    LastName = book.RentedTo.LastName
                }
            };
        }

        public static AuthorDto Convert(this Author author)
        {
            var result = new AuthorDto()
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName
            };

            if (author.Books is not null)
            {
                result.Books = author.Books.Select(b => b.Convert()).ToList();
            }

            return result;
        }

        public static MemberDto Convert(this Member member)
        {
            return  new MemberDto()
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName
            };
        }
    }
}
