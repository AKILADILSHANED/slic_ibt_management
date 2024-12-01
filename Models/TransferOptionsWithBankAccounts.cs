using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class TransferOptionsWithBankAccounts
    {
        public Dictionary<String, BankAccountForTransferMethod>? BankAccountList { get; set; }

        public Dictionary<String, TransferOptionsForTransferMethod>? TransferOptionList { get; set; }

        [Required]
        public String SendingAccountID { get; set; }

        [Required]
        public String OptionID { get; set; }
    }
}
