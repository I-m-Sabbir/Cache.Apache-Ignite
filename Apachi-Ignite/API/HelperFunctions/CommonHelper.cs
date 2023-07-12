using Apache.Ignite.Core.Cache.Query;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.HelperFunctions;

public static class CommonHelper
{
    public static List<T> FieldsQueryCursorToList<T>(IFieldsQueryCursor objects)
    {
        var result = new List<T>();
        var type = typeof(T);
        var fields = objects.FieldNames;

        foreach (var item in objects)
        {
            var obj = GetInstance(type);
            
            if (obj == null)
                throw new NullReferenceException();

            if (fields.Count != item.Count)
                throw new Exception("Invalid object..!");

            for(int i = 0; i < item.Count; i++)
            {
                var propertyName = fields[i].ToLower();
                propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
                type.GetProperty(propertyName)!.SetValue(obj, item[i]);
            }

            result.Add((T)obj);
        }


        return result;
    }

    private static object? GetInstance(Type type)
    {
        ConstructorInfo? constructor = type.GetConstructor(new Type[] { });
        
        return (constructor == null) ? null : constructor.Invoke(new object[] { });
    }
}
