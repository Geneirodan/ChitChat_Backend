namespace Identity.Web.Requests;

internal record ChangeEmailConfirmedRequest(string newEmail, string confirmationToken);