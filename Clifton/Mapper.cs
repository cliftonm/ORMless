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
    // And then there's this "update": https://www.codeproject.com/Tips/885770/Simple-Model-Entity-Mapper-in-Csharp-2
    // And if you want a mapper that builds the mapper expressions: https://github.com/AutoMapper/AutoMapper
    public static class MapExtensionMethods
    {
        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            var ret = MapTo(source.GetType(), source, target);

            return ret;
        }

        public static TTarget CreateMapped<TTarget>(this object source) where TTarget : new()
        {
            return MapTo(source.GetType(), source, new TTarget());
        }

        public static TTarget CreateMapped<TSource, TTarget>(this TSource source) where TTarget : new()
        {
            return source.MapTo(new TTarget());
        }

        private static TTarget MapTo<TTarget>(Type tSource, object source, TTarget target)
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

            var trgFields = (from PropertyInfo aProp in target.GetType().GetProperties(flags)
                             where aProp.CanWrite   //check if prop is writeable
                             select new
                             {
                                 Name = aProp.Name,
                                 Alias = aProp.GetCustomAttribute<MapperPropertyAttribute>()?.Name,
                                 Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType
                             }).ToList();

            var commonFields = trgFields.In(srcFields, /* T1 */ t => t.Alias ?? t.Name, /* T2 */ t => t.Name).ToList();

            foreach (var field in commonFields)
            {
                var value = tSource.GetProperty(field.Alias ?? field.Name).GetValue(source, null);
                PropertyInfo propertyInfos = target.GetType().GetProperty(field.Name);
                propertyInfos.SetValue(target, value, null);
            }

            return target;
        }
    }
}
