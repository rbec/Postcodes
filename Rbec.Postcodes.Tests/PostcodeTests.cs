using System.IO;
using Xunit;

namespace Rbec.Postcodes.Tests
{
    public class PostcodeTests
    {
        [Fact]
        public void Test1()
        {
            foreach (var line in File.ReadLines(@"C:\temp\postcodes.txt"))
            {
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