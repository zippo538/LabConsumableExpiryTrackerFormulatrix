namespace LabConsumableExpiryTracker.Validators.Helpers;

public static class LotHelperValidator
{
    public static bool NotBeBlank(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
    public static bool BeInThePastOrPresent(DateTime value, TimeProvider timeProvider)
    {
        if (value == default)
        {
            return false;
        }
        return value.ToUniversalTime() <= timeProvider.GetUtcNow().UtcDateTime;
    }
}