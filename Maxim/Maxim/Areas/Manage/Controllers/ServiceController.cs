using Maxim.DAL;
using Maxim.Helpers;
using Maxim.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maxim.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ServiceController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.Services.ToList();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Service service)
        {
            if (service.ImageFile != null)
            {
                if (service.ImageFile.ContentType != "image/png" && service.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File content type must be either png or jpeg!");
                    return View();
                }
                if (service.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "File size should be less than 2 MB!");
                    return View();
                }
            }
           

            if (!ModelState.IsValid)
            {
                return View();
            }

            service.ImageName = FileManager.Save(_env.WebRootPath, "uploads/services", service.ImageFile);

            _context.Services.Add(service);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Service service = _context.Services.FirstOrDefault(x => x.Id == id);

            if (service == null)
            {
                return RedirectToAction("error", "dashboard");
            }

            return View(service);
        }

        [HttpPost]
        public IActionResult Edit(Service service)
        {
            Service existService = _context.Services.FirstOrDefault(x => x.Id == service.Id);

            if(existService == null)
            {
                return RedirectToAction("error", "dashboard");
            }

            if (service.ImageFile != null)
            {
                if (service.ImageFile.ContentType != "image/png" && service.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File content type must be either png or jpeg!");
                    return View();
                }
                if (service.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "File size should be less than 2 MB!");
                    return View();
                }
                if (!ModelState.IsValid)
                {
                    return View();
                }

                string newFileName = FileManager.Save(_env.WebRootPath, "uploads/services", service.ImageFile);

                FileManager.Delete(_env.WebRootPath, "uploads/services", existService.ImageName);

                existService.ImageName = newFileName;
                

            }

            existService.Title = service.Title;
            existService.Description = service.Description;

            _context.SaveChanges();

            return RedirectToAction("Index");

        }

        public IActionResult Delete(int id)
        {
            Service service = _context.Services.FirstOrDefault(x => x.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            FileManager.Delete(_env.WebRootPath, "uploads/services", service.ImageName);

            _context.Services.Remove(service);
            _context.SaveChanges();

            return Ok();
        }
    }
}
