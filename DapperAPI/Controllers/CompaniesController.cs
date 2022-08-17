using DapperAPI.Dto;
using DapperAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DapperAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompaniesController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _companyRepository.GetAll();

            return Ok(companies);
        }

        [HttpGet("{id}", Name ="CompanyById")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var company = await _companyRepository.GetCompany(id);
            if (company is null)
                return NotFound();

            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody]CompanyForCreationDto company)
        {
            var createdCoompany = await _companyRepository.CreateCompany(company);

            return CreatedAtRoute("CompanyById", new {id= createdCoompany.Id}, createdCoompany);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyForUpdateDto company)
        {
            var dbCompany = await _companyRepository.GetCompany(id);
            if (dbCompany is null) 
            return NotFound();

            await _companyRepository.UpdateCompany(id, company);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var dbCompany = await _companyRepository.GetCompany(id);
            if (dbCompany is null)
                return NotFound();

            await _companyRepository.DeleteCompany(id);

            return NoContent();
        }

        [HttpGet("ByEmployeeId/{id}")]
        public async Task<IActionResult> GetCompanyForEmployee(int id)
        {
            var company = await _companyRepository.GetCompanyByEmployeeId(id);
            if (company is null)
                return NotFound();

            return Ok(company);
        }

        [HttpGet("{id}/MultipleResult")]
        public async Task<IActionResult> GetMultipleResults(int id)
        {
            var company = await _companyRepository.GetMultipleResult(id);
            if (company is null)
                return NotFound();

            return Ok(company);
        }


        [HttpGet("MultipleMapping")]
        public async Task<IActionResult> GetMultipleMappingd()
        {
            var company = await _companyRepository.MultipleMapping();
           

            return Ok(company);
        }
    }
}
