using System;
using System.Linq;
using ErasmusPlus.Common.Authorization;
using ErasmusPlus.Common.Database;
using ErasmusPlus.Common.SharedModels;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ErasmusPlus.Migrations
{
    using ErasmusPlus.Models.Database;
    using ErasmusPlus.Models.Identity;
    using Microsoft.AspNet.Identity;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ErasmusDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ErasmusDbContext context)
        {
            //Just in case if debug is needed
            //System.Diagnostics.Debugger.Launch();

            //Create initial university, faculty and studies
            var uni = context.Universities.SingleOrDefault(x => x.Name == "VGTU");
            var uniId = 0;
            if (uni == null)
            {
                var university = context.Universities.Add(new University()
                {
                    Address = "Sauletekis",
                    City = "Vilnius",
                    Country = "Lithuania",
                    ErasmusCode = "ABC123",
                    Name = "VGTU"
                });
                context.SaveChanges();
                var faculty = context.Faculties.Add(new Faculty() {Name = "FMF", University = university});
                var faculty2 = context.Faculties.Add(new Faculty() {Name = "Electronics", University = university});
                var faculty3 = context.Faculties.Add(new Faculty() {Name = "Mechanics", University = university});
                context.SaveChanges();
                var studyField = context.FieldsOfStudy.Add(new FieldOfStudy() {Name = "Informatics", Degree = Degree.Bachelor, Faculty = faculty});
                var studyField2 = context.FieldsOfStudy.Add(new FieldOfStudy() {Name = "Electronics engineering", Degree = Degree.Bachelor, Faculty = faculty2});
                var studyField3 = context.FieldsOfStudy.Add(new FieldOfStudy() {Name = "Auto mechanics", Degree = Degree.Bachelor, Faculty = faculty3});
                context.SaveChanges();
                var studySubject = context.StudySubjects.Add(new StudySubjects()
                {
                    Name = "Math",
                    Language = "English",
                    Credits = 10,
                    Evaluation = "Grade",
                    Semester = 1,
                    FieldOfStudy = studyField
                });
                var studySubject2 = context.StudySubjects.Add(new StudySubjects()
                {
                    Name = "Electronics",
                    Language = "English",
                    Credits = 10,
                    Evaluation = "Grade",
                    Semester = 1,
                    FieldOfStudy = studyField2
                });
                var studySubject3 = context.StudySubjects.Add(new StudySubjects()
                {
                    Name = "Mechanics",
                    Language = "English",
                    Credits = 10,
                    Evaluation = "Grade",
                    Semester = 1,
                    FieldOfStudy = studyField3
                });
                context.SaveChanges();
                uniId = university.Id;
            }

            var uni2 = context.Universities.SingleOrDefault(x => x.Name == "VU");
            if (uni2 == null)
            {
                var university2 = context.Universities.Add(new University()
                {
                    Address = "Universiteto g. 3",
                    City = "Vilnius",
                    Country = "Lithuania",
                    ErasmusCode = "ABC321",
                    Name = "VU"
                });
                context.SaveChanges();
                var faculty = context.Faculties.Add(new Faculty() { Name = "Philosophy", University = university2 });
                var faculty2 = context.Faculties.Add(new Faculty() { Name = "History", University = university2 });
                var faculty3 = context.Faculties.Add(new Faculty() { Name = "Business", University = university2 });
                context.SaveChanges();
                var studyField = context.FieldsOfStudy.Add(new FieldOfStudy() { Name = "Philosophy", Degree = Degree.Bachelor, Faculty = faculty });
                var studyField2 = context.FieldsOfStudy.Add(new FieldOfStudy() { Name = "History", Degree = Degree.Bachelor, Faculty = faculty2 });
                var studyField3 = context.FieldsOfStudy.Add(new FieldOfStudy() { Name = "Business", Degree = Degree.Bachelor, Faculty = faculty3 });
                context.SaveChanges();
                var studySubject = context.StudySubjects.Add(new StudySubjects()
                {
                    Name = "Psichology",
                    Language = "English",
                    Credits = 10,
                    Evaluation = "Grade",
                    Semester = 1,
                    FieldOfStudy = studyField
                });
                var studySubject2 = context.StudySubjects.Add(new StudySubjects()
                {
                    Name = "Ancient history",
                    Language = "English",
                    Credits = 10,
                    Evaluation = "Grade",
                    Semester = 1,
                    FieldOfStudy = studyField2
                });
                var studySubject3 = context.StudySubjects.Add(new StudySubjects()
                {
                    Name = "Business management",
                    Language = "English",
                    Credits = 10,
                    Evaluation = "Grade",
                    Semester = 1,
                    FieldOfStudy = studyField3
                });
                context.SaveChanges();
            }

            var uni3 = context.Universities.SingleOrDefault(x => x.Name == "Stanford University");
            if (uni3 == null)
            {
                var university2 = context.Universities.Add(new University()
                {
                    Address = "450 Serra Mall",
                    City = "Stanford",
                    Country = "United States",
                    ErasmusCode = "US_STN_450",
                    Name = "Stanford University"
                });
                context.SaveChanges();
                var faculty = context.Faculties.Add(new Faculty() { Name = "Biology", University = university2 });
                context.SaveChanges();
                var studyField = context.FieldsOfStudy.Add(new FieldOfStudy() { Name = "Biology", Degree = Degree.Bachelor, Faculty = faculty });
                context.SaveChanges();
                var studySubject = context.StudySubjects.Add(new StudySubjects()
                {
                    Name = "Biology",
                    Language = "English",
                    Credits = 10,
                    Evaluation = "Grade",
                    Semester = 1,
                    FieldOfStudy = studyField
                });

                context.SaveChanges();
            }

            var uniA = context.Universities.Single(x => x.Name == "VGTU");
            var uniB = context.Universities.Single(x => x.Name == "VU");
            var uniC = context.Universities.Single(x => x.Name == "Stanford University");
            context.UniversityAgreements.Add(new UniversityAgreements()
            {
                SourceUniversity = uniA,
                TargetUniversity = uniB
            });
            context.UniversityAgreements.Add(new UniversityAgreements()
            {
                SourceUniversity = uniA,
                TargetUniversity = uniC
            });
            context.UniversityAgreements.Add(new UniversityAgreements()
            {
                SourceUniversity = uniB,
                TargetUniversity = uniC
            });

            context.SaveChanges();

            //Setup roles
            var admin = context.Roles.SingleOrDefault(x => x.Name == UserRoles.Administrator) ??
                        context.Roles.Add(new IdentityRole(UserRoles.Administrator));
            var coord = context.Roles.SingleOrDefault(x => x.Name == UserRoles.Coordinator) ??
                        context.Roles.Add(new IdentityRole(UserRoles.Coordinator));
            var foreign = context.Roles.SingleOrDefault(x => x.Name == UserRoles.Foreign) ??
                          context.Roles.Add(new IdentityRole(UserRoles.Foreign));
            var student = context.Roles.SingleOrDefault(x => x.Name == UserRoles.Student) ??
                          context.Roles.Add(new IdentityRole(UserRoles.Student));
            var uniAdmin = context.Roles.SingleOrDefault(x => x.Name == UserRoles.UniversityAdmin) ??
                           context.Roles.Add(new IdentityRole(UserRoles.UniversityAdmin));
            context.SaveChanges();

            //Create initial users
            var store = new UserStore<ErasmusUser>(context);
            var manager = new UserManager<ErasmusUser>(store);
            CreateUpdateUser(manager, "admin@erasmusplus.com", "Admin", UserRoles.Administrator, null);
            CreateUpdateUser(manager, "student@erasmusplus.com", "Student", UserRoles.Student, uni.Id);
            CreateUpdateUser(manager, "coordinator@erasmusplus.com", "Coordinator", UserRoles.Coordinator, uni.Id);
            CreateUpdateUser(manager, "foreign@erasmusplus.com", "Foreign", UserRoles.Foreign, uni.Id);
            CreateUpdateUser(manager, "uniadmin@erasmusplus.com", "UniversityAdmin", UserRoles.UniversityAdmin, uni.Id);
        }

        private static void CreateUpdateUser(UserManager<ErasmusUser> manager, string email, string username, string role, int? uniId = null)
        {
            //Check for existing user
            var user = manager.Users.SingleOrDefault(x => x.Email == email);
            if (user == null)
            {
                //Doesn't exist - create
                user = new ErasmusUser
                {
                    Email = email, UserName = username, EmailConfirmed = true,
                    Birthday = new DateTime(1990, 1, 1),
                    UniversityId = uniId
                };
                manager.Create(user, "Slaptaþodis1");
                manager.AddToRole(user.Id, role);
            }
            else
            {
                //Exists update
                user.UserName = username;
                user.EmailConfirmed = true;
                manager.RemovePassword(user.Id);
                manager.AddPassword(user.Id, "Slaptaþodis1");
                manager.Update(user);
                manager.RemoveFromRoles(user.Id, UserRoles.AllRoles);
                manager.AddToRole(user.Id, role);
            }
        }
    }
}
