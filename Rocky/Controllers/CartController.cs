using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using Rocky.Utility;
using Rocky.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty] // Automatically binds to post Actions
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            IList<ShoppingCart> shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WebConstants.SessionCart) ?? new List<ShoppingCart>();

            List<int> productIdInCartList = shoppingCartList.Select(x => x.ProductId).ToList();
            IEnumerable<Product> prodList = _context.Products.Where(x => productIdInCartList.Contains(x.Id));
           
            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claimUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //var claimUserId = User.FindFirstValue(ClaimTypes.Name);

            IList<ShoppingCart> shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WebConstants.SessionCart) ?? new List<ShoppingCart>();

            List<int> productIdInCartList = shoppingCartList.Select(x => x.ProductId).ToList();
            IEnumerable<Product> prodList = _context.Products.Where(x => productIdInCartList.Contains(x.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _context.ApplicationUsers.FirstOrDefault(x => x.Id == claimUserId.Value),
            };

            return View(ProductUserVM);
        }

        public IActionResult Remove(int id)
        {   
            IList<ShoppingCart> shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WebConstants.SessionCart) ?? new List<ShoppingCart>();

            var productToRemove = shoppingCartList.First(x => x.ProductId == id);
            if(productToRemove == null)
                throw new InvalidOperationException("Failed to remove product from cart");

            shoppingCartList.Remove(productToRemove);

            // Update Session Store 
            HttpContext.Session.Set<IList<ShoppingCart>>(WebConstants.SessionCart, shoppingCartList);  

            return RedirectToAction(nameof(Index));
        }
    }
}
