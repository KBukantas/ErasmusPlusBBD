using System.Collections.Generic;
using System.Linq;
using ErasmusPlus.Common.Database;
using ErasmusPlus.Models.Identity;
using ErasmusPlus.Models.ViewModels.Student;

namespace ErasmusPlus.Models.BLL
{
    public class CommonBusinessLogic
    {
        public List<FacultyItem> GetFaultiesByUniversityId(int universityId)
        {
            using (var db = new ErasmusDbContext())
            {
                var faculties = db.Faculties.Where(x => x.UniversityId == universityId).ToList();
                return faculties.Select(x => new FacultyItem()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
            }
        }

        public List<University> GetUniversitiesList()
        {
            using (var db = new ErasmusDbContext())
            {
                var universities = db.Universities.ToList();
                return universities;
            }
        }
    }
}