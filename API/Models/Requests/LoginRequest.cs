using API.Utils;
using FluentValidation;
using API.Entities;
using API.Repositories;

namespace API.Models.Requests;

public class LoginRequest: BaseRequest
{
    public string? Email { get; set; } = "";
    public string? Password { get; set; } = "";
}

public class LoginRequestValidator : BaseValidator<LoginRequest>
{
    private readonly AccountRepository _accountRepository;

    public LoginRequestValidator(AccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    protected override void AddRules(LoginRequest request)
    {
        Account? account = _accountRepository.FindByEmail(request.Email?.Trim() ?? "");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Please fill email.")
            .Must(email => account != null)
            .WithMessage("Email not found");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Please fill password.")
            .Must(password => IsRightPassword(request.Password, account?.Password ?? null))
            .WithMessage("Wrong password.");
    }

    private static bool IsRightPassword(string? requestPassword, string? currentPassword)
    {
        bool isRequestBlank = Strings.IsBlank(requestPassword);
        bool isPasswordBlank = Strings.IsBlank(currentPassword);
        if (isPasswordBlank)
        {
            return isRequestBlank;
        }
        if (isRequestBlank)
        {
            return isPasswordBlank;
        }
        string dbPassword = Encryptions.Decrypt(currentPassword);
        return dbPassword.Equals(requestPassword.Trim());
    }
}