using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Jordnaer.Models;
using Jordnaer.Services;
using Jordnaer.Interfaces;

namespace Jordnaer.Pages.Items
{
    public class UpdateModel : PageModel
    {
        private readonly IItemService _itemService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public Item Item { get; set; }

        [BindProperty]
        public IFormFile Photo { get; set; }

        public UpdateModel(IItemService itemService, IWebHostEnvironment webHostEnvironment)
        {
            _itemService = itemService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            Item = await _itemService.GetItemFromIdAsync(id);

            if (Item == null)
            {
                return NotFound();
            }


            return Page();
        }


        public async Task<IActionResult> OnPost()
        {
            int itemId = Convert.ToInt32(Request.Form["ItemId"]);
            if (Photo != null && Photo.Length > 0)
            {
                // Save the uploaded file
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(uploadsFolder);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }

                // Update the Item's image path
                Item.ItemImg = uniqueFileName;
            }

            bool updateResult = await _itemService.UpdateItemAsync(Item, itemId);

            if (updateResult)
            {
                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the item.");
                return Page();
            }
        }



    }
}


