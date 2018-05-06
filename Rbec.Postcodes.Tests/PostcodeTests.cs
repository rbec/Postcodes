using System.IO;
using Xunit;

namespace Rbec.Postcodes.Tests
{
    public class PostcodeTests
    {
        [Fact]
        public void TestNoSpace1()
        {
            var postcode = Postcode.Parse("BN68BA");
            Assert.Equal("BN6 8BA", postcode.ToString());
        }

        [Fact]
        public void TestNoSpace2()
        {
            var postcode = Postcode.Parse("AB101AA");
            Assert.Equal("AB10 1AA", postcode.ToString());
        }

        [Fact]
        public void TestSingleSpace1()
        {
            var postcode = Postcode.Parse("BN6 8BA");
            Assert.Equal("BN6 8BA", postcode.ToString());
        }

        [Fact]
        public void TestSingleSpace2()
        {
            var postcode = Postcode.Parse("AB10 1AA");
            Assert.Equal("AB10 1AA", postcode.ToString());
        }

        [Fact]
        public void TestSimple2()
        {
            var postcode = Postcode.Parse("BN6  8BA");
            Assert.Equal(Postcode.Parse("BN6 8BA"), postcode);
        }

        [Fact]
        public void TestAll()
        {
            foreach (var line in File.ReadLines(@"C:\temp\postcodes.txt"))
            {
               // var postcode = Postcode.Parse(line.Replace(" ", ""));
                var postcode = Postcode.Parse(line);
                Assert.Equal(line, postcode.ToString());
            }
        }

        //[Fact]
        //public void SavePostcodes()
        //{
        //    File.WriteAllLines(@"C:\temp\postcodes.csv", File.ReadLines(@"C:\temp\full\pen.csv").Skip(1).Select(line => line.Substring(0, line.IndexOf(','))), Encoding.ASCII);
        //}
    }
}