using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rocky.DataAccess.Repository.Interfaces;
using Rocky.Models;
using Rocky.Utility;
using Rocky.Utility.BrainTree;
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
    [Authorize(Roles = WebConstants.Roles.Customer)]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly IEmailSender _emailSender;

        private readonly IProductRepository _productRepo;
        private readonly IApplicationUserRepository _appUserRepo;

        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;
        private readonly IInquiryDetailRepository _inquiryDetailRepo;

        private readonly IOrderHeaderRepository _orderHeaderRepo;
        private readonly IOrderDetailRepository _orderDetailRepo;

        private readonly IBrainTreeGate _brainTree;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IWebHostEnvironment webHostEnviroment, IEmailSender emailSender,
            IProductRepository productRepo, IApplicationUserRepository appUserRepo,
            IInquiryHeaderRepository inquiryHeaderRepo, IInquiryDetailRepository inquiryDetailRepo,
            IOrderHeaderRepository orderHeaderRepo, IOrderDetailRepository orderDetailRepo,
            IBrainTreeGate brainTree)
        {
            _productRepo = productRepo;
            _appUserRepo = appUserRepo;
            _webHostEnviroment = webHostEnviroment;
            _emailSender = emailSender;

            _inquiryHeaderRepo = inquiryHeaderRepo;
            _inquiryDetailRepo = inquiryDetailRepo;

            _orderHeaderRepo = orderHeaderRepo;
            _orderDetailRepo = orderDetailRepo;

            _brainTree = brainTree;
        }

        public IActionResult Index()
        {
            IList<ShoppingCart> shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WebConstants.Sessions.ShoppingCartList) ?? new List<ShoppingCart>();

            List<int> productIdInCartList = shoppingCartList.Select(x => x.ProductId).ToList();
            IEnumerable<Product> prodList = _productRepo.GetAll(x => productIdInCartList.Contains(x.Id));

            foreach (var cartObj in shoppingCartList)
            {
                Product productTemp = prodList.First(x => x.Id == cartObj.ProductId);
                productTemp.TempSqft = cartObj.SqFt;
            }
            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost(IEnumerable<Product> products)
        {
            IList<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (var prod in products)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, SqFt = prod.TempSqft });
            }
            HttpContext.Session.Set(WebConstants.Sessions.ShoppingCartList, shoppingCartList);

            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            ApplicationUser appUser;

            if (User.IsInRole(WebConstants.Roles.Admin))
            {
                int sessionInquiryHeaderId = HttpContext.Session.Get<int>(WebConstants.Sessions.InquiryHeaderId);
                if (sessionInquiryHeaderId > 0)
                {
                    // Admin Updating an Inquiry)
                    InquiryHeader inquiryHeader = _inquiryHeaderRepo.FirstOrDefault(x => x.Id == sessionInquiryHeaderId);
                    appUser = new ApplicationUser()
                    {
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
                        PhoneNumber = inquiryHeader.PhoneNumber
                    };
                }
                else
                {
                    // Admin creating a new Inquiry
                    appUser = new ApplicationUser();
                }

                var gateway = _brainTree.GetGateway();
                var clientToken = gateway.ClientToken.Generate();
                ViewBag.ClientToken = clientToken;
            }
            else // Customer creating a new Inquiry
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                //var claimUserId = User.FindFirstValue(ClaimTypes.Name);
                var claimUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                appUser = _appUserRepo.FirstOrDefault(x => x.Id == claimUserId.Value);
            }

            // Getting Session Shopping Cart List - Get Products Details
            IList<ShoppingCart> shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WebConstants.Sessions.ShoppingCartList) ?? new List<ShoppingCart>();
            List<int> productIdInCartList = shoppingCartList.Select(x => x.ProductId).ToList();
            IEnumerable<Product> prodList = _productRepo.GetAll(x => productIdInCartList.Contains(x.Id));

            // Add SqFt info to the ProductList
            foreach (ShoppingCart cartItem in shoppingCartList)
            {
                Product prodTemp = prodList.FirstOrDefault(x => x.Id == cartItem.ProductId);
                prodTemp.TempSqft = cartItem.SqFt;
            }

            // Creating a VM with User Details and Product Details
            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = appUser,
                ProductList = prodList.ToList()
            };

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claimUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (User.IsInRole(WebConstants.Roles.Admin))
            {
                OrderHeader orderHeader = CreateOrder(claimUserId);

                return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeader.Id });
            }
            else
            {
                await CreateInquiry(claimUserId);
            }
            return RedirectToAction(nameof(InquiryConfirmation));
        }

        private OrderHeader CreateOrder(Claim claimUserId)
        {
            OrderHeader orderHeader = new OrderHeader()
            {
                CreatedByUserId = claimUserId.Value,
                FinalOrderTotal = ProductUserVM.ProductList.Sum(p => p.Price * p.TempSqft),
                City = ProductUserVM.ApplicationUser.City,
                StreetAddress = ProductUserVM.ApplicationUser.StreetAddress,
                State = ProductUserVM.ApplicationUser.State,
                PostalCode = ProductUserVM.ApplicationUser.PostalCode,
                FullName = ProductUserVM.ApplicationUser.Email,
                PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                OrderDate = DateTime.Now,
                OrderStatus = WebConstants.OrderStatus.Pending
            };

            _orderHeaderRepo.Add(orderHeader);
            _orderHeaderRepo.Save(); // database generate id for orderHeader

            foreach (var prod in ProductUserVM.ProductList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId = orderHeader.Id,
                    PricePerSqFt = prod.Price,
                    Sqft = prod.TempSqft,
                    ProductId = prod.Id
                };
                _orderDetailRepo.Add(orderDetail);
                _orderDetailRepo.Save();
            }

            return orderHeader;
        }

        private async Task CreateInquiry(Claim claimUserId)
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

            await _emailSender.SendEmailAsync(WebConstants.Settings.EmailAdmin, subject, messageBody);

            InquiryHeader inquiryHeader = new InquiryHeader()
            {
                ApplicationUserId = claimUserId.Value,
                FullName = ProductUserVM.ApplicationUser.FullName,
                Email = ProductUserVM.ApplicationUser.Email,
                PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                InquiryDate = DateTime.Now
            };

            _inquiryHeaderRepo.Add(inquiryHeader);
            _inquiryHeaderRepo.Save();

            foreach (var product in ProductUserVM.ProductList)
            {
                InquiryDetail inquiry = new InquiryDetail()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = product.Id,
                };
                _inquiryDetailRepo.Add(inquiry);
            }
            _inquiryDetailRepo.Save();

            TempData[WebConstants.Notifications.Success] = "Action Completed Successfully";
        }

        public IActionResult InquiryConfirmation(int id = 0)
        {
            // if id > 0, send Order to the view (Order Confirmation)
            // else send null to the view (Inquiry Confirmation)
            OrderHeader orderHeader = _orderHeaderRepo.FirstOrDefault(x => x.Id == id);
            HttpContext.Session.Clear();
            return View(orderHeader);
        }

        public IActionResult Remove(int id)
        {
            IList<ShoppingCart> shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WebConstants.Sessions.ShoppingCartList) ?? new List<ShoppingCart>();

            var productToRemove = shoppingCartList.First(x => x.ProductId == id);
            if (productToRemove == null)
                throw new InvalidOperationException("Failed to remove product from cart");

            shoppingCartList.Remove(productToRemove);

            // Update Session Store
            HttpContext.Session.Set<IList<ShoppingCart>>(WebConstants.Sessions.ShoppingCartList, shoppingCartList);

            TempData[WebConstants.Notifications.Success] = "Action Completed Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> productList)
        {
            List<ShoppingCart> shoppingCartist = new List<ShoppingCart>();
            foreach (var product in productList)
            {
                shoppingCartist.Add(new ShoppingCart() { ProductId = product.Id, SqFt = product.TempSqft });
            }
            HttpContext.Session.Set(WebConstants.Sessions.ShoppingCartList, shoppingCartist);

            return RedirectToAction(nameof(Index));
        }
    }
}