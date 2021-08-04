using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

using InnovationTest.Data;
using InnovationTest.Models;
using InnovationTest.Pages.Testing.Moduls;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;




namespace InnovationTest.Pages.Reports
{
    public class UniverSubjectModel : PageModel
    {
        private readonly InnovationTestContext testContext;
        public dynamic TotalReport { get; private set; }

        [BindProperty(SupportsGet = true)]
        public int UniversityId { get; set; }
        public SelectList UniversityList { get; set; }



        [BindProperty(SupportsGet = true)]
        public int TestNameId { get; set; }

        public SelectList TestNamesList { get; set; }



        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now.Date;


        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now.Date.AddDays(1);


        public string ScoreSort { get; set; }
        public string UniversitySort { get; set; }

        public string TestNameSort { get; set; }


        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public double avgScore { get; private set; }
        public double avgMark { get; private set; }
        public double avgTestPeriod { get; private set; }
        public UniverSubjectModel(InnovationTestContext _testContext)
        {
            testContext = _testContext;
        }
        public async Task<IActionResult> OnGetAsync(string _sortOrder, int _testNameId, int _universityId, string _currentFilter, string _startDate, string _endDate)
        {



            IQueryable<University> univers;
            if (UniversityId > 0)
            {
                univers = from univer in testContext.University
                          where univer.Id == UniversityId
                          select univer;
            }
            else
            {
                univers = from univer in testContext.University
                          select univer;
            }




            IQueryable<AnswerDetailRes> answerDetails = from anDet in testContext.AnswerDetail
                                                        group anDet by anDet.TestResultId into gr
                                                        select new AnswerDetailRes
                                                        {
                                                            id = (int)(gr.Key == null ? default(int) : gr.Key),
                                                            totalAnswer = gr.Count(),
                                                            rightAnswer = gr.Count(ct => ct.ChosenAnswerNumber == 1)
                                                        };









            IQueryable<TestName> testNames;
            if (TestNameId > 0)
            {
                testNames = from testName in testContext.TestName
                            where testName.Id == TestNameId
                            select testName;
            }
            else
            {
                testNames = from testName in testContext.TestName
                            select testName;
            }



            var TotalReports = await (from univer in univers
                                      from student in testContext.Student
                                      from testResult in testContext.TestResults
                                      where testResult.StartTime.CompareTo(StartDate) > 0 && testResult.StartTime.CompareTo(EndDate) < 0
                                      from testName in testNames
                                      from answerDetail in answerDetails
                                      orderby answerDetail.rightAnswer descending
                                      where
                                     univer.Id == student.UniversityId
                                     && student.Id == testResult.StudentId
                                     && testResult.TestNameId == testName.Id
                                     && testResult.Id == answerDetail.id

                                      select new TotalReport
                                      {
                                          univerId = univer.Id,
                                          univerName = univer.Name,
                                          studId = student.Id,
                                          studName = student.Name,
                                          studLastName = student.LastName,
                                          studPatronymic = student.Patronymic,
                                          testNameId = testName.Id,
                                          testName = testName.Name,
                                          testResultId = testResult.Id,
                                          testStartDate = testResult.StartTime,
                                          testPeriod = testResult.TestPeriod,
                                          questionCount = testResult.QuestionCount,
                                          totalAnswer = answerDetail.totalAnswer,
                                          rightAnswer = answerDetail.rightAnswer,
                                          mark = Mark.GetMak(testResult.QuestionCount, answerDetail.rightAnswer)
                                      })
                               .ToListAsync();



            var xx = from totRep in TotalReports
                     group totRep by totRep.univerName into gr1
                     orderby gr1.Key
                     from gr2 in
                     (from stud in gr1
                      group stud by stud.testName into gr3
                      orderby gr3.Key
                      select gr3
                      )

                     group gr2 by gr1.Key;

            TotalReport = xx
               .ToList();






            UniversityList = new SelectList(await testContext.University.AsNoTracking().ToListAsync(), "Id", "Name");
            TestNamesList = new SelectList(await testContext.TestName.AsNoTracking().ToListAsync(), "Id", "Name");




            return Page();

        }
    }
}
