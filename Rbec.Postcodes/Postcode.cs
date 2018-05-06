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

        private static uint EncodeLetter(char c) =>
            (uint)c - 'A';

        private static uint EncodeNumber(char c) =>
            (uint)c - '0';

        private static uint EncodeNumberOrLetter(char c)
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
            var data = EncodeLetter(ToUpperFast(Next(s, ref i)));
            data *= 27;
            if (!IsNumberFast(s[i]))
                data += EncodeLetter(ToUpperFast(Next(s, ref i))) + 1;
            data = data * 10 + EncodeNumber(Next(s, ref i));

            var j = i;
            Next(s, ref j);
            data *= 37;
            if (!IsLetterFast(s[j]))
            {
                data += EncodeNumberOrLetter(ToUpperFast(Next(s, ref i))) + 1;
            }

            data = data * 10 + EncodeNumber(Next(s, ref i));
            data = data * 26 + EncodeLetter(ToUpperFast(Next(s, ref i)));
            return new Postcode(data * 26 + EncodeLetter(ToUpperFast(s[i])));
        }

        private static uint DivRem(uint a, uint b, out uint remainder)
        {
            var result = a / b;
            remainder = a % b;
            //remainder = a - result * b;
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
            if (obj is null) return false;
            return obj is Postcode postcode && Equals(postcode);
        }

        public override int GetHashCode()
        {
            return (int)_data;
        }
    }
}