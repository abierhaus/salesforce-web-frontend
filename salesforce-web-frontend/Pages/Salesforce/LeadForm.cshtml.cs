using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using salesforce_web_frontend.Models;
using salesforce_web_frontend.Services;

namespace salesforce_web_frontend.Pages.Salesforce
{
    public class LeadFormModel : PageModel
    {
        public ISalesforceService SalesforceService { get; set; }

        private readonly ILogger<LeadFormModel> _logger;

        public LeadFormModel(ILogger<LeadFormModel> logger, ISalesforceService salesforceService)
        {
            _logger = logger;
            SalesforceService = salesforceService;
        }


        public SalesforceSObjectResponse SalesforceSObjectResponse { get; set; }

        public void OnGet()
        {
        }

        public async Task OnPostSubmit(Lead lead)
        {
            SalesforceSObjectResponse =  await SalesforceService.CreateLeadAsync(lead);

        }
    }
}
