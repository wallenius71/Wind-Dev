using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Helper;

namespace VerifyInputFile
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();

            // X mins delay
            string repeat = "1";//Utils.GetKeyFromConfig("Repeat");
            m_delay = new TimeSpan(0, 0, int.Parse(repeat), 0, 0);

        }

        protected override void OnStart(string[] args)
        {
            ThreadStart ts = new ThreadStart(this.ServiceMain);
            m_shutdownEvent = new ManualResetEvent(false);

            // create the worker thread
            m_thread = new Thread(ts);

            // go ahead and start the worker thread
            m_thread.Start();

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            m_shutdownEvent.Set();

            // wait for the thread to stop giving it 10 seconds

            m_thread.Join(10000);

            // call the base class 

            base.OnStop();
        }

        protected void ServiceMain()
        {
            bool bSignaled = false;
            int nReturnCode = 0;

            while (true)
            {
                // wait for the event to be signaled
                // or for the configured delay
                bSignaled = m_shutdownEvent.WaitOne(m_delay, true);

                // if we were signaled to shutdow, exit the loop
                if (bSignaled == true)
                    break;

                // let's do some work
                nReturnCode = Execute();
            }
        }

        protected virtual int Execute()
        {
            VerifyInputFile vif = new VerifyInputFile();

            vif.Start();

            return 0;
        }

        protected Thread            m_thread;
        protected ManualResetEvent    m_shutdownEvent;
        protected TimeSpan            m_delay;

    }
}
