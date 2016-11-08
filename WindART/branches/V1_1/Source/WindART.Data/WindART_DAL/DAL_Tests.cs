using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using NUnit.Framework;

using System.IO;
using System.Reflection;

namespace WindART.DAL
{
    [TestFixture ]
    public class DAL_Tests
    {
        IDataOrder _dataorder;
        IDataOrderFactory _dataOrderFactory;

        [Test]
        public void TestConnection()
        {
            _dataOrderFactory = new DataOrderFactory(@"10.96.3.138",DataSourceType.OSIPI);
            _dataorder = _dataOrderFactory.getDataOrder();
            Assert.IsInstanceOf<PiDBDataOrder>(_dataorder);
        }

        [Test]
        public void TestWindART_PIConnection()
        {
            _dataOrderFactory = new DataOrderFactory(DataSourceType.WindART_PI );
            _dataorder = _dataOrderFactory.getDataOrder();
            Assert.IsInstanceOf <PiDBDataOrder>(_dataorder);
        }

        [Test]
        public void TestWindART_SQLConnection()
        {
            _dataOrderFactory = new DataOrderFactory(DataSourceType.WindART_SQL );
            _dataorder = _dataOrderFactory.getDataOrder();
            Assert.IsInstanceOf<SQL2005DataOrder >(_dataorder);
        }
    }

   
}
