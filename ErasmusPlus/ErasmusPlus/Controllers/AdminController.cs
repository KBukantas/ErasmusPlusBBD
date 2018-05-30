using System;
using System.Collections.Generic;
using System.Linq;
using ErasmusPlus.Common.Authorization;
using ErasmusPlus.Models.BLL;
using ErasmusPlus.Models.ViewModels.Admin;
using System.Web.Mvc;
using ErasmusPlus.Common.Database;
using ErasmusPlus.Common.SharedModels;
using ErasmusPlus.Models;
using ErasmusPlus.Models.ViewModels.Student;
using ErasmusPlus.Models.Extensions;
using ErasmusPlus.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace ErasmusPlus.Controllers
{

    [Authorize]
    public class AdminController : Controller
    {
        private static AdminBusinessLogic _adminBusinessLogic;
        private static CommonBusinessLogic _commonBusinessLogic;

        public AdminController()
        {
            if (_adminBusinessLogic == null)
            {
                _adminBusinessLogic = new AdminBusinessLogic();
            }

            if (_commonBusinessLogic == null)
            {
                _commonBusinessLogic = new CommonBusinessLogic();
            }
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult UserPermissions()
        {
            var model = _adminBusinessLogic.GetUserPermissionsModel();
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult EditPermissions(string userId)
        {
            var model = _adminBusinessLogic.GetUserPermissionsForUser(userId);
            if (model == null)
            {
                return View("UserError", new UserError("Invalid user id provided"));
            }
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        [HttpPost]
        public ActionResult EditPermissions(UserPermissionsModel model)
        {
            try
            {
                _adminBusinessLogic.ChangeUserPermissions(model);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
        
            return RedirectToAction("UserPermissions");
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        [HttpGet]
        public ActionResult GetFacultyDataByUniId(int universityId)
        {
            if (universityId == 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var data = _adminBusinessLogic.GetFacultyDataByUniversityId(universityId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult DrillDown()
        {
            var model = _adminBusinessLogic.GetDrillDownModel();
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult UniversityAdmins()
        {
            var model = new List<UserViewModel>();
            var universityAdmins = _adminBusinessLogic.GetUsersListByRole(UserRoles.UniversityAdmin);
            model.AddRange(universityAdmins.Select(x => new UserViewModel()
            {
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                PersonalCode = x.PersonalIdCode,
                Telephone = x.PhoneNumber,
                UniversityId = x.UniversityId,
                Username = x.UserName,
                Birthday = x.Birthday,
                StudentId = x.StudentId,
                RoleName = "University administrator",
                UserId = x.Id
            }));
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult NewUniversityAdmin()
        {
            var model = new UserViewModel();
            //var universities = _commonBusinessLogic.GetUniversitiesList();
            //universities.Insert(0, new University() { Name = "Select university", Id = 0 });
            //model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
            model.Birthday = DateTime.Today;
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        [HttpPost]
        public ActionResult NewUniversityAdmin(UserViewModel user)
        {
            var universities = _commonBusinessLogic.GetUniversitiesList();
            if (ModelState.IsValid)
            {
                try
                {
                    _adminBusinessLogic.CreateNewUniAdmin(user, UserRoles.UniversityAdmin);
                }
                catch (FormValidationException e)
                {
                    e.ValidationToModelState(ModelState);
                    universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                    user.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
                    return View(user);
                }
                
                return RedirectToAction("UniversityAdmins");
            }
            universities.Insert(0, new University(){Name = "Select university", Id = 0});
            user.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = user.UniversityId == x.Id }), "Value", "Text");
            return View(user);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult EditUniversityAdmin(string userId)
        {
            UserViewModel model = _adminBusinessLogic.GetUserViewModelById(userId, UserRoles.UniversityAdmin);
            if (model != null)
            {
                var universities = _commonBusinessLogic.GetUniversitiesList();
                universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = model.UniversityId == x.Id }), "Value", "Text");
            }

            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        [HttpPost]
        public ActionResult EditUniversityAdmin(UserViewModel model)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors, (state, error) => error.ErrorMessage).ToList();
            //For now the password field needs to be ignored, if there's only 1 error then it's the password
            if (errors.Count != 1)
            {
                var universities = _commonBusinessLogic.GetUniversitiesList();
                universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = model.UniversityId == x.Id }), "Value", "Text");
                return View(model);
            }

            try
            {
                _adminBusinessLogic.UpdateUser(model, UserRoles.UniversityAdmin);
            }
            catch (FormValidationException e)
            {
                e.ValidationToModelState(ModelState);
                var universities = _commonBusinessLogic.GetUniversitiesList();
                universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = model.UniversityId == x.Id }), "Value", "Text");
                return View(model);
            }

            return RedirectToAction("UniversityAdmins");
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult DeleteUniversityAdmin(string userId)
        {
            _adminBusinessLogic.DeleteUser(userId, UserRoles.UniversityAdmin);
            return RedirectToAction("ForeighnAndCoordinator");
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult ForeighnAndCoordinator()
        {
            var model = new List<UserViewModel>();
            var coordinators = _adminBusinessLogic.GetUsersListByRole(UserRoles.Coordinator);
            var foreign = _adminBusinessLogic.GetUsersListByRole(UserRoles.Foreign);
            model.AddRange(coordinators.Select(x => new UserViewModel()
            {
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                PersonalCode = x.PersonalIdCode,
                Telephone = x.PhoneNumber,
                UniversityId = x.UniversityId,
                Username = x.UserName,
                Birthday = x.Birthday,
                StudentId = x.StudentId,
                RoleName = "Coordinator",
                UserId = x.Id
            }));
            model.AddRange(foreign.Select(x => new UserViewModel()
            {
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                PersonalCode = x.PersonalIdCode,
                Telephone = x.PhoneNumber,
                UniversityId = x.UniversityId,
                Username = x.UserName,
                Birthday = x.Birthday,
                StudentId = x.StudentId,
                RoleName = "Foreign",
                UserId = x.Id
            }));
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        [HttpPost]
        public ActionResult NewStudent(UserViewModel user)
        {
            var universities = _commonBusinessLogic.GetUniversitiesList();
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(user.StudentId))
                {
                    ModelState.AddModelError("StudentId", "Student ID is required");
                    universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                    user.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
                    return View(user);
                }
                try
                {
                    _adminBusinessLogic.CreateNewUser(user, UserRoles.Student);
                }
                catch (FormValidationException e)
                {
                    e.ValidationToModelState(ModelState);
                    universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                    user.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
                    return View(user);
                }

                return RedirectToAction("Students", "Coordinator");
            }
            universities.Insert(0, new University() { Name = "Select university", Id = 0 });
            user.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
            return View(user);
        }

        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        public ActionResult NewStudent()
        {
            var model = new UserViewModel();
            var universities = _commonBusinessLogic.GetUniversitiesList();
            universities.Insert(0, new University() { Name = "Select university", Id = 0 });
            model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
            model.Birthday = DateTime.Today;
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        [HttpPost]
        public ActionResult NewCoordinator(UserViewModel user)
        {
            var universities = _commonBusinessLogic.GetUniversitiesList();
            if (ModelState.IsValid)
            {
                try
                {
                    _adminBusinessLogic.CreateNewUser(user, UserRoles.Coordinator);
                }
                catch (FormValidationException e)
                {
                    e.ValidationToModelState(ModelState);
                    universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                    user.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
                    return View(user);
                }

                return RedirectToAction("ForeighnAndCoordinator");
            }
            universities.Insert(0, new University() { Name = "Select university", Id = 0 });
            user.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
            return View(user);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult NewCoordinator()
        {
            var model = new UserViewModel();
            var universities = _commonBusinessLogic.GetUniversitiesList();
            universities.Insert(0, new University() { Name = "Select university", Id = 0 });
            model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
            model.Birthday = DateTime.Today;
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult EditCoordinator(string userId)
        {
            UserViewModel model = _adminBusinessLogic.GetUserViewModelById(userId, UserRoles.Coordinator);
            if (model != null)
            {
                var universities = _commonBusinessLogic.GetUniversitiesList();
                universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = model.UniversityId == x.Id }), "Value", "Text");
            }

            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        [HttpPost]
        public ActionResult EditCoordinator(UserViewModel model)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors, (state, error) => error.ErrorMessage).ToList();
            //For now the password field needs to be ignored, if there's only 1 error then it's the password
            if (errors.Count != 1)
            {
                var universities = _commonBusinessLogic.GetUniversitiesList();
                universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                model.Universities =
                    new SelectList(
                        universities.Select(x => new SelectListItem()
                        {
                            Text = x.Name,
                            Value = x.Id.ToString(),
                            Selected = model.UniversityId == x.Id
                        }), "Value", "Text");
                return View(model);
            }
            
            try
            {
                _adminBusinessLogic.UpdateUser(model, UserRoles.Coordinator);
            }
            catch (FormValidationException e)
            {
                e.ValidationToModelState(ModelState);
                var universities = _commonBusinessLogic.GetUniversitiesList();
                universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = model.UniversityId == x.Id }), "Value", "Text");
                return View(model);
            }

            return RedirectToAction("ForeighnAndCoordinator");
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult DeleteCoordinator(string userId)
        {
            _adminBusinessLogic.DeleteUser(userId, UserRoles.Coordinator);
            return RedirectToAction("ForeighnAndCoordinator");
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        [HttpPost]
        public ActionResult NewForeign(UserViewModel user)
        {
            var universities = _commonBusinessLogic.GetUniversitiesList();
            if (ModelState.IsValid)
            {
                try
                {
                    _adminBusinessLogic.CreateNewUser(user, UserRoles.Foreign);
                }
                catch (FormValidationException e)
                {
                    e.ValidationToModelState(ModelState);
                    universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                    user.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
                    return View(user);
                }

                return RedirectToAction("ForeighnAndCoordinator");
            }
            universities.Insert(0, new University() { Name = "Select university", Id = 0 });
            user.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
            return View(user);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult NewForeign()
        {
            var model = new UserViewModel();
            var universities = _commonBusinessLogic.GetUniversitiesList();
            universities.Insert(0, new University() { Name = "Select university", Id = 0 });
            model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
            model.Birthday = DateTime.Today;
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult EditForeign(string userId)
        {
            UserViewModel model = _adminBusinessLogic.GetUserViewModelById(userId, UserRoles.Foreign);
            if (model != null)
            {
                var universities = _commonBusinessLogic.GetUniversitiesList();
                universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = model.UniversityId == x.Id}), "Value", "Text");
            }

            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        [HttpPost]
        public ActionResult EditForeign(UserViewModel model)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors, (state, error) => error.ErrorMessage).ToList();
            //For now the password field needs to be ignored, if there's only 1 error then it's the password
            if (errors.Count != 1)
            {
                var universities = _commonBusinessLogic.GetUniversitiesList();
                universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = model.UniversityId == x.Id }), "Value", "Text");
                return View(model);
            }

            try
            {
                _adminBusinessLogic.UpdateUser(model, UserRoles.Foreign);
            }
            catch (FormValidationException e)
            {
                e.ValidationToModelState(ModelState);
                var universities = _commonBusinessLogic.GetUniversitiesList();
                universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = model.UniversityId == x.Id }), "Value", "Text");
                return View(model);
            }

            return RedirectToAction("ForeighnAndCoordinator");
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult DeleteForeign(string userId)
        {
            _adminBusinessLogic.DeleteUser(userId, UserRoles.Foreign);
            return RedirectToAction("ForeighnAndCoordinator");
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult Universities()
        {
            UniversitiesViewModel model = _adminBusinessLogic.GetUniversitiesViewModel();
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult Faculties()
        {
            FacultiesViewModel model = _adminBusinessLogic.GetFacultiesViewModel();
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult FieldsOfStudies()
        {
            FieldsOfStudiesViewModel model = _adminBusinessLogic.GetFieldsOfStudiesViewModel();
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult StudySubjects()
        {
            var model = _adminBusinessLogic.GetStudySubjectsViewModel();
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult NewUniversity()
        {
            var model = _adminBusinessLogic.GetNewUniversityViewModel();
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult NewFaculty()
        {
            var model = _adminBusinessLogic.GetNewFacultyViewModel();
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult NewFieldOfStudy()
        {
            var model = _adminBusinessLogic.GetNewFieldOfStudyViewModel();
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult NewStudySubject()
        {
            var model = _adminBusinessLogic.GetNewStudySubjectViewModel();
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult UniversityAgreement()
        {
            UniversityAgreementViewModel model = _adminBusinessLogic.GetUniversityAgreementViewModel();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult GetTargetAgreementUniversities(int sourceUniversityId)
        {
            List<KeyValueIntStr> model = _adminBusinessLogic.GetTargetAgreementUniversities(sourceUniversityId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult UniversityAgreements()
        {
            UniversityAgreementsViewModel model = _adminBusinessLogic.GetUniversityAgreements();
            return View(model);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        [HttpPost]
        public ActionResult UniversityAgreement(UniversityAgreementViewModel model)
        {
            if (model.SourceUniversityId == 0 || model.TargetUniversityId == 0)
            {
                ModelState.AddModelError("University", "Missing university selection.");
            }
            try
            {
                _adminBusinessLogic.SaveNewUniversityAgreement(model.SourceUniversityId, model.TargetUniversityId);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message);
            }
            return RedirectToActionPermanent("UniversityAgreements");
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult NewUniversity(NewUniversityViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _adminBusinessLogic.CreateUniversity(model);
                    ViewData.Add("LastActionResult", "University created");
                    return RedirectToActionPermanent("Universities");
                }
                return View(model);
            }
            catch (Exception e)
            {
                return Json(new {result = "ERROR", message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult NewStudySubject(NewStudySubjectViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _adminBusinessLogic.CreateStudySubject(model);
                    ViewData.Add("LastActionResult", "Study subject created");
                    return RedirectToActionPermanent("StudySubjects");
                }
                return View(model);
            }
            catch (Exception e)
            {
                return Json(new { result = "ERROR", message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult NewFieldOfStudy(NewFieldOfStudyViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _adminBusinessLogic.CreateNewFieldOfStudy(model);
                    ViewData.Add("LastActionResult", "Field of study created");
                    return RedirectToActionPermanent("FieldsOfStudies");
                }
                return View(model);
            }
            catch (Exception e)
            {
                return Json(new { result = "ERROR", message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult NewFaculty(NewFacultyViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _adminBusinessLogic.CreateNewFaculty(model);
                    ViewData.Add("LastActionResult", "Faculty created");
                    return RedirectToActionPermanent("Faculties");
                }
                return View(model);
            }
            catch (Exception e)
            {
                return Json(new { result = "ERROR", message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = UserRoles.Administrator)]
        [HttpGet]
        public List<FacultyItem> GetFaculties(int universityId)
        {
            return _commonBusinessLogic.GetFaultiesByUniversityId(universityId);
        }

        [Authorize(Roles = UserRoles.UniversityAdmin)]
        public ActionResult PersonalInfo()
        {
            UserViewModel model = _adminBusinessLogic.GetUserViewModelById(User.Identity.GetUserId(), UserRoles.UniversityAdmin);
            return View(model);
        }

        [Authorize(Roles = UserRoles.UniversityAdmin)]
        [HttpPost]
        public ActionResult PersonalInfo(UserViewModel model)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors, (state, error) => error.ErrorMessage).ToList();
            //For now the password field needs to be ignored, if there's only 1 error then it's the password
            if (errors.Count != 1)
            {
                return View(model);
            }

            try
            {
                _adminBusinessLogic.UpdateUser(model, UserRoles.UniversityAdmin, User.Identity.GetUserId());
            }
            catch (FormValidationException e)
            {
                e.ValidationToModelState(ModelState);
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

    }
}