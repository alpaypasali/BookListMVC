using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _database;

        [BindProperty]
        public Book Book { get; set; }

        public BooksController(ApplicationDbContext database)
        {
            _database = database;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Book = new Book(); //prepare a creation by default

            if (id != null) //detemins if it is an update
            {
                Book = _database.Books.FirstOrDefault(book => book.Id == id); //sets the new Book state

                if (Book == null)
                {
                    return NotFound();
                }
            }

            return View(Book);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Book.Id == 0)
                {
                    //Create
                    _database.Add(Book);
                }
                else
                {
                    //Update
                    _database.Books.Update(Book);
                }

                _database.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(Book);
        }

        #region API Calls

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _database.Books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _database.Books.FirstOrDefaultAsync(book => book.Id == id);
            if (bookFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _database.Books.Remove(bookFromDb);
            await _database.SaveChangesAsync();

            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
