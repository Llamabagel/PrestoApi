using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PrestoApi.Models.Presto;

namespace PrestoApi.Controllers
{
    [Route("api")]
    public class PrestoController : Controller
    {
        /// <summary>
        /// A required string of JSON required for getting the list of transactions for a PRESTO card.
        /// </summary>
        private const string TransactionRequestBody =
                "{\"TransactionType\":\"0\",\"Agency\":\"-1\",\"SelectedMonth\":\"07/24/2015 - 07/23/2017\",\"Pagesize\":\"\",\"currentModel\":{\"ID\":\"a968c771-13bc-4792-a726-00b381fac83f\",\"Title\":\"My PRESTO Card Activity\",\"ButtonText\":\"\",\"Description\":\"Please see below to view your PRESTO card balance and transaction activity. You can change the time period and transit agency in the dropdown menu to view all available reports.\",\"DefaultPeriodId\":null,\"DefaultServiceProviderId\":null,\"PrintButton\":{\"__interceptors\":[{}],\"BackgroundColor\":{\"__interceptors\":[{}],\"Value\":\"white\"},\"Icon\":null,\"BorderColor\":{\"__interceptors\":[{}],\"Value\":\"grey\"},\"ButtonText\":\"Print\",\"ButtonID\":\"\",\"ButtonLink\":null,\"TextColor\":{\"__interceptors\":[{}],\"Value\":\"black\"},\"ID\":\"ce95ff13-62f0-4d4a-a040-0babb1ed15d5\",\"Name\":\"Print\",\"AriaLabel\":\"\",\"Styles\":\"btn__background--white btn__border--grey btn__text--black\"},\"GridSize\":\"10\",\"TransitAgencyLabel\":\"Transit Agency\",\"DateRangeLabel\":\"Date Range\",\"TransactionTypeLabel\":\"Transaction Type\",\"FromDateLabel\":\"From Date*\",\"ToDateLabel\":\"To Date*\",\"ViewButton\":\"View\",\"ExportButton\":\"Export to CSV\",\"TransitHistoryReportTab\":\"Transaction History Report View\",\"TransitHistoryReportTablink\":{\"Anchor\":\"\",\"Class\":\"\",\"Text\":\"Card Activity\",\"Query\":\"\",\"Title\":\"\",\"Url\":\"/en/dashboard/card-activity\",\"Target\":\"Active Browser\",\"TargetId\":\"f169c305-9d34-44e5-93c4-a03ebe4a6b45\",\"Type\":4},\"TransitUsageReportTab\":\"Transit Usage Report View\",\"TransitUsageReportTablink\":{\"Anchor\":\"\",\"Class\":\"\",\"Text\":\"Transit Usage Report\",\"Query\":\"\",\"Title\":\"\",\"Url\":\"/en/dashboard/transit-usage-report\",\"Target\":\"Active Browser\",\"TargetId\":\"4e22fa7c-2dac-44fb-b531-52ab0fd13800\",\"Type\":4},\"ExportToolTip\":\"What is .CSV?. It is a file format that can be used with any spreadsheet program.\",\"TransactionTypes\":[\"0\",\"1\",\"2\",\"3\",\"4\"],\"CardTypeFilter\":[{\"__interceptors\":[{}],\"ID\":\"eb980fc3-dc9b-46d5-b347-1d2fcc8074ec\",\"Index\":\"-1\",\"Value\":\"All \"},{\"__interceptors\":[{}],\"ID\":\"ceae3450-f9df-4e9e-af52-479c9926227e\",\"Index\":\"1\",\"Value\":\"ServiceProvider1\"},{\"__interceptors\":[{}],\"ID\":\"5c43261a-d7e2-4ab2-9d87-72a08de23998\",\"Index\":\"2\",\"Value\":\"ServiceProvider2\"}],\"DateFilter\":[{\"__interceptors\":[{}],\"ID\":\"255bc53d-c0e1-47b3-9864-e0afd7b48e9b\",\"MonthID\":\"0\",\"MonthName\":\"All 3 Months\",\"IsDisplay\":\"True\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"8461bec8-29a2-477e-97cd-43020e08f577\",\"MonthID\":\"1\",\"MonthName\":\"January\",\"IsDisplay\":\"False\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"1e4e9684-c6a5-4865-8fd1-819df36e7a6d\",\"MonthID\":\"2\",\"MonthName\":\"February\",\"IsDisplay\":\"False\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"d6232958-ca18-46a9-b0ff-c96d224a6927\",\"MonthID\":\"3\",\"MonthName\":\"March\",\"IsDisplay\":\"False\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"b45bdfca-4161-42c9-a598-5b5d1f6d399d\",\"MonthID\":\"4\",\"MonthName\":\"April\",\"IsDisplay\":\"False\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"6edc8a8a-b94d-43da-9bc5-09fcd148b166\",\"MonthID\":\"5\",\"MonthName\":\"May\",\"IsDisplay\":\"False\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"36a1cf39-3dec-4cab-bbc3-7de2d62d02ee\",\"MonthID\":\"6\",\"MonthName\":\"June\",\"IsDisplay\":\"False\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"aeecf0a8-79e3-4e0e-bac9-98aec4c78b4b\",\"MonthID\":\"7\",\"MonthName\":\"July\",\"IsDisplay\":\"False\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"882f62aa-d4dc-44ad-9c70-b92dd53a901e\",\"MonthID\":\"8\",\"MonthName\":\"August\",\"IsDisplay\":\"True\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"25a9a2d6-3d7a-4b2d-974e-c999c104160b\",\"MonthID\":\"9\",\"MonthName\":\"September\",\"IsDisplay\":\"True\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"aa48209c-17bb-4ff5-84e8-c6f644c087f1\",\"MonthID\":\"10\",\"MonthName\":\"October\",\"IsDisplay\":\"True\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"d7b7ff51-8ff0-4e7a-bfb3-ff99f1deb92c\",\"MonthID\":\"11\",\"MonthName\":\"November\",\"IsDisplay\":\"False\",\"DaxPeriodID\":\"\"},{\"__interceptors\":[{}],\"ID\":\"dda9a0a1-9cd8-4cd2-b09f-53ad7f359ff0\",\"MonthID\":\"12\",\"MonthName\":\"December\",\"IsDisplay\":\"False\",\"DaxPeriodID\":\"\"}],\"ColumnNames\":[{\"__interceptors\":[{}],\"Name\":\"Date\",\"RegEx\":\"\",\"ErrorMessage\":\"\",\"IsRequired\":\"False\",\"DataType\":\"String\",\"ControlType\":\"Column\",\"Label\":\"Date\",\"CustomID\":\"\",\"FieldDescription\":null,\"Datasource\":\"\",\"DaxField\":\"Date\",\"MatchesWith\":null,\"IsReviewRequired\":\"False\",\"RequiredFieldMessage\":\"This field is required\",\"MaxLength\":\"\",\"FieldId\":\"\",\"Format\":null,\"ID\":\"dc2a9f16-1578-4681-8c57-a47868d78668\",\"RegExAdditional\":\"\",\"ErrorMessageAdditional\":\"\",\"MinAmountRegEx\":\"\",\"MinAmountMessage\":\"\",\"MaxAmountRegEx\":\"\",\"MaxAmountMessage\":\"\",\"ShowInMobile\":false,\"StringFieldName\":null,\"SelectionList\":null},{\"__interceptors\":[{}],\"Name\":\"TransitAgency\",\"RegEx\":\"\",\"ErrorMessage\":\"\",\"IsRequired\":\"False\",\"DataType\":\"String\",\"ControlType\":\"Column\",\"Label\":\"Transit Agency\",\"CustomID\":\"\",\"FieldDescription\":null,\"Datasource\":\"\",\"DaxField\":\"ServiceProviderName\",\"MatchesWith\":null,\"IsReviewRequired\":\"False\",\"RequiredFieldMessage\":\"This field is required\",\"MaxLength\":\"\",\"FieldId\":\"\",\"Format\":null,\"ID\":\"ed376159-c8ed-4e61-a854-b1bb1c2e9d01\",\"RegExAdditional\":\"\",\"ErrorMessageAdditional\":\"\",\"MinAmountRegEx\":\"\",\"MinAmountMessage\":\"\",\"MaxAmountRegEx\":\"\",\"MaxAmountMessage\":\"\",\"ShowInMobile\":false,\"StringFieldName\":null,\"SelectionList\":null},{\"__interceptors\":[{}],\"Name\":\"Location\",\"RegEx\":\"\",\"ErrorMessage\":\"\",\"IsRequired\":\"False\",\"DataType\":\"String\",\"ControlType\":\"Column\",\"Label\":\"Location\",\"CustomID\":\"\",\"FieldDescription\":null,\"Datasource\":\"\",\"DaxField\":\"Location\",\"MatchesWith\":null,\"IsReviewRequired\":\"False\",\"RequiredFieldMessage\":\"This field is required\",\"MaxLength\":\"\",\"FieldId\":\"\",\"Format\":null,\"ID\":\"1fcac431-3c1b-4971-a2c8-c969d76d6202\",\"RegExAdditional\":\"\",\"ErrorMessageAdditional\":\"\",\"MinAmountRegEx\":\"\",\"MinAmountMessage\":\"\",\"MaxAmountRegEx\":\"\",\"MaxAmountMessage\":\"\",\"ShowInMobile\":false,\"StringFieldName\":null,\"SelectionList\":null},{\"__interceptors\":[{}],\"Name\":\"Type\",\"RegEx\":\"\",\"ErrorMessage\":\"\",\"IsRequired\":\"False\",\"DataType\":\"String\",\"ControlType\":\"Column\",\"Label\":\"Type \",\"CustomID\":\"\",\"FieldDescription\":null,\"Datasource\":\"\",\"DaxField\":\"Type\",\"MatchesWith\":null,\"IsReviewRequired\":\"False\",\"RequiredFieldMessage\":\"This field is required\",\"MaxLength\":\"\",\"FieldId\":\"\",\"Format\":null,\"ID\":\"78fce83b-9edb-48e5-a5ae-fdd8598958b5\",\"RegExAdditional\":\"\",\"ErrorMessageAdditional\":\"\",\"MinAmountRegEx\":\"\",\"MinAmountMessage\":\"\",\"MaxAmountRegEx\":\"\",\"MaxAmountMessage\":\"\",\"ShowInMobile\":false,\"StringFieldName\":null,\"SelectionList\":null},{\"__interceptors\":[{}],\"Name\":\"ServiceClass\",\"RegEx\":\"\",\"ErrorMessage\":\"\",\"IsRequired\":\"False\",\"DataType\":\"String\",\"ControlType\":\"Column\",\"Label\":\"Service Class\",\"CustomID\":\"\",\"FieldDescription\":null,\"Datasource\":\"\",\"DaxField\":\"\",\"MatchesWith\":null,\"IsReviewRequired\":\"False\",\"RequiredFieldMessage\":\"This field is required\",\"MaxLength\":\"\",\"FieldId\":\"\",\"Format\":null,\"ID\":\"56b127b9-1e3d-4021-9fed-d6b59689e3db\",\"RegExAdditional\":\"\",\"ErrorMessageAdditional\":\"\",\"MinAmountRegEx\":\"\",\"MinAmountMessage\":\"\",\"MaxAmountRegEx\":\"\",\"MaxAmountMessage\":\"\",\"ShowInMobile\":false,\"StringFieldName\":null,\"SelectionList\":null},{\"__interceptors\":[{}],\"Name\":\"Discount\",\"RegEx\":\"\",\"ErrorMessage\":\"\",\"IsRequired\":\"True\",\"DataType\":\"String\",\"ControlType\":\"Column\",\"Label\":\"Discount\",\"CustomID\":\"\",\"FieldDescription\":null,\"Datasource\":\"\",\"DaxField\":\"DiscountSum\",\"MatchesWith\":null,\"IsReviewRequired\":\"False\",\"RequiredFieldMessage\":\"This field is required\",\"MaxLength\":\"\",\"FieldId\":\"\",\"Format\":null,\"ID\":\"e7c825a8-7277-40e7-9ff0-14677ebc22e5\",\"RegExAdditional\":\"\",\"ErrorMessageAdditional\":\"\",\"MinAmountRegEx\":\"\",\"MinAmountMessage\":\"\",\"MaxAmountRegEx\":\"\",\"MaxAmountMessage\":\"\",\"ShowInMobile\":false,\"StringFieldName\":null,\"SelectionList\":null},{\"__interceptors\":[{}],\"Name\":\"Amount\",\"RegEx\":\"\",\"ErrorMessage\":\"\",\"IsRequired\":\"False\",\"DataType\":\"String\",\"ControlType\":\"Column\",\"Label\":\"Amount\",\"CustomID\":\"\",\"FieldDescription\":null,\"Datasource\":\"\",\"DaxField\":\"Amount\",\"MatchesWith\":null,\"IsReviewRequired\":\"False\",\"RequiredFieldMessage\":\"This field is required\",\"MaxLength\":\"\",\"FieldId\":\"\",\"Format\":null,\"ID\":\"213f9eb7-ca81-4858-af0d-c9756a9900e1\",\"RegExAdditional\":\"\",\"ErrorMessageAdditional\":\"\",\"MinAmountRegEx\":\"\",\"MinAmountMessage\":\"\",\"MaxAmountRegEx\":\"\",\"MaxAmountMessage\":\"\",\"ShowInMobile\":false,\"StringFieldName\":null,\"SelectionList\":null},{\"__interceptors\":[{}],\"Name\":\"Balance\",\"RegEx\":\"\",\"ErrorMessage\":\"\",\"IsRequired\":\"False\",\"DataType\":\"String\",\"ControlType\":\"Column\",\"Label\":\"Balance\",\"CustomID\":\"\",\"FieldDescription\":null,\"Datasource\":\"\",\"DaxField\":\"Balance\",\"MatchesWith\":null,\"IsReviewRequired\":\"False\",\"RequiredFieldMessage\":\"This field is required\",\"MaxLength\":\"\",\"FieldId\":\"\",\"Format\":null,\"ID\":\"98e45fad-d7e7-43f2-9513-202ab9ed9077\",\"RegExAdditional\":\"\",\"ErrorMessageAdditional\":\"\",\"MinAmountRegEx\":\"\",\"MinAmountMessage\":\"\",\"MaxAmountRegEx\":\"\",\"MaxAmountMessage\":\"\",\"ShowInMobile\":false,\"StringFieldName\":null,\"SelectionList\":null}],\"Travels\":null,\"Agencies\":[\"BRA\",\"BUR\",\"GOT\",\"HAM\",\"MIS\",\"OAK\",\"OCT\",\"GTA\",\"TTC\",\"UPE\",\"YOR\",\"DUR\",\"RTL\"],\"PartialCardActivityTitle\":\"Latest Card Activity\",\"ViewFullHistory\":{\"Anchor\":\"\",\"Class\":\"\",\"Text\":\"Go to Card Activity\",\"Query\":\"\",\"Title\":\"View Full History\",\"Url\":\"/en/dashboard/card-activity\",\"Target\":\"Active Browser\",\"TargetId\":\"f169c305-9d34-44e5-93c4-a03ebe4a6b45\",\"Type\":4},\"ErrorMessage\":null,\"EmptyRecordsMessage\":\"Sorry, but we don't have any transactions for your PRESTO card for the selected month.\",\"ImgQuestionMark\":{\"Alt\":\"question-mark\",\"Border\":\"\",\"Class\":\"\",\"Height\":12,\"HSpace\":0,\"Src\":\"/~/media/AFMS/Images/ToolTip/questionMarkTooltip.ashx\",\"VSpace\":0,\"Width\":11,\"MediaId\":\"89751fc7-7cf9-4ddf-9de5-d08d2b7aaead\",\"Title\":\"\"},\"AltSortDownImg\":{\"Alt\":\"Sort by date in descending order\",\"Border\":\"\",\"Class\":\"\",\"Height\":8,\"HSpace\":0,\"Src\":\"/~/media/AFMS/Images/Accordion/AccordianDownArrow.ashx\",\"VSpace\":0,\"Width\":14,\"MediaId\":\"50006a58-1315-4bb7-9f04-a9cf45c5e999\",\"Title\":\"\"},\"AltSortUpImg\":{\"Alt\":\"Sort by date in ascending order\",\"Border\":\"\",\"Class\":\"\",\"Height\":8,\"HSpace\":0,\"Src\":\"/~/media/AFMS/Images/Accordion/AccordianUpArrow.ashx\",\"VSpace\":0,\"Width\":14,\"MediaId\":\"21a862b9-57e8-45ef-a46f-cef620af28e7\",\"Title\":\"\"},\"AutoAdjustmentLabel\":\"Auto Adjustment Missed Tap Off\",\"AdjustmentDescription\":\"A missed tap off adjustment is an amount deducted from your card if you don’t tap off at the end of your trip on GO Transit. You will be charged the fare to the furthest distance on the train line or bus route.\",\"SearchButton\":{\"__interceptors\":[{}],\"BackgroundColor\":{\"__interceptors\":[{}],\"Value\":\"emphasis\"},\"Icon\":null,\"BorderColor\":{\"__interceptors\":[{}],\"Value\":\"emphasis\"},\"ButtonText\":\"Search\",\"ButtonID\":\"\",\"ButtonLink\":null,\"TextColor\":{\"__interceptors\":[{}],\"Value\":\"white\"},\"ID\":\"0500c2f3-0db0-48dc-82d7-1697fa22e86c\",\"Name\":\"Search\",\"AriaLabel\":\"\",\"Styles\":\"btn__background--emphasis btn__border--emphasis btn__text--white\"},\"TransitServiceAltLabel\":\"Transit Service to display my PRESTO card activity\",\"MonthsAltLabel\":\"Months to display my PRESTO card activity\",\"SearchAltLabel\":\"Search my PRESTO card activity for the selected transit provider and month\",\"PrintAltLabel\":\"Print my PRESTO card activity\",\"IsCardActivityFull\":true,\"ListOfTravelsIsEmpty\":false,\"AgenciesFirstOption\":\"ALL\",\"MobileColumnNames\":[{\"__interceptors\":[{}],\"Name\":\"Transaction\",\"RegEx\":\"\",\"ErrorMessage\":\"\",\"IsRequired\":\"False\",\"DataType\":\"String\",\"ControlType\":\"Column\",\"Label\":\"Transaction\",\"CustomID\":\"\",\"FieldDescription\":null,\"Datasource\":\"\",\"DaxField\":\"Transaction\",\"MatchesWith\":null,\"IsReviewRequired\":\"False\",\"RequiredFieldMessage\":\"This field is required\",\"MaxLength\":\"\",\"FieldId\":\"\",\"Format\":null,\"ID\":\"4310b952-279c-4fcd-ba86-c27d852366cf\",\"RegExAdditional\":\"\",\"ErrorMessageAdditional\":\"\",\"MinAmountRegEx\":\"\",\"MinAmountMessage\":\"\",\"MaxAmountRegEx\":\"\",\"MaxAmountMessage\":\"\",\"ShowInMobile\":false,\"StringFieldName\":null,\"SelectionList\":null},{\"__interceptors\":[{}],\"Name\":\"Balance\",\"RegEx\":\"\",\"ErrorMessage\":\"\",\"IsRequired\":\"False\",\"DataType\":\"String\",\"ControlType\":\"Column\",\"Label\":\"Balance\",\"CustomID\":\"\",\"FieldDescription\":null,\"Datasource\":\"\",\"DaxField\":\"Balance\",\"MatchesWith\":null,\"IsReviewRequired\":\"False\",\"RequiredFieldMessage\":\"This field is required\",\"MaxLength\":\"\",\"FieldId\":\"\",\"Format\":null,\"ID\":\"98e45fad-d7e7-43f2-9513-202ab9ed9077\",\"RegExAdditional\":\"\",\"ErrorMessageAdditional\":\"\",\"MinAmountRegEx\":\"\",\"MinAmountMessage\":\"\",\"MaxAmountRegEx\":\"\",\"MaxAmountMessage\":\"\",\"ShowInMobile\":false,\"StringFieldName\":null,\"SelectionList\":null}],\"SelectLabel\":\"Select\",\"ClearLabel\":\"Clear\",\"SelectDateRange\":\"Select Date Range\",\"FromDateRequired\":\"Please enter a From Date\",\"ToDateRequired\":\"Please enter a To Date\",\"FromDateValidation\":\"Please enter valid From Date. From Date must use format mm/dd/yyyy\",\"ToDateValidation\":\"Please enter valid To Date. To Date must use format mm/dd/yyyy\",\"StartDateDescription\":\"Start of date range. The format is the two digit month, front slash, two digit day, front slash, four digit year\",\"EndDateDescription\":\"End of date range.  The format is the two digit month, front slash, two digit day, front slash, four digit year\",\"ToDateRangeValidation\":\"To Date must be greater than or equal to ‘From Date’ and less than or equal to today’s date.\",\"FromDateRangeValidation\":\"From Date must be a date between today and past 24 months.\",\"CardNumberLabel\":\"PRESTO Card number\",\"BackButtonLabel\":\"Go to previous page of Card Activity\",\"PageNumberLabel\":\"Go to page {0}\",\"NextButtonLabel\":\"Go to next page of Card Activity\"}}"
            ;

