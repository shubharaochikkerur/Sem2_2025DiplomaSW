using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentInfoLoginRoles.Models;

namespace StudentInfoLoginRoles.Controllers
{
    public class FeeTransactionsController : Controller
    {
        private readonly ApplicationContext _context;

        public FeeTransactionsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: FeeTransactions
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.FeeTransactions.Include(f => f.Student);
            return View(await applicationContext.ToListAsync());
        }

        // GET: FeeTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feeTransaction = await _context.FeeTransactions
                .Include(f => f.Student)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (feeTransaction == null)
            {
                return NotFound();
            }

            return View(feeTransaction);
        }

        // GET: FeeTransactions/Create
        public IActionResult Create()
        {

            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FirstName");
            return View();
        }

        // POST: FeeTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,StudentId,TransactionDate,Mode,TransactionAmount")] FeeTransaction feeTransaction)
        {
            /*
             * I am replacing this with ExecuteSqlRaw so that the trigger will run
             * the trigger gets blocked because of OUTPUT clause in EF Core
            if (ModelState.IsValid)
            {
                _context.Add(feeTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FirstName", feeTransaction.StudentId);
            return View(feeTransaction);
            */

            if (ModelState.IsValid)
            {
                // Run the INSERT manually to avoid EF's OUTPUT clause
                await _context.Database.ExecuteSqlRawAsync(
                    @"INSERT INTO FeeTransactions (StudentId, TransactionDate, Mode, TransactionAmount)
            VALUES ({0}, {1}, {2}, {3})",
                    feeTransaction.StudentId,
                    feeTransaction.TransactionDate,
                    feeTransaction.Mode,
                    feeTransaction.TransactionAmount);

                // Trigger runs automatically here, updating Students.FeePending
                return RedirectToAction(nameof(Index));
            }


            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FirstName", feeTransaction.StudentId);
            return View(feeTransaction);
        }

        // GET: FeeTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feeTransaction = await _context.FeeTransactions.FindAsync(id);
            if (feeTransaction == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FirstName", feeTransaction.StudentId);
            return View(feeTransaction);
        }

        // POST: FeeTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionId,StudentId,TransactionDate,Mode,TransactionAmount")] FeeTransaction feeTransaction)
        {
            if (id != feeTransaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feeTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeeTransactionExists(feeTransaction.TransactionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FirstName", feeTransaction.StudentId);
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FirstName", feeTransaction.StudentId);
            return View(feeTransaction);
        }

        // GET: FeeTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feeTransaction = await _context.FeeTransactions
                .Include(f => f.Student)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (feeTransaction == null)
            {
                return NotFound();
            }

            return View(feeTransaction);
        }

        // POST: FeeTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feeTransaction = await _context.FeeTransactions.FindAsync(id);
            if (feeTransaction != null)
            {
                _context.FeeTransactions.Remove(feeTransaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeeTransactionExists(int id)
        {
            return _context.FeeTransactions.Any(e => e.TransactionId == id);
        }
    }
}
