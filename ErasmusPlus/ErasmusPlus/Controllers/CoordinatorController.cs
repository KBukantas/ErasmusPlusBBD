using System;
using System.Collections.Generic;
using ErasmusPlus.Common.Authorization;
using ErasmusPlus.Models.BLL;
using ErasmusPlus.Models.ViewModels.Coordinator;
using System.Web.Mvc;
using ErasmusPlus.Models.ViewModels.Admin;
using System.Linq;
using ErasmusPlus.Common.Database;
using ErasmusPlus.Common.SharedModels;
using ErasmusPlus.Models.Extensions;
using Microsoft.AspNet.Identity;
using ErasmusPlus.Models;

namespace ErasmusPlus.Controllers
{
    [Authorize]
    public class CoordinatorController : Controller
    {
        private static CoordinatorBusinessLogic _coordinatorBusinessLogic;
        private static AdminBusinessLogic _adminBusinessLogic;
        private static CommonBusinessLogic _commonBusinessLogic;

        public CoordinatorController()
        {
            if (_adminBusinessLogic == null)
            {
                _adminBusinessLogic = new AdminBusinessLogic();
            }

            if (_commonBusinessLogic == null)
            {
                _commonBusinessLogic = new CommonBusinessLogic();
            }

            if (_coordinatorBusinessLogic == null)
            {
                _coordinatorBusinessLogic = new CoordinatorBusinessLogic();
            }
        }

