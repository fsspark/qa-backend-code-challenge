using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Services;
using Moq;

namespace Betsson.OnlineWallets.Tests.src.test.DepositFundsTests;

[TestClass]
public class BetssonOnlineWalletService_DepositFundsTests
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
    public async Task DepositFundsAsync_DepositFundsToWallet_ReturnCorrectBalance()
    {
        //Arrange
        Deposit deposit = BuildDeposit(10);
        OnlineWalletEntry walletEntry = BuildDefaultOnlineWalletEntry();
        mockOnlineWalletRepository
        .Setup(walletRepository => walletRepository.GetLastOnlineWalletEntryAsync())
        .ReturnsAsync(walletEntry);
        Balance balance = await onlineWalletService.GetBalanceAsync();
        OnlineWalletEntry walletDepositEntry = BuildOnlieWalletDepositEntry(deposit, balance);
        mockOnlineWalletRepository
        .Setup(depositEntry => depositEntry.InsertOnlineWalletEntryAsync(walletDepositEntry));
        Balance expectedBalance = BuildExpectedBalance(balance, deposit);

        //Act
        Balance result = await onlineWalletService.DepositFundsAsync(deposit);

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedBalance.Amount, result.Amount);
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

    private static OnlineWalletEntry BuildOnlieWalletDepositEntry(Deposit deposit, Balance balance)
    {
        return new()
        {
            Amount = deposit.Amount,
            BalanceBefore = balance.Amount,
            EventTime = DateTimeOffset.UtcNow
        };
    }
    private static Deposit BuildDeposit(int depositAmount)
    {
        return new()
        {
            Amount = depositAmount
        };
    }

    private static Balance BuildExpectedBalance(Balance balance, Deposit deposit)
    {
        return new()
        {
            Amount = balance.Amount + deposit.Amount
        };
    }

}