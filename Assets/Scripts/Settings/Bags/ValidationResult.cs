namespace Elektronik.Settings.Bags
{
    public partial class SettingsBag
    {
        public struct ValidationResult
        {
            public static readonly ValidationResult Succeeded = new ValidationResult
            {
                Success = true,
                Message = ""
            };

            public static ValidationResult Failed(string message) => new ValidationResult
            {
                Success = false,
                Message = message
            };
            
            public bool Success;
            public string Message;
        }
    }
}