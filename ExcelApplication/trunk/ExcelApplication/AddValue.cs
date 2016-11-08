using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelApplication
{
    public class AddValue
    {
        private string m_Display;
        private int m_Value;
        public AddValue(string Display, int Value)
        {
            m_Display = Display;
            m_Value = Value;
        }
        public string Display
        {
            get { return m_Display; }
        }
        public int Value
        {
            get { return m_Value; }
        }
    }
}
