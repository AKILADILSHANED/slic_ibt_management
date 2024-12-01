﻿using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DeleteBalanceDTO
    {
        [Required]
        public String BalanceID { get; set; }

        [Required]
        public DateTime BalanceDate { get; set; }

        [Required]
        [RegularExpression(@"^-?\d+(\.\d+)?$")]
        public decimal BalanceAmount { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String AccountNumber { get; set; }

        [Required]
        public String BankName { get; set; }

        [Required]
        public int IsDeleted { get; set; }

    }
}
