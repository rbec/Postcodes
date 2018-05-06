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

        public static bool IsLetter(char c) => c >= 'A';
        public static bool IsNumber(char c) => c < 'A';

        private static int EncodeLetter(char c, ref bool isInvalid)
        {
            if (c >= 'a')
                c = (char) (c - 32);
            isInvalid |= c < 'A' || c > 'Z';
            return c - 'A';
        }

        private static int EncodeNumber(char c, ref bool isInvalid)
        {
            isInvalid |= c < '0' || c > '9';
            return c - '0';
        }

        private static char Consume(string s, ref int i)
        {
            while (i < s.Length)
            {
                if (s[i] != ' ')
                    return s[i++];
                i++;
            }
            return default(char);
        }

        public static bool TryParse(string s, out Postcode postcode)
        {
            var isInvalid = false;
            postcode = default(Postcode);
            if (s == null)
                return false;

            var i = 0;
            var current = Consume(s, ref i);
            var data = EncodeLetter(current, ref isInvalid);
            current = Consume(s, ref i);

            data *= 27;
            if (IsLetter(current))
            {
                data += EncodeLetter(current, ref isInvalid) + 1;
                current = Consume(s, ref i);
            }

            data *= 10;
            data += EncodeNumber(current, ref isInvalid);

            current = Consume(s, ref i);
            var next = Consume(s, ref i);

            data *= 37;
            if (IsNumber(next))
            {
                if (IsNumber(current))
                    data += EncodeNumber(current, ref isInvalid) + 1;
                else
                    data += EncodeLetter(current, ref isInvalid) + 11;
                current = next;
                next = Consume(s, ref i);
            }

            data *= 10;
            data += EncodeNumber(current, ref isInvalid);

            data *= 26;
            data += EncodeLetter(next, ref isInvalid);

            data *= 26;
            current = Consume(s, ref i);
            data += EncodeLetter(current, ref isInvalid);

            if (isInvalid || Consume(s, ref i) != default(char))
                return false;

            postcode = new Postcode(data);
            return true;
        }

        public static Postcode Parse(string s)
        {
            if (TryParse(s, out var postcode))
                return postcode;

            throw new FormatException($"'{s}' is not a valid postcode");
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
            return (char) (remainder + 'A');
        }

        private static char DecodeLetterOrSpace(ref int c)
        {
            c = DivRem(c, 27, out var remainder);

            if (remainder == 0)
                return ' ';
            return (char) (remainder + 'A' - 1);
        }

        private static char DecodeNumberOrLetterOrSpace(ref int c)
        {
            c = DivRem(c, 37, out var remainder);

            if (remainder == 0)
                return ' ';
            if (remainder <= 10)
                return (char) (remainder + '0' - 1);
            return (char) (remainder + 'A' - 11);
        }

        private static char DecodeNumber(ref int c)
        {
            c = DivRem(c, 10, out var remainder);
            return (char) (remainder + '0');
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