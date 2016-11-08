using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace WindowsApplication1.Tests
{
    [TestFixture]
    public class Tests
    {
        HeaderAppender appender;
        void setup()
        {
            appender = new HeaderAppender();
        }
        [Test]
        public void testFindAllPRNs()
        {
            string folderselected=Utils.GetFolder ();
            appender.AddHeaders(folderselected);
            Console.WriteLine(appender.PRNs.Count);
            Assert.False(appender.PRNs.Count  == 0);
        }
    }
}
