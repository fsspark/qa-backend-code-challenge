using RestSharp;
using System.Net;
using Newtonsoft.Json;
using Betsson.OnlineWallets.API.Tests.src.Models;

namespace Betsson.OnlineWallets.API.Tests.src.test.GetBalanceTests;

[TestClass]
public class GetBalanceTests : BaseTest
{
    [TestMethod]
    public async Task GetBalance_GetInitialBalanceNoTransactions_BalanceZero()
    {
        MakeBalanceZero();
        var getBalancerequest = new RestRequest(GET_BALANCE_ENDPOINT);
        var expectedBalance = 0;

        var response = await restClient.GetAsync(getBalancerequest);
        var responseData = JsonConvert.DeserializeObject<Balance>(response.Content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(expectedBalance, responseData.Amount);
    }

    [TestMethod]
    public async Task GetBalance_GetBalanceAfterDeposit_CorrectBalance()
    {
        decimal depositAmount = 100;
        MakeBalanceZero();
        MakeDeposit(depositAmount);
        var getBalancerequest = new RestRequest(GET_BALANCE_ENDPOINT);

        var response = await restClient.GetAsync(getBalancerequest);
        var responseData = JsonConvert.DeserializeObject<Balance>(response.Content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(depositAmount, responseData.Amount);
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task GetBalance_WrongMethod_ErrorStatusCode()
    {
        var getBalancerequest = new RestRequest(GET_BALANCE_ENDPOINT);

        var response = await restClient.PostAsync(getBalancerequest);

        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }
}