        [Authorize(Roles = UserRoles.Coordinator)]
        public ActionResult Pending(int id)
        {
            //TODO should check if the coordinator can edit this particular agreement for now can edit all (pass user id to BLL, get list of universities for that coordinator, validate against agreement)
            try
            {
                _coordinatorBusinessLogic.ChangeState(id, AgreementState.Pending);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }

        [Authorize(Roles = UserRoles.Coordinator)]
        public ActionResult Active(int id)
        {
            //TODO should check if the coordinator can edit this particular agreement for now can edit all (pass user id to BLL, get list of universities for that coordinator, validate against agreement)
            try
            {
                _coordinatorBusinessLogic.ChangeState(id, AgreementState.Active);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }

        [Authorize(Roles = UserRoles.Coordinator)]
        public ActionResult Sent(int id)
        {
            //TODO should check if the coordinator can edit this particular agreement for now can edit all (pass user id to BLL, get list of universities for that coordinator, validate against agreement)
            try
            {
                _coordinatorBusinessLogic.ChangeState(id, AgreementState.Sent);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }

        [Authorize(Roles = UserRoles.Coordinator)]
        public ActionResult Approved(int id)
        {
            //TODO should check if the coordinator can edit this particular agreement for now can edit all (pass user id to BLL, get list of universities for that coordinator, validate against agreement)
            try
            {
                _coordinatorBusinessLogic.ChangeState(id, AgreementState.Approved);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }

        [Authorize(Roles = UserRoles.Coordinator)]
        public ActionResult Canceled(int id)
        {
            //TODO should check if the coordinator can edit this particular agreement for now can edit all (pass user id to BLL, get list of universities for that coordinator, validate against agreement)
            try
            {
                _coordinatorBusinessLogic.ChangeState(id, AgreementState.Canceled);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }

        [Authorize(Roles = UserRoles.Coordinator)]
        public ActionResult Close(int id)
        {
            //TODO should check if the coordinator can edit this particular agreement for now can edit all (pass user id to BLL, get list of universities for that coordinator, validate against agreement)
            try
            {
                _coordinatorBusinessLogic.ChangeState(id, AgreementState.Closed);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }

        [Authorize(Roles = UserRoles.Coordinator)]
        [HttpPost]
        public ActionResult Decline(int id, string reason)
        {
            //TODO should check if the coordinator can edit this particular agreement for now can edit all (pass user id to BLL, get list of universities for that coordinator, validate against agreement)
            try
            {
                _coordinatorBusinessLogic.ChangeState(id, AgreementState.Declined, reason);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }

        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        public ActionResult Agreements()
        {
            StudentAgreementsViewModel model = _coordinatorBusinessLogic.GetAgreementsListViewModel(User.Identity.GetUserId());
            return View(model);
        }
        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        public ActionResult IncomingAgreements()
        {
            StudentAgreementsViewModel model = _coordinatorBusinessLogic.GetIncomingAgreementsListViewModel(User.Identity.GetUserId());
            return View(model);
        }
        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        public ActionResult OutgoingAgreements()
        {
            StudentAgreementsViewModel model = _coordinatorBusinessLogic.GetOutgoingAgreementsListViewModel(User.Identity.GetUserId());
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        public ActionResult StudentAgreementDetails(int id)
        {
            try
            {
                StudentAgreementDetailsView model = _coordinatorBusinessLogic.GetStudentAgreementDetailsView(id);
                return View(model);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Coordinator", "StudentAgreementDetails"));
            }
        }

        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        public ActionResult Students()
        {

            var model = new List<UserViewModel>();
           // var userIds = role.Users.Select(x => x.UserId).ToList();
            var students = _coordinatorBusinessLogic.GetUsersListByRole(UserRoles.Student, User.Identity.GetUserId());
            model.AddRange(students.Select(x => new UserViewModel()
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
                UserId = x.Id
            }));
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        public ActionResult EditStudent(string userId)
        {
            UserViewModel model = _adminBusinessLogic.GetUserViewModelById(userId, UserRoles.Student);
            if (model != null)
            {
                var universities = _commonBusinessLogic.GetUniversitiesList();
                universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = model.UniversityId == x.Id }), "Value", "Text");
            }

            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        [HttpPost]
        public ActionResult EditStudent(UserViewModel model)
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
                _adminBusinessLogic.UpdateUser(model, UserRoles.Student);
            }
            catch (FormValidationException e)
            {
                e.ValidationToModelState(ModelState);
                var universities = _commonBusinessLogic.GetUniversitiesList();
                universities.Insert(0, new University() { Name = "Select university", Id = 0 });
                model.Universities = new SelectList(universities.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = model.UniversityId == x.Id }), "Value", "Text");
                return View(model);
            }

            return RedirectToAction("Students");
        }

        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        public ActionResult DeleteStudent(string userId)
        {
            _coordinatorBusinessLogic.DeleteUser(userId, UserRoles.Student);
            return RedirectToAction("Students");
        }

        [Authorize(Roles = UserRoles.Coordinator)]
        public ActionResult PersonalInfo()
        {
            UserViewModel model = _adminBusinessLogic.GetUserViewModelById(User.Identity.GetUserId(), UserRoles.Coordinator);
            return View(model);
        }

        [Authorize(Roles = UserRoles.Coordinator)]
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
                _adminBusinessLogic.UpdateUser(model, UserRoles.Coordinator, User.Identity.GetUserId());
            }
            catch (FormValidationException e)
            {
                e.ValidationToModelState(ModelState);
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        public ActionResult NewStudent()
        {
            var model = new NewUserViewModel();
            model.Birthday = DateTime.Today;
            return View(model);
        }

        [Authorize(Roles = UserRoles.AdminAndCoordinator)]
        [HttpPost]
        public ActionResult NewStudent(NewUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _coordinatorBusinessLogic.CreateNewUser(user, UserRoles.Student, User.Identity.GetUserId());
                }
                catch (FormValidationException e)
                {
                    e.ValidationToModelState(ModelState);
                    return View(user);
                }

                return RedirectToAction("Students");
            }
            return View(user);
        }
        [Authorize(Roles = UserRoles.Coordinator)]
        public ActionResult EditPermissions(string userId)
        {
            var model = _adminBusinessLogic.GetUserPermissionsForUser(userId);
            if (model == null)
            {
                return View("UserError", new UserError("Invalid user id provided"));
            }
            return View(model);
        }
        [Authorize(Roles = UserRoles.Coordinator)]
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

        [Authorize(Roles = UserRoles.Coordinator)]
        public ActionResult Received(int id)
        {
            //TODO should check permissions
            try
            {
                _coordinatorBusinessLogic.ChangeState(id, AgreementState.Received);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }
    }
}