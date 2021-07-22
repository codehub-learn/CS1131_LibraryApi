using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS1131_LibraryApi.Data;
using CS1131_LibraryApi.Domain;
using CS1131_LibraryApi.Dto;
using Microsoft.EntityFrameworkCore;

namespace CS1131_LibraryApi.Services
{
    public class MemberService : IMemberService
    {
        private readonly LibContext _context;


        public MemberService(LibContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a member's basic details
        /// </summary>
        /// <param name="id">Id of the member</param>
        /// <returns>MemberDto</returns>
        public async Task<MemberDto> GetMember(int id)
        {
            var member = await _context.Members.SingleOrDefaultAsync(a => a.Id == id);
            return member.Convert();
        }

        /// <summary>
        /// Returns all members
        /// </summary>
        /// <returns>List of MemberDto</returns>
        public async Task<List<MemberDto>> GetAllMembers()
        {
            return await _context.Members.Select(m => m.Convert()).ToListAsync();
        }

        /// <summary>
        /// Rents a book to a member
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="bookId"></param>
        /// <returns>True if the rent is set, false if the book is already rented</returns>
        /// <exception cref="NotFoundException">Raises a NotFoundException if the book does not exist</exception>
        public async Task<bool> Rent(int memberId, int bookId)
        {
            var book = await _context.Books.SingleOrDefaultAsync(a => a.Id == bookId);
            if (book.RentedToId is not null) return false;

            book.RentedToId = memberId;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Returns a book
        /// </summary>
        /// <param name="bookId">Id of the book to be returned</param>
        /// <returns>True if the return is successful, false of the book is not currently rented</returns>
        /// <exception cref="NotFoundException">Raises a NotFoundException if the book does not exist</exception>
        public async Task<bool> Return(int bookId)
        {
            var book = await _context.Books.SingleOrDefaultAsync(a => a.Id == bookId);
            if (book is null) throw new NotFoundException("The book id is invalid or the book has been removed.");

            if (book.RentedToId != null)
            {
                book.RentedToId = null;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a new member to the database
        /// </summary>
        /// <param name="dto">Representation of the new member</param>
        /// <returns>MemberDto</returns>
        public async Task<MemberDto> AddMember(MemberDto dto)
        {

            Member member = new Member()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return member.Convert();
        }

        /// <summary>
        /// Returns members that match specified criteria
        /// </summary>
        /// <param name="firstName">Member First Name (Optional)</param>
        /// <param name="lastName">Member Last Name (Optional)</param>
        /// <param name="email">Member Email (Optional)</param>
        /// <returns>List of MemberDto</returns>
        public async Task<List<MemberDto>> Search(string firstName, string lastName, string email)
        {
            IQueryable<Member> results = _context.Members;

            if (firstName != null)
            {
                results = results.Where(m => m.FirstName.ToLower().Contains(firstName.ToLower()));
            }

            if (lastName != null)
            {
                results = results.Where(m => m.LastName.ToLower().Contains(lastName.ToLower()));
            }

            if (email != null)
            {
                results = results.Where(m => m.Email.ToLower().Contains(email.ToLower()));
            }

            return await results.Select(m => m.Convert()).ToListAsync();
        }

        /// <summary>
        /// Updates the provided fields of a member
        /// </summary>
        /// <param name="memberId">Id of the member to update</param>
        /// <param name="dto">Request body with properties to update</param>
        /// <returns>Updated MemberDto Representation</returns>
        /// <exception cref="NotFoundException">Raises a NotFoundException if the member does not exist</exception>
        public async Task<MemberDto> Update(int memberId, MemberDto dto)
        {
            Member member = await _context.Members
                .SingleOrDefaultAsync(m => m.Id == memberId);

            if (member is null) throw new NotFoundException("The member id is invalid or has been removed.");

            if (dto.FirstName is not null) member.FirstName = dto.FirstName;
            if (dto.LastName is not null) member.LastName = dto.LastName;
            if (dto.Email is not null) member.Email = dto.Email;

            await _context.SaveChangesAsync();

            return member.Convert();
        }

        /// <summary>
        /// Updates all fields of a member. Properties not provided are set to null.
        /// </summary>
        /// <param name="memberId">Id of the member to update</param>
        /// <param name="dto">Request body with new member representation</param>
        /// <returns>Updated MemberDto Representation</returns>
        /// <exception cref="NotFoundException">Raises a NotFoundException if the member does not exist</exception>
        public async Task<MemberDto> Replace(int memberId, MemberDto dto)
        {
            Member member = await _context.Members
                .SingleOrDefaultAsync(m => m.Id == memberId);

            if (member is null) throw new NotFoundException("The member id is invalid or has been removed.");

            member.FirstName = dto.FirstName;
            member.LastName = dto.LastName;
            member.Email = dto.Email;

            await _context.SaveChangesAsync();

            return member.Convert();
        }

        /// <summary>
        /// Deletes a member.
        /// </summary>
        /// <param name="id">Id of the member to delete</param>
        /// <returns>True after deletion, false if the member does not exist.</returns>
        public async Task<bool> Delete(int id)
        {
            Member member = await _context.Members.SingleOrDefaultAsync(m => m.Id == id);
            if (member is null) return false;

            _context.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
