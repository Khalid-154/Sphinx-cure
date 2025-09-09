

using Microsoft.EntityFrameworkCore;
using Sphinx_cure_.DAL.Database;
using Sphinx_cure_.DAL.Entities;
using Sphinx_cure_.DAL.Repo.Abstractions;

namespace Sphinx_cure_.DAL.Repo.Implementations
{
    public class PatientRepo: IPatientRepo
    {
        private readonly SphinxCureDbContext _context;
        public PatientRepo(SphinxCureDbContext context)
        {
            _context = context;
        }

        public async Task<List<Patient>> GetAllAsync()
        {
            return await _context.Patients
                         .Where(p => !p.IsDeleted) 
                         .OrderBy(p => p.Name)
                         .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                                 .Where(p => p.Id == id) 
                                 .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
        }

        public async Task UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Patient patient)
        {
            patient.Delete();
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Patient>> SearchByNameAsync(string name)
        {
            return await _context.Patients
                                 .Where(p => !p.IsDeleted && p.Name.Contains(name))
                                 .OrderBy(p => p.Name)
                                 .ToListAsync();
        }
    }
}
