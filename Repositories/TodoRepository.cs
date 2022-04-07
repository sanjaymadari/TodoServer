using Dapper;
using TodoServer.Models;
using TodoServer.Utilities;

namespace TodoServer.Repositories;


public interface ITodoRepository
{
    Task<Todo> Create(Todo todo);
    Task<Todo> GetById(long Id);
    Task<bool> Update(Todo todo);
    Task<bool> Delete(long Id);
    Task<List<Todo>> GetTodos();
    Task<List<Todo>> GetMyTodos(long UserId);
}
public class TodoRepository : BaseRepository, ITodoRepository
{
    public TodoRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<Todo> Create(Todo todo)
    {
        var query = $@"INSERT INTO ""{TableNames.todos}"" (user_id, description, is_completed)
        VALUES(@UserId, @Description, @IsCompleted) RETURNING *;";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Todo>(query, todo);
    }

    public async Task<bool> Delete(long Id)
    {
        var query = $@"DELETE FROM ""{TableNames.todos}"" WHERE id = @Id;";
        using (var con = NewConnection)
            return (await con.ExecuteAsync(query, new { Id })) == 1;
    }

    public async Task<Todo> GetById(long Id)
    {
        var query = $@"SELECT * FROM ""{TableNames.todos}"" WHERE id = @Id;";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Todo>(query, new { Id });
    }

    public async Task<List<Todo>> GetMyTodos(long UserId)
    {
        var query = $@"SELECT * FROM ""{TableNames.todos}"" WHERE user_id = @UserId ORDER BY id;";
        using (var con = NewConnection)
            return (await con.QueryAsync<Todo>(query, new { UserId })).ToList();
    }

    public async Task<List<Todo>> GetTodos()
    {
        var query = $@"SELECT * FROM ""{TableNames.todos}"" ORDER BY id";
        using (var con = NewConnection)
            return (await con.QueryAsync<Todo>(query)).ToList();
    }

    public async Task<bool> Update(Todo todo)
    {
        var query = $@"UPDATE ""{TableNames.todos}"" SET description = @Description, is_completed = @IsCompleted WHERE id = @Id;";
        using (var con = NewConnection)
            return (await con.ExecuteAsync(query, todo)) == 1;
    }
}