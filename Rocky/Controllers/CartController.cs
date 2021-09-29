using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using Rocky.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Rocky.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

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
    }
}
