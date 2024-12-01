
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using SLICGL_IBT_Management.Models;

namespace SLICGL_IBT_Management.DB_Connection
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserCategory> UserCategory { get; set; }
        public DbSet<UserCategoryAvailability> UserCategoryAvailability { get; set; }
        public DbSet<Bank> Bank { get; set; }
        public DbSet<BankAccount> BankAccount { get; set; }
        public DbSet<AccountBalance> AccountBalance { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<FundRequest> FundRequest { get; set; }
        public DbSet<Repo> Repo { get; set; }
        public DbSet<TransferOption> TransferOption { get; set; }
        public DbSet<TransferMethod> TransferMethod { get; set; }
        public DbSet<IBTSheet> IBTSheet { get; set; }
        public DbSet<Transfers> Transfers { get; set; }
        public DbSet<BalanceAdjustment> BalanceAdjustment { get; set; }
        public DbSet<RepoBalanceAdjustment> RepoBalanceAdjustment { get; set; }
        public DbSet<FundRequestAdjustments> FundRequestAdjustments { get; set; }
        public DbSet<OverdraftRecoverAdjustment> OverdraftRecoverAdjustment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(User =>
            {
                User.HasKey(key => key.User_Id); 
                User.Property(key => key.User_Id)
                .IsRequired()  // Nullabe == false..
                .HasMaxLength(7); // Settingup max length

            }
            );

            modelBuilder.Entity<UserCategoryAvailability>()
                .HasKey(uca => new { uca.User_Id, uca.CategoryID });


            modelBuilder.Entity<Bank>(Bank =>
            {
                Bank.HasKey(key => key.BankID); 
                Bank.Property(key => key.BankID)
                .IsRequired()  // Nullabe == false..
                .HasMaxLength(7); // Settingup max length

            }
            );

            modelBuilder.Entity<BankAccount>(BankAccount =>
            {
                BankAccount.HasKey(key => key.AccountID); 
                BankAccount.Property(key => key.AccountID)
                .IsRequired()  // Nullabe == false..
                .HasMaxLength(8); // Settingup max length
               
                modelBuilder.Entity<BankAccount>()
                .HasOne(usr => usr.Creator)
                .WithMany()
                .HasForeignKey(usr => usr.User_Id)
                .OnDelete(DeleteBehavior.Restrict);
            }
            );

            modelBuilder.Entity<AccountBalance>(AccountBalance =>
            {
                AccountBalance.HasKey(key => key.BalanceID); 
                AccountBalance.Property(key => key.BalanceID)
                .IsRequired()  // Nullabe == false..
                .HasMaxLength(14); // Settingup max length

                modelBuilder.Entity<AccountBalance>()
                .HasOne(Acc => Acc.IDUser)
                .WithMany()
                .HasForeignKey(Acc => Acc.User_Id)
                .OnDelete(DeleteBehavior.Restrict);

            }
            );

            modelBuilder.Entity<Payment>(Payment =>
            {
                Payment.HasKey(key => key.PaymentID); // Set the primary key as User_Id...
                Payment.Property(key => key.PaymentID)
                .IsRequired()  // Nullabe == false..
                .HasMaxLength(7); // Settingup max length

                modelBuilder.Entity<Payment>()
                .HasOne(pmnt => pmnt.IDUserRegistered)
                .WithMany()
                .HasForeignKey(pmnt => pmnt.RegisteredBy)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Payment>()
                    .HasOne(pmnt => pmnt.IDUserDelete)
                    .WithMany()
                    .HasForeignKey(pmnt => pmnt.DeletedBy)
                    .OnDelete(DeleteBehavior.Restrict);

            }
            );

            modelBuilder.Entity<FundRequest>(FundRequest =>
            {
                FundRequest.HasKey(key => key.RequestID); // Set the primary key as User_Id...
                FundRequest.Property(key => key.RequestID)
                .IsRequired()  // Nullabe == false..
                .HasMaxLength(12); // Settingup max length

                modelBuilder.Entity<FundRequest>()
                .HasOne(fr => fr.Approver).WithMany().HasForeignKey(fr => fr.ApprovedBy).OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<FundRequest>()
                    .HasOne(req => req.Requester).WithMany().HasForeignKey(req => req.RequestBy)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<FundRequest>()
                    .HasOne(del => del.Deleter).WithMany().HasForeignKey(del => del.DeletedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<FundRequest>()
                    .HasOne(acc => acc.RequestedAccount).WithMany()
                    .HasForeignKey(acc => acc.AccountID)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<FundRequest>()
                .HasOne(pmnt => pmnt.PaymentRequest)
                .WithMany()
                .HasForeignKey(pmnt => pmnt.PaymentID)
                .OnDelete(DeleteBehavior.Restrict);

            }
            );

            modelBuilder.Entity<Repo>(Repo =>
            {
                Repo.HasKey(key => key.RepoID);
                Repo.Property(key => key.RepoID)
                .IsRequired()  
                .HasMaxLength(14); 

                modelBuilder.Entity<Repo>()
                .HasOne(usr => usr.Creator)
                .WithMany()
                .HasForeignKey(usr => usr.CreateBy)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Repo>()
                .HasOne(acc => acc.Account)
                .WithMany()
                .HasForeignKey(acc => acc.AccountID)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Repo>()
                .HasOne(usr => usr.Deleter)
                .WithMany()
                .HasForeignKey(usr => usr.DeleteBy)
                .OnDelete(DeleteBehavior.Restrict);
            }
            );

            modelBuilder.Entity<TransferOption>(TransferOption =>
            {
                TransferOption.HasKey(key => key.OptionID);
                TransferOption.Property(key => key.OptionID)
                .IsRequired()
                .HasMaxLength(7);

                modelBuilder.Entity<TransferOption>()
                .HasOne(usr => usr.Creator)
                .WithMany()
                .HasForeignKey(usr => usr.CreateBy)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<TransferOption>()
                .HasOne(usr => usr.Deleter)
                .WithMany()
                .HasForeignKey(usr => usr.DeletedBy)
                .OnDelete(DeleteBehavior.Restrict);

            }
            );

            modelBuilder.Entity<TransferMethod>(TransferMethod =>
            {
                TransferMethod.HasKey(key => key.MethodID);
                TransferMethod.Property(key => key.MethodID)
                .IsRequired()
                .HasMaxLength(12);

                modelBuilder.Entity<TransferMethod>()
                .HasOne(Acc => Acc.AccountSender)
                .WithMany()
                .HasForeignKey(Acc => Acc.SendingAccount)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<TransferMethod>()
                .HasOne(Acc => Acc.AccountReceiver)
                .WithMany()
                .HasForeignKey(Acc => Acc.ReceivingAccount)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<TransferMethod>()
                .HasOne(optn => optn.Option)
                .WithMany()
                .HasForeignKey(optn => optn.TransferOption)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<TransferMethod>()
                .HasOne(Usr => Usr.Creator)
                .WithMany()
                .HasForeignKey(Usr => Usr.CreateBy)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<TransferMethod>()
                .HasOne(Usr => Usr.Deleter)
                .WithMany()
                .HasForeignKey(Usr => Usr.DeletedBy)
                .OnDelete(DeleteBehavior.Restrict);

            }
            );

            modelBuilder.Entity<IBTSheet>(Sheet =>
            {
                Sheet.HasKey(key => key.SheetID);
                Sheet.Property(key => key.SheetID)
                .IsRequired()
                .HasMaxLength(13);

                modelBuilder.Entity<IBTSheet>()
                .HasOne(usr => usr.Creator)
                .WithMany()
                .HasForeignKey(usr => usr.CreateBy)
                .OnDelete(DeleteBehavior.Restrict);

            }
            );

            modelBuilder.Entity<Transfers>(Transfers =>
            {
                Transfers.HasKey(key => key.TransferId);
                Transfers.Property(key => key.TransferId)
                .IsRequired()
                .HasMaxLength(12);

                modelBuilder.Entity<Transfers>()
                .HasOne(fromaccount => fromaccount.FromAccount)
                .WithMany()
                .HasForeignKey(fromaccount => fromaccount.FromBankAccount)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Transfers>()
                .HasOne(toaccount => toaccount.ToAccount)
                .WithMany()
                .HasForeignKey(toaccount => toaccount.ToBankAccount)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Transfers>()
                .HasOne(mthd => mthd.Method)
                .WithMany()
                .HasForeignKey(mthd => mthd.TransferMethod)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Transfers>()
                .HasOne(approve => approve.Approver)
                .WithMany()
                .HasForeignKey(approve => approve.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Transfers>()
                .HasOne(delete => delete.Deleter)
                .WithMany()
                .HasForeignKey(delete => delete.DeletedBy)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Transfers>()
                .HasOne(balance => balance.Balance)
                .WithMany()
                .HasForeignKey(balance => balance.AccountBalance)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

                modelBuilder.Entity<Transfers>()
                .HasOne(usr => usr.Creator)
                .WithMany()
                .HasForeignKey(usr => usr.CreateBy)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Transfers>()
                .HasOne(pmnt => pmnt.TransferPayment)
                .WithMany()
                .HasForeignKey(pmnt => pmnt.Payment)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

                modelBuilder.Entity<Transfers>()
                .HasOne(sht => sht.Sheet)
                .WithMany()
                .HasForeignKey(sht => sht.IBTSheet)
                .OnDelete(DeleteBehavior.Restrict);

            }
            );

            modelBuilder.Entity<BalanceAdjustment>(adjustmnt =>
            {
                adjustmnt.HasKey(key => key.AdjustmentID);
                adjustmnt.Property(key => key.AdjustmentID)
                .IsRequired()
                .HasMaxLength(18);

                modelBuilder.Entity<BalanceAdjustment>()
                .HasOne(usr => usr.Adjuster)
                .WithMany()
                .HasForeignKey(usr => usr.AdjustBy)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<BalanceAdjustment>()
                .HasOne(bal => bal.BalanceID)
                .WithMany()
                .HasForeignKey(bal => bal.Balance)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<BalanceAdjustment>()
                .HasOne(usr => usr.ReversedUser)
                .WithMany()
                .HasForeignKey(usr => usr.ReversedBy)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<BalanceAdjustment>()
                .HasOne(tfr => tfr.Transfer)
                .WithMany()
                .HasForeignKey(tfr => tfr.TransferID)
                .OnDelete(DeleteBehavior.Restrict);

            }
            );

            modelBuilder.Entity<RepoBalanceAdjustment>(adjustmnt =>
            {
                adjustmnt.HasKey(key => key.AdjustmentID);
                adjustmnt.Property(key => key.AdjustmentID)
                .IsRequired()
                .HasMaxLength(17);

                modelBuilder.Entity<RepoBalanceAdjustment>()
                .HasOne(usr => usr.Adjuster)
                .WithMany()
                .HasForeignKey(usr => usr.AdjustBy)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<RepoBalanceAdjustment>()
                .HasOne(repo => repo.IDRepo)
                .WithMany()
                .HasForeignKey(repo => repo.Repo)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<RepoBalanceAdjustment>()
                .HasOne(usr => usr.ReversedUser)
                .WithMany()
                .HasForeignKey(usr => usr.ReversedBy)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<RepoBalanceAdjustment>()
                .HasOne(tfr => tfr.Transfer)
                .WithMany()
                .HasForeignKey(tfr => tfr.TransferID)
                .OnDelete(DeleteBehavior.Restrict);

            }
            );

            modelBuilder.Entity<FundRequestAdjustments>(adjustmnt =>
            {
                adjustmnt.HasKey(key => key.AdjustmentID);
                adjustmnt.Property(key => key.AdjustmentID)
                .IsRequired()
                .HasMaxLength(17);

                modelBuilder.Entity<FundRequestAdjustments>()
                .HasOne(req => req.Request)
                .WithMany(fra => fra.FundRequestAdjustments)
                .HasForeignKey(fra => fra.RequestID)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<FundRequestAdjustments>()
                .HasOne(tfr => tfr.Transfer)
                .WithOne(fra => fra.RequestAdjustment)
                .HasForeignKey<FundRequestAdjustments>(fra => fra.TransferID)
                .OnDelete(DeleteBehavior.Restrict);
            }
            );

            modelBuilder.Entity<OverdraftRecoverAdjustment>(adjustmnt =>
            {
                adjustmnt.HasKey(key => key.AdjustmentID);
                adjustmnt.Property(key => key.AdjustmentID)
                .IsRequired()
                .HasMaxLength(16);

                modelBuilder.Entity<OverdraftRecoverAdjustment>()
                .HasOne(req => req.Request)
                .WithOne(odra => odra.OverdraftRecoverAdjustment)
                .HasForeignKey<OverdraftRecoverAdjustment>(req => req.RequestID)
                .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<OverdraftRecoverAdjustment>()
                .HasOne(bal => bal.Balance)
                .WithOne(odra => odra.OverdraftRecoverAdjustment)
                .HasForeignKey<OverdraftRecoverAdjustment>(bal => bal.BalanceID)
                .OnDelete(DeleteBehavior.Restrict);
            }
            );
        }

    }
}
