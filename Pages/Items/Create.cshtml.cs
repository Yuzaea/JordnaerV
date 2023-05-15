using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Jordnaer.Models;
using Jordnaer.Interfaces;

namespace Jordnaer.Pages.Items
{
    public class CreateModel : PageModel
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IItemService itemService;

        [BindProperty]
        public Item Item { get; set; }

        [BindProperty]
        public IFormFile Photo { get; set; }

        public CreateModel(IItemService itemservice, IWebHostEnvironment webHost)
        {
            webHostEnvironment = webHost;
            itemService = itemservice;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Photo != null)
            {
                if (!string.IsNullOrEmpty(Item.ItemImg))
                {
                    string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images/ItemImages", Item.ItemImg);
                    System.IO.File.Delete(filePath);
                }

                Item.ItemImg = await ProcessUploadedFile();
            }

            await itemService.CreateItemAsync(Item);
            return RedirectToPage("ShowAllItems");
        }

        private async Task<string> ProcessUploadedFile()
        {
            string uniqueFileName = null;
            if (Photo != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/ItemImages");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(uploadsFolder);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }
            }
            return uniqueFileName;
        }


    }
}


