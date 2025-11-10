using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentInfoLoginRoles.Models;
using StudentInfoLoginRoles.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace StudentInfoLoginRoles.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _env;

        public StudentsController(ApplicationContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult NotifySelector(int studentId)
        {
            ViewBag.NotifierOptions = new SelectList(new[]
            {
                new { Value = "Email", Text = "Email Notification" },
                new { Value = "SMS", Text = "SMS Notification" }
            }, "Value", "Text");
            ViewBag.StudentId = studentId;
            return View();
        }

        [HttpPost]
        public IActionResult NotifySelector(int studentId, string notifierType)
        {
            IFeeNotifier notifier;
            switch (notifierType)
            {
                case "Email":
                    notifier = new EmailNotifier(_context);
                    break;
                case "SMS":
                    notifier = new SmsNotifier(_context);
                    break;
                default:
                    notifier = new EmailNotifier(_context);
                    break;
            }
            string result = notifier.Notify(studentId);
            ViewBag.AlertMessage = result;
            return View("NotifyResult");
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            return View(await _context.Students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["CourseCode"] = new SelectList(_context.CourseDetails, "CourseCode", "CourseCode");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,FirstName,LastName,CourseCode,Email,Phone,FeePending")]
            Student student, IFormFile? photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null)
                {
                    // 1️. Check if file is empty
                    if (photo.Length == 0)
                    {
                        ModelState.AddModelError("photo", "The file is empty.");
                        return View(student);
                    }

                    // 2️. Check file size (max 50 MB)
                    long maxFileSize = 50 * 1024 * 1024; // 50 MB in bytes
                    if (photo.Length > maxFileSize)
                    {
                        ModelState.AddModelError("photo", "File size exceeds 50 MB limit.");
                        return View(student);
                    }

                    // 3️. Check file type (must be an image)
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("photo", "Invalid file type. Only image files are allowed.");
                        return View(student);
                    }
                    // 4. Check MIME type (content type)
                    var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedMimeTypes.Contains(photo.ContentType))
                    {
                        ModelState.AddModelError("photo", "Invalid file content. Please upload a valid image file.");
                        return View(student);
                    }

                    // 5. Prevent duplicate filenames
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "studentphotos");
                    Directory.CreateDirectory(uploadsFolder);

                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(photo.FileName);
                    string safeFileName = fileNameWithoutExt + fileExtension;
                    string filePath = Path.Combine(uploadsFolder, safeFileName);
                    int counter = 1;

                    while (System.IO.File.Exists(filePath))
                    {
                        safeFileName = $"{fileNameWithoutExt}_{counter}{fileExtension}";
                        filePath = Path.Combine(uploadsFolder, safeFileName);
                        counter++;
                    }

                    // Save file to wwwroot/studentphotos
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    // Save relative path to DB
                    student.PhotoPath = "/studentphotos/" + safeFileName;
                }
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseCode"] = new SelectList(_context.CourseDetails, "CourseCode", "CourseCode");
            return View(student);
        }


        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["CourseCode"] = new SelectList(_context.CourseDetails, "CourseCode", "CourseCode");
            
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,FirstName,LastName,CourseCode,Email,Phone,FeePending")] 
        Student student, IFormFile photo)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            //var existingStudent = await _context.Students.FindAsync(id);
            //if (existingStudent == null)
            //    return NotFound();

            if (ModelState.IsValid)
            {
                // Update regular bound fields
                //existingStudent.FirstName = student.FirstName;
                //existingStudent.LastName = student.LastName;
                //existingStudent.CourseCode = student.CourseCode;
                //existingStudent.Email = student.Email;
                //existingStudent.Phone = student.Phone;
                //existingStudent.FeePending = student.FeePending;

                if (photo != null)
                {
                    // 1️. Check if file is empty
                    if (photo.Length == 0)
                    {
                        ModelState.AddModelError("photo", "The file is empty.");
                        return View(student);
                    }

                    // 2️. Check file size (max 50 MB)
                    long maxFileSize = 50 * 1024 * 1024; // 50 MB in bytes
                    if (photo.Length > maxFileSize)
                    {
                        ModelState.AddModelError("photo", "File size exceeds 50 MB limit.");
                        return View(student);
                    }

                    // 3️. Check file type (must be an image)
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("photo", "Invalid file type. Only image files are allowed.");
                        return View(student);
                    }
                    // 4. Check MIME type (content type)
                    var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedMimeTypes.Contains(photo.ContentType))
                    {
                        ModelState.AddModelError("photo", "Invalid file content. Please upload a valid image file.");
                        return View(student);
                    }

                    // 5. Prevent duplicate filenames
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "studentphotos");
                    Directory.CreateDirectory(uploadsFolder);

                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(photo.FileName);
                    string safeFileName = fileNameWithoutExt + fileExtension;
                    string filePath = Path.Combine(uploadsFolder, safeFileName);
                    int counter = 1;

                    while (System.IO.File.Exists(filePath))
                    {
                        safeFileName = $"{fileNameWithoutExt}_{counter}{fileExtension}";
                        filePath = Path.Combine(uploadsFolder, safeFileName);
                        counter++;
                    }

                    // Save file to wwwroot/studentphotos
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    // Save relative path to DB
                    student.PhotoPath = "/studentphotos/" + safeFileName;
                }

                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
