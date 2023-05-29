using Jordnaer.Interfaces;
using Jordnaer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jordnaer.Pages.Members
{
    public class CreateModel : PageModel
    {
        private readonly IMemberService memberService;

        [BindProperty]
        public Member Member { get; set; }

        public CreateModel(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            bool memberCreated = await memberService.CreateMemberAsync(Member);

            if (memberCreated)
            {
                return RedirectToPage("/Members/ShowAllMembers");
            }
            else
            {
                return RedirectToPage("Error");
            }
        }
    }
}

