using System;
using System.Linq;
using ErasmusPlus.Common.Authorization;
using ErasmusPlus.Common.SharedModels;
using ErasmusPlus.Models;
using ErasmusPlus.Models.BLL;
using ErasmusPlus.Models.ViewModels.Admin;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using ErasmusPlus.Common.Database;
using ErasmusPlus.Models.Extensions;
using System.Collections.Generic;
using ErasmusPlus.Models.ViewModels.Student;
using System.Security.Claims;
using System.Web;
using Microsoft.Owin.Security;

namespace ErasmusPlus.Controllers
{
    public class UniAdminController : Controller
    {
        private static AdminBusinessLogic _adminBusinessLogic;
        private static UniAdminBusinessLogic _uniAdminBusinessLogic;
        private static CommonBusinessLogic _commonBusinessLogic;

        public UniAdminController()
        {
            if (_adminBusinessLogic == null)
            {
                _adminBusinessLogic = new AdminBusinessLogic();
            }

            if (_commonBusinessLogic == null)
            {
                _commonBusinessLogic = new CommonBusinessLogic();
            }

            if (_uniAdminBusinessLogic == null)
            {
                _uniAdminBusinessLogic = new UniAdminBusinessLogic();
            }
        }

        [Authorize(Roles = UserRoles.UniversityAdmin)]
        public ActionResult UserPermissions()
        {
            var model = _adminBusinessLogic.GetUserPermissionsModel(User.Identity.GetUserId());
            return View(model);
        }

        [Authorize(Roles = UserRoles.UniversityAdmin)]
        public ActionResult EditPermissions(string userId)
        {
            var model = _adminBusinessLogic.GetUserPermissionsForUser(userId);
            if (model == null)
            {
                return View("UserError", new UserError("Invalid user id provided"));
            }
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        [HttpPost]
        public ActionResult NewCoordinator(NewUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _uniAdminBusinessLogic.CreateNewUser(user, UserRoles.Coordinator, User.Identity.GetUserId());
                }
                catch (FormValidationException e)
                {
                    e.ValidationToModelState(ModelState);
                    return View(user);
                }

                return RedirectToAction("ForeighnAndCoordinator");
            }
            return View(user);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult NewCoordinator()
        {
            var model = new NewUserViewModel();
            model.Birthday = DateTime.Today;
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult EditCoordinator(string userId)
        {
            NewUserViewModel model = _uniAdminBusinessLogic.GetUserViewModelById(userId, UserRoles.Coordinator);
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
                return View(model);
            }

            try
            {
                _uniAdminBusinessLogic.UpdateCoordinator(model, UserRoles.Coordinator, User.Identity.GetUserId());
            }
            catch (FormValidationException e)
            {
                e.ValidationToModelState(ModelState);
                return View(model);
            }

            return RedirectToAction("ForeighnAndCoordinator");
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult DeleteCoordinator(string userId)
        {
            _uniAdminBusinessLogic.DeleteUser(userId, UserRoles.Coordinator);
            return RedirectToAction("ForeighnAndCoordinator");
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        [HttpPost]
        public ActionResult NewForeign(NewUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _uniAdminBusinessLogic.CreateNewUser(user, UserRoles.Foreign, User.Identity.GetUserId());
                }
                catch (FormValidationException e)
                {
                    e.ValidationToModelState(ModelState);
                    return View(user);
                }

                return RedirectToAction("ForeighnAndCoordinator");
            }
            return View(user);
        }


