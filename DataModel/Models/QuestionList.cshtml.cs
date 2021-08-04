using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InnovationTest.Data;
using InnovationTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InnovationTest.Pages.Reports
{
    public class QuestionListModel : PageModel
    {
        InnovationTestContext _testContext;
        public TestName TestName { get; set; }
        public QuestionListModel(InnovationTestContext testContext)
        {
            _testContext = testContext;

        }
        public async Task<IActionResult> OnGetAsync(int testNameId)
        {

            TestName =await _testContext.TestName
                .AsNoTracking()
                .Include(tn => tn.Teacher)
                .ThenInclude(tc => tc.University)
                .Include(tn => tn.TestQuestions)
                .SingleOrDefaultAsync(tn => tn.Id == testNameId);

            return Page();


        }
    }
}
