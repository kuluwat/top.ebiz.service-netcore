using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Web;

namespace top.ebiz.service.Models
{
    public static class StringExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }
        public static List<string> GetShiftIdFromtree(this string text)
        {

            return text?.Split('|').Where(s => !s.IsNullOrEmpty() && s.Contains("shift_")).Select(x => x.Replace("shift_", "").Replace("&", "")).ToList();
        }
        public static int? StringToInt(this string text)
        {
            var test = 0;
            
            if (int.TryParse(text, out test))
            {
                return test;            }
            else
                return null;
        }
        public static bool IsNumber(this string text)
        {
            var test = 0;
            return int.TryParse(text, out test);
        }

        public static List<T> MapToList<T>(this DbDataReader dr) where T : new()
        {
            if (dr != null && dr.HasRows)
            {
                var entity = typeof(T);
                var entities = new List<T>();
                var propDict = new Dictionary<string, PropertyInfo>();
                var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                propDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);

                while (dr.Read())
                {
                    try
                    {
                        T newObject = new T();
                        for (int index = 0; index < dr.FieldCount; index++)
                        {
                            var gg = dr.GetName(index).ToUpper();
                            if (propDict.ContainsKey(dr.GetName(index).ToUpper()))
                            {
                                var info = propDict[dr.GetName(index).ToUpper()];
                                if ((info != null) && info.CanWrite)
                                {
                                    try
                                    {
                                        var val = dr.GetValue(index);
                                        info.SetValue(newObject, (val == DBNull.Value) ? null : val, null);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }
                        entities.Add(newObject);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return entities;
            }
            return null;
        }
    }
    public static class DateTimeExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.

    }
    public static class IntExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
    }
    public static class FloatExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
    }
}