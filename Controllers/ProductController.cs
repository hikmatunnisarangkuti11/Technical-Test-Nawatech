using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project1.Data;
using Project1.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Project1.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10, int deletedPageNumber = 1, int deletedPageSize = 10)
        {
            var activeProductsQuery = _context.Products
                .Include(p => p.ProductCategory)
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id);

            var totalCount = activeProductsQuery.Count();

            var products = activeProductsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var deletedProductsQuery = _context.Products
                .Include(p => p.ProductCategory)
                .Where(p => p.IsDeleted)
                .OrderBy(p => p.Id);

            var deletedCount = deletedProductsQuery.Count();

            var deletedProducts = deletedProductsQuery
                .Skip((deletedPageNumber - 1) * deletedPageSize)
                .Take(deletedPageSize)
                .ToList();

            ViewData["PageNumber"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalCount"] = totalCount;

            ViewData["DeletedPageNumber"] = deletedPageNumber;
            ViewData["DeletedPageSize"] = deletedPageSize;
            ViewData["DeletedTotalCount"] = deletedCount;

            ViewBag.DeletedProducts = deletedProducts;

            return View(products);
        }

        [HttpPost]
        public IActionResult Restore(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null && product.IsDeleted)
            {
                product.IsDeleted = false;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
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

        [HttpGet]
        public IActionResult Search(string term)
        {
            var results = _context.Products
                .Include(p => p.ProductCategory)
                .Where(p => !p.IsDeleted && p.Name.Contains(term))
                .Select(p => new {
                    p.Id,
                    p.Name,
                    CategoryName = p.ProductCategory.Name,
                    p.Stock,
                    p.Price,
                    p.Picture
                })
                .ToList();

            return Json(results);
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
        public IActionResult Edit(Product updatedProduct, IFormFile? pictureFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == updatedProduct.Id);
                if (existingProduct == null)
                    return NotFound();

                updatedProduct.Stock = existingProduct.Stock;

                if (pictureFile != null && pictureFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(pictureFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        pictureFile.CopyTo(stream);
                    }

                    updatedProduct.Picture = fileName;
                }
                else
                {
                    updatedProduct.Picture = existingProduct.Picture;
                }

                _context.Products.Update(updatedProduct);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.ProductCategories.Where(c => !c.IsDeleted).ToList(), "Id", "Name", updatedProduct.ProductCategoryId);
            return View(updatedProduct);
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
            if (product != null && !product.IsDeleted)
            {
                product.IsDeleted = true;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
