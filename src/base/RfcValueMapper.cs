using SharpSapRfc.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SharpSapRfc
{
    public abstract class RfcValueMapper
    {
        protected abstract NumberFormatInfo GetNumberFormat();
        
        protected NumberFormatInfo CommaDecimalNumberFormat 
        {
            get { return new NumberFormatInfo() { NumberDecimalSeparator = "," }; } 
        }

        protected NumberFormatInfo PeriodDecimalNumberFormat 
        {
            get { return new NumberFormatInfo() { NumberDecimalSeparator = "." }; } 
        }

        protected static Dictionary<Type, Dictionary<string, string>> cachedEnumMembers = new Dictionary<Type, Dictionary<string, string>>();

        protected static void EnsureEnumTypeIsCached(Type enumType)
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
                    var memberInfo = enumType.GetMember(name);
                    var attributes = memberInfo[0].GetCustomAttributes(typeof(RfcEnumValueAttribute), false);
                    if (attributes.Length > 0)
                    {
                        var enumValue = ((RfcEnumValueAttribute)attributes[0]).Value;
                        membersByName.Add(name, enumValue);
                    }
                }
                cachedEnumMembers.Add(enumType, membersByName);
            }
        }

        public virtual object FromRemoteValue(Type type, object value)
        {
             if (type.IsEnum)
                return this.ConvertToEnum(type, value);

            if (value == null || value.ToString().Equals(""))
            {
                if (type.IsNullable())
                    return null;

                return Activator.CreateInstance(type);
            }

            if (type.Equals(typeof(Boolean)))
                return AbapBool.FromString(value.ToString());

            if (type.Equals(typeof(DateTime)))
                return AbapDateTime.FromString(value.ToString());
            
            if (type.Equals(typeof(DateTime?)))
                return AbapDateTime.FromString(value.ToString(), true);
            
            if (type.Equals(typeof(Decimal)))
                return Convert.ToDecimal(value, value.ToString().Contains(",") ? this.CommaDecimalNumberFormat : this.PeriodDecimalNumberFormat);
            
            if (type.Equals(typeof(Double)))
                return Convert.ToDouble(value, value.ToString().Contains(",") ? this.CommaDecimalNumberFormat : this.PeriodDecimalNumberFormat);

            if (type.Equals(typeof(byte[])))
                return this.ToBytes(value);

            if (type.Equals(typeof(Stream)))
                return new MemoryStream(this.ToBytes(value));   

            return Convert.ChangeType(value, type);
        }

        private byte[] ToBytes(object value)
        {
            if (value is string)
                return Convert.FromBase64String(value.ToString());
            
            return (byte[])value;
        }

        public virtual object ConvertToEnum(Type enumType, object remoteValue)
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

        public virtual object ToRemoteValue(AbapDataType remoteType, object value)
        {
            if (value == null)
                return null;

            Type valueType = value.GetType();

            if (valueType == typeof(Boolean))
                return AbapBool.ToString((Boolean)value);

            if (valueType.Equals(typeof(double)))
                return ((double)value).ToString(this.GetNumberFormat());

            if (valueType.Equals(typeof(float)))
                return ((float)value).ToString(this.GetNumberFormat());
            
            if (valueType.Equals(typeof(Decimal)))
                return ((Decimal)value).ToString(this.GetNumberFormat());
            
            if (valueType.Equals(typeof(Double)))
                return ((Double)value).ToString(this.GetNumberFormat());

            if (valueType.IsEnum)
                return ConvertFromEnum(remoteType, value);
                      
            if (remoteType == AbapDataType.DATE)
                return AbapDateTime.ToDateString((DateTime)value);
            
            if (remoteType == AbapDataType.TIME)
                return AbapDateTime.ToTimeString((DateTime)value);

            return value;
        }

        public virtual object ConvertFromEnum(AbapDataType remoteType, object value)
        {
            if (remoteType == AbapDataType.INTEGER ||
                remoteType == AbapDataType.SHORT)
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
