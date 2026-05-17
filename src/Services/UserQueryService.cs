using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Exceptions;
using Rotinik.Data;
using Rotinik.DTOs.User;

namespace Rotinik.Services;

public class UserQueryService : IUserQueryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserQueryService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserProfileDto?> GetPublicProfileAsync(string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        if (user == null)  
            throw new NotFoundException("User not found.");

        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task<UserResponseDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)  
            throw new NotFoundException("User not found.");

        return _mapper.Map<UserResponseDto>(user);
    }
}