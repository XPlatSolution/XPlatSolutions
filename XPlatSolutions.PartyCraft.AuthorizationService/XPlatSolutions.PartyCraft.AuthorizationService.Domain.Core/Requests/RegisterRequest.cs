﻿using System.ComponentModel.DataAnnotations;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;

public class RegisterRequest
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string? SecondName { get; set; }
    [Required]
    public string Email { get; set; }
    public string? Phone { get; set; }
    [Required]
    public string Password { get; set; }
}