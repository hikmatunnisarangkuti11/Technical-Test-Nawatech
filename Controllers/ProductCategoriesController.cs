using Microsoft.AspNetCore.Mvc;
using Project1.Data;
using Project1.Models;

public class ProductCategoriesController : Controller
{
    private readonly AppDbContext _context;

    public ProductCategoriesController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var categories = _context.ProductCategories
            .Where(c => !c.IsDeleted)
            .ToList();
        return View(categories);
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
    public IActionResult Create(ProductCategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool isDuplicate = _context.ProductCategories
                .Any(c => c.Name.ToLower() == model.NewCategory.Name.ToLower() && !c.IsDeleted);

            if (isDuplicate)
            {
                ModelState.AddModelError("NewCategory.Name", "Category name already exists.");
                model.ExistingCategories = _context.ProductCategories.Where(c => !c.IsDeleted).ToList();
                return View(model);
            }

            _context.ProductCategories.Add(model.NewCategory);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Category created successfully!";
            return RedirectToAction("Index");
        }

        model.ExistingCategories = _context.ProductCategories.Where(c => !c.IsDeleted).ToList();
        return View(model);
    }


    public IActionResult Edit(int id)
    {
        var category = _context.ProductCategories.Find(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    [HttpPost]
    public IActionResult Edit(ProductCategory category)
    {
        if (ModelState.IsValid)
        {
            _context.ProductCategories.Update(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(category);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var category = _context.ProductCategories.Find(id);
        if (category != null)
        {
            category.IsDeleted = true;
            _context.SaveChanges();
        }
        return RedirectToAction("Index", "ProductCategories");
    }
}
