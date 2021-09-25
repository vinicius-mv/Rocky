using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky.Data;
using Rocky.Models;
using Rocky.ViewModels;
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
            // ViewBag - ViewData: transfer data from Controllers to Views where temporary data is not in a Model, not vice - versa, life cycle = current httpRequest
            // ViewBag is a dyannmic property and actually is a wrapper around ViewData
            // ViewData is a dictionary, values must be cast before use
            // loosely typed view - not ideal
            // IEnumerable<SelectListItem> CategoryDropDown = _context.Categories.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            // ViewData["CategoryDropDown"] = CategoryDropDown;
            // ViewBag.CategoryDropDown = CategoryDropDown;

            // ViewModel(VM): we could achieve the same result, thus we obtain strongly typed Views and Validations with DataAnnotations

            var productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _context.Categories.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
            };

            // Create
            if (id == null || id == 0)
            {
                return View(productVM);
            }

            // Update
            productVM.Product = _context.Products.Find(id);
            if (productVM.Product == null)
                return NotFound();

            return View(productVM);
        }

        // POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                if(productVM.Product.Id > 0)
                    _context.Products.Update(productVM.Product);
                else
                    _context.Products.Add(productVM.Product);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(productVM);
        }
    }
}
