using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultiMiner.Utility.Serialization
{
    public static class ObjectCopier
    {
        public static void CopyObject<TSource, TDestination>(TSource source, TDestination destination, params string[] excludedProperties)
        {
            var sourceProperties = TypeDescriptor.GetProperties(typeof(TSource)).Cast<PropertyDescriptor>();
            var destinationProperties = TypeDescriptor.GetProperties(typeof(TDestination)).Cast<PropertyDescriptor>();

            foreach (var sourceProperty in sourceProperties)
            {
                if (excludedProperties.Contains(sourceProperty.Name))
                    continue;

                var destinationProperty = destinationProperties.FirstOrDefault(prop => prop.Name == sourceProperty.Name);
                if (destinationProperty != null)
                {
                    object sourceValue = sourceProperty.GetValue(source);
                    object destinationValue;
                    Type destinationPropertyType = destinationProperty.PropertyType;

                    //special handling for nullable types
                    Type underlyingType = Nullable.GetUnderlyingType(destinationPropertyType);

                    //special hanlding for enums
                    if (underlyingType == null && destinationPropertyType.IsEnum)
                        underlyingType = Enum.GetUnderlyingType(destinationPropertyType);

                    if (underlyingType != null)
                    {
                        if (sourceValue == null)
                            destinationValue = null;
                        else
                        {
                            destinationValue = Convert.ChangeType(sourceValue, underlyingType);
                            destinationProperty.SetValue(destination, destinationValue);
                        }
                    }
                    else
                    {
                        destinationValue = Convert.ChangeType(sourceValue, destinationPropertyType);
                        destinationProperty.SetValue(destination, destinationValue);
                    }
                }
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
