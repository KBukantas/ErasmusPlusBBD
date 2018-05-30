using System;
using ErasmusPlus.Common.SharedModels;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;
using System.Linq;

namespace ErasmusPlus.Models.Extensions
{
    public static class Extenssions
    {
        public static string GetClaimData(this IIdentity identity, string claimName)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(claimName);
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserUniversityId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("UniversityId");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static void ValidationToModelState(this FormValidationException exception, ModelStateDictionary modelState)
        {
            if (exception.ModelErrors.Any())
            {
                foreach (var formError in exception.ModelErrors)
                {
                    modelState.AddModelError(formError.Key, formError.Value);
                }
            }
        }

        public static int? ToIntNullable(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            else
            {
                try
                {
                    return Convert.ToInt32(str);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}