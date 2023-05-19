using CRL.Model.FS;
using CRL.Model.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CRL.EmailService
{
    public class EmailDbContext:DbContext
    {
        public EmailDbContext()
            : base("name=CBLDataContext")
        {

            Database.SetInitializer<EmailDbContext>(null);
        }

        public DbSet<Email> Emails { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<FinancialStatement>();
        }
    }
}
