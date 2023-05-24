using Jordnaer.Models;
using System.Data.SqlClient;

namespace Jordnaer.Interfaces
{
    public interface IMemberService
    {
    Task<bool> CreateMemberAsync(Member member);
        Task<Member> GetMemberByIdAsync(int memberId);
        Task<bool> UpdateMemberAsync(Member member, int memberId);
        Task<bool> DeleteMemberAsync(int memberId);
        Task<int> FindHighestMemberIdAsync();
        Task<List<Orders>> GetMemberOrdersAsync(int memberId);


        List<Member> GetAllMembers();

        Member VerifyMember(string email, string passWord);

        Member GetLoggedMember(string email);

        Member GetLoggedMemberID(int memberId);


        Task<bool> EmailExistsAsync(string email);


    }

}

