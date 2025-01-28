namespace HDUA.Helpers
{
    using BCrypt.Net;

    public static class PasswordHelper
    {
        // Método para hashear la contraseña
        public static string HashPassword(string password)
        {
            return BCrypt.HashPassword(password);
        }

        // Método para verificar la contraseña
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Verify(password, hashedPassword);
        }
    }
}
