using AutoPartsStore.Model.Auth;
using Microsoft.AspNetCore.Identity;

namespace AutoPartsStore.Services.Auth
{
    public class AccountService(AccountRepository accountRepository, JwtService jwtService)
    {
        public void Register(string userName, string firstName, string password)
        {
            var account = new Account
            {
                UserName = userName,
                FirstName = firstName,
                ID = Guid.NewGuid(),

            };
            var passHash = new PasswordHasher<Account>().HashPassword(account, password);
            account.PasswordHash = passHash;
            accountRepository.Add(account);
        }

        public string Login(string userName, string password)
        {
            var account = accountRepository.GetByUserName(userName);
            var result = new PasswordHasher<Account>()
                .VerifyHashedPassword(
                    account, account.PasswordHash, password);
            if (result == PasswordVerificationResult.Success)
            {
                return jwtService.GenerateToken(account);
            }
            else
            {
                throw new Exception("Unauthorized");
            }
        }
    }
}
