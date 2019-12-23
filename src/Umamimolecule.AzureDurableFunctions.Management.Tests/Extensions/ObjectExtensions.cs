using System.Reflection;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests.Extensions
{
    public static class ObjectExtensions
    {
        public static void SetPropertyValue(this object @this, string name, object value)
        {
            @this.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(@this, value);
        }
    }
}
