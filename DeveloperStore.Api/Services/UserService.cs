using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Application.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UsersResultDto> GetAllAsync(int page, int size, string? order, string? username, string? email, string? status, string? role)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(u => u.Username.Contains(username));
            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(u => u.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(u => u.Status == status);
            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(u => u.Role == role);

            // Ordenação
            if (!string.IsNullOrWhiteSpace(order))
            {
                var orderParts = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in orderParts.Select((o, i) => new { o, i }))
                {
                    var info = part.o.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var field = info[0];
                    var desc = info.Length > 1 && info[1].ToLower() == "desc";

                    if (field.ToLower() == "username")
                        query = desc
                            ? (part.i == 0 ? query.OrderByDescending(u => u.Username) : ((IOrderedQueryable<User>)query).ThenByDescending(u => u.Username))
                            : (part.i == 0 ? query.OrderBy(u => u.Username) : ((IOrderedQueryable<User>)query).ThenBy(u => u.Username));

                    if (field.ToLower() == "email")
                        query = desc
                            ? (part.i == 0 ? query.OrderByDescending(u => u.Email) : ((IOrderedQueryable<User>)query).ThenByDescending(u => u.Email))
                            : (part.i == 0 ? query.OrderBy(u => u.Email) : ((IOrderedQueryable<User>)query).ThenBy(u => u.Email));
                }
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            var data = items.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                Status = u.Status,
                Role = u.Role
            });

            return new UsersResultDto
            {
                Data = data,
                TotalItems = totalItems,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user == null ? null : new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Status = user.Status,
                Role = user.Role
            };
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                Password = dto.Password,
                Status = dto.Status,
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Status = user.Status,
                Role = user.Role
            };
        }

        public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.Email = dto.Email;
            user.Username = dto.Username;
            user.Status = dto.Status;
            user.Role = dto.Role;

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Status = user.Status,
                Role = user.Role
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
