using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultiMiner.Utility
{
    public static class ObjectCopier
    {
        public static void CopyObject<TSource, TDestination>(TSource source, TDestination destination)
        {
            var sourceProperties = TypeDescriptor.GetProperties(typeof(TSource)).Cast<PropertyDescriptor>();
            var destinationProperties = TypeDescriptor.GetProperties(typeof(TDestination)).Cast<PropertyDescriptor>();
            
            foreach (var entityProperty in sourceProperties)
            {
                var property = entityProperty;
                var convertProperty = destinationProperties.FirstOrDefault(prop => prop.Name == property.Name);
                if (convertProperty != null)
                    convertProperty.SetValue(destination, Convert.ChangeType(entityProperty.GetValue(source), convertProperty.PropertyType));
            }
        }

        public static TDestination CloneObject<TSource, TDestination>(TSource source) where TDestination : new()
        {
            TDestination result = new TDestination();

            CopyObject(source, result);

            return result;
        }

        public static TDestination DeepCloneObject<TSource, TDestination>(TSource source)
        {
            TDestination objResult = default(TDestination);
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, source);

                ms.Position = 0;
                objResult = (TDestination)bf.Deserialize(ms);
            }
            return objResult;
        }
    }
}
