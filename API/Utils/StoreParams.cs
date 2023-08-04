using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace API.Utils;

public static class StoreParams
{

    public static void StoreTempData(ViewDataDictionary tempData, params object[] actionArgumentsValues)
    {
        tempData.Clear();
        foreach (var obj in actionArgumentsValues)
        {
            if (obj == null)
            {
                continue;
            }

            foreach (var property in obj.GetType().GetProperties())
            {
                try
                {
                    tempData.Add(property.Name, property.GetValue(obj));
                }
                catch
                {
                    Console.WriteLine("Save failed");
                    // ignored
                }
            }
        }
    }

}