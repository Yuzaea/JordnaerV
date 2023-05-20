namespace Jordnaer.Models
{
    public class Member
    {


        public int MemberID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
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
