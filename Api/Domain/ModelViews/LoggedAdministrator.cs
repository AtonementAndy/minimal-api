﻿namespace minimal_api.Domain.ModelViews
{
    public record LoggedAdministrator
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
