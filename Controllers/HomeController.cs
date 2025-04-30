
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text; 
using PizzaShop.Models; 


namespace PizzaShop.Controllers { 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
           
            return View(new Order());
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult Index(Order order, string myButton) 
        {
         
            if (myButton == "Clear")
            {
                ModelState.Clear(); // Clear validation state
                return View(new Order()); // Return a fresh view
            }

            // Handle "PreCompute Order" or "Place Order" button clicks [cite: 4]
            if (myButton == "PreCompute" || myButton == "PlaceOrder")
            {
                // Remove calculated fields from validation check as they are outputs
                ModelState.Remove("TotalCost");
                ModelState.Remove("OrderSummary");
                ModelState.Remove("Email"); 

                // Check if PizzaSize and PizzaType are provided [cite: 7, 8]
                if (ModelState.IsValid)
                {
                    decimal totalCost = 0;

                    // Calculate base cost from Pizza Size
                    switch (order.PizzaSize)
                    {
                        case "Small": totalCost += 4.99m; break;
                        case "Medium": totalCost += 5.99m; break;
                        case "Large": totalCost += 6.99m; break;
                        case "X-Large": totalCost += 7.99m; break;
                    }

                    // Add cost for each selected topping
                    if (order.Toppings != null)
                    {
                        totalCost += order.Toppings.Count * 0.99m;
                    }

                    order.TotalCost = totalCost;

                    // Build the order summary string - Base part
                    string toppingsList = (order.Toppings != null && order.Toppings.Count > 0)
                                            ? string.Join(", ", order.Toppings)
                                            : "None";

                    StringBuilder summaryBuilder = new StringBuilder();
                    summaryBuilder.AppendLine($"Toppings: {toppingsList}");
                    summaryBuilder.AppendLine($"Size: {order.PizzaSize}");
                    summaryBuilder.AppendLine($"Type: {order.PizzaType}");
                    summaryBuilder.Append($"Total amount: {totalCost:C}"); 

                    // *** ADD CONDITION FOR THANK YOU MESSAGE ***
                    if (myButton == "PlaceOrder") 
                    {
                        summaryBuilder.Append("; Thank you for your business!"); 
                    }

                    order.OrderSummary = summaryBuilder.ToString();

             

                    return View(order); 
                }
                else
                {
   
                    return View(order);
                }
            }

           
            return View(order);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public class ErrorViewModel
        {
            public string? RequestId { get; set; }
            public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        }
    }
}