namespace RpgManager.Core.Services
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Text { get; set; }

        public OperationResult(bool success, string text)
        {
            this.Success = success;
            this.Text = text;
        }
    }
}
