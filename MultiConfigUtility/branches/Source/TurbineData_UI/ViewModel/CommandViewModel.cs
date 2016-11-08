using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Analysis_UI
{
    public class CommandViewModel:ViewModelBase 
    {
        public CommandViewModel(string displayname, ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            base.DisplayName = displayname;
            this.Command = command;
        }

        public ICommand Command { get; private set; }
    }
}
