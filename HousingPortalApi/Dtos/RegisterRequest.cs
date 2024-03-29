﻿namespace HousingPortalApi.Dtos
{
    public class RegisterRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Major { get; set; } = null!;
        public int GraduationYear { get; set; }

    }
}