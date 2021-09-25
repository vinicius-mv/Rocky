using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky.Data;
using Rocky.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _context.Products.ToList();

            // load related 'Category' to the 'Product' (Manual Eager Loading)
            foreach (var product in products)
            {
                product.Category = _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }

            return View(products);
        }

        // GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        // POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            _context.Remove(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            // ViewBags and ViewDatas are used to pass aditional data to Views, usually from Controllers
            IEnumerable<SelectListItem> CategoryDropDown = _context.Categories.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            ViewBag.CategoryDropDown = CategoryDropDown;

            Product product = null;
            // Create
            if (id == null || id == 0)
            {
                product = new Product();
                return View(product);
            }

            // Update
            product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(product);
        }
    }
}
