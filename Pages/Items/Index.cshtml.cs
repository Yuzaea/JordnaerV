using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Jordnaer.Models;
using Jordnaer.Interfaces;


namespace Jordnaer.Pages.Items
{
    public class IndexModel : PageModel
    {
        private readonly IItemService itemService;

        public IndexModel(IItemService itemService)
        {
            this.itemService = itemService;
        }

        public List<Item> Item { get; set; }

        public async Task OnGetAsync()
        {
            Item = await itemService.GetAllItemsAsync();
        }
    }
}
