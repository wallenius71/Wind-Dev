using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;

namespace WindART_UI.ViewModel
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
