using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ws.Dto;
using ws.Extensions;
using ws.Models;
using ws.Repositories;

namespace ws.Controllers;

[ApiController]
[Route("api/users")]
public class AppUserController : ControllerBase
{
    private readonly IUserRepository _userRepo;

    public AppUserController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedUsers([FromQuery] RequestPaginatedUsersDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var (users, total) = await _userRepo.GetPaginatedAsync(dto);
            
            var result = users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                Firstname = u.Firstname,
                Lastname = u.Lastname,
                Address = u.Address,
                DateOfBirth = u.DateOfBirth,
                Age = u.Age
            }).ToList();

            return Ok(new { success = true, data = result, total, page = dto.Page, pageSize = dto.PageSize });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An internal error occurred: {ex.Message}");
        }
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await _userRepo.GetByUserIdAsync(userId);
        if (user == null) return NotFound(new { message = "User not found." });

        var result = new UserResponseDto
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Address = user.Address,
            DateOfBirth = user.DateOfBirth,
            Age = user.Age
        };

        return this.Success(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] RequestCreateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var user = new User
            {
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                Age = dto.Age
            };

            var id = await _userRepo.CreateAsync(user);
            return this.Success(new { message = "User created successfully", userId = id });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An internal error occurred." });
        }
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] RequestUpdateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var incomingUser = new User
            {
                Firstname = dto.Firstname ?? string.Empty,
                Lastname = dto.Lastname ?? string.Empty,
                Address = dto.Address ?? string.Empty,
                DateOfBirth = dto.DateOfBirth ?? default,
                Age = dto.Age ?? default
            };

            await _userRepo.UpdateAsync(userId, incomingUser);
            return this.Success(new { message = "User updated successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "User not found." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal error occurred." });
        }
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        try
        {
            await _userRepo.DeleteUserAsync(userId);
            return this.Success(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal error occurred." });
        }
    }
}
