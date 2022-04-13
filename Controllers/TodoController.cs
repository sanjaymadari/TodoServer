using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoServer.DTOs;
using TodoServer.Models;
using TodoServer.Repositories;
using TodoServer.Utilities;

namespace TodoServer.Controllers;

[ApiController]
[Route("api/todo")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly ITodoRepository _todo;
    private readonly IUserRepository _user;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoRepository todo, IUserRepository user, ILogger<TodoController> logger)
    {
        _todo = todo;
        _user = user;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> CreateTodo([FromBody] TodoDTO todo)
    {
        var currentUserId = GetCurrentUserId();
        var toCreateTodo = new Todo
        {
            UserId = currentUserId,
            Description = todo.Description,
        };
        var createdTodo = await _todo.Create(toCreateTodo);
        return Ok("Created");
    }

    [HttpGet]
    public async Task<ActionResult<List<Todo>>> GetTodos()
    {
        var todos = await _todo.GetTodos();
        return Ok(todos);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTodo(long id, [FromBody] TodoUpdateDTO todo)
    {
        var existing = await _todo.GetById(id);
        var currentUserId = GetCurrentUserId();
        if (currentUserId != existing.UserId)
            return Unauthorized("Your not authorized to update.");
        if (existing == null)
            return NotFound("Todo not found");
        var toUpdateTodo = existing with
        {
            Description = todo.Description ?? existing.Description,
            IsCompleted = todo.IsCompleted,
        };
        var didUpdate = await _todo.Update(toUpdateTodo);
        if (!didUpdate)
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not update todo");
        return Ok("Updated");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodo(long id)
    {
        var todo = await _todo.GetById(id);
        var currentUserId = GetCurrentUserId();
        if (currentUserId != todo.UserId)
            return Unauthorized("Your not authorized to delete.");

        if (todo == null)
            return NotFound("Todo not found");
        var didDelete = await _todo.Delete(id);
        if (!didDelete)
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not delete todo");
        return Ok("Deleted");
    }

    private long GetCurrentUserId()
    {
        // var identity = HttpContext.User.Identity as ClaimsIdentity;

        var userClaims = User.Claims;

        return Int64.Parse(userClaims.FirstOrDefault(c => c.Type == TodoConstants.Id)?.Value);
    }
}
