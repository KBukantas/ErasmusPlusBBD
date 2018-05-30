using ErasmusPlus.Common.Database;
using ErasmusPlus.Models.Identity;
using ErasmusPlus.Models.ViewModels.Admin;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Data.Entity;
using ErasmusPlus.Common.SharedModels;
using ErasmusPlus.Models.Database;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using ErasmusPlus.Models.ViewModels;
using System;
using ErasmusPlus.Models.Extensions;
using WebGrease.Css.Extensions;
using FieldOfStudy = ErasmusPlus.Common.Database.FieldOfStudy;

namespace ErasmusPlus.Models.BLL
{
    public class UniAdminBusinessLogic
    {

        public List<KeyValueIntStr> GetTargetAgreementUniversities(int sourceUniversityId)
        {
            using (var db = new ErasmusDbContext())
            {
                var list = new List<KeyValueIntStr>();
                var universities = db.Universities.ToList();
                var sourceUni = universities.SingleOrDefault(x => x.Id == sourceUniversityId);
                if (sourceUni == null)
                {
                    return list;
                }
                else
                {
                    universities.Remove(sourceUni);
                    list.AddRange(universities.Select(x => new KeyValueIntStr()
                    {
                        Key = x.Id,
                        Value = x.Name
                    }).ToList());
                }
                return list;
            }
        }

        public UniversityAgreementsViewModel GetUniversityAgreements(string userId)
        {
            var model = new UniversityAgreementsViewModel();
            using (var db = new ErasmusDbContext())
            {
                var user = db.Users.SingleOrDefault(x => x.Id == userId);
                model.UniversityAgreements.AddRange(db.UniversityAgreements.Where(x => x.SourceUniversityId == user.UniversityId).ToList().Select(x =>
                    new UniversityAgreementsView()
                    {
                        Id = x.Id,
                        SourceUniversity = x.SourceUniversity.Name,
                        TargetUniversity = x.TargetUniversity.Name
                    }));
            }
            return model;
        }

        public UniversityAgreementViewModel GetUniversityAgreementViewModel(string userId)
        {
            var model = new UniversityAgreementViewModel();
            using (var db = new ErasmusDbContext())
            {
                var user = db.Users.SingleOrDefault(x => x.Id == userId);
                model.SourceUniversityName = user.University.Name;
                model.TargetUniversityList = new SelectList(db.Universities.Where(x => x.Id != user.UniversityId).Select(x => new KeyValueIntStr()
                {
                    Key = x.Id,
                    Value = x.Name
                }).ToList(), "Key", "Value");
            }
            return model;
        }

        public void SaveNewUniversityAgreement(int sourceId, int targetId)
        {
            var ids = new List<int>
            {
                sourceId,
                targetId
            };

            using (var db = new ErasmusDbContext())
            {
                var universities = db.Universities.Where(x => ids.Contains(x.Id)).ToList();
                if (universities.Count != 2)
                {
                    throw new ValidationException("Invalid university found.");
                }

                db.UniversityAgreements.Add(new UniversityAgreements()
                {
                    SourceUniversity = universities.SingleOrDefault(x => x.Id == sourceId),
                    TargetUniversity = universities.SingleOrDefault(x => x.Id == targetId)
                });
                db.SaveChanges();
            }

        }

        public void SaveNewUniversityAgreement(string userId, int targetId)
        {
            using (var db = new ErasmusDbContext())
            {
                var user = db.Users.SingleOrDefault(x => x.Id == userId);
                var targetUni = db.Universities.SingleOrDefault(x => x.Id == targetId);
                if (user.University != null && targetUni != null)
                {
                    db.UniversityAgreements.Add(new UniversityAgreements()
                    {
                        SourceUniversityId = user.UniversityId.Value,
                        TargetUniversityId = targetUni.Id
                    });
                    db.SaveChanges();
                }
            }

        }

