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
    public class ApplicationTypesController : Controller
    {
        private IApplicationTypeRepository _appTypeRepo;

        public ApplicationTypesController(IApplicationTypeRepository appTypeRepo)
        {
            _appTypeRepo = appTypeRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> appTypes = _appTypeRepo.GetAll();

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
            _appTypeRepo.Add(appType);
            _appTypeRepo.Save();

            return RedirectToAction(nameof(Index));
        }

        // GET - EDIT
        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
                return NotFound();

            var appType = _appTypeRepo.Find(id.Value);
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
                _appTypeRepo.Update(appType);
                _appTypeRepo.Save();

                return RedirectToAction(nameof(Index));
            }
            return View(appType);
        }

        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if(id == null || id == 0)
                return NotFound();

            var appType = _appTypeRepo.Find(id.Value);
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

            var appType = _appTypeRepo.Find(id.Value);
            if(appType == null)
                return NotFound();

            _appTypeRepo.Remove(appType);
            _appTypeRepo.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
