using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HomestayWeb.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using HomestayWeb.Contants;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomestayWeb.Pages.HomeStays
{
    [Authorize(Policy = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ProjectHomeStayContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(ProjectHomeStayContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Homestay Homestay { get; set; }

        [Required(ErrorMessage = "Please choose at least one file")]
        [DataType(DataType.Upload)]
        [BindProperty]
        public List<IFormFile> FileUploads { get; set; }

        public List<string> UploadedFiles { get; set; }

        public IActionResult OnGet()
        {
            UploadedFiles = new List<string>();
            ViewData["HomeStayType"] = new SelectList(HomeStayType.Instance);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Homestay == null)
            {
                return Page();
            }

            UploadedFiles = new List<string>();

            _context.Homestays.Add(Homestay);
            await _context.SaveChangesAsync();

            if (FileUploads != null && FileUploads.Count > 0)
            {
                foreach (var fileUpload in FileUploads)
                {
                    if (fileUpload.Length > 0)
                    {
                        var uploadsFolderPath = Path.Combine(_environment.WebRootPath, "Images");
                        if (!Directory.Exists(uploadsFolderPath))
                        {
                            Directory.CreateDirectory(uploadsFolderPath);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;
                        var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileUpload.CopyToAsync(fileStream);
                        }

                        Image image = new Image()
                        {
                            HomstayId = Homestay.HomestayId,
                            Link = Path.Combine("/Images", uniqueFileName)
                        };
                        _context.Images.Add(image);
                        await _context.SaveChangesAsync();

                        UploadedFiles.Add(Path.Combine("/Images", uniqueFileName));
                    }
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
