using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Windows.Input;
using WindART;


namespace WindART_UI
{
    [TestFixture]
    public class UITests
    {
        private ViewModelBase _vm;
        [SetUp]
        public void SetUp()
        {
            Window1ViewModel viewmodel = new Window1ViewModel();
            _vm = viewmodel;

 



        }

    }
}
