using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ReadLater5.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoriesController(ICategoryService categoryService, UserManager<IdentityUser> userManager)
        {
            _categoryService = categoryService;
            _userManager = userManager;
        }

        // GET: Categories
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var model = _categoryService.GetUserCategories(userId);
            return View(model);
        }

        // GET: Categories/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }

            var userId = _userManager.GetUserId(User);
            var category = _categoryService.GetUserCategory((int)id, userId);
            if (category == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);
            }

            return View(category);

        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.UserId = _userManager.GetUserId(User);
                _categoryService.CreateCategory(category);
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }

            var userId = _userManager.GetUserId(User);
            var category = _categoryService.GetUserCategory((int)id, userId);
            if (category == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);
            }

            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryService.UpdateCategory(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }

            var userId = _userManager.GetUserId(User);
            var category = _categoryService.GetUserCategory((int)id, userId);
            if (category == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var category = _categoryService.GetUserCategory((int)id, userId);

            if (category != null) 
                _categoryService.DeleteCategory(category);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateAjax(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Category name cannot be null or empty string.");
            }

            var userId = _userManager.GetUserId(User);
            var category = _categoryService.CreateCategory(new Category { Name = name, UserId = userId });
            return Json(new { id = category.Id, name = category.Name, userId = userId });
        }
    }
}
