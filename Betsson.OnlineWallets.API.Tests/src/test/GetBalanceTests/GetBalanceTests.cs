using RestSharp;
using System.Net;
using Newtonsoft.Json;
using Betsson.OnlineWallets.API.Tests.src.Models;

namespace Betsson.OnlineWallets.API.Tests.src.test.GetBalanceTests;

[TestClass]
public class GetBalanceTests
{
    private readonly String BASE_URL = "http://localhost:8087";
    private readonly String GET_BALANCE_ENDPOINT = "/onlinewallet/balance";
    private readonly String WIDTHDRAWAL_ENDPOINT = "/onlinewallet/withdraw";
    private readonly String DEPOSIT_ENDPOINT = "/onlinewallet/deposit";
    private RestClient? restClient;

    [TestInitialize]
    public void Setup()
    {
        restClient = new RestClient(new RestClientOptions
        {
            BaseUrl = new Uri(BASE_URL)

        });
    }

    [TestMethod]
    public async Task GetBalanceAsync_GetInitialBalanceNoTransactions_BalanceZero()
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
    public async Task GetBalanceAsync_GetBalanceAfterDeposit_CorrectBalance()
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
    public async Task GetBalanceAsync_WrongMethod_ErrorStatusCode()
    {
        var getBalancerequest = new RestRequest(GET_BALANCE_ENDPOINT);

        var response = await restClient.PostAsync(getBalancerequest);

        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    private void MakeBalanceZero()
    {
        var getBalancerequest = new RestRequest(GET_BALANCE_ENDPOINT);
        var initialBalance = restClient.Get<Balance>(getBalancerequest);
        var withdrawalRequest = new RestRequest(WIDTHDRAWAL_ENDPOINT);

        if (initialBalance.Amount > 0)
        {
            withdrawalRequest.AddJsonBody(new
            {
                initialBalance.Amount
            });
            restClient.Post(withdrawalRequest);
        }
    }

    private void MakeDeposit(decimal amount)
    {
        var getBalancerequest = new RestRequest(GET_BALANCE_ENDPOINT);
        var initialBalance = restClient.Get<Balance>(getBalancerequest);
        var depositRequest = new RestRequest(DEPOSIT_ENDPOINT);

        if (initialBalance.Amount == 0)
        {
            depositRequest.AddJsonBody(new
            {
                Amount = amount
            });
            restClient.Post(depositRequest);
        }
    }
}

