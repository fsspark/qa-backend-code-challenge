

using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Services;
using Moq;

namespace Betsson.OnlineWallets.Tests.src.test;

[TestClass]
public class BetssonOnlineWallet_BalanceTests{
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
}