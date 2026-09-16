using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Constants
{
    public static class AuthMessages
    {
        public const string TokenExpired = "Token has expired.";
        public const string InvalidCredentials = "Invalid username or password.";
        public const string LoginSuccessful = "Login successful.";
        public const string EmptyCredentials = "Username or password cannot be empty.";
        public const string RegistrationSuccessful = "User registered successfully.";
        public const string UsernameAlreadyExists = "Username already exists.";
        public const string InvalidRefreshToken = "Invalid or expired refresh token.";
        public const string UserNotFound = "User not found.";
        public const string TokenRefreshed = "Token refreshed successfully.";
        public const string LogoutSuccessful = "Logged out successfully.";
    }
}