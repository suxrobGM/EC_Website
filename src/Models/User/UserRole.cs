﻿using Microsoft.AspNetCore.Identity;
using SuxrobGM.Sdk.Utils;

// ReSharper disable once CheckNamespace
namespace EC_Website.Models.UserModel
{
    public enum Role
    {
        Editor,
        Developer,
        Moderator,
        Admin,
        SuperAdmin                              
    }

    public class UserRole : IdentityRole
    {
        public UserRole(Role role): base(role.ToString())
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Id = GeneratorId.GenerateLong();
            Role = role;            
        }

        public Role Role { get; set; }        
    }
}
