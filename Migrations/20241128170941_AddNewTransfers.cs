using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLICGL_IBT_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTransfers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    TransferId = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransferAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FromBankAccount = table.Column<string>(type: "nvarchar(8)", nullable: false),
                    ToBankAccount = table.Column<string>(type: "nvarchar(8)", nullable: false),
                    TransferMethod = table.Column<string>(type: "nvarchar(12)", nullable: false),
                    IsApproved = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(7)", nullable: true),
                    IsDeleted = table.Column<int>(type: "int", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(7)", nullable: true),
                    AccountBalance = table.Column<string>(type: "nvarchar(14)", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(7)", nullable: false),
                    Payment = table.Column<string>(type: "nvarchar(7)", nullable: true),
                    IBTSheet = table.Column<string>(type: "nvarchar(13)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.TransferId);
                    table.ForeignKey(
                        name: "FK_Transfers_AccountBalance_AccountBalance",
                        column: x => x.AccountBalance,
                        principalTable: "AccountBalance",
                        principalColumn: "BalanceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_BankAccount_FromBankAccount",
                        column: x => x.FromBankAccount,
                        principalTable: "BankAccount",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_BankAccount_ToBankAccount",
                        column: x => x.ToBankAccount,
                        principalTable: "BankAccount",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_IBTSheet_IBTSheet",
                        column: x => x.IBTSheet,
                        principalTable: "IBTSheet",
                        principalColumn: "SheetID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_Payment_Payment",
                        column: x => x.Payment,
                        principalTable: "Payment",
                        principalColumn: "PaymentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_TransferMethod_TransferMethod",
                        column: x => x.TransferMethod,
                        principalTable: "TransferMethod",
                        principalColumn: "MethodID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_Users_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BalanceAdjustment",
                columns: table => new
                {
                    AdjustmentID = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    AdjustedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdjustedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdjustBy = table.Column<string>(type: "nvarchar(7)", nullable: false),
                    IsReversed = table.Column<int>(type: "int", nullable: false),
                    ReversedBy = table.Column<string>(type: "nvarchar(7)", nullable: true),
                    Balance = table.Column<string>(type: "nvarchar(14)", nullable: false),
                    TransferID = table.Column<string>(type: "nvarchar(12)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceAdjustment", x => x.AdjustmentID);
                    table.ForeignKey(
                        name: "FK_BalanceAdjustment_AccountBalance_Balance",
                        column: x => x.Balance,
                        principalTable: "AccountBalance",
                        principalColumn: "BalanceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BalanceAdjustment_Transfers_TransferID",
                        column: x => x.TransferID,
                        principalTable: "Transfers",
                        principalColumn: "TransferId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BalanceAdjustment_Users_AdjustBy",
                        column: x => x.AdjustBy,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BalanceAdjustment_Users_ReversedBy",
                        column: x => x.ReversedBy,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundRequestAdjustments",
                columns: table => new
                {
                    AdjustmentID = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    AdjustedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdjustedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdjustBy = table.Column<string>(type: "nvarchar(7)", nullable: false),
                    IsReversed = table.Column<int>(type: "int", nullable: false),
                    ReversedBy = table.Column<string>(type: "nvarchar(7)", nullable: true),
                    RequestID = table.Column<string>(type: "nvarchar(12)", nullable: false),
                    TransferID = table.Column<string>(type: "nvarchar(12)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundRequestAdjustments", x => x.AdjustmentID);
                    table.ForeignKey(
                        name: "FK_FundRequestAdjustments_FundRequest_RequestID",
                        column: x => x.RequestID,
                        principalTable: "FundRequest",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundRequestAdjustments_Transfers_TransferID",
                        column: x => x.TransferID,
                        principalTable: "Transfers",
                        principalColumn: "TransferId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundRequestAdjustments_Users_AdjustBy",
                        column: x => x.AdjustBy,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundRequestAdjustments_Users_ReversedBy",
                        column: x => x.ReversedBy,
                        principalTable: "Users",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "RepoBalanceAdjustment",
                columns: table => new
                {
                    AdjustmentID = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    AdjustedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdjustedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdjustBy = table.Column<string>(type: "nvarchar(7)", nullable: false),
                    IsReversed = table.Column<int>(type: "int", nullable: false),
                    ReversedBy = table.Column<string>(type: "nvarchar(7)", nullable: true),
                    Repo = table.Column<string>(type: "nvarchar(14)", nullable: false),
                    TransferID = table.Column<string>(type: "nvarchar(12)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepoBalanceAdjustment", x => x.AdjustmentID);
                    table.ForeignKey(
                        name: "FK_RepoBalanceAdjustment_Repo_Repo",
                        column: x => x.Repo,
                        principalTable: "Repo",
                        principalColumn: "RepoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RepoBalanceAdjustment_Transfers_TransferID",
                        column: x => x.TransferID,
                        principalTable: "Transfers",
                        principalColumn: "TransferId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RepoBalanceAdjustment_Users_AdjustBy",
                        column: x => x.AdjustBy,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RepoBalanceAdjustment_Users_ReversedBy",
                        column: x => x.ReversedBy,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            
            migrationBuilder.CreateIndex(
                name: "IX_BalanceAdjustment_AdjustBy",
                table: "BalanceAdjustment",
                column: "AdjustBy");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceAdjustment_Balance",
                table: "BalanceAdjustment",
                column: "Balance");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceAdjustment_ReversedBy",
                table: "BalanceAdjustment",
                column: "ReversedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceAdjustment_TransferID",
                table: "BalanceAdjustment",
                column: "TransferID");

            
            migrationBuilder.CreateIndex(
                name: "IX_FundRequestAdjustments_AdjustBy",
                table: "FundRequestAdjustments",
                column: "AdjustBy");

            migrationBuilder.CreateIndex(
                name: "IX_FundRequestAdjustments_RequestID",
                table: "FundRequestAdjustments",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_FundRequestAdjustments_ReversedBy",
                table: "FundRequestAdjustments",
                column: "ReversedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FundRequestAdjustments_TransferID",
                table: "FundRequestAdjustments",
                column: "TransferID",
                unique: true,
                filter: "[TransferID] IS NOT NULL");

            

            migrationBuilder.CreateIndex(
                name: "IX_RepoBalanceAdjustment_AdjustBy",
                table: "RepoBalanceAdjustment",
                column: "AdjustBy");

            migrationBuilder.CreateIndex(
                name: "IX_RepoBalanceAdjustment_Repo",
                table: "RepoBalanceAdjustment",
                column: "Repo");

            migrationBuilder.CreateIndex(
                name: "IX_RepoBalanceAdjustment_ReversedBy",
                table: "RepoBalanceAdjustment",
                column: "ReversedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RepoBalanceAdjustment_TransferID",
                table: "RepoBalanceAdjustment",
                column: "TransferID");

            
            migrationBuilder.CreateIndex(
                name: "IX_Transfers_AccountBalance",
                table: "Transfers",
                column: "AccountBalance");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ApprovedBy",
                table: "Transfers",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_CreateBy",
                table: "Transfers",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_DeletedBy",
                table: "Transfers",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_FromBankAccount",
                table: "Transfers",
                column: "FromBankAccount");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_IBTSheet",
                table: "Transfers",
                column: "IBTSheet");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_Payment",
                table: "Transfers",
                column: "Payment");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ToBankAccount",
                table: "Transfers",
                column: "ToBankAccount");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TransferMethod",
                table: "Transfers",
                column: "TransferMethod");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
