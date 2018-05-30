using System;
using System.Collections.Generic;
using ErasmusPlus.Common.Database;
using ErasmusPlus.Models.Database;
using Newtonsoft.Json;

namespace ErasmusPlus.Common.SharedModels
{
    [Serializable]
    public class AgreementStorage
    {
        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public University SourceUniversity { get; set; }

        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public University TargetUniversity { get; set; }

        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public Faculty SourceFaculty { get; set; }

        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public Faculty TargetFaculty { get; set; }

        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public FieldOfStudy SourceFieldOfStudy { get; set; }

        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public FieldOfStudy TargetFieldOfStudy { get; set; }

        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public List<StudySubjects> SourceStudySubjects { get; set; }

        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public List<StudySubjects> TargetStudySubjects { get; set; }

        public int Semester { get; set; }
        public string Language { get; set; }
        public LanguageLevel LanguageLevel { get; set; }
        public decimal Scholarship { get; set; }

        public string FinancingSource { get; set; }
        public string StoragePath { get; set; }
        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public ErasmusUser Student { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string State { get; set; }
    }
}