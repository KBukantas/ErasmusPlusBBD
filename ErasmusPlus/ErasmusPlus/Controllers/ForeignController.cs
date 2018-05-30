using ErasmusPlus.Models.BLL;
using System.Web.Mvc;
using ErasmusPlus.Common.Authorization;
using ErasmusPlus.Models.ViewModels.Coordinator;
using Microsoft.AspNet.Identity;
using ErasmusPlus.Common.SharedModels;
using ErasmusPlus.Models;
using ErasmusPlus.Models.Extensions;
using ErasmusPlus.Models.ViewModels.Admin;
using System.Linq;
using System;

namespace ErasmusPlus.Controllers
{
    [Authorize]
    public class ForeignController : Controller
    {
        private static ForeignBusinessLogic _foreignBusinessLogic;
        private static AdminBusinessLogic _adminBusinessLogic;
        private static CommonBusinessLogic _commonBusinessLogic;

        public ForeignController()
        {
            if (_adminBusinessLogic == null)
            {
                _adminBusinessLogic = new AdminBusinessLogic();
            }

            if (_commonBusinessLogic == null)
            {
                _commonBusinessLogic = new CommonBusinessLogic();
            }

            if (_foreignBusinessLogic == null)
            {
                _foreignBusinessLogic = new ForeignBusinessLogic();
            }
        }

        [Authorize(Roles = UserRoles.Foreign)]
        public ActionResult Agreements()
        {
            StudentAgreementsViewModel model = _foreignBusinessLogic.GetAgreementsListViewModel(User.Identity.GetUserId());
            return View(model);
        }

        [Authorize(Roles = UserRoles.Foreign)]
        public ActionResult StudentAgreementDetails(int id)
        {
            try
            {
                StudentAgreementDetailsView model = _foreignBusinessLogic.GetStudentAgreementDetailsView(id);
                return View(model);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Foreign", "StudentAgreementDetails"));
            }
        }

        [Authorize(Roles = UserRoles.Foreign)]
        public ActionResult PersonalInfo()
        {
            UserViewModel model = _adminBusinessLogic.GetUserViewModelById(User.Identity.GetUserId(), UserRoles.Foreign);
            return View(model);
        }

        [Authorize(Roles = UserRoles.Foreign)]
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
                _adminBusinessLogic.UpdateUser(model, UserRoles.Foreign, User.Identity.GetUserId());
            }
            catch (FormValidationException e)
            {
                e.ValidationToModelState(ModelState);
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }


        [Authorize(Roles = UserRoles.Foreign)]
        public ActionResult Messages()
        {
            return View();
        }

        [Authorize(Roles = UserRoles.Foreign)]
        [HttpPost]
        public ActionResult Decline(int id, string reason)
        {
            //TODO should check permissions
            try
            {
                _foreignBusinessLogic.ChangeState(id, AgreementState.Declined, reason);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }

        [Authorize(Roles = UserRoles.Foreign)]
        public ActionResult Canceled(int id)
        {
            //TODO should check permissions
            try
            {
                _foreignBusinessLogic.ChangeState(id, AgreementState.Canceled);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }

        [Authorize(Roles = UserRoles.Foreign)]
        public ActionResult Received(int id)
        {
            //TODO should check permissions
            try
            {
                _foreignBusinessLogic.ChangeState(id, AgreementState.Received);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }

        [Authorize(Roles = UserRoles.Foreign)]
        public ActionResult Approved(int id)
        {
            //TODO should check permissions
            try
            {
                _foreignBusinessLogic.ChangeState(id, AgreementState.Approved);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }

        [Authorize(Roles = UserRoles.Foreign)]
        public ActionResult Active(int id)
        {
            //TODO should check permissions
            try
            {
                _foreignBusinessLogic.ChangeState(id, AgreementState.Active);
            }
            catch (FormValidationException e)
            {
                return View("UserError", new UserError(e.Error));
            }
            return RedirectToAction("Agreements");
        }
    }
}