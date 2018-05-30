using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ErasmusPlus.Models.Identity;
using ErasmusPlus.Models.ViewModels.Coordinator;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using System.IO;
using ErasmusPlus.Common.SharedModels;
using Newtonsoft.Json;

namespace ErasmusPlus.Models.BLL
{
    public class ForeignBusinessLogic
    {
        internal StudentAgreementsViewModel GetAgreementsListViewModel(string userId)
        {
            var model = new StudentAgreementsViewModel();
            using (var db = new ErasmusDbContext())
            {
                var user = db.Users.Single(x => x.Id == userId);

                //Restricts only by assigned university, otherwise will show all
                if (user.UniversityId != null)
                {
                    model.StudentAgreements = db.Agreements.Where(x => (x.State != AgreementState.Sent || x.State != AgreementState.Received || x.State != AgreementState.Approved) && (x.SourceUniversityId == user.UniversityId || x.TargetUniversityId == user.UniversityId) ).ToList().Select(x => new StudentAgreementListView()
                    {
                        Id = x.Id,
                        ErasmusUser = x.ErasmusUser,
                        SourceUniversity = x.SourceUniversity,
                        TargetUniversity = x.TargetUniversity,
                        EndDate = x.EndDate,
                        State = x.State,
                        StartDate = x.StartDate
                    }).ToList();
                }
                else
                {
                    model.StudentAgreements = db.Agreements.Where(x => x.State != AgreementState.Sent || x.State != AgreementState.Received || x.State != AgreementState.Approved).ToList().Select(x => new StudentAgreementListView()
                    {
                        Id = x.Id,
                        ErasmusUser = x.ErasmusUser,
                        SourceUniversity = x.SourceUniversity,
                        TargetUniversity = x.TargetUniversity,
                        EndDate = x.EndDate,
                        State = x.State,
                        StartDate = x.StartDate
                    }).ToList();
                }
            }
            return model;
        }

        public StudentAgreementDetailsView GetStudentAgreementDetailsView(int id)
        {
            var model = new StudentAgreementDetailsView();
            using (var db = new ErasmusDbContext())
            {
                var agreement = db.Agreements.SingleOrDefault(x => x.Id == id);
                if (agreement == null)
                {
                    return null;
                }
                model.Student = agreement.ErasmusUser;
                model.LanguageLevel = agreement.LanguageLevel;
                model.SourceUniversity = agreement.SourceUniversity;
                model.Semester = agreement.Semester;
                model.EndDate = agreement.EndDate;
                model.TargetUniversity = agreement.TargetUniversity;
                model.StartDate = agreement.StartDate;
                model.Language = agreement.Language;
                model.State = Enum.GetName(typeof(AgreementState), agreement.State);
                model.StoragePath = agreement.StoragePath;
                model.FinancingSource = agreement.FinancingSource;
                model.Scholarship = agreement.Scholarship;
                model.TargetStudySubjects = agreement.TargetSubjects.ToList();
                model.TargetFaculty = agreement.TargetFaculty;
                model.SourceFieldOfStudy = agreement.SourceFieldOfStudy;
                model.TargetFieldOfStudy = agreement.TargetFieldOfStudy;
                model.SourceStudySubjects = agreement.SourceSubjects.ToList();
                model.SourceFaculty = agreement.SourceFaculty;
            }
            return model;

            //NOTE for use with Azure storage account
            //var model = new StudentAgreementDetailsView();
            //using (var db = new ErasmusDbContext())
            //{
            //    var agreement = db.Agreements.SingleOrDefault(x => x.Id == id);
            //    if (agreement == null) throw new ValidationException("Agreement not found.");
            //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            //    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //    CloudBlobContainer container = blobClient.GetContainerReference("erasmus");
            //    var blob = container.ServiceClient.GetBlobReferenceFromServer(new Uri(agreement.StoragePath));
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        blob.DownloadToStream(memoryStream);
            //        var text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            //        var blobData = JsonConvert.DeserializeObject<AgreementStorage>(text);
            //        model = new StudentAgreementDetailsView()
            //        {
            //            Student = blobData.Student,
            //            LanguageLevel = blobData.LanguageLevel,
            //            SourceUniversity = blobData.SourceUniversity,
            //            Semester = blobData.Semester,
            //            EndDate = blobData.EndDate,
            //            TargetUniversity = blobData.TargetUniversity,
            //            StartDate = blobData.StartDate,
            //            Language = blobData.Language,
            //            State = blobData.State,
            //            StoragePath = blobData.StoragePath,
            //            FinancingSource = blobData.FinancingSource,
            //            Scholarship = blobData.Scholarship,
            //            TargetStudySubjects = blobData.TargetStudySubjects,
            //            TargetFaculty = blobData.TargetFaculty,
            //            SourceFieldOfStudy = blobData.SourceFieldOfStudy,
            //            TargetFieldOfStudy = blobData.TargetFieldOfStudy,
            //            SourceStudySubjects = blobData.SourceStudySubjects,
            //            SourceFaculty = blobData.SourceFaculty
            //        };
            //    }
            //}
            //return model;
        }

        public void ChangeState(int id, AgreementState state, string reason = null)
        {
            using (var db = new ErasmusDbContext())
            {
                var agreement = db.Agreements.SingleOrDefault(x => x.Id == id);
                if (agreement == null)
                {
                    throw new FormValidationException("Agreement not found");
                }

                agreement.State = state;
                if (reason != null)
                {
                    agreement.DeclineReason = reason;
                }
                db.SaveChanges();
            }
        }
    }
}