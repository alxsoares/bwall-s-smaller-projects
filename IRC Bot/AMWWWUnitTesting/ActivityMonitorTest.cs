using ActivityMonitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ModularIrcBot;

namespace AMWWWUnitTesting
{
    
    
    /// <summary>
    ///This is a test class for ActivityMonitorTest and is intended
    ///to contain all ActivityMonitorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ActivityMonitorTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }
        //
        #endregion



        /// <summary>
        ///A test for ProcessMessage
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ActivityMonitor.dll")]
        public void ProcessMessageTest()
        {
            ActivityMonitor_Accessor target = new ActivityMonitor_Accessor(); // TODO: Initialize to an appropriate value
            Random rand = new Random();
            bool inDay = true;
            MSG m = new MSG("user", "user", "user", "#chan", "message");
            m.when = DateTime.Today;
            while (inDay)
            {
                target.ProcessMessage(m);
                m.when = m.when.AddSeconds(rand.Next() % 256);
                if (m.when.DayOfYear > DateTime.Today.DayOfYear)
                    inDay = false;
            }
            target.WriteStatsToWWW("www");
            Assert.AreEqual(1, 1);
        }
    }
}
