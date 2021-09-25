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

        // GET - EDIT
        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
                return NotFound();

            var appType = _context.ApplicationTypes.Find(id);
            if(appType == null)
                return NotFound();

            return View(appType);
        }

        // POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType appType)
        {
            if(ModelState.IsValid)
            {
                _context.ApplicationTypes.Update(appType);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(appType);
        }

        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if(id == null || id == 0)
                return NotFound();

            var appType = _context.ApplicationTypes.Find(id);
            if(appType == null)
                return NotFound();

            return View(appType);
        }

        // POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if(id == null || id == 0)
                return NotFound();

            var appType = _context.ApplicationTypes.Find(id);
            if(appType == null)
                return NotFound();

            _context.Remove(appType);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
