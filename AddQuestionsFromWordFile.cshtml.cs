using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data;
using InnovationTest.Models;
using InnovationTest.Data;

namespace InnovationTest.Pages.AddTests.AddQuestionAnswers
{
    public class FileUpload
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
        public string SuccessMessage { get; set; }
    }
    public class AddFromFileModel : PageModel
    {
        private readonly InnovationTestContext _context;

        private readonly ILogger<IndexModel> _logger;
            private string fullPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "UploadImages";

        public int TestNameId { get; private set; }
        public AddFromFileModel(ILogger<IndexModel> logger, InnovationTestContext context)
            {
                _logger = logger;
                _context = context;
            }
            [BindProperty]
            public FileUpload fileUpload { get; set; }
            public IActionResult OnGet(int testNameId)
            {
                if (testNameId == 0)
                    return NotFound();

                    TestNameId = testNameId;
                    ViewData["SuccessMessage"] = "";
            
                return Page();
            }
            public async  Task<IActionResult> OnPostUpload(FileUpload fileUpload, int testNameId)
            {

            if(testNameId==0)
            {
                ModelState.AddModelError("Id", "Id ýalnyňlyk. Programmista ýüzleniň.");
                return Page();
            };


            TestNameId = testNameId;

            string fileExt= Path.GetExtension(fileUpload.FormFile.FileName);
            if (!fileExt.Equals(".docx"))
            ModelState.AddModelError("FileExt", "Word resminamanyň giňelmesi *.docx bolmaly");
            


            if (!ModelState.IsValid)
            {
                
                ViewData["SuccessMessage"] = "Ýalňyşlyk! Täzeden synanşyň. Word resminama saýlaň.";
                
                return Page();
            }

            
            Stream stream = new MemoryStream();
            fileUpload.FormFile.CopyTo(stream);
            using (var doc = WordprocessingDocument.Open(stream, false))
            {
                List<TestQuestion> testQuestions = new List<TestQuestion>();
                // To create a temporary table   
                DataTable dt = new DataTable();
                // int rowCount = 0;

                // Find the first table in the document.   
                Table table;
                try
                {
                     table = doc.MainDocumentPart.Document.Body.Elements<Table>().First();
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("WithoutTable","Word faýlyň içinde tablisa ýok.");
                    return Page();
                }
                // To get all rows from table  
                IEnumerable<TableRow> rows = table.Elements<TableRow>();

                if(rows.Count()<2)
                {
                    ModelState.AddModelError("WithoutTable", "Tablisada setiriň sany birden köp bolmaly.");
                    return Page();
                }

                // To read data from rows and to add records to the temporary table  
                int k = 0;
                foreach (TableRow row in rows)
                {
                    

                    if(k==0)
                    {
                        k++;
                        continue;
                    }

                   

                   
                    List<TableCell> tbCells = row.Elements<TableCell>().ToList<TableCell>();
                    if(tbCells.Count!=6 )
                    {
                        ModelState.AddModelError("Cell error", "Tablisa alty sütünden ybarat bolmaly.");
                        return Page();
                    }

                    
                    if(String.IsNullOrEmpty(tbCells[1].InnerText.Trim()) || String.IsNullOrEmpty(tbCells[2].InnerText.Trim()) || String.IsNullOrEmpty(tbCells[3].InnerText.Trim()) || String.IsNullOrEmpty(tbCells[4].InnerText.Trim()) 
                        || String.IsNullOrEmpty(tbCells[5].InnerText.Trim()))
                     {
                        ModelState.AddModelError("Cell error", "Tablisada öýjük boş bolmaly däl.");
                        return Page();

                    }

                    TestQuestion testQuestion = new TestQuestion();
                    testQuestion.TestNameId = testNameId;
                    testQuestion.Question = tbCells[1].InnerText;
                    testQuestion.Answer1 = tbCells[2].InnerText;
                    testQuestion.Answer2 = tbCells[3].InnerText;
                    testQuestion.Answer3 = tbCells[4].InnerText;
                    testQuestion.Answer4 = tbCells[5].InnerText;
                    testQuestions.Add(testQuestion);

                   
                }
                try
                {
                    _context.TestQuestions.AddRange(testQuestions);
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    ViewData["SuccessMessage"] = "Ýalňyşlyk! Täzeden synanşyň. Word resminamada bir sany tablisa bolmaly we tablisa nusga gabat gelmeli";
                    _logger.LogError(ex, "Error adding test questions to database");
                    return Page();
                }
                return RedirectToPage("./Index", new { testNameId });

               
            }

            
            }
     }
    
}
