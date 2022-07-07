﻿using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;

public interface IUsersAccess
{
    Task<User?> GetUser(string login, string password);
    Task<User?> GetUserById(string? id);
    Task<User?> GetUserByLogin(string? login);
    Task<User?> GetUserByEmail(string? email);
    Task<List<User>> GetAll();
    Task<bool> AddUser(User user);
    Task SetVerified(string userId);
    Task ChangePassword(string userId, string passwordHash);
}