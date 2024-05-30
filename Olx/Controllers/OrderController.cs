using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Models;
using Olx.ViewModels;

namespace Olx.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly ShopDbContext _context;
    private readonly IEmailSender _emailSender;

    public OrderController(ShopDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    [Route("/delivery/{id:int:min(1)}")]
    public async Task<IActionResult> Delivery(int? id)
    {
        var product = _context.Products.Include(product => product.Category)
            .ThenInclude(category => category.Parent).Include(p => p.Owner)
            .Include(p => p.Category).FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        
        var order = new Order
        {
            ProductId = product.Id,
            Product = product,
            BuyerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Buyer = _context.Users.Find(User.FindFirstValue(ClaimTypes.NameIdentifier))
        };
        
        List<Category> categoryChain = [];
        var category = product.Category;
        while (category is not null)
        {
            categoryChain.Add(category);
            category = category.Parent;
        }
        
        var viewModel = new OrderViewModel
        {
            Order = order,
            Title = "Доставка",
            Categories = categoryChain
        };
        
        ViewData["OrderViewModel"] = viewModel;
        return View(order);
    }

    [HttpPost]
    [Route("/delivery/{id:int:min(1)}")]
    public async Task<IActionResult> Delivery(Order order, string deliveryMethod)
    {
        if (!ModelState.IsValid)
        {
            return View(order);
        }

        order.Id = 0; // Reset the id to avoid conflicts
        order.DeliveryMethod = (DeliveryMethod)int.Parse(deliveryMethod);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return RedirectToAction("Payment", "Order", new {id = order.ProductId});
    }
    
    [Route("/payment/{id:int:min(1)}")]
    public async Task<IActionResult> Payment(int? id)
    {
        var order = _context.Orders.Include(o => o.Product).ThenInclude(product => product.Category)
            .Include(o => o.Buyer)
            .FirstOrDefault(o => o.ProductId == id && o.BuyerId == User.FindFirstValue(ClaimTypes.NameIdentifier) && !o.IsPaymentCompleted);
        if (order is null)
        {
            return NotFound();
        }
        
        List<Category> categoryChain = [];
        var category = order.Product.Category;
        while (category is not null)
        {
            categoryChain.Add(category);
            category = category.Parent;
        }
        
        var viewModel = new OrderViewModel
        {
            Order = order,
            Title = "Оплата",
            Categories = categoryChain
        };
        
        ViewData["OrderViewModel"] = viewModel;
        return View(order);
    }
    
    [HttpPost]
    [Route("/payment/{id:int:min(1)}")]
    public async Task<IActionResult> Payment(int id, string? cardNumber, string? cardExpirationDate, string? cardCvv, string? paymentMethod)
    {
        var order = _context.Orders.Include(order => order.Product).ThenInclude(product => product.Category)
            .ThenInclude(category => category.Parent).Include(o => o.Product).ThenInclude(product => product.Category)
            .Include(o => o.Buyer)
            .FirstOrDefault(o => o.ProductId == id && o.BuyerId == User.FindFirstValue(ClaimTypes.NameIdentifier) && !o.IsPaymentCompleted);
        if (order is null)
        {
            return NotFound();
        }

        var cardNeeded = paymentMethod is "byCard" or "prepayment";
        if (cardNumber is not null && cardNeeded)
        {
            if (!LuhnAlgorithm(cardNumber))
            {
                ModelState.AddModelError("CardNumber", "Невалідний номер картки");
            }
        }
        
        if (cardExpirationDate is not null && cardNeeded)
        {
            if (!checkExpirationDate(cardExpirationDate))
            {
                ModelState.AddModelError("CardExpirationDate", "Невалідний термін дії картки");
            }
        }
        
        if (cardCvv is not null && cardNeeded)
        {
            if (!CheckCvv(cardCvv))
            {
                ModelState.AddModelError("CardCvv", "Невалідний CVV код");
            }
        }
        
        if (!ModelState.IsValid)
        {
            List<Category> categoryChain = [];
            var category = order.Product.Category;
            while (category is not null)
            {
                categoryChain.Add(category);
                category = category.Parent;
            }
        
            var viewModel = new OrderViewModel
            {
                Order = order,
                Title = "Оплата",
                Categories = categoryChain
            };
            ViewData["OrderViewModel"] = viewModel;
            return View(order);
        }
        
        order.IsPaymentCompleted = true;
        order.State = OrderState.Processing;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        await _emailSender.SendEmailAsync(order.ReceiverEmail, "Замовлення успішно оплачено",
            $"Ваше замовлення на {order.Product.Name} успішно оплачено. Сума: {order.Product.Price}. Очікуйте на підтвердження від продавця.");
        return RedirectToAction("Index", "Home");
    }
    
    private bool LuhnAlgorithm(string cardNumber)
    {
        int sum = 0;
        for (int i = 0; i < cardNumber.Length; i++)
        {
            int digit = cardNumber[i] - '0';
            if (i % 2 == 0)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }
            sum += digit;
        }
        return sum % 10 == 0;
    }
    
    private bool CheckCvv(string cvv)
    {
        return cvv.Length is 3 or 4 && cvv.All(char.IsDigit);
    }
    
    private bool checkExpirationDate(string expirationDate)
    {
        if (expirationDate.Length != 5)
        {
            return false;
        }
        if (expirationDate[2] != '/')
        {
            return false;
        }
        if (!expirationDate.Substring(0, 2).All(char.IsDigit) || !expirationDate.Substring(3, 2).All(char.IsDigit))
        {
            return false;
        }
        var month = int.Parse(expirationDate.Substring(0, 2));
        var year = int.Parse(expirationDate.Substring(3, 2));
        var expiration = new DateTime(2000 + year, month, 1);
        return expiration > DateTime.Now;
    }
}