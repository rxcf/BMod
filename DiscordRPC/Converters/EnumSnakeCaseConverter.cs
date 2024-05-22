﻿
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

 
namespace DiscordRPC.Converters
{
  internal class EnumSnakeCaseConverter : JsonConverter
  {
    public virtual bool CanConvert(Type objectType) => objectType.IsEnum;

    public virtual object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.Value == null)
        return (object) null;
      object obj = (object) null;
      return this.TryParseEnum(objectType, (string) reader.Value, out obj) ? obj : existingValue;
    }

    public virtual void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      Type type = value.GetType();
      string name = Enum.GetName(type, value);
      foreach (MemberInfo member in type.GetMembers(BindingFlags.Static | BindingFlags.Public))
      {
        if (member.Name.Equals(name))
        {
          object[] customAttributes = member.GetCustomAttributes(typeof (EnumValueAttribute), true);
          if (customAttributes.Length != 0)
            name = ((EnumValueAttribute) customAttributes[0]).Value;
        }
      }
      writer.WriteValue(name);
    }

    public bool TryParseEnum(Type enumType, string str, out object obj)
    {
      if (str == null)
      {
        obj = (object) null;
        return false;
      }
      Type enumType1 = enumType;
      if (enumType1.IsGenericType && enumType1.GetGenericTypeDefinition() == typeof (Nullable<>))
        enumType1 = ((IEnumerable<Type>) enumType1.GetGenericArguments()).First<Type>();
      if (!enumType1.IsEnum)
      {
        obj = (object) null;
        return false;
      }
      foreach (MemberInfo member in enumType1.GetMembers(BindingFlags.Static | BindingFlags.Public))
      {
        foreach (EnumValueAttribute customAttribute in member.GetCustomAttributes(typeof (EnumValueAttribute), true))
        {
          if (str.Equals(customAttribute.Value))
          {
            obj = Enum.Parse(enumType1, member.Name, true);
            return true;
          }
        }
      }
      obj = (object) null;
      return false;
    }
  }
}
