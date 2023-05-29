using Jordnaer.Interfaces;
using Jordnaer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jordnaer.Pages.Members
{
    public class DeleteModel : PageModel {

            private readonly IMemberService memberService;

            public DeleteModel(IMemberService memberService)
            {
                this.memberService = memberService;
            }

            public Member Member { get; set; }

            public async Task<IActionResult> OnGet(int id)
            {
                Member = await memberService.GetMemberByIdAsync(id);

                if (Member == null)
                    return NotFound();

                return Page();
            }

            public async Task<IActionResult> OnPost(int id)
            {
                try
                {
                    var deletedMember = await memberService.DeleteMemberAsync(id);

                    if (deletedMember == null)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to delete the member.");
                        return Page();
                    }

                    return RedirectToPage("ShowAllMembers");
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, "An error occurred while deleting the member.");
                    return Page();
                }
            }
        }
    }

