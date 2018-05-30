using System;
using System.Collections.Generic;

namespace ErasmusPlus.Common.SharedModels
{
    public class FormValidationException : Exception
    {
        public Dictionary<string, string> ModelErrors { get; set; }
        public string Error { get; set; }

        public FormValidationException(Dictionary<string, string> errors)
        {
            ModelErrors = errors;
        }

        public FormValidationException(string error)
        {
            Error = error;
        }
    }
}
