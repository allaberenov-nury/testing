using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InnovationTest.Extensions;
using InnovationTest.Models;
using InnovationTest.RandomFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InnovationTest.Pages.Testing.Moduls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InnovationTest.Pages.Testing
{
  //  [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    
    public class TestProcessModel : PageModel
    {
        public string StartTime { get; private set; }
        public string CurrentTime { get; private set; }
        public int TestPeriod { get; private set; }
        public string ShowTimeInfo { get; private set; }
        public string ShowMark { get; private set; }
        public bool ShowReport { get; private set; } = false;
        public string ReportInfor { get; private set; }
        [ViewData]
        public string InformQuestion { get; set; }
        public string ShowQuestion { get; private set; }
        public int ShowQuestionOrderNumber { get; private set; }
        public int ShowQuestionCount { get; private set; }
        public List<string> ShowAnswersList { get; private set; }
        private readonly InnovationTest.Data.InnovationTestContext _context;
        ILogger<TestProcessModel> logger;


        // For report
        public string RepUniversityName { get; private set; }
        public string RepStudentFAA { get; private set; }
        public string RepTestname { get; private set; }

        public int RepStudentId { get; private set; }
        public int RepTestResultId { get; private set; }
        


        public TestProcessModel(InnovationTest.Data.InnovationTestContext context, ILogger<TestProcessModel>  _logger)
        {
            logger = _logger;
            _context = context;
        }
       
        public async Task<IActionResult> OnPostAsync(int _questionCount = -1, int _testingInitial=0,int _questionOrderNumber = 0, int _checkedAnswer = -1, int _testNameId = 0, int _scores = -1,  bool _timeEnd=false, int _testPeriod=0 )

        {
            ModelState.Clear();

            logger.LogInformation(2, "beg _questionOrderNumber={_questionOrderNumber}", _questionOrderNumber);

            int scores = 0;
           
            if (_testingInitial==1)
            {
               
                SessionClass.SetTestStartTime(HttpContext, DateTime.Now);
               // bool rea=SessionClass.SetTestingInitial(HttpContext, 1);

                bool resTestNameID = SessionClass.SetTestNameId(HttpContext, _testNameId);

                int studentId = SessionClass.GetStudentId(HttpContext);
                logger.LogInformation(10, "Res table _testNameId={_testNameId}", _testNameId);
                logger.LogInformation(10, "Res table _questionCount={_questionCount}", _questionCount);
                logger.LogInformation(10, "Res table _questionCount={_questionCount}", _questionCount);
                logger.LogInformation(10, "Res table studentId={studentId}", studentId);


                if (studentId < 1)
                    RedirectToPage("../Students/Edit");

                
                int testResultTabelId = await TableClasses.AddRowsToTestResultTable(_testNameId, _questionCount, studentId, _context);
                if (testResultTabelId == 0)
                    throw new Exception("Error adding TestResult");
                bool sesTestResult = SessionClass.SetTestResultTableId(HttpContext, testResultTabelId);


                bool randomQues = RandomQuestion.GenerateRandomQuestionNumbersList(HttpContext, _questionCount);
                SessionClass.SetScores(HttpContext, 0);
                
                
                SessionClass.SetTestPeriod(HttpContext, _testPeriod);
                SessionClass.SetTestIsFinished(HttpContext, 0);
            }







            scores = SessionClass.GetScores(HttpContext);

            if (_checkedAnswer > -1)
            {
                logger.LogInformation(1, "checked={chek}", _checkedAnswer);
                int rightAnswer = SessionClass.GetRightAnswerNumber(HttpContext);

                SessionClass.GetRanomAnswersNumbersListFromSession(HttpContext);

               

                List<int> randomAnswersNumberListFromSession = SessionClass.GetRanomAnswersNumbersListFromSession(HttpContext);
                int chosenAnswerNumber=RandomAnswersList.GetChosenAnswerNumber(randomAnswersNumberListFromSession, _checkedAnswer);
                logger.LogInformation(1, "chosenAnswerNummber={chosenAnswerNumber}", chosenAnswerNumber);
              

                

                int questionId = SessionClass.GetTestQuestionId(HttpContext);
                int testResultId = SessionClass.GetTestResultTabelId(HttpContext);

                bool ansResTab = await TableClasses.AddRowsToAnswerDetailTable(questionId, chosenAnswerNumber, testResultId, _context);
                if (ansResTab == false)
                    throw new Exception("Error add to table Answer detail");
                
                
                if (chosenAnswerNumber==1)
                    SessionClass.SetScores(HttpContext, ++scores);

               


            }


            if(_timeEnd == true && _checkedAnswer<0)
            {
                --_questionOrderNumber;
                logger.LogInformation(2, "end _questionOrderNumber={_questionOrderNumber}", _questionOrderNumber);
            }


            /////////////////////// The end of test ///////////////

            TestPeriod = SessionClass.GetTestPeriod(HttpContext);

            logger.LogInformation(2, "TestPeriod={TestPeriod}", TestPeriod);
         
            



            if ((_questionOrderNumber >= _questionCount) || (_timeEnd==true))
            {
                logger.LogInformation(1, "_timeEnd={_timeEnd}", _timeEnd);
                DateTime testEndTime = DateTime.Now;
                
                  DateTime testStartTime = SessionClass.GetTestStartTime(HttpContext);

              

                int questionId = SessionClass.GetTestQuestionId(HttpContext);
                int testResultTableId = SessionClass.GetTestResultTabelId(HttpContext);
                double totalMinutes = DateTime.Now.Subtract(testStartTime).TotalMinutes;
                logger.LogInformation(2, "totalMinutes={totalMinutes}", totalMinutes);


                double testPeriod = 0;

                testPeriod = Math.Round(totalMinutes, 2, MidpointRounding.AwayFromZero);
                
                logger.LogInformation(2, "testPeriod={testPeriod}", testPeriod);

                if (testPeriod > (double) TestPeriod)
                    testPeriod = (double) TestPeriod;
                
                
                    

                

                bool updateRowsTestResultTable = await TableClasses.UpdateRowsTestResultTable(testResultTableId, testStartTime, testPeriod, _context);
                        if (updateRowsTestResultTable == false)
                            throw new Exception("Error add to table Result table");


                   
               // scores = SessionClass.GetScores(HttpContext);
                logger.LogInformation(2, "_questionCount={_questionCount}", _questionCount);
                logger.LogInformation(2, "_questionOrderNumber={_questionOrderNumber}", _questionOrderNumber);
                logger.LogInformation(2, "scores={scores}", scores);
                ReportInfor = InformPanel.TestAnswerResultProcess(_questionCount, _questionOrderNumber, scores);



                //////  Report Student info  //////////////// 
                int studentId = SessionClass.GetStudentId(HttpContext);

                Student stud=await _context.Student
                    .AsNoTracking()
                    .Include(st=>st.University)
                    .SingleOrDefaultAsync(st => st.Id == studentId);
                
                RepStudentFAA = stud.FullName;
                RepTestResultId = testResultTableId;
                RepUniversityName = stud.University.Name;
                RepStudentId = studentId;


                TestResult testResult = await _context.TestResults
                    .AsNoTracking()
                    .Include(tr => tr.TestName)
                    .SingleOrDefaultAsync(tr => tr.Id == testResultTableId);


                RepTestname = testResult.TestName.Name;






                ////////////////////
                ShowMark = "Siziň bahaňyz: "+Mark.GetMak(_questionCount, scores).ToString();
                ShowTimeInfo = "Synagyň başlanan wagty: " + testStartTime.ToString() + ". Synagyň dowamlylygy: "+ testPeriod + " minut.";
                ShowReport = true;
            }
            else
            {


               

                
                // continue testing

                logger.LogInformation(2, "else _questionOrderNumber={_questionOrderNumber}", _questionOrderNumber);

                

                List<int> randomQuestionList = RandomQuestion.GetRandomQuestionNumbersList(HttpContext);
                

                int randomQuestionNumber = randomQuestionList[_questionOrderNumber];
                int testNameId = SessionClass.GetTestNameId(HttpContext);


                TestQuestion testQuestion = await _context.TestQuestions.AsNoTracking().Where(qs => qs.TestNameId == testNameId).OrderBy(or => or.Id).Skip(randomQuestionNumber).Take(1).SingleOrDefaultAsync();

                SessionClass.SetTestQuestionId(HttpContext, testQuestion.Id);

                ShowQuestion = testQuestion.Question;

                List<string> answersList = RandomAnswersList.GetAnsewrsList(testQuestion);

                List<int> answerRandomNumberList = RandomClass.GetRandomNumbersList(0, 4);

                SessionClass.SetRanomAnswersNumbersListToSession(HttpContext, answerRandomNumberList);
                //int correctAnswer = answerRandomNumberList[0];


                //int correctAnswer;
                ShowAnswersList = RandomAnswersList.GetRandomAnsewrsList(answersList, answerRandomNumberList);
               // bool resCor = SessionClass.SetRightAnswerNumber(HttpContext, correctAnswer);

                //int scores = SessionClass.GetScores(HttpContext);
                InformQuestion = InformPanel.TestAnswerResultProcess(_questionCount, _questionOrderNumber, scores);
                ShowQuestionCount = _questionCount;
                ShowQuestionOrderNumber = ++_questionOrderNumber;
                

                /////////////
                StartTime = SessionClass.GetTestStartTime(HttpContext).ToString("yyyy-MM-ddTHH:mm:ss");
                CurrentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
               
               
               
                ////////////////

            }

            
            
            return Page();
        }
       
    }
}
