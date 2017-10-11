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
    /// <summary>
    /// Response object for PRESTO requests
    /// </summary>
    public class Response
    {
        public Response()
        {
            Cards = new List<CardResponse>();
        }

        /// <summary>
        /// The responding account's username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Whether the account is of a registered type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The list of cards being responded 
        /// </summary>
        public IList<CardResponse> Cards { get; set; }

        /// <summary> Whether any errors were reported when accessing this account
        /// <para>See <see cref="ResponseCode"/> for a list of possible error codes</para>
        /// </summary>
        public int Error { get; set; }
    }

    /// <summary>
    /// The response object for a single PRESTO card.
    /// </summary>
    public class CardResponse
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CardResponse()
        {
            Products = new List<ProductResponse>();
            Transactions = new List<TransactionResponse>();
            Pending = new List<PendingResponse>();
        }

        /// <summary>
        /// The card's nickname
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The card's serial number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// The current balance on the card
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// The expiration date of the card
        /// </summary>
        public long Expiration { get; set; }

        /// <summary>
        /// The list of <see cref="ProductResponse"/> associated with this card.
        /// <para></para>
        /// <seealso cref="ProductResponse"/>
        /// </summary>
        public IList<ProductResponse> Products { get; set; }

        /// <summary>
        /// The list of <see cref="TransactionResponse"/> on this card
        /// <para>Transactions are usually payments done by the card such as fare payements, and pass usages</para>
        /// <seealso cref="TransactionResponse"/>
        /// </summary>
        public IList<TransactionResponse> Transactions { get; set; }

        /// <summary>
        /// The list of <see cref="PendingResponse"/> for this card.
        /// </summary>
        public IList<PendingResponse> Pending { get; set; }
    }

    /// <summary>
    /// A transaction that may have taken place on a PRESTO card
    /// </summary>
    public class TransactionResponse
    {
        /// <summary>
        /// The date and time that this transaction took place
        /// </summary>
        public long Date { get; set; }

        /// <summary>
        /// The transit agency in which this transaction took place (e.g. OC Transpo, TTC)
        /// </summary>
        public string Agency { get; set; }

        /// <summary>
        /// The name of the location where this transaction took place.
        /// Usually the name of a stop, or station.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The type of transaction that took place.
        /// <para>Transactions encapsulates fare payments, transit pass payments, activation
        /// and fare loading.</para>
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Not sure what this is, but it's included in the csv file..
        /// </summary>
        public string ServiceClass { get; set; }

        /// <summary>
        /// Any discount that was applied to the transaction(?).
        /// </summary>
        public string Discount { get; set; }

        /// <summary>
        /// The balance subtracted by this transaction
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// The remaining balance after the transaction
        /// </summary>
        public string Balance { get; set; }
    }

    /// <summary>
    /// A PRESTO "product" that could be on a card.
    /// <para>Each registered card has at least one product on it which is usually the base fare on the card.</para>
    /// <para>Additional products include monthly or yearly passes, and autoload(?).</para>
    /// </summary>
    public class ProductResponse
    {
        /// <summary>
        /// The balance that this product contains, if any
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// The name of this product.
        /// <para>Used in determining the names of passes, usually.</para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The start date at which this product becomes valid.
        /// <para>Used for monthly passes</para>
        /// </summary>
        public long ValidityStartDate { get; set; }

        /// <summary>
        /// The end date at which this product becomes valid.
        /// <para>Used for monthly passes</para>
        /// </summary>
        public long ValidityEndDate { get; set; }
    }

    /// <summary>
    /// A set of response error codes for PRESTO website access
    /// </summary>
    public static class ResponseCode
    {
        /// <summary>
        /// No error. Everything is OK.
        /// </summary>
        public const int AccessOk = 0;

        /// <summary>
        /// The user has tried to login using an incorrect username or password
        /// </summary>
        public const int WrongUsernamePassword = 5;

        /// <summary>
        /// The user has tried to login using the wrong type of account. (i.e. Registered instead of Anonymous)
        /// </summary>
        public const int WrongType = 10;

        /// <summary>
        /// The user's account has been locked. Likely due to too many failed login atempts.
        /// </summary>
        public const int AccountLocked = 20;

        /// <summary>
        /// The user has tried to login anonymously using an invalid serial code (a.k.a card number)
        /// </summary>
        public const int BadSerialCode = 30;

        /// <summary>
        /// Just in case anything else explodes or something
        /// </summary>
        public const int OtherError = -1;

        /// <summary>
        /// Status code when a login attempt is made with insufficient authentication data.
        /// Only used when logging in using non-password authentication methods.
        /// </summary>
        public const int BadAuth = 50;
    }

    /// <summary>
    /// A pending amount of money waiting to be loaded onto a physical PRESTO card.
    /// </summary>
    public class PendingResponse
    {
        /// <summary>
        /// The amount of money pending
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// The name of the pending load
        /// </summary>
        public string Name { get; set; }
    }
}
