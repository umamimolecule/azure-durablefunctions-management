using System;
using System.Collections.Generic;
using System.Linq;
using Umamimolecule.AzureDurableFunctionsStatus.Exceptions;

namespace Umamimolecule.AzureDurableFunctionsStatus.Utility
{
    public static class Converters
    {
        public static Func<string, IEnumerable<TEnum>> EnumCollectionConverter<TEnum>(string parameterName) where TEnum : struct
        {
            return value =>
            {
                try
                {
                    List<TEnum> result = new List<TEnum>();
                    var items = value.Split(new string[] { ",", ";", " " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in items)
                    {
                        try
                        {
                            result.Add(Enum.Parse<TEnum>(item, true));
                        }
                        catch (ArgumentException ae)
                        {
                            var validValues = string.Join(", ", Enum.GetNames(typeof(TEnum)));
                            var message = string.Format(Resources.ExceptionMessages.InvalidParameterExceptionWithValues, item, parameterName, validValues);
                            throw new InvalidParameterException(message, parameterName, item, ae);
                        }
                    }

                    return result;
                }
                catch (InvalidParameterException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new InvalidParameterException(parameterName, value, e);
                }
            };
        }

        public static Func<string, IEnumerable<string>> StringCollectionConverter(string parameterName)
        {
            string[] delimiters = new string[] { ",", ";", " " };
            return value => value.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        }

        public static Func<string, DateTime> DateTimeConverter(string parameterName)
        {
            return value =>
            {
                try
                {
                    return DateTime.Parse(value);
                }
                catch (Exception e)
                {
                    throw new InvalidParameterException(parameterName, value, e);
                }
            };
        }

        public static Func<string, int> IntConverter(string parameterName)
        {
            return value =>
            {
                try
                {
                    return int.Parse(value);
                }
                catch (Exception e)
                {
                    throw new InvalidParameterException(parameterName, value, e);
                }
            };
        }

        public static Func<string, bool> BoolConverter(string parameterName)
        {
            // Truthy values are: yes, 1, true
            // Falsy values are:  no, 0, false
            // Anything else is considered invalid
            string[] truthy = new string[] { "yes", "1", "true" };
            string[] falsy = new string[] { "no", "0", "false" };

            return value =>
            {
                if (truthy.Any(x => string.Compare(x, value, true) == 0))
                {
                    return true;
                }
                else if (falsy.Any(x => string.Compare(x, value, true) == 0))
                {
                    return false;
                }

                throw new InvalidParameterException(parameterName, value);
            };
        }

    }
}
