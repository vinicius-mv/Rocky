using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.DataAccess;
using Rocky.DataAccess.Repository.Interfaces;
using Rocky.Models;
using Rocky.Utility;
using Rocky.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rocky.Controllers
{
    [Authorize(Roles = WebConstants.Roles.Admin)]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public ProductsController(IProductRepository productRepo, IWebHostEnvironment webHostEnvironment)
        {
            _productRepo = productRepo;
            _webHostEnviroment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            //IEnumerable<Product> products = _context.Products.Include(x => x.Category).Include(x => x.ApplicationType);

            string navPropsNames = $"{nameof(Product.Category)},{nameof(Product.ApplicationType)}";
            IEnumerable<Product> products = _productRepo.GetAll(includeProperties: navPropsNames, isTracking: false);

            // load related 'Category' to the 'Product' (Manual Eager Loading)
            //foreach (var product in products)
            //{
            //    product.Category = _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            //    product.ApplicationType = _context.ApplicationTypes.FirstOrDefault(c => c.Id == product.ApplicationTypeId);
            //}

            return View(products.ToList());
        }

        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            // Eager Loading to 'Include' beforehand Category on Product
            string navPropsName = $"{nameof(Product.Category)},{nameof(Product.ApplicationType)}";
            Product product = _productRepo.FirstOrDefault(includeProperties: navPropsName);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var product = _productRepo.Find(id.Value);
            if (product == null)
                return NotFound();

            // Set image path
            string ImagesFolderPath = _webHostEnviroment.WebRootPath + WebConstants.Paths.ProductImages;
            string oldImagePath = Path.Combine(ImagesFolderPath, product.Image);

            // Delete Image from server
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);

            _productRepo.Remove(product);
            _productRepo.Save();

            TempData[WebConstants.Notifications.Success] = "Action Completed Successfully";

            return RedirectToAction(nameof(Index));
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
                CategorySelectList = _productRepo.GetAllCategoriesDropDownList(),
                ApplicationTypeSelectList = _productRepo.GetAllApplicationTypesDropDownList()
            };

            // Create
            if (id == null || id == 0)
                return View(productVM);

            // Update
            productVM.Product = _productRepo.Find(id.Value);
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
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnviroment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    // Creating
                    // set new Image path
                    string ImagesFolderPath = webRootPath + WebConstants.Paths.ProductImages;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    string newImagePath = Path.Combine(ImagesFolderPath, fileName + extension);

                    // Copying form image to server
                    using (var fileStream = new FileStream(newImagePath, FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                    _productRepo.Add(productVM.Product);
                }
                else
                {
                    // Updating
                    var productDb = _productRepo.FirstOrDefault(filter: x => x.Id == productVM.Product.Id, isTracking: false);
                    if (productDb == null)
                        return NotFound();

                    if (files.Count > 0)
                    {
                        // set new image path
                        string ImagesFolderPath = webRootPath + WebConstants.Paths.ProductImages;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        string newImagePath = Path.Combine(ImagesFolderPath, fileName + extension);

                        // remove old file
                        var oldFile = Path.Combine(ImagesFolderPath, productDb?.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        // copy new image to server
                        using (var fileStream = new FileStream(newImagePath, FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Image = fileName + extension;
                    }
                    else // Product updated, but image is not updated -> keep the old image path
                    {
                        productVM.Product.Image = productDb.Image;
                    }
                    _productRepo.Update(productVM.Product);
                }
                _productRepo.Save();

                TempData[WebConstants.Notifications.Success] = "Action Completed Successfully";

                return RedirectToAction(nameof(Index));
            }
            productVM.CategorySelectList = _productRepo.GetAllCategoriesDropDownList();
            productVM.ApplicationTypeSelectList = _productRepo.GetAllApplicationTypesDropDownList();

            return View(productVM);
        }
    }
}
