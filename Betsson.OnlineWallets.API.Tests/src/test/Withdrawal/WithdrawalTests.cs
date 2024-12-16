using System.Net;
using Betsson.OnlineWallets.API.Tests.src.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Betsson.OnlineWallets.API.Tests.src.test.Withdrawal;

[TestClass]
public class WithdrawalTests : BaseTest
{
    [TestMethod]
    public async Task Withdrawal_NoPreviousBalanceEmptyBody_BalanceZero()
    {
        MakeBalanceZero();
        var withdrawalRequest = new RestRequest(WIDTHDRAWAL_ENDPOINT);
        withdrawalRequest.AddJsonBody(new
        {
        });

        var response = await restClient.PostAsync(withdrawalRequest);
        var responseData = JsonConvert.DeserializeObject<Balance>(response.Content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(0, responseData.Amount);
    }

    [TestMethod]
    public async Task Withdrawal_WithdrawalAmountLessThanBalance_CorrectBalance()
    {
        decimal withdrawAmount = 100;
        decimal depositAmount = 300;
        var expectedBalance = depositAmount - withdrawAmount;
        MakeBalanceZero();
        MakeDeposit(depositAmount);
        var withdrawalRequest = new RestRequest(WIDTHDRAWAL_ENDPOINT);
        withdrawalRequest.AddJsonBody(new
        {
            Amount = withdrawAmount
        });

        var response = await restClient.PostAsync(withdrawalRequest);
        var responseData = JsonConvert.DeserializeObject<Balance>(response.Content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(expectedBalance, responseData.Amount);
    }

    [TestMethod]
    public async Task Withdrawal_WithdrawalAmountZero_PreviousBalance()
    {
        decimal withdrawAmount = 0;
        decimal depositAmount = 300;
        MakeBalanceZero();
        MakeDeposit(depositAmount);
        var withdrawalRequest = new RestRequest(WIDTHDRAWAL_ENDPOINT);
        withdrawalRequest.AddJsonBody(new
        {
            Amount = withdrawAmount
        });

        var response = await restClient.PostAsync(withdrawalRequest);
        var responseData = JsonConvert.DeserializeObject<Balance>(response.Content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(depositAmount, responseData.Amount);
    }
    
    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task Withdrawal_NegativeAmount_BadRequest()
    {
        var expectedExceptionMessage = "'Amount' must be greater than or equal to '0'.";
        decimal withdrawalAmount = -1;
        MakeBalanceZero();
        var withdrawalRequest = new RestRequest(WIDTHDRAWAL_ENDPOINT);
        withdrawalRequest.AddJsonBody(new
        {
            Amount = withdrawalAmount
        });

        var response = await restClient.PostAsync(withdrawalRequest);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual(expectedExceptionMessage, response.ErrorMessage);
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task Withdrawal_MissingRequiredFields_BadRequest()
    {
        var expectedExceptionMessage = "The withdrawalRequest field is required.";
        MakeBalanceZero();
        var withdrawalRequest = new RestRequest(WIDTHDRAWAL_ENDPOINT);
        withdrawalRequest.AddJsonBody(new
        {
            Amount = ""
        });

        var response = await restClient.PostAsync(withdrawalRequest);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual(expectedExceptionMessage, response.ErrorMessage);
    }    

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task Withdrawal_WithdrawalAmountGreaterThanBalance_BadRequest()
    {
        var expectedExceptionMessage = "Invalid withdrawal amount. There are insufficient funds.";
        decimal withdrawalAmount = 600;
        decimal depositAmount = 200;
        MakeBalanceZero();
        MakeDeposit(depositAmount);
        var withdrawalRequest = new RestRequest(WIDTHDRAWAL_ENDPOINT);
        withdrawalRequest.AddJsonBody(new
        {
            Amount = withdrawalAmount
        });

        var response = await restClient.PostAsync(withdrawalRequest);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual(expectedExceptionMessage, response.ErrorMessage);
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task Withdrawal_WrongMethod_MethodNotAllowedException()
    {
        var withdrawalRequest = new RestRequest(WIDTHDRAWAL_ENDPOINT);

        var response = await restClient.GetAsync(withdrawalRequest);

        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }   
}
