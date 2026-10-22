namespace RpgManager.Core.Services
{
    public class OperationResult
    {
        public bool Success { get;}
        public string Message { get;}

        public OperationResult(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }
    }
}
