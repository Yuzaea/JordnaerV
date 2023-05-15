using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Jordnaer.Models;
using Jordnaer.Interfaces;
namespace Jordnaer.Pages.Items
{
    public class DeleteMultipleModel : PageModel
    {
        private readonly IItemService _itemService;

        public DeleteMultipleModel(IItemService itemService)
        {
            _itemService = itemService;
        }

        public List<Item> Items { get; set; }


        [BindProperty]
        public List<int> SelectedItems { get; set; }


        public async Task OnGet()
        {
            Items = await _itemService.GetAllItemsAsync();
        }




        public async Task<IActionResult> OnPostDeleteSelected()
        {
            if (SelectedItems == null || SelectedItems.Count == 0)
            {
                // No items selected for deletion, handle accordingly
                ModelState.AddModelError(string.Empty, "No items selected for deletion.");
                return Page();
            }

            try
            {
                // Delete selected items
                foreach (var itemId in SelectedItems)
                {
                    await _itemService.DeleteItemAsync(itemId);
                }

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during deletion
                ModelState.AddModelError(string.Empty, "An error occurred while deleting items.");
                return Page();
            }
        }


    }
}
