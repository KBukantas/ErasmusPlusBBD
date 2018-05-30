using ErasmusPlus.Models.Identity;
using ErasmusPlus.Models.ViewModels.Student;
using System.Linq;
using System.Web.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using ErasmusPlus.Common.Database;
using ErasmusPlus.Common.SharedModels;
using ErasmusPlus.Models.Database;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Data.Entity;
using ErasmusPlus.Models.ViewModels.Coordinator;

namespace ErasmusPlus.Models.BLL
{
    public class StudentBusinessLogic
    {
        public AgreementViewModel GetStudentAgreementViewModel(string userId)
        {
            var model = new AgreementViewModel();
            using (var db = new ErasmusDbContext())
            {
                var user = db.Users.SingleOrDefault(x => x.Id == userId);
                model.SourceUniversities = new SelectList(new List<SelectListItem>()
                {
                    new SelectListItem()
                    {
                        Text = user.University.Name,
                        Value = user.University.Id.ToString()
                    }
                }, "Value", "Text");
            }
            return model;
        }

        public AgreementViewModel GetStudentAgreementViewModel(string userId, int agreementId)
        {
            try
            {
                using (var db = new ErasmusDbContext())
                {
                    var agreement = db.Agreements.Where(x => x.Id == agreementId).Include(x => x.SourceUniversity)
                        .Include(x => x.SourceUniversity.Faculties).Include(x => x.TargetUniversity)
                        .Include(x => x.TargetUniversity.Faculties).SingleOrDefault();

                    if (agreement == null)
                    {
                        throw new FormValidationException("Agreement not found");
                    }
                    if (agreement.ErasmusUserId != userId)
                    {
                        throw new FormValidationException("You are not allowed to view this agreement");
                    }
                    if (agreement.State != AgreementState.New)
                    {
                        throw new FormValidationException("Only allowed to edit agreements that are new");
                    }

                    var model = new AgreementViewModel()
                    {
                        From = agreement.StartDate,
                        To = agreement.EndDate,
                        Language = agreement.Language,
                        LanguageLevel = agreement.LanguageLevel,
                        SelectedSemester = agreement.Semester,
                        SelectedSourceUniversity = agreement.SourceUniversityId,
                        SelectedTargetUniversity = agreement.TargetUniversityId,
                        SelectedSourceFaculty = agreement.SourceFacultyId,
                        SelectedTargetFaculty = agreement.TargetFacultyId,
                        SelectedSourceFieldOfStudy = agreement.SourceFieldOfStudyId,
                        SelectedTargetFieldOfStudy = agreement.TargetFieldOfStudyId,
                        SelectedSourceStudySubjects = agreement.SourceSubjects.Select(x => x.Id).ToList(),
                        SelectedTargetStudySubjects = agreement.TargetSubjects.Select(x => x.Id).ToList(),
                    };

                    var universities = db.Universities.ToList();
                    model.SourceUniversities = new SelectList(universities.Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                        Selected = model.SelectedSourceUniversity == x.Id
                    }).ToList(), "Value", "Text");

                    var targetUniversities = db.UniversityAgreements
                        .Where(x => x.SourceUniversityId == model.SelectedSourceUniversity)
                        .Select(x => x.TargetUniversity).ToList();
                    model.TargetUniversities = new SelectList(targetUniversities.Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                        Selected = model.SelectedTargetUniversity == x.Id
                    }), "Value", "Text");

                    model.SourceFaculties = new SelectList(agreement.SourceUniversity.Faculties.Select(x =>
                        new SelectListItem()
                        {
                            Text = x.Name,
                            Value = x.Id.ToString(),
                            Selected = model.SelectedSourceFaculty == x.Id
                        }), "Value", "Text");

                    model.TargetFaculties = new SelectList(agreement.TargetUniversity.Faculties.Select(x =>
                        new SelectListItem()
                        {
                            Text = x.Name,
                            Value = x.Id.ToString(),
                            Selected = model.SelectedSourceFaculty == x.Id
                        }), "Value", "Text");


