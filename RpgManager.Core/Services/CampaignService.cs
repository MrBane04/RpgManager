namespace RpgManager.Core.Services
{
    public class CampaignService
    {
        public OperationResult CreateCampaign(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                OperationResult resultFail = new OperationResult(false, "Podaj poprawną wartość.");
                return resultFail;
            }
            else
            {
                OperationResult resultSuccess = new OperationResult(true, "Sukces");
                return resultSuccess;
            }
        }
    }
}