        // Convert the big chunk of text above into a JSON object for easier management of the dates paramters
        private static readonly JObject TransactionJsonRequest = JObject.Parse(TransactionRequestBody);

        // POST api/cards
        [HttpPost]
        [Route("cards")]
        public IActionResult Post([FromBody] Request value)
        {
            if (value == null)
            {
                return BadRequest();
            }

            return Ok(GetAccountContent(value).Result);
        }

        // POST api/summary
        [HttpPost]
        [Route("summary")]
        public IActionResult Summary([FromBody] AccountRequest value)
        {
            if (value == null)
            {
                return BadRequest();
            }

            return Ok(GetAccountSummary(value).Result);
        }

        /// <summary>
        /// Gets the PRESTO summary of a given PRESTO account.
        /// </summary>
        /// <param name="request">The account to get a summary for</param>
        /// <returns>Task containing a summay of the account including a list of basic information about the PRESTO cards for the given account</returns>
        private static async Task<SummaryAccount> GetAccountSummary(AccountRequest request)
        {
            var summaryAccount = new SummaryAccount
            {
                Username = request.Username.Trim(),
                Type = request.Type
            };

            var cookieContainer = new CookieContainer();
            var client =
                new HttpClient(new HttpClientHandler {UseCookies = true, CookieContainer = cookieContainer})
                {
                    BaseAddress = new Uri("https://www.prestocard.ca")
                };

             // Login to the PRESTO website.
            var loginResult = Login(request, ref client, ref cookieContainer);
             // Record any login failures and return the response
             if (loginResult != ResponseCode.AccessOk)
             {
                 summaryAccount.Error = loginResult;
                 return summaryAccount;
             }
            

            // Get the PRESTO dashboard
            var nextResult = client.GetAsync("/en/dashboard").Result;
            // Using?
            var newBody = nextResult.Content.ReadAsStreamAsync().Result;

            // Load dashboard page into an XML Document for parsing
            var doc = new HtmlDocument();
            doc.Load(newBody);

            // Finds a <script> element containing useful JSON data about the user's cards
            var root = doc.DocumentNode;
            var p = root
                .Descendants()
                .Single(n => n.GetAttributeValue("class", "").Equals("col-xs-12 col-md-3 layout-two-cols__left"))
                .Descendants("script")
                .Single();

            // Takes out uneccessary JS fragments from the <script> tag
            var cardJson = p.InnerText.Substring(39);
            cardJson = cardJson.Substring(0, cardJson.Length - 4);
            cardJson = cardJson.Replace("new Date(", "");
            cardJson = cardJson.Replace("),", ",");

            var o = JObject.Parse(cardJson);

            // Gather the sumarized PRESTO card information
            if (request.Type == Models.Presto.Request.TypeRegistered)
            {
                // Get the number of fare cards listed for this account
                var fareCount = o["AutoRenewManageVM"]["Customer"]["FareMedias"].ToList().Count;
                for (var i = 0; i < fareCount; i++)
                {
                    var fare = o["AutoRenewManageVM"]["Customer"]["FareMedias"][i];
                    // Status = 1 means valid card(?)
                    if ((int) fare["Status"] == 1)
                    {
                        summaryAccount.Cards.Add(new SummaryCard
                        {
                            Name = (string) fare["NickName"] != "" ? (string) fare["NickName"] : "Your Card",
                            Number = (string) fare["VisibleId"]
                        });
                    }
                }
            }
            else
            {
                summaryAccount.Cards.Add(new SummaryCard
                {
                    Name = "Your Card",
                    Number = (string) o["CardNumber"]
                });
            }

            return summaryAccount;
        }

