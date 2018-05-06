using System;

namespace Rbec.Postcodes
{
    public struct Postcode : IComparable<Postcode>, IEquatable<Postcode>
    {
        private readonly uint _data;

        private Postcode(uint data)
        {
            _data = data;
        }

        public static char ToUpperFast(char c)
        {
            if (c >= 'a') return (char)(c - 32);
            return c;
        }

        public static bool IsLetterFast(char c)
        {
            return c >= 'A';
        }


        public static bool IsNumberFast(char c)
        {
            return c < 'A';
        }

        public static char[] Normalise(string s)
        {
            var chars = new char[7];
            var i = 0;
            chars[0] = ToUpperFast(s[i++]);
            chars[1] = IsNumberFast(s[i]) ? ' ' : ToUpperFast(s[i++]);
            chars[2] = s[i++];
            chars[3] = IsLetterFast(s[i + 1]) ? ' ' : ToUpperFast(s[i++]);
            while (s[i] == ' ')
                i++;
            chars[4] = s[i++];
            chars[5] = ToUpperFast(s[i++]);
            chars[6] = ToUpperFast(s[i]);
            return chars;
        }

        private static uint EncodeLetter(char c) =>
            (uint)c - 'A';

        private static uint EncodeNumber(char c) =>
            (uint)c - '0';

        private static uint EncodeLetterOrSpace(char c) =>
            c == ' '
                ? 0
                : EncodeLetter(c) + 1;

        private static uint EncodeNumberOrLetterOrSpace(char c)
        {
            if (c == ' ')
                return 0;
            if (IsNumberFast(c))
                return EncodeNumber(c) + 1;
            return EncodeLetter(c) + 11;
        }

        private static uint DivRem(uint a, uint b, out uint remainder)
        {
            var result = a / b;
            remainder = a - result * b;
            return result;
        }

        private static char DecodeLetter(ref uint c)
        {
            c = DivRem(c, 26, out var remainder);
            return (char)(remainder + 'A');
        }

        private static char DecodeLetterOrSpace(ref uint c)
        {
            c = DivRem(c, 27, out var remainder);

            if (remainder == 0)
                return ' ';
            return (char)(remainder + 'A' - 1);
        }

        private static char DecodeNumberOrLetterOrSpace(ref uint c)
        {
            c = DivRem(c, 37, out var remainder);

            if (remainder == 0)
                return ' ';
            if (remainder <= 10)
                return (char)(remainder + '0' - 1);
            return (char)(remainder + 'A' - 11);
        }

        private static char DecodeNumber(ref uint c)
        {
            c = DivRem(c, 10, out var remainder);
            return (char)(remainder + '0');
        }

        public static Postcode Parse(string s)
        {
            var chars = Normalise(s);
            var data = EncodeLetter(chars[0]);
            data = data * 27 + EncodeLetterOrSpace(chars[1]);
            data = data * 10 + EncodeNumber(chars[2]);
            data = data * 37 + EncodeNumberOrLetterOrSpace(chars[3]);
            data = data * 10 + EncodeNumber(chars[4]);
            data = data * 26 + EncodeLetter(chars[5]);
            return new Postcode(data * 26 + EncodeLetter(chars[6]));
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

        public int CompareTo(Postcode other)
        {
            return _data.CompareTo(other._data);
        }

        public bool Equals(Postcode other)
        {
            return _data == other._data;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Postcode postcode && Equals(postcode);
        }

        public override int GetHashCode()
        {
            return (int)_data;
        }
    }
}