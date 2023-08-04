using API.Entities;
using API.Models.Exceptions;
using API.Models.Requests;
using API.Models.Responses;
using API.Repositories;
using API.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController
{
    private readonly AccountRepository _accountRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public AccountController(AccountRepository accountRepository, IMapper mapper, CustomerRepository customerRepository)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
        _customerRepository = customerRepository;
    }

    [HttpGet]
    [Route("{id}")]
    public AccountResponse Profile(int id)
    {
        Account account = _accountRepository.GetOne(id);
        AccountResponse accountResponse = _mapper.Map<AccountResponse>(account);
        accountResponse.Customer = _mapper.Map<CustomerResponse>(account.Customer);
        return accountResponse;
    }

    [HttpPatch]
    [Route("{id}/Profile")]
    public AccountResponse UpdateProfile(int id, UpdateProfileRequest request)
    {
        Account account = _accountRepository.GetOne(id);
        Customer? customer = account.Customer;

        customer.CompanyName = request.CompanyName;
        customer.ContactName = request.ContactName;
        customer.ContactTitle = request.ContactTitle;
        customer.Address = request.Address;

        _customerRepository.Update(customer);

        account.Email = request.Email;
        _accountRepository.Update(account);

        return _mapper.Map<AccountResponse>(account);
    }

    [HttpPatch]
    [Route("{id}/Password")]
    public AccountResponse UpdatePassword(int id, UpdatePasswordRequest request)
    {
        Account account = _accountRepository.GetOne(id);
        
        account.Password = Encryptions.Encrypt(request.NewPassword);
        _accountRepository.Update(account);

        return _mapper.Map<AccountResponse>(account);
    }

    [HttpPost]
    [Route("Login")]
    public AuthUser Login(LoginRequest request)
    {
        Account? account = _accountRepository.FindByEmail(request.Email.Trim());
        account = GetTokenizedAccount(account);

        return _mapper.Map<AuthUser>(account);
    }

    [HttpPost]
    [Route("{id}/RefreshToken")]
    public AuthUser Login(int id, string? accessToken)
    {
        Account? account = _accountRepository.GetOne(id);
        if (account.AccessToken != null && !account.AccessToken.Equals(accessToken))
        {
            throw ApiException.Unauthorized("Invalid token.");
        }
        account = GetTokenizedAccount(account);

        return _mapper.Map<AuthUser>(account);
    }

    private Account GetTokenizedAccount(Account account)
    {
        var claims = TokenUtils.GetClaimsOf(account, DateTime.Now.AddMinutes(1));

        var newToken = TokenUtils.GenerateAccessToken(claims);
        var newRefreshToken = TokenUtils.GenerateRefreshToken();
        account.AccessToken = newToken;
        account.RefreshToken = newRefreshToken;
        account.TokenExpireAt = DateTime.Now.AddDays(1);
        return _accountRepository.Update(account);
    }
    
    [HttpPost]
    [Route("SignUp")]
    public void Register(SignUpRequest request)
    {
        var cus = new Customer
        {
            CustomerId = Strings.GenerateRandomStringWithLength(5),
            CompanyName = request.CompanyName,
            ContactName = request.ContactName,
            ContactTitle = request.ContactTitle,
            Address = request.Address
        };
        var newAcc = new Account
        {
            Email = request.Email,
            Password = Encryptions.Encrypt(request.Password),
            CustomerId = cus.CustomerId,
            Role = 2
        };
        _customerRepository.Add(cus);
        _accountRepository.Add(newAcc);
        Mails.SendTo(request.Email, "Welcome to our website", $"Welcome to our website");
    }

    [HttpPost]
    [Route("Logout/{id}")]
    public AuthUser Logout(int id)
    {
        Account? account = _accountRepository.GetOne(id);
        account.AccessToken = null;
        account.RefreshToken = null;
        account.TokenExpireAt = null;
        _accountRepository.Update(account);

        return _mapper.Map<AuthUser>(account);
    }

    [HttpPost]
    [Route("ForgotPassword")]
    public AuthUser ForgotPassword(ForgotPasswordRequest request)
    {
        Account? account = _accountRepository.FindByEmail(request.Email);
        
        string newPassword = Utils.Strings.GenerateRandomStringWithLength(12);
        account.Password = Encryptions.Encrypt(newPassword);
        _accountRepository.Update(account);

        Mails.SendTo(request.Email, "Forgot password request", $"Your new passsword is: {newPassword}");
        return _mapper.Map<AuthUser>(account);
    }

}