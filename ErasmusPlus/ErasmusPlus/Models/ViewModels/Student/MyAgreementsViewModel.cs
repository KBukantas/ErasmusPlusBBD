using ErasmusPlus.Common.SharedModels;
using System;

namespace ErasmusPlus.Models.ViewModels.Student
{
    public class MyAgreementsViewModel
    {
        public int Id { get; set; }
        public AgreementState State { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UniversityFromName { get; set; }
        public string UniversityToName { get; set; }
    }
}