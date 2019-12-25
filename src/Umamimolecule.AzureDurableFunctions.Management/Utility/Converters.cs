using System;
using System.Collections.Generic;
using System.Linq;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;

namespace Umamimolecule.AzureDurableFunctions.Management.Utility
{
    public static class Converters
    {
        public static Func<string, IEnumerable<TEnum>> EnumCollectionConverter<TEnum>(string parameterName, IEnumerable<TEnum> validValues = null) where TEnum : struct
        {
            return value =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return null;
                    }

                    List<TEnum> result = new List<TEnum>();
                    var items = value.Split(new string[] { ",", ";", " " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in items)
                    {
                        try
                        {
                            var enumValue = (TEnum)Enum.Parse(typeof(TEnum), item, true);
                            if (validValues != null && !validValues.Contains(enumValue))
                            {
                                var validValueString = string.Join(", ", validValues);
                                var message = string.Format(Resources.ExceptionMessages.InvalidParameterExceptionWithValues, item, parameterName, validValueString);
                                throw new InvalidParameterException(message, parameterName, item);
                            }

                            result.Add(enumValue);
                        }
                        catch (ArgumentException ae)
                        {
                            var validValueString = string.Join(", ", Enum.GetNames(typeof(TEnum)));
                            var message = string.Format(Resources.ExceptionMessages.InvalidParameterExceptionWithValues, item, parameterName, validValueString);
                            throw new InvalidParameterException(message, parameterName, item, ae);
                        }
                    }

                    return result;
                }
                catch (InvalidParameterException)
                {
                    throw;
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
