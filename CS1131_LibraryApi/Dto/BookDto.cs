using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CS1131_LibraryApi.Domain;

namespace CS1131_LibraryApi.Dto
{
    public class BookDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public AuthorDto Author { get; set; }
        public string Publisher { get; set; }
    }
}
