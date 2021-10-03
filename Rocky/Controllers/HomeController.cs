using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky.DataAccess;
using Rocky.DataAccess.Repository.Interfaces;
using Rocky.Models;
using Rocky.Utility;
using Rocky.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IProductRepository _productRepo;

        private readonly ICategoryRepository _categoryRepo;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepo, ICategoryRepository categoryRepo)
        {
            _logger = logger;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public IActionResult Index()
        {
            string productNavProps = $"{nameof(Product.Category)},{nameof(Product.ApplicationType)}";

            var homeVM = new HomeVM()
            {
                Products = _productRepo.GetAll(includeProperties: productNavProps, isTracking: false),
                Categories = _categoryRepo.GetAll(isTracking: false),
            };

            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            if (id <= 0)
                return NotFound();

            // check if there is any Product on the Session Store
            var shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WebConstants.Sessions.ShoppingCartList) ?? new List<ShoppingCart>();

            string productNavProps = $"{nameof(Product.Category)},{nameof(Product.ApplicationType)}";
            var detailsVM = new DetailsVM()
            {
                Product = _productRepo.FirstOrDefault(p => p.Id == id, includeProperties: productNavProps, isTracking: false),
                ExistsInCart = ShoppingCartContains(productId: id, shoppingCartList)
            };

            return View(detailsVM);
        }

        private static bool ShoppingCartContains(int productId, IList<ShoppingCart> shoppingCartList)
        {
            foreach (var shoppingCartItem in shoppingCartList)
            {
                if (shoppingCartItem.ProductId == productId)
                    return true;
            }
            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPost(int id)
        {
            // check if there is any Product on the Session Store
            var shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WebConstants.Sessions.ShoppingCartList) ?? new List<ShoppingCart>();

            shoppingCartList.Add(new ShoppingCart { ProductId = id });

            // Update the Session Store
            HttpContext.Session.Set(WebConstants.Sessions.ShoppingCartList, shoppingCartList);

            TempData[WebConstants.Notifications.Success] = "Action Completed Successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            // check if there is any Product on the Session Store
            var shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WebConstants.Sessions.ShoppingCartList) ?? new List<ShoppingCart>();

            var itemToRemove = shoppingCartList.SingleOrDefault(x => x.ProductId == id);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            // Update the Session Store
            HttpContext.Session.Set(WebConstants.Sessions.ShoppingCartList, shoppingCartList);

            TempData[WebConstants.Notifications.Success] = "Action Completed Successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ViewModels.ErrorVM { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
