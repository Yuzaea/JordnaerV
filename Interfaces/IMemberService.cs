using Jordnaer.Models;

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

}
}
