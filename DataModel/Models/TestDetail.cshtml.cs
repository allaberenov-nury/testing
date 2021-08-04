using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InnovationTest.Data;
using InnovationTest.Pages.Testing.Moduls;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InnovationTest.Pages.Reports
{
    public class AnswerDetailView
    {
        public int QuestionId { get;  set; }
        public int ChosenAnswerNumber { get;  set; }
        public string Result { get; set; }
       
    }
    public class TestDetailModel : PageModel
    {
        public string University { get; private set; }
        public string StudentId { get; private set; }
        public string StudentName { get; private set; }
        public string Testname { get; private set; }
        public string TestResult { get; private set; }
        public string TestResultId { get; private set; }
        public string TestMark { get; private set; }
        public List<AnswerDetailView> AnswerDetails { get; private set; }
        InnovationTestContext innovationTestContext;
        public TestDetailModel(InnovationTestContext _innovationTestContext)
        {
            innovationTestContext = _innovationTestContext;
        }
        public async Task<IActionResult> OnGet(int _univerId, int _studId, int _testResultId, int _testNameId)
        {

            University =(await innovationTestContext
                .University
                .AsNoTracking()
                .SingleOrDefaultAsync(un => un.Id == _univerId)).Name;
            University = "Okuw jaýy: " + University;
            var st = await innovationTestContext
                .Student
                .AsNoTracking()
                .SingleOrDefaultAsync(st => st.Id == _studId);
            StudentId = "Talyp Id: "+st.Id;
            StudentName ="Talyp: "+ st.FullName;

            var testName = await innovationTestContext
                .TestName
                .AsNoTracking()
                .SingleOrDefaultAsync(tn => tn.Id == _testNameId);

            var testResult = await innovationTestContext
                .TestResults
                .AsNoTracking()
                .SingleOrDefaultAsync(tr => tr.Id == _testResultId);


            Testname = "Synagyň ady: " + testName.Name + ". Sorag sany: " + testResult.QuestionCount;

            TestResultId ="Synag Id: " +testResult.Id;
            TestResult="Başlanan wagty: " + testResult.StartTime + ". Synagyň dowamlylygy: " + testResult.TestPeriod+" minut.";

            //IQueryable<AnswerDetailRes>
             AnswerDetails = await (from anDet in innovationTestContext.AnswerDetail
                                 where anDet.TestResultId == _testResultId
                                 select new AnswerDetailView()
                                 {
                                     QuestionId = anDet.QuestionId,
                                     ChosenAnswerNumber = anDet.ChosenAnswerNumber,
                                     Result = (anDet.ChosenAnswerNumber == 1)?"Dogry":"Ýalňyş"
                                 }).ToListAsync();


            int answerdCount = AnswerDetails.Count();
            int rightAnswerdCount = AnswerDetails.Count(ans=>ans.Result.Equals("Dogry"));
            int mark = Mark.GetMak(testResult.QuestionCount, rightAnswerdCount);

            TestMark = "Jogap berlen sorag: " + answerdCount + ". Dogry jogap: " + rightAnswerdCount + ". Baha: " + mark;

            return Page();
        }
    }
}
