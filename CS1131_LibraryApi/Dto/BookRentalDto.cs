using System;
using System.ComponentModel.DataAnnotations;


namespace CS1131_LibraryApi.Dto
{
    public class BookRentalDto : BookDto
    {
        public MemberDto RentedTo { get; set; }
    }
}
