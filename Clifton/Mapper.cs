using System;
using System.Linq;
using System.Reflection;

namespace Clifton
{
    public class MapperPropertyAttribute : Attribute
    {
        public string Name { get; set; }

        public MapperPropertyAttribute()
        {
        }
    }

    // https://www.codeproject.com/Tips/807820/Simple-Model-Entity-Mapper-in-Csharp
    public static class MapExtensionMethods
    {
        public static TTarget MapTo<TSource, TTarget>(this TSource aSource, TTarget aTarget)
        {
            var ret = MapTo(aSource.GetType(), aSource, aTarget);

            return ret;
        }

        public static TTarget CreateMapped<TTarget>(this object aSource) where TTarget : new()
        {
            return MapTo(aSource.GetType(), aSource, new TTarget());
        }

        public static TTarget CreateMapped<TSource, TTarget>(this TSource aSource) where TTarget : new()
        {
            return aSource.MapTo(new TTarget());
        }

        private static TTarget MapTo<TTarget>(Type tSource, object aSource, TTarget aTarget)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            var srcFields = (from PropertyInfo aProp in tSource.GetProperties(flags)
                             where aProp.CanRead     //check if prop is readable
                             select new
                             {
                                 Name = aProp.Name,
                                 Alias = (string)null,
                                 Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType
                             }).ToList();

            var trgFields = (from PropertyInfo aProp in aTarget.GetType().GetProperties(flags)
                             where aProp.CanWrite   //check if prop is writeable
                             select new
                             {
                                 Name = aProp.Name,
                                 Alias = aProp.GetCustomAttribute<MapperPropertyAttribute>()?.Name,
                                 Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType
                             }).ToList();

            var commonFields = trgFields.In(srcFields, /* T1 */ t => t.Alias ?? t.Name, /* T2 */ t => t.Name).ToList();

            foreach (var aField in commonFields)
            {
                var value = tSource.GetProperty(aField.Alias ?? aField.Name).GetValue(aSource, null);
                PropertyInfo propertyInfos = aTarget.GetType().GetProperty(aField.Name);
                propertyInfos.SetValue(aTarget, value, null);
            }

            return aTarget;
        }
    }
}
