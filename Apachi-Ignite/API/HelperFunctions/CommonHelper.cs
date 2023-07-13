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
        var fields = GetPropertyNames(type);

        foreach (var item in objects)
        {
            var obj = GetInstance(type);
            
            if (obj == null)
                throw new NullReferenceException();

            if (fields.Count != item.Count)
                throw new Exception("Invalid object..!");

            for(int i = 0; i < item.Count; i++)
            {
                var propertyName = fields[i];
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

    private static List<string> GetPropertyNames(Type type)
    {
        var result = new List<string>();
        var fields = type.GetProperties();
        foreach(var field in fields)
        {
            var name = field.Name;
            result.Add(name);
        }

        return result;
    }

}
