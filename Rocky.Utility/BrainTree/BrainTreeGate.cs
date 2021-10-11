using Braintree;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
    }
}