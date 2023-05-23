using System.Collections.Generic;
using System.Threading.Tasks;
using Jordnaer.Models;
using Jordnaer.Services;
using Jordnaer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Jordnaer.Pages.Items
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService orderService;
        private readonly IItemService itemService;

        public List<Item> Items { get; set; }

        public IndexModel(IOrderService orderService, IItemService itemService)
        {
            this.orderService = orderService;
            this.itemService = itemService;
        }

        public async Task OnGetAsync()
        {
            Items = await itemService.GetAllItemsAsync();
        }
    }
}