        private static async Task<IList<Response>> GetAccountContent(Request request)
        {
            // Updates the TransactionJsonRequest to force it to take the current date for getting transactions
            TransactionJsonRequest["SelectedMonth"] = "07/24/2009 - " +
                                                      TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local)
                                                          .ToString("MM/dd/yyyy");

            // List of responses to collect
            var responses = new List<Response>();

            foreach (var requestedAccount in request.Accounts)
            {
                responses.Add(GetAccountCards(requestedAccount, request.OmitExtraInfo).Result);
            }

            return responses;
        }

        /// <summary>
        /// Get updated card information for all the requested cards for an account
        /// </summary>
        /// <param name="account">The requested account info</param>
        /// <param name="omitExtraInfo">Whether to exclude Transactions and Pending items</param>
        /// <returns>Response data for the account</returns>
        private static async Task<Response> GetAccountCards(AccountRequest account, bool omitExtraInfo)
        {
            var newResponse = new Response
            {
                Username = account.Username.Trim(),
                Type = account.Type
            };

            var cookieContainer = new CookieContainer();
            // Create an HttpClient instance to handle all web operations on the PRESTO card site for this account.
            var client =
                new HttpClient(new HttpClientHandler {UseCookies = true, CookieContainer = cookieContainer})
                {
                    BaseAddress = new Uri("https://www.prestocard.ca")
                };

            // Login to the PRESTO website.
            var loginResult = Login(account, ref client, ref cookieContainer);
            
            // Record any login failures and return the response
            if (loginResult != ResponseCode.AccessOk)
            {
                newResponse.Error = loginResult;
                return newResponse;
            }

            // Navigate to the PRESTO dashboard
            var nextResult = client.GetAsync("/en/dashboard").Result;
            var newBody = nextResult.Content.ReadAsStreamAsync().Result;

            // Load dashboard page into an XML Document for parsing
            var doc = new HtmlDocument();
            doc.Load(newBody);

            // Finds a <script> element containing a LOT of useful JSON data about the accounts's cards
            // This allows us to avoid needing to parse through the rest of the HTML to find the required information
            // for each presto card.
            var scriptTag = doc.DocumentNode
                .Descendants()
                .Single(n => n.GetAttributeValue("class", "").Equals("col-xs-12 col-md-3 layout-two-cols__left"))
                .Descendants("script")
                .Single();

            // Takes out uneccessary JS fragments from the <script> tag.
            // Strips the start of the javascript out
            var cardJson = scriptTag.InnerText.Substring(39);
            // Strips the end of the javascript out
            cardJson = cardJson.Substring(0, cardJson.Length - 4);
            // Removes JavaScript's "new Date(" constructor and leaves the unix timestamp intact for use later
            cardJson = cardJson.Replace("new Date(", "");
            cardJson = cardJson.Replace("),", ",");

            var data = JObject.Parse(cardJson);

            // Gather the sumarized PRESTO card information
            if (account.Type == Models.Presto.Request.TypeRegistered)
            {
                // Get the number of fare cards listed for this account
                var fareCount = data["AutoRenewManageVM"]["Customer"]["FareMedias"].ToList().Count;
                for (var j = 0; j < fareCount; j++)
                {
                    var fare = data["AutoRenewManageVM"]["Customer"]["FareMedias"][j];
                    // Status = 1 means valid card(?)
                    // Checks to make sure that the fare card is one of the requested cards for the account
                    if ((int) fare["Status"] == 1 && account.Cards.Contains((string) fare["VisibleId"]))
                    {
                        // Creates a new skeleton response object for the selected card. Contains the nickname, number, and expiry date of the card.
                        var card = new CardResponse
                        {
                            Name = (string) fare["NickName"] != "" ? (string) fare["NickName"] : "Your Card",
                            Number = (string) fare["VisibleId"],
                            Expiration = (long) fare["ExpiryDate"]
                        };

                        // Gets the list of products loaded on this card.
                        var productCount = fare["Products"].ToList().Count;
                        for (var k = 0; k < productCount; k++)
                        {
                            var productJson = fare["Products"][k];
                            // Create a new Product object
                            var product = new ProductResponse
                            {
                                Name = (string) productJson["ProductName"],
                                Balance = (decimal) productJson["Balance"],
                                ValidityStartDate = (long) productJson["ValidityStartDate"],
                                ValidityEndDate = (long) productJson["ValidityEndDate"]
                            };

                            // If the product has no name, it is probably the default fare product
                            // The null-named product's balance is therefore probably the card's balance
                            if (product.Name == null)
                            {
                                card.Balance = product.Balance;
                            }

                            // Add the product to the card's products
                            card.Products.Add(product);
                        }
                        // Transactions are considered "Extra Info"
                        if (!omitExtraInfo)
                        {
                            // Create an Http request that will change the dashboard to display a different card
                            var cardHttpRequest = new HttpRequestMessage(HttpMethod.Post,
                                "https://www.prestocard.ca/api/sitecore/Global/UpdateFareMediaSession?id=lowerFareMediaId&class=lowerFareMediaId")
                            {
                                Content = new StringContent("setFareMediaSession=" + card.Number,
                                    Encoding.UTF8, "application/x-www-form-urlencoded")
                            };
                            await client.SendAsync(cardHttpRequest);

                            try
                            {
                                // Modify the CardActivity Filter to display trasaction history as far back as possible
                                var transactionHttpRequest = new HttpRequestMessage(HttpMethod.Post,
                                    "https://www.prestocard.ca/api/sitecore/Paginator/CardActivityFilteredIndex")
                                {
                                    Content = new StringContent(TransactionJsonRequest.ToString(),
                                        Encoding.UTF8, "application/json")
                                };

                                transactionHttpRequest.Headers.Add("accept", "*/*");
                                transactionHttpRequest.Headers.Add("accept-encoding", "gzip, deflate, br");
                                transactionHttpRequest.Headers.Add("x-requested-with", "XMLHttpRequest");

                                await client.SendAsync(transactionHttpRequest);

                                // Exports the user's transaction history to a CSV file
                                var csvRequest =
                                    client.GetAsync(
                                        "https://www.prestocard.ca/api/sitecore/Paginator/CardActivityExportCSV");
                                var responseAsync = csvRequest.Result.Content.ReadAsStreamAsync().Result;
                                var csv = new CsvReader(new StreamReader(responseAsync));

                                csv.Configuration.RegisterClassMap<TransactionClassMap>();
                                csv.Configuration.HasHeaderRecord = true;

                                // Read the transaction records
                                var transactions = csv.GetRecords<TransactionResponse>().ToList();
                                card.Transactions = transactions;
                            }
                            catch (Exception e)
                            {
                                Console.Out.Write(e.Message);
                            }
                        }

                        // Add the updated card to the list of responding cards
                        newResponse.Cards.Add(card);
                    }

                    if (omitExtraInfo) continue;
                    // Get the number of pending loads.
                    var pendingLoadList = data["pendingLoadList"].ToList();
                    foreach (var t in pendingLoadList)
                    {
                        // Create the PendingResponse object from the json data.
                        var pending = new PendingResponse
                        {
                            Name = (string) t["Product"]["ItemId"],
                            Balance = (decimal) t["SubTotal"]
                        };

                        // Iterate through all of the fares to find the correct one to add the pending values to
                        for (var m = 0; m < fareCount; m++)
                        {
                            // Add the pending load to the correct ard
                            if (newResponse.Cards[m].Number == (string) t["FareMedia"]["VisibleId"])
                            {
                                newResponse.Cards[m].Pending.Add(pending);
                            }
                        }
                    }
                }
            }
            else if (account.Type == Models.Presto.Request.TypeAnonymous && data["Balance"] != null)
            {
                // Creates a new skeleton response object for the selected card. Contains the nickname, number, and expiry date of the card.
                var card = new CardResponse
                {
                    Name = "Your Card",
                    Number = account.Username,
                    Balance = decimal.Parse((string) data["Balance"], NumberStyles.Currency)
                };
                newResponse.Cards.Add(card);
            }

            return newResponse;
        }
        
