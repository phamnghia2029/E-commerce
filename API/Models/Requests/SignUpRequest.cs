using API.Repositories;
using FluentValidation;

namespace API.Models.Requests;

public class SignUpRequest: BaseRequest
{
    
    public string? Email { get; set; } = "";
    
    
    public string? Password { get; set; } = "";
    
    
    public string? CompanyName { get; set; } = "";
    
    
    public string? Address { get; set; } = "";

    
    public string? ContactName { get; set; } = "";
    
    
    public string? ContactTitle { get; set; } = "";
}

public class SignUpRequestValidator : BaseValidator<SignUpRequest>
{
    private readonly AccountRepository _accountRepository;

    public SignUpRequestValidator(AccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    protected override void AddRules(SignUpRequest request)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Please fill email.")
            .Must(email => _accountRepository.FindByEmail(request.Email?.Trim()) == null)
            .WithMessage("Email already existed.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Please fill password.")
            .Must(password => password.Length >= 8 && password.Length <= 32)
            .WithMessage("Password must between 8 - 32 characters.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Please fill company name.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Please fill address.");

        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("Please fill contact name.");

        RuleFor(x => x.ContactTitle)
            .NotEmpty().WithMessage("Please fill contact title.");
    }
}