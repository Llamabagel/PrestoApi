using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrestoApi.Models.Presto
{
    /// <summary>
    /// A summarized version of a PRESTO account including a list of basic information on any cards tied with this account
    /// </summary>
    public class SummaryAccount
    {
        /// <summary>
        /// The account's username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Any errors that may have occured while accessing this account.
        /// <para>Possible errors are listed in <see cref="ResponseCode"/></para>
        /// </summary>
        public int Error { get; set; }

        /// <summary>
        /// Whether this account is a registered account
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The list of PRESTO cards tied with the summmary account.
        /// <para></para>
        /// <seealso cref="SummaryCard"/>
        /// </summary>
        public IList<SummaryCard> Cards { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SummaryAccount()
        {
            Cards = new List<SummaryCard>();
        }
    }

    /// <summary>
    /// A summarized version of a PRESTO card.
    /// <para>The summary only contains the name and serial number of the card</para>
    /// </summary>
    public class SummaryCard
    {
        /// <summary>
        /// The name of the card
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The 17-digit serial number on the back of the PRESTO card
        /// </summary>
        public string Number { get; set; }
    }
}
