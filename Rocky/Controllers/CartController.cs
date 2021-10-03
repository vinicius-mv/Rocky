using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rocky.DataAccess;
using Rocky.DataAccess.Repository.Interfaces;
using Rocky.Models;
using Rocky.Utility;
using Rocky.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize(Roles = WebConstants.CustomerRole)]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly IEmailSender _emailSender;

        private readonly IProductRepository _productRepo;
        private readonly IApplicationUserRepository _appUserRepo;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;
        private readonly IInquiryDetailRepository _inquiryDetailRepo;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IProductRepository productRepo, IApplicationUserRepository appUserRepo, IInquiryHeaderRepository inquiryHeaderRepo,
            IInquiryDetailRepository inquiryDetailRepo, IWebHostEnvironment webHostEnviroment, IEmailSender emailSender)
        {
            _productRepo = productRepo;
            _appUserRepo = appUserRepo;
            _webHostEnviroment = webHostEnviroment;
            _emailSender = emailSender;

            _inquiryHeaderRepo = inquiryHeaderRepo;
            _inquiryDetailRepo = inquiryDetailRepo;
        }

        public IActionResult Index()
        {
            IList<ShoppingCart> shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WebConstants.SessionCart) ?? new List<ShoppingCart>();

            List<int> productIdInCartList = shoppingCartList.Select(x => x.ProductId).ToList();
            IEnumerable<Product> prodList = _productRepo.GetAll(x => productIdInCartList.Contains(x.Id));

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
            IEnumerable<Product> prodList = _productRepo.GetAll(x => productIdInCartList.Contains(x.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _appUserRepo.FirstOrDefault(x => x.Id == claimUserId.Value),
                ProductList = prodList.ToList()
            };

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost()
        {
            var pathToInquiryTemplate = $"{_webHostEnviroment.WebRootPath}{Path.DirectorySeparatorChar}templates{Path.DirectorySeparatorChar}Inquiry.html";

            var subject = "New Inquiry";
            var htmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(pathToInquiryTemplate))
            {
                htmlBody = sr.ReadToEnd();
            }
            // html placeholders
            // Name: {0} 
            // Email: {1}
            // Phone: {2}
            // Products: {3}

            var productListSB = new StringBuilder();
            foreach (var product in ProductUserVM.ProductList)
            {
                productListSB.Append($" - Name: {product.Name} <span style='font-size: 14px;'> (ID: {product.Id}) </span> <br />");
            }

            // replace placeholders with args: {0} -> ProductUserVM.ApplicationUser.FullName, etc..
            string messageBody = string.Format(htmlBody,
                ProductUserVM.ApplicationUser.FullName,         // {0} -> FullName
                ProductUserVM.ApplicationUser.Email,            // {1} -> Email
                ProductUserVM.ApplicationUser.PhoneNumber,      // {2} -> PhoneNumber
                productListSB.ToString());                      // {3} -> Products

            await _emailSender.SendEmailAsync(WebConstants.EmailAdmin, subject, messageBody);

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult Remove(int id)
        {
            IList<ShoppingCart> shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WebConstants.SessionCart) ?? new List<ShoppingCart>();

            var productToRemove = shoppingCartList.First(x => x.ProductId == id);
            if (productToRemove == null)
                throw new InvalidOperationException("Failed to remove product from cart");

            shoppingCartList.Remove(productToRemove);

            // Update Session Store 
            HttpContext.Session.Set<IList<ShoppingCart>>(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }
    }
}
