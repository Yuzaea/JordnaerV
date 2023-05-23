using System.ComponentModel.DataAnnotations;

namespace Jordnaer.Models
{
    public class Member
    {

        [Required(ErrorMessage = "Id is required")]
        public int MemberID { get; set; }
        public string Name { get; set; }


        [Required(ErrorMessage = "Email id is required")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                            ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password id is required")]
        public string Password { get; set; }
        public List<Orders> Orders { get; set; }


       public Member()
        {

            Orders = new List<Orders>();
        }


        public Member(int memberID, string name, string email, string password)
        {
            MemberID = memberID;
            Name = name;
            Email = email;
            Password = password;
            Orders = new List<Orders>();
        }


    }
}
