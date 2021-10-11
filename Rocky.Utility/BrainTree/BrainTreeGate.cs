using Braintree;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky.Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        private readonly BrainTreeSettings _brainTreeSettings;
        private readonly IConfiguration _config;
        private IBraintreeGateway _brainTreeGateway;

        public BrainTreeGate(IConfiguration config)
        {
            _config = config;
            _brainTreeSettings = _config.GetSection("BrainTree").Get<BrainTreeSettings>();
        }

        public IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(_brainTreeSettings.Enviroment, _brainTreeSettings.MerchantId, _brainTreeSettings.PublicKey, _brainTreeSettings.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if (_brainTreeGateway == null)
                _brainTreeGateway = CreateGateway();

            return _brainTreeGateway;
        }

        public async Task<Result<Transaction>> ProcessPaymentAsync(string orderId, decimal amount, string clientTokenNonce)
        {
            var request = new TransactionRequest
            {
                Amount = amount,
                PaymentMethodNonce = clientTokenNonce,
                OrderId = orderId,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var gateway = GetGateway();

            return await Task.Factory.StartNew(() => gateway.Transaction.Sale(request));
        }

        public async Task<string> GenerateClientTokenNonceAsync()
        {
            return await Task.Factory.StartNew(() => GetGateway().ClientToken.Generate());
        }
    }
}