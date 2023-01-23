using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Carpeddit.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, object> AsDictionary<T>(this T source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return typeof(T).GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }
    }
}
