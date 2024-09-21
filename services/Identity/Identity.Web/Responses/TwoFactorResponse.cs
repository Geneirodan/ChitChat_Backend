namespace Identity.Web.Responses;

internal sealed record TwoFactorResponse(string SharedKey, IEnumerable<string>? RecoveryCodes);