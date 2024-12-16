using System.Net;
using Betsson.OnlineWallets.API.Tests.src.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Betsson.OnlineWallets.API.Tests.src.test.Deposit;

[TestClass]
public class MakeDepositTests : BaseTest
{
    [TestMethod]
    public async Task DepositFunds_NoPreviousTransaction_CorrectBalance()
    {
        decimal depositAmount = 150;
        MakeBalanceZero();
        var depositRequest = new RestRequest(DEPOSIT_ENDPOINT);
        depositRequest.AddJsonBody(new
        {
            Amount = depositAmount
        });

        var response = await restClient.PostAsync(depositRequest);
        var responseData = JsonConvert.DeserializeObject<Balance>(response.Content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(depositAmount, responseData.Amount);
    }

    [TestMethod]
    public async Task DepositFunds_PreviousTransaction_CorrectBalance()
    {
        decimal currentdepositAmount = 150;
        decimal previousDepositAmount = 100;
        decimal expectedBalance = currentdepositAmount + previousDepositAmount;
        MakeBalanceZero();
        MakeDeposit(previousDepositAmount);
        var depositRequest = new RestRequest(DEPOSIT_ENDPOINT);
        depositRequest.AddJsonBody(new
        {
            Amount = currentdepositAmount
        });

        var response = await restClient.PostAsync(depositRequest);
        var responseData = JsonConvert.DeserializeObject<Balance>(response.Content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(expectedBalance, responseData.Amount);
    }

    [TestMethod]
    public async Task DepositFunds_EmptyBody_PreviousBalance()
    {
        decimal depositAmount = 150;
        MakeBalanceZero();
        MakeDeposit(depositAmount);
        var depositRequest = new RestRequest(DEPOSIT_ENDPOINT);
        depositRequest.AddJsonBody(new
        {
        });

        var response = await restClient.PostAsync(depositRequest);
        var responseData = JsonConvert.DeserializeObject<Balance>(response.Content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(depositAmount, responseData.Amount);
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task DepositFunds_NegativeAmount_BadRequest()
    {
        var expectedExceptionMessage = "'Amount' must be greater than or equal to '0'.";
        decimal depositAmount = -1;
        MakeBalanceZero();
        var depositRequest = new RestRequest(DEPOSIT_ENDPOINT);
        depositRequest.AddJsonBody(new
        {
            Amount = depositAmount
        });

        var response = await restClient.PostAsync(depositRequest);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual(expectedExceptionMessage, response.ErrorMessage);
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task DepositFunds_WrongMethod_MethodNotAllowedException()
    {
        var depositRequest = new RestRequest(DEPOSIT_ENDPOINT);

        var response = await restClient.GetAsync(depositRequest);

        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task DepositFunds_MissingRequiredFields_BadRequest()
    {
        var expectedExceptionMessage = "The depositRequest field is required.";
        MakeBalanceZero();
        var depositRequest = new RestRequest(DEPOSIT_ENDPOINT);
        depositRequest.AddJsonBody(new
        {
            Amount = ""
        });

        var response = await restClient.PostAsync(depositRequest);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual(expectedExceptionMessage, response.ErrorMessage);
    }
}
