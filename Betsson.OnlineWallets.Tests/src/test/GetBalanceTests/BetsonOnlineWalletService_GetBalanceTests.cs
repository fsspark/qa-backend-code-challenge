

using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Services;
using Moq;

namespace Betsson.OnlineWallets.Tests.src.test.GetBalanceTests;

[TestClass]
public class BetssonOnlineWalletService_GetBalanceTests{
    private OnlineWalletService? onlineWalletService;
    private Mock<IOnlineWalletRepository>? mockOnlineWalletRepository;

    [TestInitialize]
    public void Setup(){
        mockOnlineWalletRepository = new Mock<IOnlineWalletRepository>();
        onlineWalletService = new OnlineWalletService(mockOnlineWalletRepository.Object);
    }

    [TestMethod]
    public async Task GetBalanceAsync_NoTransactions_ReturnZero(){
        //Arrange - Simulate no transactions
        mockOnlineWalletRepository
        .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
        .ReturnsAsync((OnlineWalletEntry)null); 
        
        //Act
        Balance result = await onlineWalletService.GetBalanceAsync();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Amount);
    }

    [TestMethod]
    public async Task GetBalanceAsync_WithTransactions_ReturnCorrectBalance(){
        //Arrange
        OnlineWalletEntry lastEntry = BuildOnlineEntry(50, 150);
        mockOnlineWalletRepository
        .Setup(walletRepository => walletRepository.GetLastOnlineWalletEntryAsync())
        .ReturnsAsync(lastEntry);

        //Act
        Balance result = await onlineWalletService.GetBalanceAsync();

        //Assert
        Assert.AreEqual(lastEntry.Amount + lastEntry.BalanceBefore, result.Amount);
    }

    [TestMethod]
    public async Task GetBalanceASync_WithNegativeAmount_ReturnCorrectBalance(){
                //Arrange
        OnlineWalletEntry lastEntry = BuildOnlineEntry(-50, 150);
        mockOnlineWalletRepository
        .Setup(walletRepository => walletRepository.GetLastOnlineWalletEntryAsync())
        .ReturnsAsync(lastEntry);

        //Act
        Balance result = await onlineWalletService.GetBalanceAsync();

        //Assert
        Assert.AreEqual(lastEntry.Amount + lastEntry.BalanceBefore, result.Amount);
    }

    private OnlineWalletEntry BuildOnlineEntry(int balanceBefore, int amount){
        Random rnd = new Random();
        OnlineWalletEntry onlineWalletEntry = new OnlineWalletEntry();

        onlineWalletEntry.Id = "ENTR00" + rnd + DateTime.Now;
        onlineWalletEntry.EventTime = DateTimeOffset.UtcNow;
        onlineWalletEntry.Amount = amount;
        onlineWalletEntry.BalanceBefore = balanceBefore;

        return onlineWalletEntry;
    }
}