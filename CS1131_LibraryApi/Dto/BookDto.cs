using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS1131_LibraryApi.Dto
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AuthorDto Author { get; set; }
        public string Publisher { get; set; }
    }
}
