using Betsson.OnlineWallets.API.Tests.src.Models;
using RestSharp;

namespace Betsson.OnlineWallets.API.Tests.src.test;

[TestClass]
public class BaseTest
{
    protected readonly string BASE_URL = "http://localhost:8087";
    protected readonly string GET_BALANCE_ENDPOINT = "/onlinewallet/balance";
    protected readonly string WIDTHDRAWAL_ENDPOINT = "/onlinewallet/withdraw";
    protected readonly string DEPOSIT_ENDPOINT = "/onlinewallet/deposit";

    protected RestClient? restClient;

    [TestInitialize]
    public void Setup()
    {
        restClient = new RestClient(new RestClientOptions
        {
            BaseUrl = new Uri(BASE_URL)

        });
    }
    protected void MakeBalanceZero()
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

    protected void MakeDeposit(decimal amount)
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