        /// <summary>
        /// Logs in to the PRESTO website using the given account credentials. Saves the required authentication cookies
        /// in the client.
        /// </summary>
        /// <param name="account">The account to sign in to.</param>
        /// <param name="client">The HttpClient to save useful cookies and stuff in. By reference.</param>
        /// <param name="cookieContainer">The cookie container being used by the client</param>
        /// <returns>A <see cref="ResponseCode"/></returns>
        private static int Login(AccountRequest account, ref HttpClient client, ref CookieContainer cookieContainer)
        {
            if (account == null) return ResponseCode.OtherError;
            
            // "Log in" using authentication token instead of username and password. 
            if (account.Auth != null)
            {
                if (string.IsNullOrEmpty(account.Auth.Token) || string.IsNullOrEmpty(account.Auth.SessionId))
                {
                    return ResponseCode.BadAuth;
                }
                
                cookieContainer.Add(client.BaseAddress, new Cookie(".ASPXAUTH", account.Auth.Token));
                cookieContainer.Add(client.BaseAddress, new Cookie("ASP.NET_SessionId", account.Auth.SessionId));
                
                return ResponseCode.AccessOk;
            }
            
            string loginJson;
            HttpRequestMessage httpRequest;
            if (account.Type == Models.Presto.Request.TypeRegistered)
            {
                // Create the login request
                loginJson = "{\"custSecurity\":{\"Login\":\"" + account.Username.Trim() + "\",\"Password\":\"" +
#pragma warning disable 612
                            account.Password.Trim() + "\"},\"anonymousOrderACard\":false}";
#pragma warning restore 612
                httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    "https://www.prestocard.ca/api/sitecore/AFMSAuthentication/SignInWithAccount")
                {
                    Content = new StringContent(loginJson, Encoding.UTF8, "application/json")
                };
            }
            else
            {
                // Create the anonymous login request
                loginJson = "{\"anonymousOrderACard\":true,\"fareMediaId\":\"" + account.Username.Trim() + "\"}";
                httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    "https://www.prestocard.ca/api/sitecore/AFMSAuthentication/SignInWithFareMedia")
                {
                    Content = new StringContent(loginJson, Encoding.UTF8, "application/json")
                };
            }

            // Login to PRESTO and save the login result.       
            var loginResult = client.SendAsync(httpRequest).Result.Content.ReadAsStringAsync().Result;


            // Check for login errors. Aborts the function if any errors were detected.
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (loginResult ==
                "\"You could not be logged in to your online account. Please check your username and password and try again.\""
            )
            {
                return ResponseCode.WrongUsernamePassword;
            }
            if (loginResult ==
                "\"Your account has been locked as a result of three failed attempts to sign in. Please reset your password to access your account.\""
            )
            {
                return ResponseCode.AccountLocked;
            }
            if (loginResult ==
                "{\"result\":false,\"message\":\"Your PRESTO card is already registered to an account.Please log in using your username and password.\"}"
            )
            {
                return ResponseCode.WrongType;
            }
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (loginResult == "{\"result\":false,\"message\":\"Incorrect PRESTO card serial number\"}")
            {
                return ResponseCode.BadSerialCode;
            }

            return ResponseCode.AccessOk;
        }
        
        [HttpPost("cart")]
        public IActionResult AddToCart([FromBody] AccountRequest account, [FromQuery] string test)
        {            
            var cookieContainer = new CookieContainer();
            // Create an HttpClient instance to handle all web operations on the PRESTO card site for this account.
            var client =
                new HttpClient(new HttpClientHandler {UseCookies = true, CookieContainer = cookieContainer})
                {
                    BaseAddress = new Uri("https://www.prestocard.ca")
                };
            
            // Login to PRESTO and save the login result.       
            var loginResult = Login(account, ref client, ref cookieContainer);
            if (loginResult != ResponseCode.AccessOk)
            {
                return BadRequest();
            }

            var uriBuilder = new UriBuilder("https://www.prestocard.ca/api/sitecore/ShoppingCart/AddCartLine")
            {
                Query = "ProductID=5637144811&ListPrice=$10.00&Concession=Adult&FareMediaID=04624415&Epurse=1"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, uriBuilder.ToString())
            {
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };
            
            request.Headers.Add("Accept", "application/json, text/javascript, */*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            
            var result = client.SendAsync(request).Result;
            
            Console.Out.WriteLine(result.Content.ReadAsStringAsync().Result);

            return Ok();
        }

        /// <summary>
        /// Gets the current shopping cart for a specific card in an account.
        /// 
        /// The AccountRequest object should only have a single card in the list. Onle one card's cart should be requested at a time.
        /// </summary>
        /// <param name="account">The account request.</param>
        /// <returns></returns>
        [HttpPost("cart/current")]
        public IActionResult GetCart([FromBody] AccountRequest account)
        {
            var resultingCart = new Cart();
                
            var cookieContainer = new CookieContainer();
            var client =
                new HttpClient(new HttpClientHandler {UseCookies = true, CookieContainer = cookieContainer})
                {
                    BaseAddress = new Uri("https://www.prestocard.ca")
                };

            var loginResult = Login(account, ref client, ref cookieContainer);
            if (loginResult != ResponseCode.AccessOk)
            {
                resultingCart.Error = "Error logging in.";
                return BadRequest(resultingCart);
            }
            
            var resultBody = client.GetAsync("/en/dashboard").Result.Content.ReadAsStreamAsync().Result;
            var doc = new HtmlDocument();
            doc.Load(resultBody);

            // Select the <script> tag that contains JSON data for the current items in the user's cart.
            var scripts = doc.DocumentNode.SelectSingleNode("//script[preceding::footer]");
            var cartJson = Regex.Match(scripts.InnerText, @"({"")([^;]*)(})").Groups[0].Value;
           
            var json = JObject.Parse(cartJson);

            resultingCart = new Cart
            {
                CardNumber = account.Cards[0],
                Id = (string) json["CartID"],
                SubTotal = decimal.Parse((string) json["SubTotalForMoneris"])
            };

            // Gets all the carts for each cards
            var fareMedias = (JArray) json["FareMedias"];
            foreach (var t in fareMedias)
            {
                // We're only interested in the cart for the requested card.
                if ((string) t["VisibleId"] != account.Cards[0]) continue;
                
                // Gets all the products from each cart
                var jsonProducts = (JArray) t["Products"];
                foreach (var p in jsonProducts)
                {
                    var product = new Product
                    {
                        Concession = (string) p["Concession"],
                        Id = (string) p["ProductID"],
                        Name = (string) p["ProductName"],
                        Price = decimal.Parse((string) p["ListPrice"], NumberStyles.Currency),
                        Quantity = int.Parse((string) p["Quantity"]),
                        LineItemId = (string) p["LineItemID"]
                    };
                        
                    resultingCart.Products.Add(product);
                }
            }            
            
            return Ok(resultingCart);
        }

        /// <summary>
        /// Gets the list of passes that the selected card is elegible for.
        /// </summary>
        /// <param name="account">The AccountRequest object for accessing the PRESTO site</param>
        /// <param name="agency">The name of the Agency to look up passes for.</param>
        /// <returns></returns>
        [HttpPost("cart/passes")]
        public IActionResult GetAvailablePasses([FromBody] AccountRequest account, [FromQuery] string agency)
        {
            var postContent =
                $"{{productFieldDetails:{{\"AgencyName\":\"{agency}\",\"ProductImageSrc\":\"/~/media/AFMS/Images/addpressIcon.ashx\",\"Details\":\"Details\",\"AddCartButtonText\":\"Add To Cart\",\"SelectCatalog\":[\"6b084e0d-eb8d-5220-8056-2dd530a67c96\",\"6edcc4c3-f38d-5d28-9ab2-107720e2796d\"],\"SelectDateTimeFormat\":[{{\"__interceptors\":[{{}}],\"ID\":\"ea8402ab-c5ef-4473-ab5c-d3b499ea8ac6\",\"DateTimeFormat\":\"MM/dd/yyyy\"}}],\"ProductsNotAvailable\":\"A transit pass may be in your shopping cart already, or can’t be purchased at this time. Please proceed to checkout.\",\"ProductsNotAvailableError\":\"A transit pass may be in your shopping cart already, or can’t be purchased at this time. Please proceed to checkout.\",\"DetailsLinkAlt\":\"Show more details for {{0}}\",\"AddButtonAlt\":\"Add {{0}} to cart\",\"DetailsCloseAlt\":\"Close {{0}} details\",\"DetailsMDPLinkAlt\":\"View Discount Plan for {{0}}\",\"DetailsMDPCloseAlt\":\"Close {{0}} Payment Plan\",\"DescriptionMDP\":\"Save an average of XX% per month\",\"UrlMDP\":\"View Discount Plan\",\"ButtonTextMDP\":\"Sign Up\",\"PopupButtonTextMDP\":\"Sign Up\",\"PopupCancelButtonTextMDP\":\"Cancel\",\"PopupTextMDP\":\"You are being redirected to the TTC 12 Month Pass sign-up page where you can complete your transaction. If you have any items in your shopping cart, they will remain there until you have completed the TTC 12 Month Pass sign-up and must be processed separately. \",\"PaymentDetailsPopupTitle\":\"TTC 12 Month Pass Details\",\"PaymentDetailsPopupDescription\":\"The TTC 12 Month Pass is a subscription service that gives PRESTO users unlimited TTC travel at a discounted rate. The monthly discounts for the TTC 12 Month Pass period pass you have selected are shown below. In order to be eligible for the discount, you must be enrolled in the program throughout the entire term. If you cancel the contract before the conclusion of your term, you will be liable to pay TTC the discount amount you receive before cancellation. For full terms and conditions, please refer to the link in the Payment Details. \r\n\",\"PaymentDetailsPopupTableDescription\":\"{{0}} Discounts\",\"PaymentDetailsPopupColumnOne\":\"Term Month\",\"PaymentDetailsPopupColumnTwo\":\"Monthly Discount\",\"PaymentDetailsPopupColumnThree\":\"Discounted Monthly Price\"}}}}";
            
            var cookieContainer = new CookieContainer();
            var client =
                new HttpClient(new HttpClientHandler {UseCookies = true, CookieContainer = cookieContainer})
                {
                    BaseAddress = new Uri("https://www.prestocard.ca")
                };

            var loginResult = Login(account, ref client, ref cookieContainer);
            if (loginResult != ResponseCode.AccessOk)
            {
                return BadRequest();
            }

            var message = new HttpRequestMessage(HttpMethod.Post, "https://www.prestocard.ca/api/sitecore/ShoppingCart/GetProductsList")
            {
                Content = new StringContent(postContent, Encoding.UTF8, "application/json")
            };
            
            var result = client.SendAsync(message).Result.Content.ReadAsStreamAsync().Result;
            var doc = new HtmlDocument();
            doc.Load(result);

            var items = doc.DocumentNode.SelectNodes(
                "//div[contains(@class, 'row loadMyCard load-card__pass-container getselectorToHide')]");
            
            Console.WriteLine(items.Count);

            foreach (var item in items)
            {
                
            }
            
            return NotFound();
        }
    }
}
