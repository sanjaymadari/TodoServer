using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoServer.DTOs;
using TodoServer.Models;
using TodoServer.Repositories;

namespace TodoServer.Controllers;

[ApiController]
[Route("api/todo")]
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
    [Authorize]
    public async Task<ActionResult> CreateTodo([FromBody] TodoDTO todo)
    {
        var id = GetCurrentUserId();
        var toCreateTodo = new Todo
        {
            UserId = Int32.Parse(id),
            Description = todo.Description,
        };
        var createdTodo = await _todo.Create(toCreateTodo);
        return Ok("Created");
    }

    [HttpGet("mytodos")]
    [Authorize]
    public async Task<ActionResult<List<Todo>>> GetMyTodos()
    {
        var id = GetCurrentUserId();
        var todos = await _todo.GetMyTodos(Int32.Parse(id));
        return Ok(todos);
    }

    [HttpGet("alltodos")]
    [Authorize]
    public async Task<ActionResult<List<Todo>>> GetTodos()
    {
        var todos = await _todo.GetTodos();
        return Ok(todos);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Todo>> GetTodo(long id)
    {
        var todo = await _todo.GetById(id);
        if (todo == null)
            return NotFound("Todo not found");
        return Ok(todo);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> UpdateTodo(long id, [FromBody] TodoUpdateDTO todo)
    {
        var existing = await _todo.GetById(id);
        var currentUserId = GetCurrentUserId();
        if(Int32.Parse(currentUserId) != existing.UserId)
            return Unauthorized("Your not authorized to update.");
        if (existing == null)
            return NotFound("Todo not found");
        var toUpdateTodo = existing with
        {
            Description = todo.Description ?? existing.Description,
            // IsCompleted = todo.IsCompleted,
        };
        var didUpdate = await _todo.Update(toUpdateTodo);
        if (!didUpdate)
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not update todo");
        return Ok("Updated");
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteTodo(long id)
    {
        var todo = await _todo.GetById(id);
        var currentUserId = GetCurrentUserId();
        if(Int32.Parse(currentUserId) != todo.UserId)
            return Unauthorized("Your not authorized to delete.");

        if(todo == null)
            return NotFound("Todo not found");
        var didDelete = await _todo.Delete(id);
        if(!didDelete)
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not delete todo");
        return Ok("Deleted");
    }

    private string GetCurrentUserId()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;

        var userClaims = identity.Claims;

        return (userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

    }

}
