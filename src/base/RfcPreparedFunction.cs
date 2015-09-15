using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpSapRfc
{
    public abstract class RfcPreparedFunction
    {
        public RfcPreparedFunction()
        {
            this.parameters = new List<RfcParameter>();
        }

        protected List<RfcParameter> parameters;
        public abstract RfcResult Execute();

        public RfcPreparedFunction AddParameter(RfcParameter parameter)
        {
            this.parameters.Add(parameter);
            return this;
        }

        public RfcPreparedFunction AddParameter(RfcParameter[] parameters)
        {
            this.parameters.AddRange(parameters);
            return this;
        }

        public RfcPreparedFunction AddParameter(object parameters)
        {
            if (parameters != null)
            {
                Type t = parameters.GetType();
                PropertyInfo[] properties = t.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                    this.parameters.Add(new RfcParameter(properties[i].Name, properties[i].GetValue(parameters, null)));
            }
            return this;
        }
    }
}
