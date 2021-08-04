using InnovationTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnovationTest.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new TestIdentityContext(
                serviceProvider.GetRequiredService<DbContextOptions<TestIdentityContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, "1qaz!QAZ", "admin@admin.com");
                await EnsureRole(serviceProvider, adminID, "testAdmin");


                // add students
                
                var innovationTestContext = new InnovationTestContext(serviceProvider.GetRequiredService<DbContextOptions<InnovationTestContext>>());

                if (innovationTestContext.University.Any() == false) { 
                    CreateUnivers(innovationTestContext);

               
                
                    string userId;
                    int universityId;
                        userId = await EnsureUser(serviceProvider, "1qaz!QAZ", "student1@innov.com");
                    await EnsureRole(serviceProvider, userId, "testUser");


                    universityId = innovationTestContext.University.SingleOrDefault(un => un.Name == "Transport").Id;
                    SeedStudentTb(innovationTestContext, userId, universityId, "Meret", "Meredow", 1);

                    ////////////////////////////////////
                    ///
                    userId = await EnsureUser(serviceProvider, "1qaz!QAZ", "student2@innov.com");
                    await EnsureRole(serviceProvider, userId, "testUser");

                    universityId = innovationTestContext.University.SingleOrDefault(un => un.Name == "Transport").Id;
                    SeedStudentTb(innovationTestContext, userId, universityId, "Sapar", "Saparow", 2);

                    //////////////////////////////////

                    userId = await EnsureUser(serviceProvider, "1qaz!QAZ", "student3@innov.com");
                    await EnsureRole(serviceProvider, userId, "testUser");

                    universityId = innovationTestContext.University.SingleOrDefault(un => un.Name == "Diller").Id;
                    SeedStudentTb(innovationTestContext, userId, universityId, "Kerim", "Kerimow", 2, patronymic:"Kerimowiç");

                    //////////////////////////////////////////
                    
                    userId = await EnsureUser(serviceProvider, "1qaz!QAZ", "student4@innov.com");
                    await EnsureRole(serviceProvider, userId, "testUser");

                    universityId = innovationTestContext.University.SingleOrDefault(un => un.Name == "Med").Id;
                    SeedStudentTb(innovationTestContext, userId, universityId, "Jemal", "Myradowa", 2,phoneNumber:"34-56-67");


                    //////////////////////////////

                    // add teachers
                    
                     userId = await EnsureUser(serviceProvider, "1qaz!QAZ", "teach1@innov.com");
                    await EnsureRole(serviceProvider, userId, "testTeacher");
                    universityId = innovationTestContext.University.SingleOrDefault(un => un.Name == "Diller").Id;
                    SeedTeacherTb(innovationTestContext, userId, "mug", universityId, "Berdi", "Berdiyew",  patronymic: "Kerimowiç");



                    userId = await EnsureUser(serviceProvider, "1qaz!QAZ", "teach2@innov.com");
                    await EnsureRole(serviceProvider, userId, "testTeacher");
                    universityId = innovationTestContext.University.SingleOrDefault(un => un.Name == "Transport").Id;
                    SeedTeacherTb(innovationTestContext, userId, "mug", universityId, "Kemal", "Kemalow" );


                    userId = await EnsureUser(serviceProvider, "1qaz!QAZ", "teach3@innov.com");
                    await EnsureRole(serviceProvider, userId, "testTeacher");
                    universityId = innovationTestContext.University.SingleOrDefault(un => un.Name == "Med").Id;
                    SeedTeacherTb(innovationTestContext, userId,"mug", universityId, "Maral", "Komekowa", patronymic: "Kerimowna",phoneNumber:"34-89-78");

                    // Seed TestNameTb

                    int teacherId;
                    teacherId = innovationTestContext.Teachers.Skip(0).Take(1).SingleOrDefault().Id;
                    SeedTestNameTb(innovationTestContext, "Kompyuter", 2, teacherId);
                    
                    teacherId = innovationTestContext.Teachers.Skip(1).Take(1).SingleOrDefault().Id;
                    SeedTestNameTb(innovationTestContext, "Himiya", 3, teacherId);

                    teacherId = innovationTestContext.Teachers.Skip(2).Take(1).SingleOrDefault().Id;
                    SeedTestNameTb(innovationTestContext, "Matematika", 3, teacherId);



                    /// Seed TestQuestionTb


                    int testNameId;
                    testNameId = innovationTestContext.TestName.Skip(0).Take(1).SingleOrDefault().Id;
                    SeedTestQuestionTb(innovationTestContext, testNameId, "Kompyuter1", "Kompyuter1 Jogap1", "Kompyuter1 Jogap2", "Kompyuter1 Jogap3", "Kompyuter1 Jogap4");
                    SeedTestQuestionTb(innovationTestContext, testNameId, "Kompyuter2", "Kompyuter2 Jogap1", "Kompyuter2 Jogap2", "Kompyuter2 Jogap3", "Kompyuter2 Jogap4");
                    SeedTestQuestionTb(innovationTestContext, testNameId, "Kompyuter3", "Kompyuter3 Jogap1", "Kompyuter3 Jogap2", "Kompyuter3 Jogap3", "Kompyuter3 Jogap4");
                    SeedTestQuestionTb(innovationTestContext, testNameId, "Kompyuter4", "Kompyuter4 Jogap1", "Kompyuter4 Jogap2", "Kompyuter4 Jogap3", "Kompyuter4 Jogap4");
                    SeedTestQuestionTb(innovationTestContext, testNameId, "Kompyuter5", "Kompyuter5 Jogap1", "Kompyuter5 Jogap2", "Kompyuter5 Jogap3", "Kompyuter5 Jogap4");


                    testNameId = innovationTestContext.TestName.Skip(1).Take(1).SingleOrDefault().Id;
                    SeedTestQuestionTb(innovationTestContext, testNameId, "Himiya1", "Himiya1 Jogap1", "Himiya1 Jogap2", "Himiya1 Jogap3", "Himiya1 Jogap4");
                    SeedTestQuestionTb(innovationTestContext, testNameId, "Himiya2", "Himiya2 Jogap1", "Himiya2 Jogap2", "Himiya2 Jogap3", "Himiya2 Jogap4");
                    SeedTestQuestionTb(innovationTestContext, testNameId, "Himiya3", "Himiya3 Jogap1", "Himiya3 Jogap2", "Himiya3 Jogap3", "Himiya3 Jogap4");
                    SeedTestQuestionTb(innovationTestContext, testNameId, "Himiya4", "Himiya4 Jogap1", "Himiya4 Jogap2", "Himiya4 Jogap3", "Himiya4 Jogap4");
                    SeedTestQuestionTb(innovationTestContext, testNameId, "Himiya5", "Himiya5 Jogap1", "Himiya5 Jogap2", "Himiya5 Jogap3", "Himiya5 Jogap4");
                    SeedTestQuestionTb(innovationTestContext, testNameId, "Himiya6", "Himiya6 Jogap1", "Himiya6 Jogap2", "Himiya6 Jogap3", "Himiya6 Jogap4");



                }


                //var innovationTestContext=new InnovationTestContext(serviceProvider.GetRequiredService<DbContextOptions<InnovationTestContext>>());
                // SeedTeacherTb(innovationTestContext, teacherID);

                // context.SaveChanges();
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                   // EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }
            

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }


        public static void SeedStudentTb(InnovationTestContext context, string studentId, int universityId, string name, string lastname,  int studyYear, string patronymic = "" , string phoneNumber= "")
        {
           
          
            var st1 = new Student
            {
                Name = name,
                LastName = lastname,
                Patronymic = patronymic,
               OwnerID = studentId,
               UniversityId=universityId,
               StudyYear=studyYear,
               PhoneNumber=phoneNumber,
                ModifiedTime=DateTime.Now
                
            };

            context.Student.Add(st1);
            context.SaveChanges();
        }
        public static void SeedTeacherTb(InnovationTestContext context, string studentId, string post, int universityId, string name, string lastname, string patronymic = "", string phoneNumber = "")
        {


            var teach1 = new Teacher
            {
                Post = post,
                Name = name,
                LastName = lastname,
                Patronymic = patronymic,
                OwnerID = studentId,
                UniversityId = universityId,
                PhoneNumber = phoneNumber,
                ModifiedTime = DateTime.Now
            };

            context.Teachers.Add(teach1);
            context.SaveChanges();
        }
        public static void CreateUnivers(InnovationTestContext context)
        {


           
            context.University.AddRange(
             new University
             {
                 Name = "Transport",
             },
           new University
           {
               Name = "Diller",
           },

             new University
             {
                 Name = "Med",
             }


            );

            context.SaveChanges();
        }
        public static void SeedTestNameTb(InnovationTestContext context, string name, int period, int teacherId)
        {
            var testName = new TestName
            {
                Name = name,
                Period = period,
                TeacherId = teacherId
            };


            context.TestName.Add(testName);
            context.SaveChanges();
        }
        public static void SeedTestQuestionTb(InnovationTestContext context, int testNameId, string question, string answer1, string answer2, string answer3, string answer4)
        {
            var testQuestion = new TestQuestion
            {
                Question=question,
                Answer1=answer1,
                Answer2=answer2,
                Answer3=answer3,
                Answer4=answer4,
                TestNameId=testNameId
            };


            context.TestQuestions.Add(testQuestion);
            context.SaveChanges();
        }
    }
   

}

