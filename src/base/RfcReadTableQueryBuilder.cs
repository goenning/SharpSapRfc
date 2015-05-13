using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc
{
    public class RfcReadTableQueryBuilder<T>
    {
        private string tableName;
        private string[] fields;
        private List<string> conditions;
        private int count;
        private int skip;
        private SapRfcConnection connection;

        public RfcReadTableQueryBuilder(SapRfcConnection connection, string tableName)
        {
            this.tableName = tableName;
            this.connection = connection;
            this.conditions = new List<string>();
        }

        public IEnumerable<T> Read()
        {
            return this.connection.ReadTable<T>(this.tableName, fields, where: conditions.ToArray(), skip: skip, count: count);
        }

        public T ReadOne()
        {
            this.count = 1;
            return this.Read().FirstOrDefault();
        }

        public RfcReadTableQueryBuilder<T> Select(params string[] fields)
        {
            this.fields = fields;
            return this;
        }

        private string FromValueToString(object value)
        {
            if (value == null)
                return "''";

            Type type = value.GetType();
            if (type == typeof(string))
                return string.Format("'{0}'", value);

            return value.ToString();
        }

        public RfcReadTableQueryBuilder<T> Where(string condition)
        {
            this.conditions.Add(condition);
            return this;
        }

        public RfcReadTableQueryBuilder<T> And(string condition)
        {
            return this.Where(string.Concat("AND ", condition));
        }

        public RfcReadTableQueryBuilder<T> Or(string condition)
        {
            return this.Where(string.Concat("OR ", condition));
        }

        public RfcReadTableQueryBuilder<T> Where(string field, RfcReadTableOption option, object value)
        {
            string optionString = this.FromOptionToString(option);
            string valueString = this.FromValueToString(value);
            return Where(string.Format("{0} {1} {2}", field, optionString, valueString));
        }

        public RfcReadTableQueryBuilder<T> And(string field, RfcReadTableOption option, object value)
        {
            string optionString = this.FromOptionToString(option);
            string valueString = this.FromValueToString(value);
            return And(string.Format("{0} {1} {2}", field, optionString, valueString));
        }

        public RfcReadTableQueryBuilder<T> Or(string field, RfcReadTableOption option, object value)
        {
            string optionString = this.FromOptionToString(option);
            string valueString = this.FromValueToString(value);
            return Or(string.Format("{0} {1} {2}", field, optionString, valueString));
        }

        private string FromOptionToString(RfcReadTableOption option)
        {
            switch (option)
            {
                case RfcReadTableOption.Equals:
                    return "EQ";
                case RfcReadTableOption.NotEquals:
                    return "NE";
                case RfcReadTableOption.GreaterThan:
                    return "GT";
                case RfcReadTableOption.LessThan:
                    return "LT";
                case RfcReadTableOption.GreaterOrEqualThan:
                    return "GE";
                case RfcReadTableOption.LessOrEqualThan:
                    return "LE";
                default:
                    throw new NotImplementedException();
            }
        }

        public RfcReadTableQueryBuilder<T> Take(int count)
        {
            this.count = count;
            return this;
        }

        public RfcReadTableQueryBuilder<T> Skip(int count)
        {
            this.skip = count;
            return this;
        }
    }
}
