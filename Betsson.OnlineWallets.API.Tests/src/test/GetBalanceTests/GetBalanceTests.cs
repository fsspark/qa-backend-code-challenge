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
    public async Task GetBalanceAsync_GetInitialBalance_NoTransactions_BalanceZero()
    {
        MakeBalanceZero();
        var getBalancerequest = new RestRequest(GET_BALANCE_ENDPOINT);
        var expectedBalance = 0;

        var response = await restClient.GetAsync(getBalancerequest);
        var responseData = JsonConvert.DeserializeObject<Balance>(response.Content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(expectedBalance, responseData.Amount);
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
}

