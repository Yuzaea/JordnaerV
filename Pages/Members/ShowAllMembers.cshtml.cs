using Jordnaer.Interfaces;
using Jordnaer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jordnaer.Pages.Members
{
    public class ShowAllMembersModel : PageModel
    {
        private readonly IMemberService memberService;

        public List<Member> Members { get; set; }

        public ShowAllMembersModel(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        public async Task OnGetAsync()
        {
            Members =  memberService.GetAllMembers();
        }
    }
}

