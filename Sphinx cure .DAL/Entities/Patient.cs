using System.ComponentModel.DataAnnotations.Schema;

namespace Sphinx_cure_.DAL.Entities
{
    public class Patient
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string FilePath { get; set; } = string.Empty;
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        public string? UserId { get; private set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; private set; }

        public Patient() { }

        public Patient(int id, string name, string filePath)
        {
            Id = id;
            Name = name;
            CreatedAt = DateTime.Now;
            FilePath = filePath;
            IsDeleted = false;
        }

        public void UpdateFile(string newFilePath)
        {
            FilePath = newFilePath;
            UpdatedAt = DateTime.Now;
        }

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.Now;
        }
    }
}
