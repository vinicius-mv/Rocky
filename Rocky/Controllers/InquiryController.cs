using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.DataAccess;
using Rocky.DataAccess.Repository.Interfaces;
using Rocky.Models;
using Rocky.Utility;
using Rocky.ViewModels;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    [Authorize(Roles = WebConstants.Roles.Admin)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;

        private readonly IInquiryDetailRepository _inquiryDetailRepo;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryHeaderRepository inquiryHeaderRepo, IInquiryDetailRepository inquiryDetailRepo)
        {
            _inquiryHeaderRepo = inquiryHeaderRepo;
            _inquiryDetailRepo = inquiryDetailRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            string navProps = $"{nameof(InquiryDetail.Product)}";
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inquiryHeaderRepo.FirstOrDefault(x => x.Id == id),
                InquiryDetails = _inquiryDetailRepo.GetAll(x => x.InquiryHeaderId == id, includeProperties: navProps)
            };

            return View(InquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPost()
        {
            this.InquiryVM.InquiryDetails = _inquiryDetailRepo.GetAll(x => x.InquiryHeaderId == this.InquiryVM.InquiryHeader.Id);

            IList<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            foreach (var details in InquiryVM.InquiryDetails)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = details.ProductId
                };
                shoppingCartList.Add(shoppingCart);
            }
            // Write New Session - ShopingCartList
            HttpContext.Session.Clear();
            HttpContext.Session.Set<IList<ShoppingCart>>(WebConstants.Sessions.ShoppingCartList, shoppingCartList);
            HttpContext.Session.Set<int>(WebConstants.Sessions.InquiryHeaderId, this.InquiryVM.InquiryHeader.Id);

            return RedirectToAction(nameof(CartController.Index), "Cart");
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inquiryHeaderRepo.GetAll() });
        }

        #endregion
    }
}
