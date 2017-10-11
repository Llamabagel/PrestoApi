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

using System.Collections.Generic;

namespace PrestoApi.Models.Presto
{
    public class Request
    {
        public const string TypeRegistered = "REGISTERED";
        public const string TypeAnonymous = "ANONYMOUS";

        /// <summary>
        /// The list of accounts to get updates for.
        /// <para>See <see cref="AccountRequest"/></para>
        /// </summary>
        public IList<AccountRequest> Accounts { get; set; }
        
        /// <summary>
        /// If true, the API will omit things like Transactions and Pending items.
        /// </summary>
        public bool OmitExtraInfo { get; set; }
    }

    public class AccountRequest
    {
        /// <summary>
        /// A list of cards to get updated info about from the PRESTO website
        /// </summary>
        public IList<string> Cards { get; set; }

        /// <summary>
        /// Whether this accound is a registered account.
        /// If false, then this "account" represents an anonymous account where the username is the
        /// serial number of the anonymous PRESTO card
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The username of the account being accessed. For anonymous cards, this is the serial number
        /// of that PRESTO card.
        /// <see cref="Registered"/>
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password of the account being accessed. Null for anonymous cards.
        /// </summary>
        public string Password { get; set; }
    }
}
