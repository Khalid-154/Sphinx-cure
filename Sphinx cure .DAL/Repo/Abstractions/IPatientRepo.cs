
using Sphinx_cure_.DAL.Entities;

namespace Sphinx_cure_.DAL.Repo.Abstractions
{
    public interface IPatientRepo
    {
        Task<List<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(Patient patient);
        Task SaveAsync();
        Task<IEnumerable<Patient>> SearchByNameAsync(string name);
    }
}
