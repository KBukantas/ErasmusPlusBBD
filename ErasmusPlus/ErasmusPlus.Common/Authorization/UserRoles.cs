namespace ErasmusPlus.Common.Authorization
{
    public static class UserRoles
    {
        public const string Administrator = "Administrator";
        public const string Student = "Student";
        public const string UniversityAdmin = "UniversityAdmin";
        public const string Coordinator = "Coordinator"; 
        public const string Foreign = "Foreign";

        public const string AdminAndUniAdmin = "Administrator,UniversityAdmin";
        public const string AdminAndCoordinator = "Administrator,Coordinator";
        public const string AdminAndStudent = "Administrator,Student";

        public static string[] AllRoles {
            get { return new []{ Administrator, Student, UniversityAdmin, Coordinator, Foreign }; }
        }

        public static string Multiple(string[] roles)
        {
            return string.Join(",", roles);
        }
    }
}