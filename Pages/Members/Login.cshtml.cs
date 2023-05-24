using Jordnaer.Interfaces;
using Jordnaer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Jordnaer.Pages.Members
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string PassWord { get; set; }

        [BindProperty]
        public int MemberID { get; set; }


        public string Message { get; set; }

        private IMemberService memberService;
        public LoginModel(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        public void OnGet()
        {
            string email = HttpContext.Session.GetString("Email");
            if (email != null)
            {
                ViewData["Email"] = email;
            }
            else
            {
                ViewData["Email"] = null;
            }
        }

        public void OnGetLogout()
        {
            HttpContext.Session.Remove("Email");

        }

        public IActionResult OnPost()
        {
            Member loginUser = memberService.VerifyMember(Email, PassWord);
            if (loginUser != null)
            {
                HttpContext.Session.SetInt32("MemberID", loginUser.MemberID);
                HttpContext.Session.SetString("Email", loginUser.Email);
                MemberID = loginUser.MemberID;
                return RedirectToPage("/items/index");
            }
            else
            {
                Message = "Invalid email or password";
                Email = "";
                PassWord = "";
                return Page();
            }
        }


    }
}