        [Authorize(Roles = UserRoles.UniversityAdmin)]
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
        public ActionResult NewForeign()
        {
            var model = new NewUserViewModel();
            model.Birthday = DateTime.Today;
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult EditForeign(string userId)
        {
            NewUserViewModel model = _uniAdminBusinessLogic.GetUserViewModelById(userId, UserRoles.Foreign);
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
                _uniAdminBusinessLogic.UpdateUser(model, UserRoles.Foreign);
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
            _uniAdminBusinessLogic.DeleteUser(userId, UserRoles.Foreign);
            return RedirectToAction("ForeighnAndCoordinator");
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult ForeighnAndCoordinator()
        {
            //User.Identity.GetUserId()

            var model = new List<UserViewModel>();
            var coordinators = _uniAdminBusinessLogic.GetUsersListByRole(UserRoles.Coordinator, User.Identity.GetUserId());
            var foreign = _uniAdminBusinessLogic.GetUsersListByRole(UserRoles.Foreign, User.Identity.GetUserId());

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
        ////UNISTUFF////

        [Authorize(Roles = UserRoles.UniversityAdmin)]
        public ActionResult Universities()
        {
            UniversitiesViewModel model = _uniAdminBusinessLogic.GetUniversitiesViewModel(User.Identity.GetUserId());
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult Faculties()
        {
            FacultiesViewModel model = _uniAdminBusinessLogic.GetFacultiesViewModel(User.Identity.GetUserUniversityId().ToIntNullable());
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult FieldsOfStudies()
        {
            FieldsOfStudiesViewModel model = _uniAdminBusinessLogic.GetFieldsOfStudiesViewModel(User.Identity.GetUserUniversityId().ToIntNullable());
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult StudySubjects()
        {
            var model = _uniAdminBusinessLogic.GetStudySubjectsViewModel(User.Identity.GetUserUniversityId().ToIntNullable());
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult NewUniversity()
        {
            var model = _adminBusinessLogic.GetNewUniversityViewModel();
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult NewFaculty()
        {
            var model = _uniAdminBusinessLogic.GetNewFacultyViewModel(User.Identity.GetUserUniversityId().ToIntNullable());
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult EditFaculty(int id)
        {
            var model = _uniAdminBusinessLogic.GetNewFacultyViewModel(User.Identity.GetUserUniversityId().ToIntNullable(), id);
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult NewFieldOfStudy()
        {
            var model = _uniAdminBusinessLogic.GetNewFieldOfStudyViewModel(User.Identity.GetUserUniversityId().ToIntNullable());
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult EditFieldOfStudy(int id)
        {
            var model = _uniAdminBusinessLogic.GetNewFieldOfStudyViewModel(User.Identity.GetUserUniversityId().ToIntNullable(), id);
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult NewStudySubject()
        {
            var model = _uniAdminBusinessLogic.GetNewStudySubjectViewModel(User.Identity.GetUserUniversityId().ToIntNullable());
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult EditStudySubject(int id)
        {
            var model = _uniAdminBusinessLogic.GetNewStudySubjectViewModel(User.Identity.GetUserUniversityId().ToIntNullable(), id);
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult UniversityAgreement()
        {
            UniversityAgreementViewModel model = _uniAdminBusinessLogic.GetUniversityAgreementViewModel(User.Identity.GetUserId());
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult GetTargetAgreementUniversities(int sourceUniversityId)
        {
            List<KeyValueIntStr> model = _adminBusinessLogic.GetTargetAgreementUniversities(sourceUniversityId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult UniversityAgreements()
        {
            UniversityAgreementsViewModel model = _uniAdminBusinessLogic.GetUniversityAgreements(User.Identity.GetUserId());
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        [HttpPost]
        public ActionResult UniversityAgreement(UniversityAgreementViewModel model)
        {
            if (model.TargetUniversityId == 0)
            {
                ModelState.AddModelError("University", "Missing university selection.");
            }
            try
            {
                _uniAdminBusinessLogic.SaveNewUniversityAgreement(User.Identity.GetUserId(), model.TargetUniversityId);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message);
            }
            return RedirectToActionPermanent("UniversityAgreements");
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult NewUniversity(NewUniversityViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var uniId = _uniAdminBusinessLogic.CreateUniversity(model, User.Identity.GetUserId());
                    ViewData.Add("LastActionResult", "University created");

                    var Identity = User.Identity as ClaimsIdentity;
                    Identity.RemoveClaim(Identity.FindFirst("UniversityId"));
                    Identity.AddClaim(new Claim("UniversityId", uniId.ToString()));
                    var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                    authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(Identity), new AuthenticationProperties() {IsPersistent = true});

                    return RedirectToActionPermanent("Universities");
                }
                return View(model);
            }
            catch (Exception e)
            {
                return Json(new { result = "ERROR", message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult NewStudySubject(NewStudySubjectViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _uniAdminBusinessLogic.CreateStudySubject(model, User.Identity.GetUserUniversityId().ToIntNullable());
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
        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult NewFieldOfStudy(NewFieldOfStudyViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _uniAdminBusinessLogic.CreateNewFieldOfStudy(model, User.Identity.GetUserUniversityId().ToIntNullable());
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
        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        public ActionResult NewFaculty(NewFacultyViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _uniAdminBusinessLogic.CreateNewFaculty(model, User.Identity.GetUserUniversityId().ToIntNullable());
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

        [Authorize(Roles = UserRoles.AdminAndUniAdmin)]
        [HttpGet]
        public List<FacultyItem> GetFaculties(int universityId)
        {
            return _commonBusinessLogic.GetFaultiesByUniversityId(universityId);
        }

        [Authorize(Roles = UserRoles.UniversityAdmin)]
        public ActionResult PersonalInfo()
        {
            UserViewModel model = _uniAdminBusinessLogic.NewGetUserViewModelById(User.Identity.GetUserId(), UserRoles.UniversityAdmin);
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