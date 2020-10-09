using CockroachDbLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CockroachDbLib.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly CockroachDbContext dbContext;

        public AccountRepository(CockroachDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<Account> GetAllAccounts()
        {
            return dbContext.accounts.ToList();
        }

        public async Task<Account> GetAccountById(int? id)
        {
            Account account = await dbContext.accounts.FindAsync(id);

            if (account == null)
            {
                return null;
            }

            return account;
        }

        public async Task CreateAccount(Account account)
        {
            await dbContext.accounts.AddAsync(account);
            dbContext.SaveChanges();
        }

        public int GetLastAccountId()
        {
            Account lastAccount = dbContext.accounts
                .OrderByDescending(t => t.id)
                .First();
            return lastAccount.id;
        }

        public void UpdateAccount(Account account)
        {
            var local = dbContext.Set<Account>()
                .Local
                .FirstOrDefault(entry => entry.id.Equals(account.id));

            if (local != null)
            {
                // detach
                dbContext.Entry(local).State = EntityState.Detached;
            }

            // set Modified flag in your entry
            dbContext.Entry(account).State = EntityState.Modified;

            dbContext.accounts.Update(account);
            dbContext.SaveChanges();
        }
    }
}
