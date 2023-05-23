namespace Jordnaer.Models
{
    public class Basket
    {
        private List<Item> items;

        public Basket()
        {
            items = new List<Item>();
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }

        public List<Item> GetItems()
        {
            return items;
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}

