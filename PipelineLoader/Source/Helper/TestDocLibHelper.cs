using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Helper
{
    [TestFixture]
    public class TestDocLibHelper
    {
        [Test]
        public void TestUpdateFileStatus()
        {
            try
            {
                DocLibHelper dlh = new DocLibHelper();

                dlh.UpdateFileStatus(2, Utils.FileStatus.Loaded, 1, 1);
                Assert.IsTrue(true);

            }
            catch(Exception e)
            {
                Assert.Fail("Couldn't update Document Library\n" + e.Message);
            }
        }


    }
}
