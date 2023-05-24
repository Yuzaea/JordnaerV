using System.Collections.Generic;
using System.Threading.Tasks;
using Jordnaer.Models;
using Jordnaer.Services;
using Jordnaer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Data.SqlClient;
using Jordnaer.Pages.Members;
using System.Security.Claims;

namespace Jordnaer.Pages.Orders
{
    public class CreateModel : PageModel
    {

        private readonly IOrderService orderService;
        private readonly IItemService itemService;
        private readonly IOrderItemService orderItemService;
        private readonly IMemberService memberService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public int MemberID { get; private set; }


        public List<Item> Items { get; set; } = new List<Item>();

        public CreateModel(IOrderService orderService, IItemService itemService, IOrderItemService orderItemService, IMemberService memberService, IHttpContextAccessor httpContextAccessor)
        {
            this.orderService = orderService;
            this.itemService = itemService;
            this.orderItemService = orderItemService;
            this.memberService = memberService;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            Items = await itemService.GetAllItemsAsync();

            int? MemberID = httpContextAccessor.HttpContext.Session.GetInt32("MemberID");
            if (MemberID.HasValue)
            {
                MemberID = MemberID.Value;
            }

            if (!MemberID.HasValue || MemberID.Value == 0)
            {
                string errorMessage = "Access denied. Please log in or create a member account.";
                return RedirectToPage("/Index", new { errorMessage });
            }

            return Page();
        }




        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int? memberId = httpContextAccessor.HttpContext.Session.GetInt32("MemberID");
            if (!memberId.HasValue || memberId.Value == 0)
            {
                string errorMessage = "Access denied. Please log in or create a member account.";
                return RedirectToPage("/Index", new { errorMessage });
            }

            var selectedItems = Request.Form["selectedItems"];
            var itemQuantities = Request.Form;

            var orderItems = new List<OrderItem>();

            int orderID = await orderService.CreateOrderAsync(memberId.Value, orderItems);

            if (orderID > 0)
            {
                foreach (var selectedItem in selectedItems)
                {
                    int itemId = int.Parse(selectedItem);
                    if (itemQuantities.TryGetValue($"itemQuantity[{itemId}]", out var quantityValue) && int.TryParse(quantityValue, out int quantity))
                    {
                        var orderItem = new OrderItem(orderID, itemId, quantity);

                        orderItems.Add(orderItem);
                    }
                    else
                    {
                        // Invalid Quanity omr?de, ved ikke om der er nogen s?
                    }
                }

                // Calculate the total price
                float totalPrice = orderService.CalculateTotalPrice(orderItems);

                // Update the order with the total price
                Jordnaer.Models.Orders order = new Jordnaer.Models.Orders
                {
                    OrderDate = DateTime.Now,
                    TotalPrice = totalPrice
                };

                bool orderUpdated = await orderService.UpdateOrderAsync(order, orderID);

                if (orderUpdated)
                {
                    // Order successfully created and updated
                    return RedirectToPage("/Orders/Index");
                }
            }

            // Failed to create order or update order
            return Page();
        }


    }
} 




