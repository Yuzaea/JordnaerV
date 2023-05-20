namespace Jordnaer.Models
{
    public class OrderItem
    {



            public int OrderID { get; set; }
    public int ItemID { get; set; }
    public int Quantity { get; set; }


        public OrderItem(int orderId, int itemId, int quantity)
        {
            OrderID = orderId;
            ItemID = itemId;
            Quantity = quantity;
        }
    }
}
