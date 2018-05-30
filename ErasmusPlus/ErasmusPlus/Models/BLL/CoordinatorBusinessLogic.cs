using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ErasmusPlus.Models.Identity;
using ErasmusPlus.Models.ViewModels.Coordinator;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using System.IO;
using ErasmusPlus.Common.SharedModels;
using Newtonsoft.Json;
using ErasmusPlus.Models.Database;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using ErasmusPlus.Models.ViewModels.Admin;
using ErasmusPlus.Common.Database;
using System.Web.Mvc;

namespace ErasmusPlus.Models.BLL
{
    public class CoordinatorBusinessLogic
    {
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
                        db.Universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
                            .ToList(), "Value", "Text");
            }
            return model;
        }


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
                var users = db.Users.Where(x => userIds.Contains(x.Id) && x.UniversityId == user.UniversityId).ToList();
                return users;
            }
        }

        public StudentAgreementsViewModel GetAgreementsListViewModel(string userId)
        {
            var model = new StudentAgreementsViewModel();
            using (var db = new ErasmusDbContext())
            {
                var user = db.Users.SingleOrDefault(x => x.Id == userId);
                model.StudentAgreements = db.Agreements.Where(x => x.State != AgreementState.New && (x.SourceUniversityId == user.UniversityId || x.TargetUniversityId == user.UniversityId)).ToList().Select(x => new StudentAgreementListView()
                {
                    Id = x.Id,
                    ErasmusUser = x.ErasmusUser,
                    SourceUniversity = x.SourceUniversity,
                    TargetUniversity = x.TargetUniversity,
                    EndDate = x.EndDate,
                    State = x.State,
                    StartDate = x.StartDate
                }).ToList();
            }
            return model;
        }
        public StudentAgreementsViewModel GetIncomingAgreementsListViewModel(string userId)
        {
            var model = new StudentAgreementsViewModel();
            using (var db = new ErasmusDbContext())
            {
                var user = db.Users.SingleOrDefault(x => x.Id == userId);
                model.StudentAgreements = db.Agreements.Where(x => x.State != AgreementState.New && x.TargetUniversityId == user.UniversityId).ToList().Select(x => new StudentAgreementListView()
                {
                    Id = x.Id,
                    ErasmusUser = x.ErasmusUser,
                    SourceUniversity = x.SourceUniversity,
                    TargetUniversity = x.TargetUniversity,
                    EndDate = x.EndDate,
                    State = x.State,
                    StartDate = x.StartDate
                }).ToList();
            }
            return model;
        }
        public StudentAgreementsViewModel GetOutgoingAgreementsListViewModel(string userId)
        {
            var model = new StudentAgreementsViewModel();
            using (var db = new ErasmusDbContext())
            {
                var user = db.Users.SingleOrDefault(x => x.Id == userId);
                model.StudentAgreements = db.Agreements.Where(x => x.State != AgreementState.New && x.SourceUniversityId == user.UniversityId).ToList().Select(x => new StudentAgreementListView()
                {
                    Id = x.Id,
                    ErasmusUser = x.ErasmusUser,
                    SourceUniversity = x.SourceUniversity,
                    TargetUniversity = x.TargetUniversity,
                    EndDate = x.EndDate,
                    State = x.State,
                    StartDate = x.StartDate
                }).ToList();
            }
            return model;
        }

        public StudentAgreementDetailsView GetStudentAgreementDetailsView(int id)
        {
            var model = new StudentAgreementDetailsView();
            using (var db = new ErasmusDbContext())
            {
                var agreement = db.Agreements.SingleOrDefault(x => x.Id == id);
                if (agreement == null)
                {
                    return null;
                }
                model.Student = agreement.ErasmusUser;
                model.LanguageLevel = agreement.LanguageLevel;
                model.SourceUniversity = agreement.SourceUniversity;
                model.Semester = agreement.Semester;
                model.EndDate = agreement.EndDate;
                model.TargetUniversity = agreement.TargetUniversity;
                model.StartDate = agreement.StartDate;
                model.Language = agreement.Language;
                model.State = Enum.GetName(typeof(AgreementState), agreement.State);
                model.StoragePath = agreement.StoragePath;
                model.FinancingSource = agreement.FinancingSource;
                model.Scholarship = agreement.Scholarship;
                model.TargetStudySubjects = agreement.TargetSubjects.ToList();
                model.TargetFaculty = agreement.TargetFaculty;
                model.SourceFieldOfStudy = agreement.SourceFieldOfStudy;
                model.TargetFieldOfStudy = agreement.TargetFieldOfStudy;
                model.SourceStudySubjects = agreement.SourceSubjects.ToList();
                model.SourceFaculty = agreement.SourceFaculty;
            }
            return model;
        }

        public void ChangeState(int id, AgreementState state, string reason = null)
        {
            using (var db = new ErasmusDbContext())
            {
                var agreement = db.Agreements.SingleOrDefault(x => x.Id == id);
                if (agreement == null)
                {
                    throw new FormValidationException("Agreement not found");
                }

                agreement.State = state;
                if (reason != null)
                {
                    agreement.DeclineReason = reason;
                }
                db.SaveChanges();
            }
        }

        
        }

    }
