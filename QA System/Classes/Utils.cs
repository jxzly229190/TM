using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace QA_System.Classes
{
    public static class Utils
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

        /// <summary>
        /// Create a SelectList from Enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static SelectList ToSelectList<TEnum>(TEnum enumObj)
        {
            var items = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new {
                             Value = (int)Enum.Parse(typeof(TEnum), e.ToString()), 
                             Text = DisplayName(e as Enum) 
                         };

            return new SelectList(items, "Value", "Text", ((int)Enum.Parse(typeof(TEnum), enumObj.ToString())).ToString());

        }

        /// <summary>
        /// Get Display Name of Enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DisplayName(this Enum value)
        {
            Type enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            MemberInfo member = enumType.GetMember(enumValue)[0];

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            var outString = ((DisplayAttribute)attrs[0]).Name;

            if (((DisplayAttribute)attrs[0]).ResourceType != null)
            {
                outString = ((DisplayAttribute)attrs[0]).GetName();
            }

            return outString;
        }

        public static string FormatCommentXml(DateTime addDate, string addUser, string comments)
        {
            string commentXml = string.Empty;
            commentXml = string.Format("<comment date='{0}' user='{1}'>{2}</comment>", addDate.ToString(), addUser, comments);
            return commentXml;
        }
    }
}