using System;
using System.Collections.Generic;
using System.Text;

namespace PIDataEdit
{
    public class PITagData
    {
        public PITagData(string tag, DateTime dateTime, String value)
        {
            _tag = tag;
            _dateTime = dateTime;
            _value = value;
        }

        private string _tag;

        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        private DateTime _dateTime;

        public DateTime DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
	
	
    }
}
