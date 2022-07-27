using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using UserService.Db;
using UserService.Db.Entities;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IRabbitManager _manager;
    private readonly AppDbContext _context;

    public UserController(AppDbContext context, IRabbitManager manager)
    {
        _manager = manager;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(UserDto newUser, CancellationToken cancellationToken)
    {
        User entity = new()
        {
            FirstName = newUser.FirstName,
            MiddleName = newUser.MiddleName,
            LastName = newUser.LastName
        };

        await _context.Users.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _manager.Publish(entity, "user-service", ExchangeType.Fanout);

        return Ok(entity);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> PutAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Users.SingleOrDefaultAsync(user => user.Id == id, cancellationToken);
    
        if (entity is null) return BadRequest();
    
        entity.IsFired = true;
    
        await _context.SaveChangesAsync(cancellationToken);
    
        _manager.Publish(entity, "user-service", ExchangeType.Fanout);
        
        return Ok();
    }
}