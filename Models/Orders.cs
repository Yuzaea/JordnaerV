namespace Jordnaer.Models
{
    public class Orders
    {

        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public float TotalPrice { get; set; }
        public int MemberID { get; set; }
    }
}
