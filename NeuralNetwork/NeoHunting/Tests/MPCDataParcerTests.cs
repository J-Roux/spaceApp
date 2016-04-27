using NUnit.Framework;

namespace NeoHunting.Tests
{
    [TestFixture]
    public class MPCDataParcerTests
    {
        [Test]
        public void GetNonNeos()
        {
            var parcer = new MPCDataParcer();
            parcer.GetNonNeos();
        }

        [Test]
        public void GetNonNeosByTYpeCode()
        {
            var parcer = new MPCDataParcer();
            parcer.GetNonNeosByTYpeCode();
        }

        [Test]
        public void ParseAndConvertToDouble()
        {
            var input = @"00001    3.34  0.12 K161D 181.38133   72.73324   80.32180   10.59166  0.0757544  0.21400734   2.7681117  0 MPO370963  6592 109 1801-2015 0.60 M-v 30h MPCLINUX   0000      (1) Ceres              20151128";
            var parcer = new MPCDataParcer();
            var result = parcer.ParseAndConvertToDouble(input);

            Assert.AreEqual(result[0], 3.34);
            Assert.AreEqual(result[3], 80.3218);
        }

        [Test]
        public void GetMinMaxes()
        {
            var parcer = new MPCDataParcer();
            parcer.GetMinMaxes();
        }
    }
}
