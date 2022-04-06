using Dapper;
using TodoServer.Models;
using TodoServer.Utilities;

namespace TodoServer.Repositories;


public interface IUserRepository
{
    Task<User> Create(User Data);
    Task<User> GetByEmail(string Email);
    Task<User> GetById(long Id);
}
public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<User> Create(User Data)
    {
        var query = $@"INSERT INTO ""{TableNames.users}"" (name, email, password)
        VALUES(@Name, @Email, @Password) RETURNING *;";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, Data);
    }

    public  async Task<User> GetByEmail(string Email)
    {
        var query = $@"SELECT * FROM ""{TableNames.users}"" WHERE email = @Email;";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, new { Email });
    }

    public async Task<User> GetById(long Id)
    {
        var query = $@"SELECT * FROM ""{TableNames.users}"" WHERE id = @Id;";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, new { Id });
    }
}