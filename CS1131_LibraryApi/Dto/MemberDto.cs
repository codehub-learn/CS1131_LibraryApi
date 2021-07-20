using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS1131_LibraryApi.Dto
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