                    var sourceStudyFields = agreement.SourceUniversity.Faculties.SingleOrDefault(x => x.Id == model.SelectedSourceFaculty).StudyFields;
                    var selectedSourceField = sourceStudyFields.Single(x => x.Id == model.SelectedSourceFieldOfStudy);
                    model.SourceFieldsOfStudy = new SelectList(sourceStudyFields.Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                        Selected = model.SelectedSourceFieldOfStudy == x.Id
                    }), "Value", "Text");

                    var targetStudyFields = agreement.TargetUniversity.Faculties.SingleOrDefault(x => x.Id == model.SelectedTargetFieldOfStudy).StudyFields;
                    var selectedTargetField = targetStudyFields.Single(x => x.Id == model.SelectedTargetFieldOfStudy);
                    model.TargetFieldsOfStudy = new SelectList(targetStudyFields.Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                        Selected = model.SelectedTargetFieldOfStudy == x.Id
                    }), "Value", "Text");

                    var semesterData = GetSemesterData(selectedSourceField.Id, selectedTargetField.Id);
                    model.Semesters = new SelectList(semesterData.Select(x => new SelectListItem()
                    {
                        Text = x.Value,
                        Value = x.Key.ToString(),
                        Selected = model.SelectedSemester == x.Key
                    }), "Value", "Text");

                    model.SourceStudySubjects = new MultiSelectList(selectedSourceField.StudySubjects.Where(x => x.Semester == model.SelectedSemester).Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                        Selected = model.SelectedSourceStudySubjects.Contains(x.Id)
                    }), "Value", "Text");

                    model.TargetStudySubjects = new MultiSelectList(selectedTargetField.StudySubjects.Where(x => x.Semester == model.SelectedSemester).Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                        Selected = model.SelectedTargetStudySubjects.Contains(x.Id)
                    }), "Value", "Text");



                    return model;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        internal StudentAgreementDetailsView GetStudentAgreementDetailsView(string userId, int id)
        {
            var model = new StudentAgreementDetailsView();
            using (var db = new ErasmusDbContext())
            {
                var agreement = db.Agreements.SingleOrDefault(x => x.Id == id);
                if (agreement == null || agreement.ErasmusUserId != userId)
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
        }

        public object GetSourceUniversityData(int universityId, string userId)
        {
            var data = new UniversityDataModel();
            using (var db = new ErasmusDbContext())
            {
                var user = db.Users.SingleOrDefault(x => x.Id == userId);
                //Guard against unexisting/false tries
                var originalUniversity = db.Universities.SingleOrDefault(x => x.Id == universityId);
                if (originalUniversity == null || user.UniversityId != universityId)
                {
                    return data;
                }

                var agreedUnis = db.UniversityAgreements.Where(x => x.SourceUniversityId == universityId)?.ToList();

                if (agreedUnis != null && agreedUnis.Any())
                {
                    var targetUniversitiesIdList = agreedUnis.Select(x => x.TargetUniversityId).ToList();
                    var targetUniversities =
                        db.Universities.Where(x => targetUniversitiesIdList.Contains(x.Id)).ToList();
                    data.TargetUniversities = targetUniversities.Select(x => new KeyValueIntStr()
                    {
                        Key = x.Id,
                        Value = x.Name
                    }).ToList();

                    data.SourceFaculties = originalUniversity.Faculties.Select(x => new KeyValueIntStr()
                    {
                        Key = x.Id,
                        Value = x.Name
                    }).ToList();
                }
            }
            return data;
        }

        public object GetFosDataByFacultyId(int facultyid)
        {
            using (var db = new ErasmusDbContext())
            {
                //Guard against unexisting/false tries
                var faculty = db.Faculties.SingleOrDefault(x => x.Id == facultyid);
                if (faculty == null)
                {
                    return null;
                }

                return faculty.StudyFields.Select(x => new KeyValueIntStr()
                {
                    Key = x.Id,
                    Value = x.Name
                }).ToList();
            }
        }

        public object GetFieldsOfStudies(int sourceFosId, int targetFosId, int semester)
        {
            using (var db = new ErasmusDbContext())
            {
                //Guard against unexisting/false tries
                var sourceFos = db.FieldsOfStudy.SingleOrDefault(x => x.Id == sourceFosId);
                if (sourceFos == null)
                {
                    return null;
                }

                var targetFos = db.FieldsOfStudy.SingleOrDefault(x => x.Id == targetFosId);
                if (targetFos == null)
                {
                    return null;
                }

                return new
                {
                    SourceStudySubjects = db.StudySubjects
                        .Where(x => x.Semester == semester && x.FieldOfStudyId == sourceFosId).Select(x =>
                            new 
                            {
                                Key = x.Id,
                                Value = x.Name,
                                Credits = x.Credits
                            }).ToList(),
                    TargetStudySubjects = db.StudySubjects
                        .Where(x => x.Semester == semester && x.FieldOfStudyId == targetFosId).Select(x =>
                            new 
                            {
                                Key = x.Id,
                                Value = x.Name,
                                Credits = x.Credits
                            }).ToList()
                };
            }
        }

        internal List<MyAgreementsViewModel> GetMyAgreements(string userId)
        {
            var model = new List<MyAgreementsViewModel>();
            using (var db = new ErasmusDbContext())
            {
                var agreements = db.Agreements.Where(x => x.ErasmusUserId == userId).ToList();
                model.AddRange(agreements.Select(x => new MyAgreementsViewModel()
                {
                    Id = x.Id,
                    EndDate = x.EndDate,
                    StartDate = x.StartDate,
                    State = x.State,
                    UniversityFromName = x.SourceUniversity.Name,
                    UniversityToName = x.TargetUniversity.Name
                }));
            }
            return model;
        }

        internal IEnumerable<KeyValueIntStr> GetSemesterData(int sourceFosId, int targetFosId)
        {
            using (var db = new ErasmusDbContext())
            {
                //Guard against unexisting/false tries
                var sourceFos = db.FieldsOfStudy.SingleOrDefault(x => x.Id == sourceFosId);
                if (sourceFos == null)
                {
                    return null;
                }

                var targetFos = db.FieldsOfStudy.SingleOrDefault(x => x.Id == targetFosId);
                if (targetFos == null)
                {
                    return null;
                }

                var semestersForSource = db.StudySubjects.Where(x => x.FieldOfStudyId == sourceFosId).ToList()
                    .Select(x => x.Semester).ToList();

                var semestersForTarget = db.StudySubjects.Where(x => x.FieldOfStudyId == targetFosId).ToList()
                    .Select(x => x.Semester).ToList();

                var matchingSemesters = semestersForSource.Intersect(semestersForTarget).Distinct();
                if (matchingSemesters.Any())
                {
                    var result = matchingSemesters.Select(x => new KeyValueIntStr()
                    {
                        Key = x,
                        Value = x.ToString()
                    });
                    return result;
                }
                else
                { 
                    return null;
                }
            }
        }

        public void DeleteAgreement(string userId, int id)
        {
            using (var db = new ErasmusDbContext())
            {
                var agreement = db.Agreements.SingleOrDefault(x => x.Id == id);
                if (agreement == null)
                {
                    throw new FormValidationException("Agreement not found");
                }
                if (agreement.ErasmusUserId != userId)
                {
                    throw new FormValidationException("Not allowed to change state for this agreement");
                }
                db.Agreements.Remove(agreement);
                db.SaveChanges();
            }
        }

        public void SubmitAgreement(string userId, int id)
        {
            using (var db = new ErasmusDbContext())
            {
                var agreement = db.Agreements.SingleOrDefault(x => x.Id == id);
                if (agreement == null)
                {
                    throw new FormValidationException("Agreement not found");
                }
                if (agreement.ErasmusUserId != userId)
                {
                    throw new FormValidationException("Not allowed to delete this agreement");
                }
                if (agreement.State != AgreementState.New)
                {
                    throw new FormValidationException("Only allowed to submit agreements that are new");
                }
                agreement.State = AgreementState.Submitted;
                db.SaveChanges();
            }
        }

        public void ValidateAndSaveStudentAgreement(ErasmusUser user, AgreementViewModel model)
        {
            using (var db = new ErasmusDbContext())
            {
                #region Validation
                var sourceUniversity = db.Universities.SingleOrDefault(x => x.Id == model.SelectedSourceUniversity);
                var targetUniversity = db.Universities.SingleOrDefault(x => x.Id == model.SelectedTargetUniversity);
                if (sourceUniversity == null) throw new ValidationException("Invalid source university");
                if (targetUniversity == null) throw new ValidationException("Invalid target university");

                var agreement = db.UniversityAgreements.SingleOrDefault(x =>
                    x.SourceUniversityId == sourceUniversity.Id && x.TargetUniversityId == targetUniversity.Id);
                if (agreement == null)
                    throw new ValidationException("Invalid selection, agreement for selected universities not found");

                var sourceFaculty =
                    sourceUniversity.Faculties.SingleOrDefault(x => x.Id == model.SelectedSourceFaculty);
                var targetFaculty =
                    targetUniversity.Faculties.SingleOrDefault(x => x.Id == model.SelectedTargetFaculty);
                if (sourceFaculty == null) throw new ValidationException("Invalid source faculty");
                if (targetFaculty == null) throw new ValidationException("Invalid target faculty");

                var sourceFos =
                    sourceFaculty.StudyFields.SingleOrDefault(x => x.Id == model.SelectedSourceFieldOfStudy);
                var targetFos =
                    targetFaculty.StudyFields.SingleOrDefault(x => x.Id == model.SelectedTargetFieldOfStudy);
                if (sourceFos == null) throw new ValidationException("Invalid source field of study");
                if (targetFos == null) throw new ValidationException("Invalid target field of study");

                var sourceStudySubjects = sourceFos.StudySubjects.Where(x => model.SelectedSourceStudySubjects.Contains(x.Id)).ToList();
                var targetStudySubjects = targetFos.StudySubjects.Where(x => model.SelectedTargetStudySubjects.Contains(x.Id)).ToList();

                var srcSsIds = sourceStudySubjects.Select(x => x.Id).ToList();
                var trgSsIds = targetStudySubjects.Select(x => x.Id).ToList();

               /*if (!srcSsIds.All(model.SelectedSourceStudySubjects.Contains) || srcSsIds.Count != model.SelectedSourceStudySubjects.Count)
                    throw new ValidationException("Invalid source study subjects found");
               if (!trgSsIds.All(model.SelectedTargetStudySubjects.Contains) || trgSsIds.Count != model.SelectedTargetStudySubjects.Count)
                    throw new ValidationException("Invalid target study subjects found");*/

                var sourceCreditCount = sourceStudySubjects.Sum(x => x.Credits);
                var targetCreditCount = targetStudySubjects.Sum(x => x.Credits);
                if (sourceCreditCount != targetCreditCount)
                {
                    throw new ValidationException("Credit count must match");
                }

                if (model.From < DateTime.UtcNow) throw new ValidationException("Date from can't be earlier than today");
                if (model.To < model.From) throw new ValidationException("Date to can't be earlier than date from");

                //TODO add whatever else is needed. Semester, min credit count, language, time range etc.
                #endregion

                var storageModel = new AgreementStorage()
                {
                    Student = user,
                    LanguageLevel = model.LanguageLevel,
                    SourceUniversity = sourceUniversity,
                    TargetUniversity = targetUniversity,
                    Semester = model.SelectedSemester,
                    Language = model.Language,
                    TargetStudySubjects = targetStudySubjects,
                    EndDate = model.To,
                    FinancingSource = "",
                    Scholarship = 0,
                    SourceFaculty = sourceFaculty,
                    SourceFieldOfStudy = sourceFos,
                    SourceStudySubjects = sourceStudySubjects,
                    StartDate = model.From,
                    State = "NEW",
                    StoragePath = "",
                    TargetFaculty = targetFaculty,
                    TargetFieldOfStudy = targetFos
                };
                var path = SaveAgreementToStorage(storageModel);
                db.Agreements.Add(new Agreement()
                {
                    EndDate = model.To,
                    ErasmusUserId = user.Id,
                    FinancingSource = "",
                    Scholarship = 0,
                    StartDate = model.From,
                    State = AgreementState.New,
                    StoragePath = path,
                    StudyField = sourceFos.Id,
                    SourceUniversityId = sourceUniversity.Id,
                    TargetUniversityId = targetUniversity.Id,
                    Language = model.Language,
                    LanguageLevel = model.LanguageLevel,
                    Semester = model.SelectedSemester,
                    SourceFieldOfStudyId = model.SelectedSourceFieldOfStudy,
                    TargetFieldOfStudyId = model.SelectedTargetFieldOfStudy,
                    SourceFacultyId = sourceFaculty.Id,
                    TargetFacultyId = targetFaculty.Id,
                    SourceSubjects = sourceStudySubjects,
                    TargetSubjects = targetStudySubjects
                });
                db.SaveChanges();
            }
        }

        private string SaveAgreementToStorage(AgreementStorage agreement)
        {
            var json = JsonConvert.SerializeObject(agreement, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatString = "yyyy-MM-dd"
            });

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("erasmus");
            container.CreateIfNotExists();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{agreement.Student.Id}_{Guid.NewGuid().ToString("D")}");
            blockBlob.UploadText(json);
            return blockBlob.Uri.ToString();
        }

        public object GetStudySubjectsByFosId(int fosId)
        {
            using (var db = new ErasmusDbContext())
            {
                //Guard against unexisting/false tries
                var fos = db.FieldsOfStudy.SingleOrDefault(x => x.Id == fosId);
                if (fos == null)
                {
                    return null;
                }

                return fos.StudySubjects.Select(x => new {
                    Key = x.Id,
                    Value = x.Name,
                    Credits = x.Credits
                }).ToList();
            }
        }

        public object GetFacultyDataByUniversityId(int universityId)
        {
            using (var db = new ErasmusDbContext())
            {
                //Guard against unexisting/false tries
                var originalUniversity = db.Universities.SingleOrDefault(x => x.Id == universityId);
                if (originalUniversity == null)
                {
                    return null;
                }

                return originalUniversity.Faculties.Select(x => new KeyValueIntStr()
                {
                    Key = x.Id,
                    Value = x.Name
                }).ToList();
            }
        }
    }
}