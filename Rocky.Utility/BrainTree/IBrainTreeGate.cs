using Braintree;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky.Utility.BrainTree
{
    public interface IBrainTreeGate
    {
        IBraintreeGateway CreateGateway();

        IBraintreeGateway GetGateway();

        Task<Result<Transaction>> ProcessPaymentAsync(string orderId, decimal amount, string clientTokenNonce);

        Task<string> GenerateClientTokenNonceAsync();
    }
}