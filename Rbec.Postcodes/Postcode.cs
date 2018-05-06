using System;

namespace Rbec.Postcodes
{
    public struct Postcode : IComparable<Postcode>, IEquatable<Postcode>
    {
        private readonly int _data;

        private Postcode(int data)
        {
            _data = data;
        }

        public static char ToUpper(char c) => c >= 'a' ? (char) (c - 32) : c;
        public static bool IsLetter(char c) => c >= 'A';
        public static bool IsNumberFast(char c) => c < 'A';
        private static int EncodeLetter(char c) => c - 'A';
        private static int EncodeNumber(char c) => c - '0';

        private static int EncodeNumberOrLetter(char c)
        {
            if (IsNumberFast(c))
                return EncodeNumber(c);
            return EncodeLetter(c) + 10;
        }

        private static char Next(string s, ref int next)
        {
            var c = s[next];
            while (s[++next] == ' ') { }
            return c;
        }

        public static Postcode Parse(string s)
        {
            var i = 0;
            var data = EncodeLetter(ToUpper(Next(s, ref i)));
            data *= 27;
            if (!IsNumberFast(s[i]))
                data += EncodeLetter(ToUpper(Next(s, ref i))) + 1;
            data = data * 10 + EncodeNumber(Next(s, ref i));

            var j = i;
            Next(s, ref j);
            data *= 37;
            if (!IsLetter(s[j]))
            {
                data += EncodeNumberOrLetter(ToUpper(Next(s, ref i))) + 1;
            }

            data = data * 10 + EncodeNumber(Next(s, ref i));
            data = data * 26 + EncodeLetter(ToUpper(Next(s, ref i)));
            return new Postcode(data * 26 + EncodeLetter(ToUpper(s[i])));
        }

        private static int DivRem(int a, int b, out int remainder)
        {
            var result = a / b;
            remainder = a - result * b;
            return result;
        }

        private static char DecodeLetter(ref int c)
        {
            c = DivRem(c, 26, out var remainder);
            return (char)(remainder + 'A');
        }

        private static char DecodeLetterOrSpace(ref int c)
        {
            c = DivRem(c, 27, out var remainder);

            if (remainder == 0)
                return ' ';
            return (char)(remainder + 'A' - 1);
        }

        private static char DecodeNumberOrLetterOrSpace(ref int c)
        {
            c = DivRem(c, 37, out var remainder);

            if (remainder == 0)
                return ' ';
            if (remainder <= 10)
                return (char)(remainder + '0' - 1);
            return (char)(remainder + 'A' - 11);
        }

        private static char DecodeNumber(ref int c)
        {
            c = DivRem(c, 10, out var remainder);
            return (char)(remainder + '0');
        }

        public override string ToString()
        {
            var chars = new char[8];
            var data = _data;
            chars[7] = DecodeLetter(ref data);
            chars[6] = DecodeLetter(ref data);
            chars[5] = DecodeNumber(ref data);
            chars[4] = ' ';
            chars[3] = DecodeNumberOrLetterOrSpace(ref data);

            var i = chars[3] == ' ' ? 3 : 2;

            chars[i--] = DecodeNumber(ref data);
            chars[i] = DecodeLetterOrSpace(ref data);
            if (chars[i] != ' ')
                i--;
            chars[i] = DecodeLetter(ref data);
            return new string(chars, i, 8 - i);
        }

        public int CompareTo(Postcode other) => _data.CompareTo(other._data);
        public bool Equals(Postcode other) => _data == other._data;
        public override bool Equals(object obj) => !(obj is null) && obj is Postcode postcode && Equals(postcode);
        public override int GetHashCode() => _data;
    }
}