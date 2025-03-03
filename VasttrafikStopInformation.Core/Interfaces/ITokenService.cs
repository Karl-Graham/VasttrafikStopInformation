namespace VasttrafikStopInformation.Core.Interfaces;

public interface ITokenService
{

    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}