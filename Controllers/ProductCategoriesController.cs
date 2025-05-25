using Microsoft.AspNetCore.Mvc;
using Project1.Data;
using Project1.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Project1.Controllers
{
    [Authorize]
    public class ProductCategoriesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public ProductCategoriesController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var categories = await _context.ProductCategories
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            var deleted = await _context.ProductCategories
                .Where(c => c.IsDeleted)
                .ToListAsync();

            ViewBag.DeletedCategories = deleted;
            ViewData["PageNumber"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalCount"] = categories.Count;

            return View(categories);
        }

        [HttpGet]
        public IActionResult Search(string term)
        {
            var results = _context.ProductCategories
                .Where(c => !c.IsDeleted && c.Name.Contains(term))
                .Select(c => new
                {
                    c.Id,
                    c.Name
                })
                .ToList();

            return Json(results);
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null) return NotFound();

            category.IsDeleted = false;
            category.DeletedAt = null;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        public IActionResult Create()
        {
            var model = new ProductCategoryViewModel
            {
                NewCategory = new ProductCategory(),
                ExistingCategories = _context.ProductCategories.Where(c => !c.IsDeleted).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicate = await _context.ProductCategories
                    .AnyAsync(c => c.Name.ToLower() == model.NewCategory.Name.ToLower() && !c.IsDeleted);

                if (isDuplicate)
                {
                    ModelState.AddModelError("NewCategory.Name", "Category name already exists.");
                    model.ExistingCategories = _context.ProductCategories.Where(c => !c.IsDeleted).ToList();
                    return View(model);
                }

                _context.ProductCategories.Add(model.NewCategory);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction("Index");
            }

            model.ExistingCategories = _context.ProductCategories.Where(c => !c.IsDeleted).ToList();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductCategory category)
        {
            if (ModelState.IsValid)
            {
                _context.ProductCategories.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category != null)
            {
                category.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
