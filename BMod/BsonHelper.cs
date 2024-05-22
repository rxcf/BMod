
using Il2CppKernys.Bson;
using System.Collections.Generic;

 
namespace BMod
{
  internal static class BsonHelper
  {
    public static List<string> sValueKeys = new List<string>()
    {
      "$time",
      "$timeutc",
      "$worlddata",
      "$wib",
      "$wiringdata",
      "$inventorydata",
      "$iib",
      "$inventorykey",
      "$ik"
    };

    public static BSONObject FormatBson(BSONObject bson)
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
