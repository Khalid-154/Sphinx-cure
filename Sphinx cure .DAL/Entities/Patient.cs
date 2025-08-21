

namespace Sphinx_cure_.DAL.Entities
{
    public class Patient
    {
        public int Id { get;private set; }
        public string Name { get;private set; }

        public string FilePath { get;  set; }
        public bool IsDeleted { get; private set; }
        


        public Patient() { }

        public Patient(int id, string name, string filePath)
        {
            Id = id;
            Name = name;
            FilePath = filePath;
            IsDeleted = false; // Default value
        }


    }
}
