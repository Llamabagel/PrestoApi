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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.CommandLineUtils;
using PrestoApi.Models.Presto;

namespace PrestoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var commandLineApplication = new CommandLineApplication(false);

            var login = commandLineApplication.Option(
                "-a | --auth",
                "Get authentication information to use for testing the API.",
                CommandOptionType.NoValue
            );

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                if (login.HasValue())
                {
                    Console.Write("Username: ");
                    var username = Console.ReadLine();

                    Console.Write("Password: ");
                    var password = Console.ReadLine();

                    TestingLogin(username, password);
                }
                else
                {
                    var host = new WebHostBuilder()
                        .UseKestrel()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseIISIntegration()
                        .UseStartup<Startup>()
                        .UseApplicationInsights()
                        .UseUrls("http://0.0.0.0:32565")
                        .Build();

                    host.Run();
                }

                return 0;
            });

            commandLineApplication.Execute(args);
        }

        private static Auth TestingLogin(string username, string password)
        {

            var loginJson =
                $"{{\"custSecurity\":{{\"Login\":\"{username}\",\"Password\":\"{password}\"}},\"anonymousOrderACard\":false}}";

            var cookieContainer = new CookieContainer();
            // Create an HttpClient instance to handle all web operations on the PRESTO card site for this account.
            var client =
                new HttpClient(new HttpClientHandler {UseCookies = true, CookieContainer = cookieContainer})
                {
                    BaseAddress = new Uri("https://www.prestocard.ca")
                };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post,
                "https://www.prestocard.ca/api/sitecore/AFMSAuthentication/SignInWithAccount")
            {
                Content = new StringContent(loginJson, Encoding.UTF8, "application/json")
            };

            var result = client.SendAsync(requestMessage).Result;

            var setCookies = result.Content.Headers.GetEnumerator();

            do
            {
                Console.WriteLine(setCookies.Current.Value);
            } while (setCookies.MoveNext());

            return null;
        }
    }
}
