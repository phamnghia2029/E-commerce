using API.Entities;

namespace API.Repositories;

public class AccountRepository : BaseRepository<Account, int>
{
    protected override int GetId(Account entity)
    {
        return entity.AccountId;
    }

    public override void PreLoad()
    {
        this.DbContext.Customers.ToList();
            
    }

    public AccountRepository(PRN221_DBContext context) : base(context)
    {
    }

    public Account? FindByEmail(string email)
    {
        return this.DbContext.Accounts.FirstOrDefault(x => x.Email.Equals(email.Trim()));
    }
}