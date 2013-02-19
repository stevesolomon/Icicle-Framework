using IcicleFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Xna.Framework;

namespace FrameworkTests
{
    
    
    /// <summary>
    ///This is a test class for RectangleFTest and is intended
    ///to contain all RectangleFTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RectangleFTest
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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for RectangleF Constructor
        ///</summary>
        [TestMethod()]
        public void RectangleFConstructorTest()
        {
            const float x = 1F;
            const float y = 10F;
            const float width = 20F;
            const float height = 40F; 
            RectangleF target = new RectangleF(x, y, width, height);

            Assert.AreEqual(x, target.X);
            Assert.AreEqual(y, target.Y);
            Assert.AreEqual(width, target.Width);
            Assert.AreEqual(height, target.Height);
        }

        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod()]
        public void ContainsTest()
        {
            RectangleF targetInside = new RectangleF(10f, 10f, 1f, 1f);
            RectangleF targetOutside = new RectangleF(100f, 100f, 1f, 1f);
            RectangleF targetIntersects = new RectangleF(20f, 20f, 40f, 40f);

            RectangleF source = new RectangleF(0f, 0f, 21f, 21f);

            Assert.IsTrue(source.Contains(targetInside));
            Assert.IsFalse(source.Contains(targetOutside));
            Assert.IsFalse(source.Contains(targetIntersects));
        }

        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod()]
        public void ContainsTest1()
        {
            RectangleF source = new RectangleF(0f, 0f, 100f, 100f);

            Vector2 targetInside = new Vector2(5f, 5f);
            Vector2 targetOutside = new Vector2(101f, 101f);
            Vector2 targetOnLine = new Vector2(100f, 100f);

            Assert.IsTrue(source.Contains(targetInside));
            Assert.IsFalse(source.Contains(targetOutside));
            Assert.IsFalse(source.Contains(targetOnLine));
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            RectangleF target = new RectangleF(101f, 105f, 65f, 35f);
            RectangleF compareTrue = new RectangleF(101f, 105f, 65f, 35f);
            RectangleF compareFalse = new RectangleF(10f, 5f, 101f, 1000f);
            
            Assert.IsTrue(target.Equals(compareTrue));
            Assert.IsFalse(target.Equals(compareFalse));
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest1()
        {
            RectangleF target = new RectangleF(101f, 105f, 65f, 35f);
            object compareTrue = new RectangleF(101f, 105f, 65f, 35f);
            object compareFalse = new RectangleF(10f, 5f, 101f, 1000f);

            Assert.IsTrue(target.Equals(compareTrue));
            Assert.IsFalse(target.Equals(compareFalse));
        }

        /// <summary>
        ///A test for Inflate
        ///</summary>
        [TestMethod()]
        public void InflateTest()
        {
            RectangleF target = new RectangleF(0f, 0f, 100f, 100f);
            float horizontalAmount = 2f;
            float verticalAmount = 2f;
            target.Inflate(horizontalAmount, verticalAmount);
            
            Assert.AreEqual(-2f, target.X);
            Assert.AreEqual(-2f, target.Y);
            Assert.AreEqual(102f, target.Right);
            Assert.AreEqual(102f, target.Bottom);
        }

        /// <summary>
        ///A test for Intersects
        ///</summary>
        [TestMethod()]
        public void IntersectsTest1()
        {
            RectangleF sourceRect = new RectangleF(0f, 0f, 100f, 100f);
            RectangleF contained = new RectangleF(10f, 10f, 10f, 10f);
            RectangleF intersectsOnRight = new RectangleF(90f, 0f, 100f, 100f);
            RectangleF noIntersection = new RectangleF(101f, 101f, 10f, 10f);
            RectangleF onTheLine = new RectangleF(100f, 100f, 1f, 1f);

            Assert.IsTrue(sourceRect.Intersects(contained));
            Assert.IsTrue(sourceRect.Intersects(intersectsOnRight));
            Assert.IsFalse(sourceRect.Intersects(noIntersection));
            Assert.IsFalse(sourceRect.Intersects(onTheLine));
        }

        /// <summary>
        ///A test for Offset
        ///</summary>
        [TestMethod()]
        public void OffsetTest()
        {
            RectangleF target = new RectangleF(0f, 0f, 100f, 100f);
 
            target.Offset(10f, 10f);

            Assert.AreEqual(10f, target.X);
            Assert.AreEqual(10f, target.Y);
            Assert.AreEqual(110f, target.Right);
            Assert.AreEqual(110f, target.Bottom);
        }

        /// <summary>
        ///A test for Offset
        ///</summary>
        [TestMethod()]
        public void OffsetTest1()
        {
            RectangleF target = new RectangleF(0f, 0f, 100f, 100f);

            target.Offset(new Vector2(10f, 10f));

            Assert.AreEqual(10f, target.X);
            Assert.AreEqual(10f, target.Y);
            Assert.AreEqual(110f, target.Right);
            Assert.AreEqual(110f, target.Bottom);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod()]
        public void OpEqualityTest()
        {
            RectangleF target = new RectangleF(0f, 0f, 100f, 100f); 
            RectangleF isEqual = new RectangleF(0f, 0f, 100f, 100f); 

            RectangleF blankRect1 = new RectangleF();
            RectangleF blankRect2 = new RectangleF();

            Assert.IsTrue(target == isEqual);
            Assert.IsTrue(blankRect1 == blankRect2);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod()]
        public void OpInequalityTest()
        {
            RectangleF target = new RectangleF(0f, 0f, 100f, 100f);
            RectangleF isNotEqual = new RectangleF(0f, 0f, 100f, 101f);

            RectangleF blankRect1 = new RectangleF();

            Assert.IsFalse(target == isNotEqual);
            Assert.IsFalse(target == blankRect1);
        }

        /// <summary>
        ///A test for Bottom
        ///</summary>
        [TestMethod()]
        public void BottomTest()
        {
            RectangleF target = new RectangleF(10f, 10f, 90f, 90f); 

            Assert.AreEqual(100f, target.Bottom);
        }

        /// <summary>
        ///A test for Empty
        ///</summary>
        [TestMethod()]
        public void EmptyTest()
        {
            RectangleF target = RectangleF.Empty;

            Assert.AreEqual(0f, target.X);
            Assert.AreEqual(0f, target.Y);
            Assert.AreEqual(0f, target.Width);
            Assert.AreEqual(0f, target.Height);
        }

        /// <summary>
        ///A test for Left
        ///</summary>
        [TestMethod()]
        public void LeftTest()
        {
            RectangleF target = new RectangleF(10f, 10f, 90f, 90f);

            Assert.AreEqual(10f, target.Left);
        }

        /// <summary>
        ///A test for Position
        ///</summary>
        [TestMethod()]
        public void PositionTest()
        {
            RectangleF target = new RectangleF(10f, 10f, 90f, 90f);
            target.Position = new Vector2(0f, 0f);

            Assert.AreEqual(0f, target.X);
            Assert.AreEqual(0f, target.Y);
            Assert.AreEqual(90f, target.Right);
            Assert.AreEqual(90f, target.Bottom);
        }

        /// <summary>
        ///A test for Right
        ///</summary>
        [TestMethod()]
        public void RightTest()
        {
            RectangleF target = new RectangleF(10f, 10f, 90f, 90f);

            Assert.AreEqual(100f, target.Right);
        }

        /// <summary>
        ///A test for Top
        ///</summary>
        [TestMethod()]
        public void TopTest()
        {
            RectangleF target = new RectangleF(10f, 10f, 90f, 90f);

            Assert.AreEqual(10f, target.Top);
        }
    }
}
