using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rocky.Controllers
{   
    public class ApplicationTypesController : Controller
    {
        private ApplicationDbContext _context;

        public ApplicationTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> appTypes = _context.ApplicationTypes;

            return View(appTypes);
        }

        // GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        // POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType appType)
        {
            _context.ApplicationTypes.Add(appType);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
