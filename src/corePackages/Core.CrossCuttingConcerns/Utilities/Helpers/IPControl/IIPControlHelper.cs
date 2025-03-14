namespace Core.CrossCuttingConcerns.Utilities.Helpers.IPControl;

public interface IIPControlHelper
{
    List<string> GetAllowedIPListAsync();
}
