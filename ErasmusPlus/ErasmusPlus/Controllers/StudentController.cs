using System;
using ErasmusPlus.Common.Authorization;
using ErasmusPlus.Models.BLL;
using ErasmusPlus.Models.ViewModels.Student;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ErasmusPlus.Models.Extensions;
using Microsoft.AspNet.Identity;
using ErasmusPlus.App_Start;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.ComponentModel.DataAnnotations;
using ErasmusPlus.Models.ViewModels.Admin;
using ErasmusPlus.Common.SharedModels;
using ErasmusPlus.Models;
using ErasmusPlus.Models.ViewModels.Coordinator;

namespace ErasmusPlus.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private static CommonBusinessLogic _commonBusinessLogic;
        private static StudentBusinessLogic _studentBusinessLogic;
        private static AdminBusinessLogic _adminBusinessLogic;
        public StudentController()
        {
            if (_commonBusinessLogic == null)
            {
                _commonBusinessLogic = new CommonBusinessLogic();
            }

            if (_studentBusinessLogic == null)
            {
                _studentBusinessLogic = new StudentBusinessLogic();
            }

            if (_adminBusinessLogic == null)
            {
                _adminBusinessLogic = new AdminBusinessLogic();
            }
        }


        [Authorize(Roles = UserRoles.Student)]
        public ActionResult StudentAgreementDetails(int id)
        {
            try
            {
                StudentAgreementDetailsView model = _studentBusinessLogic.GetStudentAgreementDetailsView(User.Identity.GetUserId() ,id);
                if (model == null)
                {
                    return View("UserError", new UserError("Unable to retrieve the agreement"));
                }
                return View(model);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Student", "StudentAgreementDetails"));
            }

        }

        [Authorize(Roles = UserRoles.Student)]
        [HttpGet]
        public ActionResult GetSourceUniData(int universityId)
        {
            if (universityId == 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var data = _studentBusinessLogic.GetSourceUniversityData(universityId, User.Identity.GetUserId());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Student)]
        [HttpGet]
        public ActionResult GetFacultyDataByUniId(int universityId)
        {
            if (universityId == 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var data = _studentBusinessLogic.GetFacultyDataByUniversityId(universityId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Student)]
        [HttpGet]
        public ActionResult GetFosDataByFacultyId(int facultyid)
        {
            if (facultyid == 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var data = _studentBusinessLogic.GetFosDataByFacultyId(facultyid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Student)]
        [HttpGet]
        public ActionResult GetStudySubjectsByFosId(int fosId)
        {
            if (fosId == 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var data = _studentBusinessLogic.GetStudySubjectsByFosId(fosId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Student)]
        [HttpGet]
        public ActionResult GetStudySubjects(int sourceFosId, int targetFosId, int semester)
        {
            if (sourceFosId == 0 || targetFosId == 0 || semester == 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var data = _studentBusinessLogic.GetFieldsOfStudies(sourceFosId, targetFosId, semester);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Student)]
        [HttpGet]
        public ActionResult GetSemesterData(int sourceFosId, int targetFosId)
        {
            if (sourceFosId == 0 || targetFosId == 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var data = _studentBusinessLogic.GetSemesterData(sourceFosId, targetFosId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Student)]
        public ActionResult MyAgreements()
        {
            var model = _studentBusinessLogic.GetMyAgreements(User.Identity.GetUserId());
            return View(model);
        }

        [Authorize(Roles = UserRoles.Student)]
        public ActionResult PersonalInfo()
        {
            UserViewModel model = _adminBusinessLogic.GetUserViewModelById(User.Identity.GetUserId(), UserRoles.Student);
            return View(model);
        }

        [Authorize(Roles = UserRoles.Student)]
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
                _adminBusinessLogic.UpdateUser(model, UserRoles.Student, User.Identity.GetUserId());
            }
            catch (FormValidationException e)
            {
                e.ValidationToModelState(ModelState);
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }


        [Authorize(Roles = UserRoles.Student)]
        public ActionResult Messages()
        {
            return View();
        }


        [Authorize(Roles = UserRoles.Student)]
        public ActionResult SubmitAgreement(int id)
        {
            try
            {
                _studentBusinessLogic.SubmitAgreement(User.Identity.GetUserId(), id);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("MyAgreements");
        }


        [Authorize(Roles = UserRoles.Student)]
        public ActionResult DeleteAgreement(int id)
        {
            try
            {
                _studentBusinessLogic.DeleteAgreement(User.Identity.GetUserId(), id);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("MyAgreements");
        }


        [Authorize(Roles = UserRoles.Student)]
        public ActionResult Agreement()
        {
            return View(_studentBusinessLogic.GetStudentAgreementViewModel(User.Identity.GetUserId()));
        }

        [Authorize(Roles = UserRoles.Student)]
        public ActionResult EditAgreement(int id)
        {
            var model = _studentBusinessLogic.GetStudentAgreementViewModel(User.Identity.GetUserId(), id);
            if (model == null)
            {
                return View("UserError", new UserError("We were unable to retrieve the agreement data, most likely the universities have changed their details. Please delete the agreement and create a new one."));
            }
            ViewBag.IsEdit = true;
            return View("Agreement", model);
        }

        [Authorize(Roles = UserRoles.Student)]
        [HttpPost]
        public ActionResult Agreement(AgreementViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Bit of bad practice, should be refactored since we're consuming resources with each request
                    var user = System.Web.HttpContext.Current.GetOwinContext()
                        .GetUserManager<ApplicationUserManager>()
                        .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                    if (user == null) throw new UnauthorizedAccessException("Unauthorized");
                    _studentBusinessLogic.ValidateAndSaveStudentAgreement(user, model);
                }
                else
                {
                    return Json(new { type = "error", message = ModelState.Values.Where(x => x.Errors.Any()).FirstOrDefault().Errors.FirstOrDefault().ErrorMessage},
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (ValidationException e)
            {
                return Json(new {type = "error", message = e.Message}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { type = "error", message = e.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new {type = "success", message = "Agreement saved successfully."}, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Student)]
        [HttpGet]
        public List<FacultyItem> GetFaculties(int universityId)
        {
            return _commonBusinessLogic.GetFaultiesByUniversityId(universityId);
        }
    }
}