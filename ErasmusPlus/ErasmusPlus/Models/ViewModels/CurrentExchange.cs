using System;
using System.Collections.Generic;
using ErasmusPlus.Common.SharedModels;

namespace ErasmusPlus.Models.ViewModels
{
    public class CurrentExchanges
    {
        public void CurrentExchange()
        {
            Exchanges = new List<CurrentExchange>();
        }

        public string CountryTitle { get; set; }
        public string SourceUniversityTitle { get; set; }
        public string TargetUniversityTitle { get; set; }
        public string StudentTitle { get; set; }
        public string FromTitle { get; set; }
        public string ForeseenReturnDateTitle { get; set; }

        public List<CurrentExchange> Exchanges { get; set; }

    }

    public class CurrentExchange
    {
        public string Country { get; set; }
        public string SourceUniversity { get; set; }
        public string TargetUniversity { get; set; }
        public string Student { get; set; }
        public DateTime From { get; set; }
        public DateTime ForeseenReturnDate { get; set; }
        public AgreementState AgreementState { get; set; }
    }
}