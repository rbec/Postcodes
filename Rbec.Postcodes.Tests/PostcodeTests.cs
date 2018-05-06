using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Rbec.Postcodes.Tests
{
    public class PostcodeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("B")]
        [InlineData("BN")]
        [InlineData("BN6")]
        [InlineData("BN6 8")]
        [InlineData("BN6 8B")]
        [InlineData("BN66 8B")]
        [InlineData("B66 8B")]
        [InlineData("B6 8B")]
        [InlineData("B6 8BAB")]
        public void TestInvalid(string s)
        {
            Assert.False(Postcode.TryParse(s, out var postcode));
            Assert.Equal(default(Postcode), postcode);
        }

        [Fact]
        public void DenormalisedParsesCorrectly()
        {
            foreach (var (expected, s) in ValidTestCases())
            {
                var postcode = Postcode.Parse(s);
                Assert.Equal(expected, postcode.ToString());
            }
        }

        public static IEnumerable<(string expected, string s)> ValidTestCases()
        {
            var normalised = new[]
                             {
                                 "M1 1AA",
                                 "M60 1NW",
                                 "CR2 6XH",
                                 "DN55 1PT",
                                 "W1A 1HQ",
                                 "EC1A 1BB"
                             };
            return normalised.SelectMany(n => DenormaliseSpaces(n.Replace(" ", "")).SelectMany(DenormaliseCase).Select(denormalised => (n, denormalised)));
        }

        private static IEnumerable<string> DenormaliseCase(string s)
        {
            yield return s;
            for (var i = 0; i < s.Length; i++)
                if (char.IsLetter(s, i))
                {
                    yield return $"{s.Substring(0, i)}{char.ToLower(s[i])}{s.Substring(i + 1)}";

                    for (var j = i + 1; j < s.Length; j++)
                        if (char.IsLetter(s, j))
                            yield return $"{s.Substring(0, i)}{char.ToLower(s[i])}{s.Substring(i + 1, j - i - 1)}{char.ToLower(s[j])}{s.Substring(j + 1)}";
                }
        }

        private static IEnumerable<string> DenormaliseSpaces(string s)
        {
            for (var i = 0; i <= s.Length; i++)
            {
                yield return $"{s.Substring(0, i)} {s.Substring(i)}";

                for (var j = i; j <= s.Length; j++)
                    yield return $"{s.Substring(0, i)} {s.Substring(i, j - i)} {s.Substring(j)}";
            }
        }

        [Fact]
        public void TestAll()
        {
            foreach (var line in File.ReadLines(@"C:\temp\postcodes.txt"))
            {
                var postcode = Postcode.Parse(line);
                Assert.Equal(line, postcode.ToString());
            }
        }
    }
}