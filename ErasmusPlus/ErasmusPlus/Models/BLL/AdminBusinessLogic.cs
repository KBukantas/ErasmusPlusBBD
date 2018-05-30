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

namespace ErasmusPlus.Models.BLL
{
    public class AdminBusinessLogic
    {
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

        public List<UserListViewModel> GetUserPermissionsModel()
        {
            var model = new List<UserListViewModel>();
            using (var db = new ErasmusDbContext())
            {
                var store = new UserStore<ErasmusUser>(db);
                var users = store.Users.ToList();
                if (users.Any())
                {
                    var manager = new UserManager<ErasmusUser>(store);
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

        internal void ChangeUserPermissions(UserPermissionsModel model)
        {
            using (var db = new ErasmusDbContext())
            {
                var user = db.Users.SingleOrDefault(x => x.Id == model.UserId);

                if (user != null)
                {
                    var permissions = db.UserPermissions.Where(x => x.UserId == user.Id);
                    if (permissions.Any())
                    {
                        db.UserPermissions.RemoveRange(permissions);
                        db.SaveChanges();
                    }

                    try
                    {
                        if (model.Permissions != null && model.Permissions.Any())
                        {
                            foreach (var perm in model.Permissions)
                            {
                                db.UserPermissions.Add(new UserPermissions()
                                {
                                    UserId = user.Id,
                                    UniversityId = perm.UniversityId,
                                    FacultyId = perm.FacultyId.HasValue ? perm.FacultyId : null
                                });
                            }
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                }
            }
        }

        public UserPermissionsViewModel GetUserPermissionsForUser(string userId)
        {
            var model = new UserPermissionsViewModel();
            using (var db = new ErasmusDbContext())
            {
                var store = new UserStore<ErasmusUser>(db);
                var user = store.Users.SingleOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    return null;
                }
                var manager = new UserManager<ErasmusUser>(store);
                model.UserId = user.Id;
                model.Email = user.Email;
                model.Role = manager.GetRoles(user.Id).FirstOrDefault();
                model.Permissions = db.UserPermissions.Where(x => x.UserId == user.Id)
                    .Select(x => new PermissionViewModel()
                    {
                        Id = x.Id,
                        UniversityId = x.UniversityId,
                        UniversityName = x.University.Name,
                        FacultyId = x.FacultyId,
                        FacultyName = x.Faculty == null ? "" : x.Faculty.Name
                    }).ToList();
                model.UniversitiesList =
                    new SelectList(
                        db.Universities.Select(x => new SelectListItem() {Text = x.Name, Value = x.Id.ToString()})
                            .ToList(), "Value", "Text");
            }
            return model;
        }

        internal object GetFacultyDataByUniversityId(int universityId)
        {
            using (var db = new ErasmusDbContext())
            {
                //Guard against unexisting/false tries
                var originalUniversity = db.Universities.SingleOrDefault(x => x.Id == universityId);
                if (originalUniversity == null)
                {
                    return null;
                }

                var faculties = originalUniversity.Faculties.Select(x => new KeyValueIntStr()
                {
                    Key = x.Id,
                    Value = x.Name
                }).ToList();
                return faculties;
            }
        }

        public DrillDownViewModel GetDrillDownModel()
        {
            var model = new DrillDownViewModel();
            using (var db = new ErasmusDbContext())
            {
                var universities = db.Universities.ToList();
                var countries = universities.Select(x => x.Country).Distinct().ToList();
                foreach (var country in countries)
                {
                    var countryDataList = new CountryDataList()
                    {
                        CountryName = country,
                        Universities = universities.Where(x => x.Country == country).Select(x =>
                            new UniversityDataList()
                            {
                                UniversityName = x.Name,
                                UniversityId = x.Id,
                            }).ToList()
                    };

                    //Will become a bottleneck for performance here, consider refactoring
                    //TODO REFACTOR
                    foreach (var university in countryDataList.Universities)
                    {
                        var uniInList = countryDataList.Universities.Single(x =>x.UniversityId == university.UniversityId);
                        var agreementsIn = db.Agreements.Where(x => x.TargetUniversityId == university.UniversityId).Include(x => x.ErasmusUser).ToList();
                        var agreementsOut = db.Agreements.Where(x => x.SourceUniversityId == university.UniversityId).Include(x => x.ErasmusUser).ToList();
                        uniInList.StudentsIn = agreementsIn.Select(x => new StudentInfoData(){LastName = x.ErasmusUser.LastName, FirstName = x.ErasmusUser.FirstName}).ToList();
                        uniInList.StudentsOut = agreementsOut.Select(x => new StudentInfoData(){LastName = x.ErasmusUser.LastName, FirstName = x.ErasmusUser.FirstName}).ToList();
                        uniInList.CountIn = uniInList.StudentsIn.Count;
                        uniInList.CountOut = uniInList.StudentsOut.Count;
                    }

                    model.Countries.Add(countryDataList);
                }
            }
            return model;
        }

        public NewFacultyViewModel GetNewFacultyViewModel()
        {
            var model = new NewFacultyViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.StudyFields = new SelectList(db.FieldsOfStudy.Where(x => x.FacultyId == null).ToList().Select(x =>
                    new KeyValueIntStr()
                    {
                        Key = x.Id,
                        Value = x.Name
                    }), "Key", "Value");
            }
            return model;
        }

        /// <summary>
        /// Security hole, never expose returned identity directly to view.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public List<ErasmusUser> GetUsersListByRole(string roleName)
        {
            var model = new NewFacultyViewModel();
            using (var db = new ErasmusDbContext())
            {
                var role = db.Roles.SingleOrDefault(x => x.Name == roleName);
                if (role == null)
                {
                    return new List<ErasmusUser>();
                }
                var userIds = role.Users.Select(x => x.UserId).ToList();
                var users = db.Users.Where(x => userIds.Contains(x.Id)).ToList();
                return users;
            }
        }

        public void CreateNewUniAdmin(UserViewModel user, string role)
        {
            using (var db = new ErasmusDbContext())
            {
                var store = new UserStore<ErasmusUser>(db);
                var manager = new UserManager<ErasmusUser>(store);
                user.UniversityId = null;
                CreateUser(manager, user, role);
            }
        }

        public void CreateNewUser(UserViewModel user, string role)
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

        public FieldsOfStudiesViewModel GetFieldsOfStudiesViewModel()
        {
            var model = new FieldsOfStudiesViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.FieldsOfStudy = db.FieldsOfStudy.ToList().Select(x => new FieldOfStudyView()
                {
                    Name = x.Name,
                    Id = x.Id,
                    Degree = x.Degree,
                    FacultyName = x.Faculty == null ? "Not assigned" : x.Faculty.Name
                }).ToList();
            }
            return model;
        }

        public FacultiesViewModel GetFacultiesViewModel()
        {
            var model = new FacultiesViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.Faculties = db.Faculties.ToList().Select(x => new FacultyView()
                {
                    Name = x.Name,
                    Id = x.Id,
                    UniversityName = x.University == null ? "Not assigned" : x.University.Name
                }).ToList();
            }
            return model;
        }

        public ViewModels.Admin.StudySubjectsViewModel GetStudySubjectsViewModel()
        {
            var model = new ViewModels.Admin.StudySubjectsViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.StudySubjects = db.StudySubjects.ToList().Select(x => new StudySubjectView()
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

        public UniversityAgreementsViewModel GetUniversityAgreements()
        {
            var model = new UniversityAgreementsViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.UniversityAgreements.AddRange(db.UniversityAgreements.ToList().Select(x =>
                    new UniversityAgreementsView()
                    {
                        Id = x.Id,
                        SourceUniversity = x.SourceUniversity.Name,
                        TargetUniversity = x.TargetUniversity.Name
                    }));
            }
            return model;
        }

        public UniversityAgreementViewModel GetUniversityAgreementViewModel()
        {
            var model = new UniversityAgreementViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.SourceUniversityList = new SelectList(db.Universities.Select(x => new KeyValueIntStr()
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

        public NewFieldOfStudyViewModel GetNewFieldOfStudyViewModel()
        {
            var model = new NewFieldOfStudyViewModel();
            using (var db = new ErasmusDbContext())
            {
                model.StudySubjects = new SelectList(db.StudySubjects.Where(x => x.FieldOfStudyId == null).ToList()
                    .Select(x => new KeyValueIntStr()
                    {
                        Key = x.Id,
                        Value = x.Name
                    }), "Key", "Value");
            }
            return model;
        }

        public void CreateUniversity(NewUniversityViewModel model)
        {
            using (var db = new ErasmusDbContext())
            {
                var faculties = db.Faculties.Where(x => model.SelectedFaculties.Contains(x.Id)).ToList();
                if (faculties.Count != model.SelectedFaculties.Count)
                {
                    throw new ValidationException("Some faculties were not found.");
                }

                db.Universities.Add(new University()
                {
                    Faculties = faculties,
                    Name = model.Name,
                    Address = model.Address,
                    City = model.City,
                    Country = model.Country,
                    ErasmusCode = model.ErasmusCode
                });

                db.SaveChanges();
            }
        }

        public void CreateNewFaculty(NewFacultyViewModel model)
        {
            using (var db = new ErasmusDbContext())
            {
                var fieldsOfStudy = db.FieldsOfStudy.Where(x => model.SelectedFos.Contains(x.Id)).ToList();
                if (fieldsOfStudy.Count != model.SelectedFos.Count)
                {
                    throw new ValidationException("Some fields of study were not found.");
                }

                db.Faculties.Add(new Faculty()
                {
                    Name = model.Name,
                    StudyFields = fieldsOfStudy
                });

                db.SaveChanges();
            }
        }

        public void CreateNewFieldOfStudy(NewFieldOfStudyViewModel model)
        {
            using (var db = new ErasmusDbContext())
            {
                var studySubjects = db.StudySubjects.Where(x => model.SelectedStudySubjects.Contains(x.Id)).ToList();
                if (studySubjects.Count != model.SelectedStudySubjects.Count)
                {
                    throw new ValidationException("Some study subjects were not found.");
                }

                db.FieldsOfStudy.Add(new FieldOfStudy()
                {
                    Name = model.Name,
                    Degree = model.Degree,
                    StudySubjects = studySubjects
                });

                db.SaveChanges();
            }
        }

        public UserViewModel GetUserViewModelById(string userId, string roleName)
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

        public void CreateStudySubject(NewStudySubjectViewModel model)
        {
            using (var db = new ErasmusDbContext())
            {
                db.StudySubjects.Add(new StudySubjects()
                {
                    Credits = model.Credits,
                    Evaluation = model.Evaluation,
                    Language = model.Language,
                    Name = model.Name,
                    Semester = model.Semester
                });

                db.SaveChanges();
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
    }
}