using Dapper;
using DapperAPI.Context;
using DapperAPI.Dto;
using DapperAPI.Entities;
using System.Data;

namespace DapperAPI.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;
        public CompanyRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Company> CreateCompany(CompanyForCreationDto company)
        {
            var query = "Insert into Companies(Name,Address,Country)Values(@Name,@Address,@Country)" +
                "Select Cast(Scope_Identity() As int)";

            var parameters = new DynamicParameters();
            parameters.Add("Name", company.Name, DbType.String);
            parameters.Add("Address", company.Address, DbType.String);
            parameters.Add("Country", company.Country, DbType.String);

            using(var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);

                var createdCompany = new Company
                {
                    Id = id,
                    Name = company.Name,
                    Address = company.Address,
                    Country = company.Country
                };

                return createdCompany;
            }
        }

        public async Task DeleteCompany(int Id)
        {
            var query = "Delete from Companies where Id = @Id";

            using(var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id });
            }
        }

        public async Task<IEnumerable<Company>> GetAll()
        {
            var query = "Select * from Companies";

            using (var connection = _context.CreateConnection())
            {
                var companies =  await connection.QueryAsync<Company>(query);

                return companies.ToList();
            }
        }

        public async Task<Company> GetCompany(int id)
        {
            var query = "Select * from Companies where Id = @Id";

            using(var connection = _context.CreateConnection())
            {
                var company = await connection.QuerySingleOrDefaultAsync<Company>(query, new {id});

                return company;
            }
        }

        public async Task<Company> GetCompanyByEmployeeId(int EmployeeId)
        {
            var procedureName = "ShowCompanyByEmployeeId";
            var parameters = new DynamicParameters();
            parameters.Add("Id",EmployeeId, DbType.Int32,ParameterDirection.Input);

            using(var connection = _context.CreateConnection())
            {
                var company = await connection.QueryFirstOrDefaultAsync<Company>
               (procedureName, parameters, commandType: CommandType.StoredProcedure);

                return company;
            }
        }

        public async Task<Company> GetMultipleResult(int Id)
        {
            var query = "Select * from Companies Where Id =@Id;" +
                "Select * from Employee where CompanyId = @Id";

            using(var connection = _context.CreateConnection())
                using(var multi =  await connection.QueryMultipleAsync(query, new {Id}))
            {
                var company =  await multi.ReadFirstOrDefaultAsync<Company>();
                if(company is not null)
                    company.Employees = (await multi.ReadAsync<Employee>()).ToList();

                return company;
            }

        }

        public async Task<List<Company>> MultipleMapping()
        {
            var query = "Select * from Companies c inner join Employee e on c.Id = e.CompanyId";

            using(var connection = _context.CreateConnection())
            {
                var companyDict =  new Dictionary<int, Company>();

                var companies = await connection.QueryAsync<Company, Employee, Company> (
                    query, (company, employee) =>
                    {
                        if(!companyDict.TryGetValue(company.Id, out var currentCompany))
                        {
                            currentCompany = company;
                            companyDict.Add(currentCompany.Id, currentCompany);
                        }

                        currentCompany.Employees.Add(employee);


                        return currentCompany;
                    }
                );

                return companies.Distinct().ToList();
            }
        }

        public async Task UpdateCompany(int id, CompanyForUpdateDto company)
        {
            var query = "Update Companies set Name = @Name, Address = @Address,Country = @Country Where Id = @Id";

            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Int32);
            parameter.Add("Name",company.Name, DbType.String);
            parameter.Add("Address",company.Address, DbType.String);
            parameter.Add("Country", company.Country,DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query,parameter);
            }
        }
    }
}
