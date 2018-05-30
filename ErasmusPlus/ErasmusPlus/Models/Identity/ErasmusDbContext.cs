using ErasmusPlus.Common.Database;
using ErasmusPlus.Models.Database;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;

namespace ErasmusPlus.Models.Identity
{
    public class ErasmusDbContext : IdentityDbContext<ErasmusUser>
    {
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<StudySubjects> StudySubjects { get; set; }
        public DbSet<FieldOfStudy> FieldsOfStudy { get; set; }
        public DbSet<UniversityAgreements> UniversityAgreements { get; set; }
        public DbSet<UserPermissions> UserPermissions { get; set; }

        public ErasmusDbContext(): base("ErasmusPlusDb", throwIfV1Schema: false)
        {
        }

        public static ErasmusDbContext Create()
        {
            return new ErasmusDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Agreement>().HasRequired(c => c.ErasmusUser).WithMany(t => t.Agreements);
            modelBuilder.Entity<Agreement>().HasRequired(c => c.ErasmusUser).WithMany(t => t.Agreements);
            modelBuilder.Entity<Agreement>().HasRequired(x => x.SourceUniversity).WithMany().HasForeignKey(x => x.SourceUniversityId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Agreement>().HasRequired(x => x.TargetUniversity).WithMany().HasForeignKey(x => x.TargetUniversityId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Agreement>().HasRequired(x => x.SourceFaculty).WithMany().HasForeignKey(x => x.SourceFacultyId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Agreement>().HasRequired(x => x.TargetFaculty).WithMany().HasForeignKey(x => x.TargetFacultyId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Agreement>().HasRequired(x => x.SourceFieldOfStudy).WithMany().HasForeignKey(x => x.SourceFieldOfStudyId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Agreement>().HasRequired(x => x.TargetFieldOfStudy).WithMany().HasForeignKey(x => x.TargetFieldOfStudyId).WillCascadeOnDelete(false);
            modelBuilder.Entity<UniversityAgreements>().HasRequired(x => x.SourceUniversity).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<UniversityAgreements>().HasRequired(x => x.TargetUniversity).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Agreement>().HasMany(x => x.SourceSubjects).WithMany(x => x.SourceAgreements).Map(x =>
            {
                x.ToTable("SourceAgreementSubjects");
                x.MapLeftKey("AgreementId");
                x.MapRightKey("SubjectId");
            });

            modelBuilder.Entity<Agreement>().HasMany(x => x.TargetSubjects).WithMany(x => x.TargetAgreements).Map(x =>
            {
                x.ToTable("TargetAgreementSubjects");
                x.MapLeftKey("AgreementId");
                x.MapRightKey("SubjectId");
            });

            modelBuilder.Entity<UserPermissions>().HasRequired(x => x.User).WithMany(x => x.UserPermissions);
            modelBuilder.Entity<UserPermissions>().HasRequired(x => x.University);
            modelBuilder.Entity<UserPermissions>().HasOptional(x => x.Faculty);
        }
    }
}