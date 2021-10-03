using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.DataAccess;
using Rocky.DataAccess.Repository.Interfaces;
using Rocky.Models;
using Rocky.Utility;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    [Authorize(Roles = WebConstants.Roles.Admin)]
    public class CategoriesController : Controller
    {
        private ICategoryRepository _catRepo;

        public CategoriesController(ICategoryRepository categoryRepo)
        {
            _catRepo = categoryRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _catRepo.GetAll();
            return View(categories);
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
                _catRepo.Add(category);
                _catRepo.Save();
                TempData[WebConstants.Notifications.Success] = "Category created succesfully";
                return RedirectToAction(nameof(Index));
            }
            TempData[WebConstants.Notifications.Error] = "Failed to create category";
            return View(category);
        }

        // GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var category = _catRepo.Find(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Update(category);
                _catRepo.Save();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var category = _catRepo.Find(id.Value);
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

            var category = _catRepo.Find(id.Value);
            if (category == null)
            {
                TempData[WebConstants.Notifications.Error] = "Failed to delete category";
                return NotFound();
            }
            _catRepo.Remove(category);
            _catRepo.Save();
            TempData[WebConstants.Notifications.Success] = "Category deleted succesfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
