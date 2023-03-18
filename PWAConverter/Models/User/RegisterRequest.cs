﻿using System.ComponentModel.DataAnnotations;

namespace PWAConverter.Models.User
{
    public class RegisterRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get;set; }
        [Required]
        public string Name { get; set; }
    }
}
