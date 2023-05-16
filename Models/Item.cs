namespace Jordnaer.Models
{
    public class Item
    {

        public int ItemID { get; set; }

        public string ItemName { get; set; }
        public string ItemImg { get; set; }

        public float ItemPrice { get; set; }

        public string ItemDescription { get; set; }
        public string ItemType { get; set; }

        public Item()
        {

        }

        public Item(int itemID, string itemName, string itemImg, float itemPrice, string itemDescription, string itemType)
        {
            ItemID = itemID;
            ItemName = itemName;
            ItemImg = itemImg;
            ItemPrice = itemPrice;
            ItemDescription = itemDescription;
            ItemType = itemType;

        }

    }
}
