namespace api.Constants
{
    public static class EmailConstants
    {
        public const string PasswordResetSent = "Si el correo ingresado está registrado, se enviará un enlace para restablecer la contraseña.";
        public const string PasswordResetGreeting = "Hemos recibido una solicitud para restablecer tu contraseña.";
        public const string PasswordResetAction = "Haz clic aquí para restablecer tu contraseña.";
        public const string PasswordResetSubject = "Restablecimiento de contraseña - Force Gym";
        public const string FailedToSend = "No se pudo enviar el correo electrónico. Intenta nuevamente más tarde.";
        public const string UserNotFound = "El correo electrónico no está registrado.";
        public const string PasswordResetSuccess = "La contraseña se restableció correctamente.";
    }
}
