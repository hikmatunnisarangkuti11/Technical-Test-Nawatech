using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project1.Data;
using Project1.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Project1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.ProductCategory)
                .Where(p => !p.IsDeleted)
                .ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(
                _context.ProductCategories.Where(c => !c.IsDeleted).ToList(),
                "Id",
                "Name"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult Create(Product product, IFormFile? pictureFile)
        {
            if (ModelState.IsValid)
            {
                if (pictureFile != null && pictureFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(pictureFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        pictureFile.CopyTo(stream);
                    }

                    product.Picture = fileName;
                }

                _context.Products.Add(product);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(
                _context.ProductCategories.Where(c => !c.IsDeleted).ToList(),
                "Id",
                "Name",
                product.ProductCategoryId
            );
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_context.ProductCategories.Where(c => !c.IsDeleted).ToList(), "Id", "Name", product.ProductCategoryId);
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? pictureFile)
        {
            if (ModelState.IsValid)
            {
                if (pictureFile != null && pictureFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(pictureFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        pictureFile.CopyTo(stream);
                    }

                    product.Picture = fileName;
                }

                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.ProductCategories.Where(c => !c.IsDeleted).ToList(), "Id", "Name", product.ProductCategoryId);
            return View(product);
        }

        public IActionResult EditStock(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product); 
        }

        [HttpPost]
        public IActionResult EditStock(int id, int stock)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            product.Stock = stock;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.IsDeleted = true;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
