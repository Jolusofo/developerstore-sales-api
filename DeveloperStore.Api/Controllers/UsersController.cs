using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET /users?_page=1&_size=10&_order="username asc"
        [HttpGet]
        public async Task<IActionResult> GetAll(
            int _page = 1,
            int _size = 10,
            string? _order = null,
            string? username = null,
            string? email = null,
            string? status = null,
            string? role = null)
        {
            var query = _context.Users.AsQueryable();

            // Filtros básicos
            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(u => u.Username.Contains(username));
            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(u => u.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(u => u.Status == status);
            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(u => u.Role == role);

            // Ordenação
            if (!string.IsNullOrWhiteSpace(_order))
            {
                var orderParts = _order.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in orderParts.Select((o, i) => new { o, i }))
                {
                    var orderInfo = part.o.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var field = orderInfo[0];
                    var desc = orderInfo.Length > 1 && orderInfo[1].ToLower() == "desc";

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

            // Paginação
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)_size);
            var items = await query.Skip((_page - 1) * _size).Take(_size).ToListAsync();

            return Ok(new
            {
                data = items,
                totalItems,
                currentPage = _page,
                totalPages
            });
        }

        // GET /users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST /users
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // PUT /users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Entry(user).CurrentValues.SetValues(updatedUser);
            await _context.SaveChangesAsync();
            return Ok(updatedUser);
        }

        // DELETE /users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User deleted successfully" });
        }
    }
}
