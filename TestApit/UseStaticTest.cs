using testwinc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace TestApit
{
    
    
    /// <summary>
    ///这是 UseStaticTest 的测试类，旨在
    ///包含所有 UseStaticTest 单元测试
    ///</summary>
    [TestClass()]
    public class UseStaticTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
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

        #region 附加测试属性
        // 
        //编写测试时，还可使用以下属性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///ListData 的测试
        ///</summary>
        [TestMethod()]
        public void ListDataTest()
        {
            ArrayList actual;
            actual = UseStatic.ListData;
            Assert.IsNotNull(actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///addDataToList 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("testwinc.exe")]
        public void addDataToListTest()
        {
            UseStatic_Accessor.addDataToList();
            Assert.Inconclusive("无法验证不返回值的方法。");
        }



        /// <summary>
        ///ListData 的测试
        ///</summary>
        [TestMethod()]
        public void ListDataTest1()
        {
            ArrayList actual;
            actual = UseStatic.ListData;
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///soutTdRtb2 的测试
        ///</summary>
        [TestMethod()]
        public void soutTdRtb2Test()
        {
            object stext = null; // TODO: 初始化为适当的值
            UseStatic.soutTdRtb2(stext);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///soutTd 的测试
        ///</summary>
        [TestMethod()]
        public void soutTdTest()
        {
            object stext = null; // TODO: 初始化为适当的值
            UseStatic.soutTd(stext);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///sout 的测试
        ///</summary>
        [TestMethod()]
        public void soutTest()
        {
            object str = null; // TODO: 初始化为适当的值
            UseStatic.sout(str);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///setWindForm 的测试
        ///</summary>
        [TestMethod()]
        public void setWindFormTest()
        {
            WindForm tt = null; // TODO: 初始化为适当的值
            UseStatic.setWindForm(tt);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///getWindForm 的测试
        ///</summary>
        [TestMethod()]
        public void getWindFormTest()
        {
            WindForm expected = null; // TODO: 初始化为适当的值
            WindForm actual;
            actual = UseStatic.getWindForm();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetListData 的测试
        ///</summary>
        [TestMethod()]
        public void GetListDataTest()
        {
            ArrayList expected = null; // TODO: 初始化为适当的值
            ArrayList actual;
            actual = UseStatic.GetListData();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///addDataToList 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("testwinc.exe")]
        public void addDataToListTest1()
        {
            UseStatic_Accessor.addDataToList();
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///UseStatic 构造函数 的测试
        ///</summary>
        [TestMethod()]
        public void UseStaticConstructorTest()
        {
            UseStatic target = new UseStatic();
            Assert.Inconclusive("TODO: 实现用来验证目标的代码");
        }
    }
}
