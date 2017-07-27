using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace PrestoApi
{
    public class DateConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, CsvPropertyMapData propertyMapData)
        {
            try
            {
                var dateTime = DateTime.Parse(text);
                var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                var diff = dateTime.ToUniversalTime() - origin;
                return (long) Math.Floor(diff.TotalSeconds);
            }
            catch (Exception e)
            {
                Console.Out.Write(e.Message);
            }
            return  base.ConvertFromString(text, row, propertyMapData);
        }
    }
}