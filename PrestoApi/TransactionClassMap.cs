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

using CsvHelper.Configuration;
using PrestoApi.Models.Presto;

namespace PrestoApi
{
    public sealed class TransactionClassMap : CsvClassMap<TransactionResponse>
    {
        public TransactionClassMap()
        {
            Map(m => m.Date).Name("Date").TypeConverter<DateConverter>();
            Map(m => m.Agency).Name("Transit Agency");
            Map(m => m.Location).Name("Location");
            Map(m => m.Type).Name("Type ");
            Map(m => m.ServiceClass).Name("Service Class");
            Map(m => m.Discount).Name("Discount");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.Balance).Name("Balance");
        }
    }
}