        public UniversitiesViewModel GetUniversitiesViewModel()
        {
            var model = new UniversitiesViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.Universities = db.Universities.ToList().Select(x => new UniversityView()
                {
                    Name = x.Name,
                    Id = x.Id,
                    Address = x.Address,
                    Country = x.Country,
                    ErasmusCode = x.ErasmusCode,
                    City = x.City
                }).ToList();
            }

            return model;
        }

        public NewUniversityViewModel GetNewUniversityViewModel()
        {
            var model = new NewUniversityViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.Faculties = new SelectList(db.Faculties.Where(x => x.UniversityId == null).ToList().Select(x =>
                    new KeyValueIntStr()
                    {
                        Key = x.Id,
                        Value = x.Name
                    }), "Key", "Value");
            }
            return model;
        }

        public NewStudySubjectViewModel GetNewStudySubjectViewModel()
        {
            var model = new NewStudySubjectViewModel();
            return model;
        }

        internal object GetUserPermissionsModel(string userId)
        {
            var model = new List<UserListViewModel>();
            using (var db = new ErasmusDbContext())
            {
                var store = new UserStore<ErasmusUser>(db);
                var manager = new UserManager<ErasmusUser>(store);

                var currentUniAdmin = manager.Users.SingleOrDefault(x => x.Id == userId);
                if (currentUniAdmin == null)
                {
                    throw new FormValidationException("Invalid user");
                }

                var thisUserUniversityIds = db.UserPermissions.Where(x => x.UserId == userId).Select(x => x.UniversityId).ToList();

                var users = db.UserPermissions.Where(x => thisUserUniversityIds.Contains(x.UniversityId))
                    .Select(x => x.User).Where(x => x.Id != userId).ToList();
                if (users.Any())
                {
                    model.AddRange(users.Select(x => new UserListViewModel()
                    {
                        Email = x.Email,
                        UserId = x.Id,
                        Role = manager.GetRoles(x.Id).FirstOrDefault()
                    }));
                }
            }
            return model;
        }

        public NewUserViewModel GetUserViewModelById(string userId, string roleName)
        {
            using (var db = new ErasmusDbContext())
            {
                var store = new UserStore<ErasmusUser>(db);
                var manager = new UserManager<ErasmusUser>(store);
                var user = manager.FindById(userId);
                if (user == null)
                {
                    return null;
                }

                if (!manager.IsInRole(userId, roleName))
                {
                    return null;
                }

                return new NewUserViewModel()
                {
                    Email = user.Email,
                    LastName = user.LastName,
                    Username = user.UserName,
                    Birthday = user.Birthday,
                    FirstName = user.FirstName,
                    PersonalCode = user.PersonalIdCode,
                    Telephone = user.PhoneNumber,
                    StudentId = user.StudentId,
                    UniversityId = user.UniversityId,
                    UserId = user.Id
                };
            }
        }
        public UserViewModel NewGetUserViewModelById(string userId, string roleName)
        {
            using (var db = new ErasmusDbContext())
            {
                var store = new UserStore<ErasmusUser>(db);
                var manager = new UserManager<ErasmusUser>(store);
                var user = manager.FindById(userId);
                if (user == null)
                {
                    return null;
                }

                if (!manager.IsInRole(userId, roleName))
                {
                    return null;
                }

                return new UserViewModel()
                {
                    Email = user.Email,
                    LastName = user.LastName,
                    Username = user.UserName,
                    Birthday = user.Birthday,
                    FirstName = user.FirstName,
                    PersonalCode = user.PersonalIdCode,
                    Telephone = user.PhoneNumber,
                    StudentId = user.StudentId,
                    UniversityId = user.UniversityId,
                    UserId = user.Id
                };
            }
        }

        /// <summary>
        /// For use when updating personal info
        /// </summary>
        /// <param name="model"></param>
        /// <param name="roleName"></param>
        /// <param name="studentId"></param>
        public void UpdateUser(UserViewModel model, string roleName, string userId)
        {
            using (var db = new ErasmusDbContext())
            {
                var store = new UserStore<ErasmusUser>(db);
                var manager = new UserManager<ErasmusUser>(store);
                var user = manager.FindById(userId);
                if (user == null)
                {
                    throw new FormValidationException(
                        new Dictionary<string, string>()
                        {
                            {"UserId", "User doesn't exists."}
                        }
                    );
                }

                user.Birthday = model.Birthday;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PersonalIdCode = model.PersonalCode;
                user.StudentId = model.StudentId;
                user.PhoneNumber = model.Telephone;
                user.Email = model.Email;
                user.UserName = model.Username;

                manager.Update(user);
            }
        }

        internal void UpdateCoordinator(UserViewModel model, string coordinator, string userId)
        {
            using (var db = new ErasmusDbContext())
            {
                var currentUser = db.Users.SingleOrDefault(x => x.Id == userId);
                

                var store = new UserStore<ErasmusUser>(db);
                var manager = new UserManager<ErasmusUser>(store);
                var user = manager.FindById(userId);
                if (user == null)
                {
                    throw new FormValidationException(
                        new Dictionary<string, string>()
                        {
                            {"UserId", "User doesn't exists."}
                        }
                    );
                }

                if (currentUser == null || currentUser.UniversityId == null || user.UniversityId != currentUser.UniversityId)
                {
                    throw new FormValidationException("Unable to edit this user due to invalid university.");
                }

                user.Birthday = model.Birthday;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PersonalIdCode = model.PersonalCode;
                user.StudentId = model.StudentId;
                user.PhoneNumber = model.Telephone;
                user.Email = model.Email;
                user.UserName = model.Username;

                manager.Update(user);
            }
        }

        internal object GetNewFieldOfStudyViewModel(int? uniId)
        {
            var model = new NewFieldOfStudyViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.StudySubjects = new MultiSelectList(db.StudySubjects.Where(x => x.UniversityId == uniId).ToList()
                    .Select(x => new KeyValueIntStr()
                    {
                        Key = x.Id,
                        Value = x.Name
                    }), "Key", "Value");
            }
            return model;
        }

        internal object GetNewFieldOfStudyViewModel(int? uniId, int id)
        {
            var model = new NewFieldOfStudyViewModel();
            using (var db = new ErasmusDbContext())
            {
                var fos = db.FieldsOfStudy.SingleOrDefault(x => x.Id == id && x.UniversityId == uniId);
                if (fos == null)
                {
                    throw new FormValidationException("Invalid field of study");
                }
                model.Id = fos.Id;
                model.Name = fos.Name;
                model.Degree = fos.Degree;
                model.StudySubjects = new MultiSelectList(db.StudySubjects.Where(x => x.UniversityId == uniId).ToList()
                    .Select(x => new KeyValueIntStr()
                    {
                        Key = x.Id,
                        Value = x.Name
                    }), "Key", "Value", fos.StudySubjects.Select(x => x.Id).ToList());
            }
            return model;
        }

        public NewFacultyViewModel GetNewFacultyViewModel(int? uniId)
        {
            var model = new NewFacultyViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.StudyFields = new MultiSelectList(db.FieldsOfStudy.Where(x => x.UniversityId == uniId).ToList().Select(x =>
                    new KeyValueIntStr()
                    {
                        Key = x.Id,
                        Value = x.Name
                    }), "Key", "Value");
            }
            return model;
        }

        public NewFacultyViewModel GetNewFacultyViewModel(int? uniId, int id)
        {
            var model = new NewFacultyViewModel();
            using (var db = new ErasmusDbContext())
            {
                var faculty = db.Faculties.SingleOrDefault(x => x.Id == id && x.UniversityId == uniId);
                if (faculty == null)
                {
                    throw new FormValidationException("Invalid faculty");
                }
                model.Id = faculty.Id;
                model.Name = faculty.Name;
                model.SelectedFos = faculty.StudyFields.Select(x => x.Id).ToList();

                model.StudyFields = new MultiSelectList(db.FieldsOfStudy.Where(x => x.UniversityId == uniId).ToList().Select(x =>
                    new KeyValueIntStr()
                    {
                        Key = x.Id,
                        Value = x.Name
                    }), "Key", "Value", model.SelectedFos);
            }
            return model;
        }

        public ViewModels.Admin.StudySubjectsViewModel GetStudySubjectsViewModel(int? uniId)
        {
            var model = new ViewModels.Admin.StudySubjectsViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.StudySubjects = db.StudySubjects.Where(x => x.UniversityId == uniId).ToList().Select(x => new StudySubjectView()
                {
                    Name = x.Name,
                    Id = x.Id,
                    Credits = x.Credits,
                    Language = x.Language,
                    Evaluation = x.Evaluation,
                    Semester = x.Semester,
                    FieldOfStudyName = x.FieldOfStudy == null ? "Not assigned" : x.FieldOfStudy.Name
                }).ToList();
            }
            return model;
        }

        public FieldsOfStudiesViewModel GetFieldsOfStudiesViewModel(int? uniId)
        {
            var model = new FieldsOfStudiesViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.FieldsOfStudy = db.FieldsOfStudy.Where(x => x.UniversityId == uniId).ToList().Select(x => new FieldOfStudyView()
                {
                    Name = x.Name,
                    Id = x.Id,
                    Degree = x.Degree,
                    FacultyName = x.Faculty == null ? "Not assigned" : x.Faculty.Name
                }).ToList();
            }
            return model;
        }

        public FacultiesViewModel GetFacultiesViewModel(int? uniId)
        {
            var model = new FacultiesViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.Faculties = db.Faculties.Where(x => x.UniversityId == uniId).ToList().Select(x => new FacultyView()
                {
                    Name = x.Name,
                    Id = x.Id,
                    UniversityName = x.University == null ? "Not assigned" : x.University.Name
                }).ToList();
            }
            return model;
        }

        internal void CreateStudySubject(NewStudySubjectViewModel model, int? uniId)
        {
            using (var db = new ErasmusDbContext())
            {
                if (model.Id != null)
                {
                    //Edit
                    var sub = db.StudySubjects.SingleOrDefault(x => x.Id == model.Id && x.UniversityId == uniId);
                    if (sub == null)
                    {
                        throw new FormValidationException("Invalid subject");
                    }

                    sub.Credits = model.Credits;
                    sub.Evaluation = model.Evaluation;
                    sub.Language = model.Language;
                    sub.Name = model.Name;
                    sub.Semester = model.Semester;
                    db.SaveChanges();
                }
                else
                {
                    //New
                    db.StudySubjects.Add(new StudySubjects()
                    {
                        Credits = model.Credits,
                        Evaluation = model.Evaluation,
                        Language = model.Language,
                        Name = model.Name,
                        Semester = model.Semester,
                        UniversityId = uniId
                    });

                    db.SaveChanges();
                }
            }
        }

        internal object GetNewStudySubjectViewModel(int? uniId)
        {
            var model = new NewStudySubjectViewModel();
            return model;
        }

        internal object GetNewStudySubjectViewModel(int? uniId, int id)
        {
            var model = new NewStudySubjectViewModel();
            using (var db = new ErasmusDbContext())
            {
                var sub = db.StudySubjects.SingleOrDefault(x => x.Id == id && x.UniversityId == uniId);
                if (sub == null)
                {
                    throw new FormValidationException("Invalid subject");
                }

                model.Id = sub.Id;
                model.Credits = sub.Credits;
                model.Evaluation = sub.Evaluation;
                model.Language = sub.Language;
                model.Name = sub.Name;
                model.Semester = sub.Semester;
            }
            return model;
        }

        public void UpdateUser(UserViewModel model, string roleName)
        {
            using (var db = new ErasmusDbContext())
            {
                var store = new UserStore<ErasmusUser>(db);
                var manager = new UserManager<ErasmusUser>(store);
                var user = manager.FindById(model.UserId);
                if (user == null)
                {
                    throw new FormValidationException(
                        new Dictionary<string, string>()
                        {
                            {"UserId", "User doesn't exists."}
                        }
                    );
                }

                model.UniversityId = db.Universities.SingleOrDefault(x => x.Id == model.UniversityId)?.Id;

                user.Birthday = model.Birthday;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PersonalIdCode = model.PersonalCode;
                user.StudentId = model.StudentId;
                user.UniversityId = model.UniversityId;
                user.PhoneNumber = model.Telephone;
                user.Email = model.Email;
                user.UserName = model.Username;

                manager.Update(user);
            }
        }

        internal void CreateNewFaculty(NewFacultyViewModel model, int? uniId)
        {
            using (var db = new ErasmusDbContext())
            {
                if (model.Id.HasValue)
                {
                    //Edit functionality
                    var faculty = db.Faculties.SingleOrDefault(x => x.Id == model.Id);
                    if (faculty == null)
                    {
                        throw new FormValidationException("Invalid faculty provided");
                    }

                    var fieldsOfStudy = new List<FieldOfStudy>();
                    if (model.SelectedFos != null && model.SelectedFos.Any())
                    {
                        fieldsOfStudy = db.FieldsOfStudy.Where(x => model.SelectedFos.Contains(x.Id)).ToList();
                        if (fieldsOfStudy.Count != model.SelectedFos.Count)
                        {
                            throw new ValidationException("Some fields of study were not found.");
                        }
                    }

                    var oldFos = faculty.StudyFields;
                    foreach (var fos in oldFos)
                    {
                        //Reset fos if removed
                        fos.FacultyId = null;
                    }
                    db.SaveChanges();

                    faculty.Name = model.Name;
                    faculty.StudyFields = fieldsOfStudy;
                    db.SaveChanges();
                    return;
                }
                else
                {
                    //New functionality
                    var fieldsOfStudy = new List<FieldOfStudy>();
                    if (model.SelectedFos != null && model.SelectedFos.Any())
                    {
                        fieldsOfStudy = db.FieldsOfStudy.Where(x => model.SelectedFos.Contains(x.Id)).ToList();
                        if (fieldsOfStudy.Count != model.SelectedFos.Count)
                        {
                            throw new ValidationException("Some fields of study were not found.");
                        }
                    }

                    db.Faculties.Add(new Faculty()
                    {
                        Name = model.Name,
                        StudyFields = fieldsOfStudy,
                        UniversityId = uniId
                    });

                    db.SaveChanges();
                }

            }
        }

        internal void CreateNewFieldOfStudy(NewFieldOfStudyViewModel model, int? uniId)
        {
            using (var db = new ErasmusDbContext())
            {
                if (model.Id.HasValue)
                {
                    //Edit
                    var fos = db.FieldsOfStudy.SingleOrDefault(x => x.Id == model.Id && x.UniversityId == uniId);
                    if (fos == null)
                    {
                        throw new FormValidationException("Invalid field of study");
                    }

                    var studySubjects = new List<StudySubjects>();
                    if (model.SelectedStudySubjects != null && model.SelectedStudySubjects.Any())
                    {
                        studySubjects = db.StudySubjects.Where(x => model.SelectedStudySubjects.Contains(x.Id)).ToList();
                        if (studySubjects.Count != model.SelectedStudySubjects.Count)
                        {
                            throw new ValidationException("Some study subjects were not found.");
                        }
                    }

                    foreach (var subject in fos.StudySubjects)
                    {
                        subject.FieldOfStudyId = null;
                    }
                    db.SaveChanges();
                    
                    fos.Degree = model.Degree;
                    fos.Name = model.Name;
                    fos.StudySubjects = studySubjects;
                    db.SaveChanges();
                }
                else
                {
                    //New
                    var studySubjects = new List<StudySubjects>();
                    if (model.SelectedStudySubjects != null && model.SelectedStudySubjects.Any())
                    {
                        studySubjects = db.StudySubjects.Where(x => model.SelectedStudySubjects.Contains(x.Id)).ToList();
                        if (studySubjects.Count != model.SelectedStudySubjects.Count)
                        {
                            throw new ValidationException("Some study subjects were not found.");
                        }
                    }

                    db.FieldsOfStudy.Add(new FieldOfStudy()
                    {
                        Name = model.Name,
                        Degree = model.Degree,
                        StudySubjects = studySubjects,
                        UniversityId = uniId
                    });

                    db.SaveChanges();
                }
            }
        }

        public void CreateNewUser(NewUserViewModel user, string role)
        {
            using (var db = new ErasmusDbContext())
            {
                var store = new UserStore<ErasmusUser>(db);
                var manager = new UserManager<ErasmusUser>(store);
                int? universityId = null;
                if (user.UniversityId != 0)
                {
                    var university = db.Universities.SingleOrDefault(x => x.Id == user.UniversityId);
                    if (university == null)
                    {
                        throw new FormValidationException(new Dictionary<string, string>() { { "University", "Invalid university selected" } });
                    }
                    universityId = university.Id;
                }

                CreateUser(manager, user, role);
            }
        }

        internal int CreateUniversity(NewUniversityViewModel model, string userId)
        {
            using (var db = new ErasmusDbContext())
            {
                var user = db.Users.SingleOrDefault(x => x.Id == userId);

                var uni = db.Universities.Add(new University()
                {
                    Name = model.Name,
                    Address = model.Address,
                    City = model.City,
                    Country = model.Country,
                    ErasmusCode = model.ErasmusCode
                });

                db.SaveChanges();

                user.UniversityId = uni.Id;
                db.SaveChanges();

                db.UserPermissions.Add(new UserPermissions()
                {
                    UserId = userId,
                    UniversityId = uni.Id
                });
                db.SaveChanges();
                return uni.Id;
            }
            
        }

        public void CreateNewUser(NewUserViewModel user, string role, string userId)
        {
            using (var db = new ErasmusDbContext())
            {
                var currentUser = db.Users.SingleOrDefault(x => x.Id == userId);
                if (currentUser == null || currentUser.UniversityId == null)
                {
                    throw new FormValidationException("Your user was not found or you are not assigned to a university.");
                }
                var store = new UserStore<ErasmusUser>(db);
                var manager = new UserManager<ErasmusUser>(store);
                user.UniversityId = currentUser.UniversityId;
                CreateUser(manager, user, role);
            }
        }

        public void DeleteUser(string userId, string roleName)
        {
            using (var db = new ErasmusDbContext())
            {
                var store = new UserStore<ErasmusUser>(db);
                var manager = new UserManager<ErasmusUser>(store);
                var user = manager.FindById(userId);
                if (user != null && manager.IsInRole(userId, roleName))
                {
                    manager.Delete(user);
                }
            }
        }
        private void CreateUser(UserManager<ErasmusUser> manager, NewUserViewModel user, string role)
        {
            //Check for existing user
            var dbUser = manager.Users.SingleOrDefault(x => x.Email == user.Email);

            if (dbUser == null)
            {
                //Doesn't exist - create
                dbUser = new ErasmusUser
                {
                    Email = user.Email,
                    UserName = user.Email,
                    EmailConfirmed = true,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Birthday = user.Birthday,
                    PhoneNumber = user.Telephone,
                    UniversityId = user.UniversityId == 0 ? null : user.UniversityId,
                    PersonalIdCode = user.PersonalCode,
                    StudentId = user.StudentId
                };
                manager.Create(dbUser, user.Password);
                manager.AddToRole(dbUser.Id, role);
            }
            else
            {
                throw new FormValidationException(
                    new Dictionary<string, string>()
                    {
                        {"Email", "User with the same email already exists."}
                    }
                );
            }
        }

        private void CreateUser(UserManager<ErasmusUser> manager, UserViewModel user, string role)
        {
            //Check for existing user
            var dbUser = manager.Users.SingleOrDefault(x => x.Email == user.Email);

            if (dbUser == null)
            {
                //Doesn't exist - create
                dbUser = new ErasmusUser
                {
                    Email = user.Email,
                    UserName = user.Email,
                    EmailConfirmed = true,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Birthday = user.Birthday,
                    PhoneNumber = user.Telephone,
                    UniversityId = user.UniversityId == 0 ? null : user.UniversityId,
                    PersonalIdCode = user.PersonalCode,
                    StudentId = user.StudentId
                };
                manager.Create(dbUser, user.Password);
                manager.AddToRole(dbUser.Id, role);
            }
            else
            {
                throw new FormValidationException(
                    new Dictionary<string, string>()
                    {
                        {"Email", "User with the same email already exists."}
                    }
                );
            }
        }


        /// <summary>
        /// Security hole, never expose returned identity directly to view.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public List<ErasmusUser> GetUsersListByRole(string roleName, string userId)
        {
           
            var model = new NewFacultyViewModel();
            using (var db = new ErasmusDbContext())
            {
                    var user = db.Users.SingleOrDefault(x => x.Id == userId);
                    var role = db.Roles.SingleOrDefault(x => x.Name == roleName);
                if (role == null)
                {
                    return new List<ErasmusUser>();
                }
                var userIds = role.Users.Select(x => x.UserId).ToList();
                var users = db.Users.Where(x => userIds.Contains(x.Id) && x.UniversityId==user.UniversityId).ToList();
                return users;
            }
        }

        public UniversitiesViewModel GetUniversitiesViewModel(string userId)
        {
            var model = new UniversitiesViewModel();
            using (var db = new ErasmusDbContext())
            {
                var store = new UserStore<ErasmusUser>(db);
                var manager = new UserManager<ErasmusUser>(store);

                var currentUniAdmin = manager.Users.SingleOrDefault(x => x.Id == userId);
                if (currentUniAdmin == null)
                {
                    throw new FormValidationException("Invalid user");
                }

                var thisUserUniversityIds = db.UserPermissions.Where(x => x.UserId == userId).Select(x => x.UniversityId).ToList();

                var Univers = db.Universities.Where(x => thisUserUniversityIds.Contains(x.Id)).ToList();

                if (Univers.Any())
                {
                    model.Universities = Univers.Select(x => new UniversityView()
                    {
                        Name = x.Name,
                        Id = x.Id,
                        Address = x.Address,
                        Country = x.Country,
                        ErasmusCode = x.ErasmusCode,
                        City = x.City
                    }).ToList();
                }

                return model;
            }


            /*public ViewModels.Admin.StudySubjectsViewModel GetStudySubjectsViewModel(int ID)
            {
                var model = new ViewModels.Admin.StudySubjectsViewModel();

                using (var db = new ErasmusDbContext())
                {
                    var thisUserUniversityIds = db.UserPermissions.Where(x => x.UniversityId == ID).Select(x => x.UniversityId).ToList();

                    var subjects = db.UserPermissions.Where(x => thisUserUniversityIds.Contains(x.UniversityId))
                        .Select(x => x.User).Where(x => x.UniversityId != ID).ToList();



                    if (subjects.Any())
                    {              
                        Name = x.Name,
                        Id = x.Id,
                        Credits = x.Credits,
                        Language = x.Language,
                        Evaluation = x.Evaluation,
                        Semester = x.Semester,
                        FieldOfStudyName = x.FieldOfStudy == null ? "Not assigned" : x.FieldOfStudy.Name
                    }).ToList();
                }
                return model;
                }*/
        }

    }
}

