using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.DataAccess;
using Rocky.DataAccess.Repository.Interfaces;
using Rocky.Models;
using Rocky.Utility;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;

        private readonly IInquiryDetailRepository _inquiryDetailRepo;

        public InquiryController(IInquiryHeaderRepository inquiryHeaderRepo, IInquiryDetailRepository inquiryDetailRepo)
        {
            _inquiryHeaderRepo = inquiryHeaderRepo;
            _inquiryDetailRepo = inquiryDetailRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
