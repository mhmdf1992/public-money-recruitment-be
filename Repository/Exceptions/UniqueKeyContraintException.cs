using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Exceptions
{
    public class UniqueKeyContraintException : Exception
    {
        string _param = "Key";
        public string Param => _param;
        string _value;
        public string Value => _value;
        public UniqueKeyContraintException(IEnumerable<string> keys) : base($"Key(s) {string.Join(",", keys)} already exist")
        {
            _value = string.Join(",", keys);
        }
    }
}
