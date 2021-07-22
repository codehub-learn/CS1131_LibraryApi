using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CS1131_LibraryApi.Dto;

namespace CS1131_LibraryApi.Services
{
    public interface IMemberService
    {
        public Task<MemberDto> GetMember(int id);
        public Task<List<MemberDto>> GetAllMembers();
        public Task<bool> Rent(int membeId, int bookId);
        public Task<bool> Return(int bookId);
        public Task<MemberDto> AddMember(MemberDto dto);
        public Task<List<MemberDto>> Search(string firtName, string lastName, string email);
        public Task<MemberDto> Update(int memberId, MemberDto dto);
        public Task<MemberDto> Replace(int memberId, MemberDto dto);
        public Task<bool> Delete(int memberId);
    }
}
