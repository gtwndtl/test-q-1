using System.Collections.Generic;
using System.Threading.Tasks;
using ws.Dto;
using ws.Models;

namespace ws.Repositories;

public interface IUserRepository
{
    Task<(List<User> items, long total)> GetPaginatedAsync(RequestPaginatedUsersDto dto);
    Task<User?> GetByUserIdAsync(string userId);
    Task<string> CreateAsync(User user);
    Task UpdateAsync(string userId, User incomingUser);
    Task DeleteUserAsync(string userId);
}
