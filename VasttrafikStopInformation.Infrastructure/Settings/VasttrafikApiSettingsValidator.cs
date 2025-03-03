using Microsoft.Extensions.Options;
using VasttrafikStopInformation.Core.Settings;

namespace VasttrafikStopInformation.Infrastructure.Settings;

/// <summary>
/// Validator for VasttrafikApiSettings
/// </summary>
public class VasttrafikApiSettingsValidator : IValidateOptions<VasttrafikApiSettings>
{
    public ValidateOptionsResult Validate(string name, VasttrafikApiSettings options)
    {
        if (string.IsNullOrEmpty(options.AuthenticationKey))
        {
            return ValidateOptionsResult.Fail("AuthenticationKey is required.");
        }
        return ValidateOptionsResult.Success;
    }
}
