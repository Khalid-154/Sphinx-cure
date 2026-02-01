using Microsoft.AspNetCore.Identity;
using Sphinx_cure_.DAL.Enums;

namespace Sphinx_cure_.DAL.Entities
{
    public class User : IdentityUser
    {
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        public virtual ICollection<Patient> Patients { get; private set; } = new List<Patient>();


        public User() { }

        public User(UserRole role, bool isDeleted = false)
        {
            Role = role;
            CreatedAt = DateTime.Now;
            IsDeleted = isDeleted;
        }

        public void AssignPatient(Patient patient)
        {
            Patients.Add(patient);
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.Now;
        }
    }
}
