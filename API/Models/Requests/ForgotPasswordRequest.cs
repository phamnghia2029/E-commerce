using API.Entities;
using API.Repositories;
using FluentValidation;

namespace API.Models.Requests;

public class ForgotPasswordRequest
{
    public string? Email { get; set; } = "";
}

public class ForgotPasswordRequestValidator : BaseValidator<ForgotPasswordRequest>
{
    private readonly AccountRepository _accountRepository;

    public ForgotPasswordRequestValidator(AccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    protected override void AddRules(ForgotPasswordRequest request)
    {
        Account? account = _accountRepository.FindByEmail(request.Email?.Trim() ?? "");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Please fill email.")
            .Must(email => account != null)
            .WithMessage("Email not found");
    }
}