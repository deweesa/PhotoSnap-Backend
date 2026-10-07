using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoSnap.API.Contracts.Users;
using PhotoSnap.API.Data;
using PhotoSnap.Entities;

namespace PhotoSnap.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(PhotoSnapDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<User>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Email)
            .ToListAsync(cancellationToken);

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id == id, cancellationToken);

        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await EmailIsInUse(normalizedEmail, cancellationToken))
        {
            return Conflict(new { message = "A user with this email already exists." });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<User>> Update(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(user => user.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await EmailIsInUse(normalizedEmail, cancellationToken, id))
        {
            return Conflict(new { message = "A user with this email already exists." });
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = normalizedEmail;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(user => user.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private Task<bool> EmailIsInUse(string email, CancellationToken cancellationToken, Guid? excludedUserId = null) =>
        dbContext.Users.AnyAsync(
            user => user.Email == email && (!excludedUserId.HasValue || user.Id != excludedUserId.Value),
            cancellationToken);
}
