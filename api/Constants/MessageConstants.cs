namespace api.Constants
{
    public static class MessageConstants
    {
        // Generic messages with parameters
        public static string EntityCreated(string entityName) => $"{entityName} se ha creado correctamente";
        public static string EntityUpdated(string entityName) => $"{entityName} se ha actualizado correctamente";
        public static string EntityDeleted(string entityName) => $"{entityName} se ha eliminado correctamente";
        public static string EntityNotFound(string entityName) => $"{entityName} no fue encontrada@";
        public static string FieldRequired(string fieldName) => $"{fieldName} es requerid@";
        
        // Specific messages (without parameters)
        public static class Generic
        {
            public const string RequiredFields = "Se requieren todos los datos necesarios";
            public const string TryAgain = "Inténtelo de nuevo";
            public const string Unauthorized = "Usuario no autenticado";
            public const string NoRecords = "No hay registros disponibles";

            public const string ServerError = "Error en el servidor";
        }
        




    }
}