using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Services;
using Moq;

namespace Betsson.OnlineWallets.Tests.src.test.WithdrawFundsTests;

[TestClass]
public class BetssonOnlineWalletService_WithdrawFundsTests
{
    private OnlineWalletService? onlineWalletService;
    private Mock<IOnlineWalletRepository>? mockOnlineWalletRepository;

    [TestInitialize]
    public void Setup()
    {
        mockOnlineWalletRepository = new Mock<IOnlineWalletRepository>();
        onlineWalletService = new OnlineWalletService(mockOnlineWalletRepository.Object);
    }

    [TestMethod]
    public async Task WithdrawFundsAsync_WithdrawAmountFromWallet_ReturnCorrectBalance()
    {
        //Arrange
        Withdrawal withdrawal = BuildWithdrawal(30);
        decimal withdrawalAmount = withdrawal.Amount;
        withdrawalAmount *= -1;
        OnlineWalletEntry onlineWalletEntry = BuildDefaultOnlineWalletEntry();
        mockOnlineWalletRepository
        .Setup(walletBalance => walletBalance.GetLastOnlineWalletEntryAsync())
        .ReturnsAsync(onlineWalletEntry);
        Balance currentBalance = await onlineWalletService.GetBalanceAsync();
        OnlineWalletEntry withdrawalOnlineWalletEntry =
        BuildOnlineWalletWithdrawalEntry(withdrawalAmount, currentBalance);
        Balance expectedBalance = BuildExpectedBalance(currentBalance, withdrawalAmount);
        mockOnlineWalletRepository
        .Setup(withdrawalEntry => withdrawalEntry.InsertOnlineWalletEntryAsync(withdrawalOnlineWalletEntry));

        //Act
        Balance actualBalance = await onlineWalletService.WithdrawFundsAsync(withdrawal);

        //Assert
        Assert.IsNotNull(actualBalance);
        Assert.AreEqual(expectedBalance.Amount, actualBalance.Amount);
    }

    private static OnlineWalletEntry BuildDefaultOnlineWalletEntry()
    {
        Random rnd = new();
        OnlineWalletEntry onlineWalletEntry = new()
        {
            Id = "ENTR00" + rnd + DateTime.Now,
            EventTime = DateTimeOffset.UtcNow,
            Amount = 100,
            BalanceBefore = 100
        };

        return onlineWalletEntry;
    }

    private static OnlineWalletEntry BuildOnlineWalletWithdrawalEntry(decimal withdrawalAmount, Balance balance)
    {
        return new()
        {
            Amount = withdrawalAmount,
            BalanceBefore = balance.Amount,
            EventTime = DateTimeOffset.UtcNow
        };

    }

    private static Withdrawal BuildWithdrawal(decimal withdrawalAmount)
    {
        return new()
        {
            Amount = withdrawalAmount
        };
    }

    private static Balance BuildExpectedBalance(Balance balance, decimal withdrawalAmount)
    {
        return new()
        {
            Amount = balance.Amount + withdrawalAmount
        };
    }
}