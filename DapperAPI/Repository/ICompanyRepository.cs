using DapperAPI.Dto;
using DapperAPI.Entities;

namespace DapperAPI.Repository
{
    public interface ICompanyRepository
    {
        public Task<IEnumerable<Company>> GetAll();
        public Task<Company> GetCompany(int id);  
        public Task<Company> CreateCompany(CompanyForCreationDto company);
        public Task UpdateCompany(int id, CompanyForUpdateDto company);
        public Task DeleteCompany(int Id);
        public Task<Company> GetCompanyByEmployeeId (int EmployeeId);   
        public Task<Company> GetMultipleResult(int Id);

        public Task<List<Company>> MultipleMapping();


    }
}
