using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataReloadUtility
{
    public class LogItem:IEquatable<LogItem>
    {
        private string _name;
        public string Name { get { return _name; }
            set
            {
                _name = value;
                Filename = Path.GetFileName(_name);
            }

                                                    }
        public string Process { get; set; }
        public string StartFolderLocation { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
        public string Filename { get; private set; }
        public bool Equals(LogItem item)
        {
            if (Date == item.Date && Process == item.Process && Message==item.Message )
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashDate= Date == null ? 0 : Date.GetHashCode();
            int hashProcess = Process == null ? 0 :Process.GetHashCode();
            int hashMessage = Message == null ? 0 : Message.GetHashCode();

            return hashDate ^ hashProcess ^ hashMessage;
        }

    }
}
