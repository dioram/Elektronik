namespace Elektronik.Settings
{
    public partial class SettingsBag
    {
        /// <summary> Result of settings validation. </summary>
        public struct ValidationResult
        {
            /// <summary> Create successful validation result. </summary>
            public static readonly ValidationResult Succeeded = new ValidationResult
            {
                Success = true,
                Message = ""
            };

            /// <summary> Create failed validation result with given message. </summary>
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