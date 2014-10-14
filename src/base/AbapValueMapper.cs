using SharpSapRfc.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpSapRfc
{
    public class AbapValueMapper
    {
        private static CultureInfo enUS = new CultureInfo("en-US");
        private static CultureInfo ptBR = new CultureInfo("pt-BR");

        private static Dictionary<Type, Dictionary<string, string>> cachedEnumMembers = new Dictionary<Type, Dictionary<string, string>>();

        private static void EnsureEnumTypeIsCached(Type enumType)
        {
            if (!enumType.IsEnum || cachedEnumMembers.ContainsKey(enumType))
                return;

            lock (enumType)
            {
                if (cachedEnumMembers.ContainsKey(enumType))
                    return;

                Dictionary<string, string> membersByName = new Dictionary<string, string>();
                foreach (string name in Enum.GetNames(enumType))
                {
                    var memInfo = enumType.GetMember(name);
                    var attributes = memInfo[0].GetCustomAttributes(typeof(RfcEnumValueAttribute), false);
                    if (attributes.Length > 0)
                    {
                        var enumValue = ((RfcEnumValueAttribute)attributes[0]).Value;
                        membersByName.Add(name, enumValue);
                    }
                }
                cachedEnumMembers.Add(enumType, membersByName);
            }
        }

        public static object FromRemoteValue(Type type, object value)
        {
            object returnValue = null;
            if (type.Equals(typeof(Boolean)))
                returnValue = AbapBool.FromString(value.ToString());
            else if (type.Equals(typeof(DateTime)))
                returnValue = AbapDateTime.FromString(value.ToString());
            else if (type.Equals(typeof(DateTime?)))
                returnValue = AbapDateTime.FromString(value.ToString(), true);
            else if (type.Equals(typeof(Int32)))
                returnValue = Convert.ToInt32(value);
            else if (type.Equals(typeof(Decimal)))
                returnValue = Convert.ToDecimal(value, value.ToString().Contains(",") ? ptBR : enUS);
            else if (type.Equals(typeof(Double)))
                returnValue = Convert.ToDouble(value, value.ToString().Contains(",") ? ptBR : enUS);
            else if (type.IsEnum)
                returnValue = ConvertToEnum(type, value);
            else if (type.Equals(typeof(byte[])))
            {
                if (value is string)
                    returnValue = Convert.FromBase64String(value.ToString());
                else
                    returnValue = (byte[])value;
            }
            else if (type.Equals(typeof(Stream)))
            {
                if (value is string)
                    returnValue = new MemoryStream(Convert.FromBase64String(value.ToString()));
                else
                    returnValue = new MemoryStream((byte[])value);
            }
            else
                returnValue = Convert.ChangeType(value, type);

            return returnValue;
        }

        private static object ConvertToEnum(Type enumType, object remoteValue)
        {
            if (remoteValue != null)
            { 
                int hashCode = 0;
                if (int.TryParse(remoteValue.ToString(), out hashCode))
                    if (Enum.IsDefined(enumType, hashCode))
                        return Enum.ToObject(enumType, hashCode);

                EnsureEnumTypeIsCached(enumType);
                foreach (var keyPair in cachedEnumMembers[enumType])
                {
                    if (keyPair.Value == remoteValue.ToString())
                        return Enum.Parse(enumType, keyPair.Key);
                }
            }

            throw new RfcMappingException(string.Format("Cannot convert from remote value '{0}' to enum type '{1}'.", remoteValue, enumType.Name));
        }

        public static object ToRemoteValue(AbapDataType remoteType, object value)
        {
            if (value == null)
                return null;

            Type valueType = value.GetType();
            object returnValue = value;

            if (valueType.Equals(typeof(Boolean)))
                returnValue = AbapBool.ToString((Boolean)value);
            else if (remoteType == AbapDataType.DATE)
                returnValue = AbapDateTime.ToString((DateTime)value, AbapDateTimeType.Date);
            else if (remoteType == AbapDataType.TIME)
                returnValue = AbapDateTime.ToString((DateTime)value, AbapDateTimeType.Time);
            else if (valueType.IsEnum)
                returnValue = ConvertFromEnum(remoteType, value);

            return returnValue;
        }

        private static object ConvertFromEnum(AbapDataType remoteType, object value)
        {
            if (remoteType == AbapDataType.INTEGER)
                return value.GetHashCode();

            if (remoteType == AbapDataType.NUMERIC)
                return value.GetHashCode().ToString();

            if (remoteType == AbapDataType.CHAR)
            {
                Type enumType = value.GetType();
                EnsureEnumTypeIsCached(enumType);
                foreach (var keyPair in cachedEnumMembers[enumType])
                {
                    if (keyPair.Key == value.ToString())
                        return keyPair.Value;
                }
                return string.Empty;
            }

            throw new RfcMappingException(string.Format("Cannot convert from Enum '{0}' to remote type '{1}'.", value, remoteType.ToString()));
        }
    }
}
