using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamManager.Classes
{
    public class Utils
    {
        public static object GetPropertyValue(object obj1, object obj2, string property)
        {
            System.Reflection.PropertyInfo propertyInfo = obj1.GetType().GetProperty(property);
            if (propertyInfo == null)
            {
                propertyInfo = obj2.GetType().GetProperty(property);
                return propertyInfo.GetValue(obj2, null);
            }
            else
                return propertyInfo.GetValue(obj1, null);

        }
    }
}