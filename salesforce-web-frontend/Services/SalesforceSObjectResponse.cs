using System.Collections.Generic;

namespace salesforce_web_frontend.Services
{
    public class SalesforceSObjectResponse
    {
        public string id { get; set; }
        public bool success { get; set; }
        public List<object> errors { get; set; }
    }
}