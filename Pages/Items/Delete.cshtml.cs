using System;
using System.Threading.Tasks;
using Jordnaer.Interfaces;
using Jordnaer.Models;
using Jordnaer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jordnaer.Pages.Items
{
    public class DeleteModel : PageModel
    {
        private readonly IItemService _itemService;

        public DeleteModel(IItemService itemService)
        {
            _itemService = itemService;
        }

        public Item Item { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            Item = await _itemService.GetItemFromIdAsync(id);

            if (Item == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPost(int id)
        {
            try
            {
                var deletedItem = await _itemService.DeleteItemAsync(id);

                if (deletedItem == null)
                {

                    ModelState.AddModelError(string.Empty, "Failed to delete the item.");
                    return Page();
                }

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, "An error occurred while deleting the item.");
                return Page();
            }
        }
    }
}

