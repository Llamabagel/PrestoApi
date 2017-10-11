/**
 *  This file is part of Llamabagel's Presto Api.
 *
 *  Llamabagel's Presto Api is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Llamabagel's Presto Api is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Llamabagel's Presto Api.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
                Username = request.Username,
                Type = request.Type
            };

            var primaryAddress = new Uri("https://prestocard.ca");
            var client =
                new HttpClient(new HttpClientHandler {UseCookies = true, CookieContainer = new CookieContainer()})
                {
                    BaseAddress = primaryAddress
                };

            string loginJson;
            HttpRequestMessage httpRequest;

            if (request.Type == Models.Presto.Request.TypeRegistered)
            {
                // Create the login request
                loginJson = "{\"custSecurity\":{\"Login\":\"" + request.Username + "\",\"Password\":\"" +
                            request.Password + "\"},\"anonymousOrderACard\":false}";
                httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    "https://www.prestocard.ca/api/sitecore/AFMSAuthentication/SignInWithAccount")
                {
                    Content = new StringContent(loginJson, Encoding.UTF8, "application/json")
                };
            }
            else
            {
                // Create the anonymous login request
                loginJson = "{\"anonymousOrderACard\":true,\"fareMediaId\":\"" + request.Username + "\"}";
                httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    "https://www.prestocard.ca/api/sitecore/AFMSAuthentication/SignInWithFareMedia")
                {
                    Content = new StringContent(loginJson, Encoding.UTF8, "application/json")
                };
            }

            // Login to PRESTO and save the required auth cookies
            var loginResult = await client.SendAsync(httpRequest);
            var body = await loginResult.Content.ReadAsStringAsync();

            // Check for login errors
            if (request.Type == Models.Presto.Request.TypeRegistered)
            {
                switch (body)
                {
                    case
                    "\"You could not be logged in to your online account. Please check your username and password and try again.\""
                    :
                        summaryAccount.Error = ResponseCode.WrongUsernamePassword;
                        return summaryAccount;
                    case
                    "\"Your account has been locked as a result of three failed attempts to sign in. Please reset your password to access your account.\""
                    :
                        summaryAccount.Error = ResponseCode.AccountLocked;
                        return summaryAccount;
                }
            }
            if (request.Type == Models.Presto.Request.TypeAnonymous)
            {
                switch (body)
                {
                    case
                    "{\"result\":false,\"message\":\"Your PRESTO card is already registered to an account.Please log in using your username and password.\"}"
                    :
                        summaryAccount.Error = ResponseCode.WrongType;
                        return summaryAccount;
                    case "{\"result\":false,\"message\":\"Incorrect PRESTO card serial number\"}":
                        summaryAccount.Error = ResponseCode.BadSerialCode;
                        return summaryAccount;
                }
            }

            // Get the PRESTO dashboard
            var nextResult = await client.GetAsync("/en/dashboard");
            // Using?
            var newBody = await nextResult.Content.ReadAsStreamAsync();

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
                responses.Add(await GetAccountCards(requestedAccount, request.OmitExtraInfo));
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
                Username = account.Username,
                Type = account.Type
            };

            // Create an HttpClient instance to handle all web operations on the PRESTO card site for this account.
            var client =
                new HttpClient(new HttpClientHandler {UseCookies = true, CookieContainer = new CookieContainer()})
                {
                    BaseAddress = new Uri("https://prestocard.ca")
                };

            // JSON string containing login details for the account.
            string loginJson;
            // The Http request for to login with the account. Either for a Registered or Anonymous account.
            HttpRequestMessage httpRequest;

            if (account.Type == Models.Presto.Request.TypeRegistered)
            {
                // Create the login request
                loginJson = "{\"custSecurity\":{\"Login\":\"" + account.Username + "\",\"Password\":\"" +
                            account.Password + "\"},\"anonymousOrderACard\":false}";
                httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    "https://www.prestocard.ca/api/sitecore/AFMSAuthentication/SignInWithAccount")
                {
                    Content = new StringContent(loginJson, Encoding.UTF8, "application/json")
                };
            }
            else
            {
                // Create the anonymous login request
                loginJson = "{\"anonymousOrderACard\":true,\"fareMediaId\":\"" + account.Username + "\"}";
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
                newResponse.Error = ResponseCode.WrongUsernamePassword;
                return newResponse;
            }
            if (loginResult ==
                "\"Your account has been locked as a result of three failed attempts to sign in. Please reset your password to access your account.\""
            )
            {
                newResponse.Error = ResponseCode.AccountLocked;
                return newResponse;
            }
            if (loginResult ==
                "{\"result\":false,\"message\":\"Your PRESTO card is already registered to an account.Please log in using your username and password.\"}"
            )
            {
                newResponse.Error = ResponseCode.WrongType;
                return newResponse;
            }
            if (loginResult == "{\"result\":false,\"message\":\"Incorrect PRESTO card serial number\"}")
            {
                newResponse.Error = ResponseCode.BadSerialCode;
                return newResponse;
            }


            // Navigate to the PRESTO dashboard
            var nextResult = await client.GetAsync("/en/dashboard");
            var newBody = await nextResult.Content.ReadAsStreamAsync();

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
                                var responseAsync = await csvRequest.Result.Content.ReadAsStreamAsync();
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
    }
}
