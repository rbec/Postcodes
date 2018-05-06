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
        public static bool IsNumber(char c) => c < 'A';
        private static int EncodeLetter(char c) => c - 'A';
        private static int EncodeNumber(char c) => c - '0';

        private static int FirstLetterOrNumberIndex(string s, int start)
        {
            while (s[start] == ' ')
            {
                start++;
            }
            return start;
        }

        private static char ConsumeCurrent(string s, ref int currentIndex)
        {
            if (currentIndex < s.Length)
            {
                var current = s[currentIndex];
                do
                {
                    currentIndex++;
                } while (currentIndex < s.Length && s[currentIndex] == ' ');
              //  while (s[++currentIndex] == ' ') { }
                return current;
            }
            return default(char);
        }

        private static bool TryEncodeLetter(char c, ref int data)
        {
            c = ToUpper(c);
            if (c < 'A' || c > 'Z')
                return false;
            data += EncodeLetter(c);
            return true;
        }

        private static bool TryEncodeNumber(char c, ref int data)
        {
            if (c < '0' || c > '9')
                return false;
            data += EncodeNumber(c);
            return true;
        }

        private static bool TryEncodeLetterOrNumber(char c, ref int data)
        {
            data++;
            if (IsNumber(c))
                return TryEncodeNumber(c, ref data);
            data += 10;
            return TryEncodeLetter(c, ref data);
        }

        //public static bool TryParse(string s, out Postcode postcode)
        //{
        //    postcode = default(Postcode);

        //    if (string.IsNullOrEmpty(s)) return false;

        //    var currentIndex = FirstLetterOrNumberIndex(s, 0);
        //    var data = 0;
        //    if (!TryEncodeLetter(ConsumeCurrent(s, ref currentIndex), ref data)) return false;

        //    data *= 27;
        //    if (currentIndex < s.Length && IsLetter(s[currentIndex]))
        //    {
        //        if (!TryEncodeLetter(ConsumeCurrent(s, ref currentIndex), ref data)) return false;
        //        data++;
        //    }

        //    data *= 10;
        //    if (!TryEncodeNumber(ConsumeCurrent(s, ref currentIndex), ref data)) return false;

        //    data *= 37;
        //    if (IsNumber(s[FirstLetterOrNumberIndex(s, currentIndex + 1)]))
        //    {
        //        if (!TryEncodeLetterOrNumber(ConsumeCurrent(s, ref currentIndex), ref data)) return false;
        //    }

        //    data *= 10;
        //    if (!TryEncodeNumber(ConsumeCurrent(s, ref currentIndex), ref data)) return false;

        //    data *= 26;
        //    if (!TryEncodeLetter(ConsumeCurrent(s, ref currentIndex), ref data)) return false;

        //    data *= 26;
        //    if (!TryEncodeLetter(ConsumeCurrent(s, ref currentIndex), ref data)) return false;

        //    postcode = new Postcode(data);
        //    return true;
        //}

        private static char Next(string s, ref int i)
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
            var i = 0;
            var c = Next(s, ref i);
            var data = EncodeLetter(c);
            c = Next(s, ref i);

            data *= 27;
            if (IsLetter(c))
            {
                data += EncodeLetter(c) + 1;
                c = Next(s, ref i);
            }

            data *= 10;
            data += EncodeNumber(c);

            c = Next(s, ref i);
            var d = Next(s, ref i);

            data *= 37;
            if (IsNumber(d))
            {
                if (IsNumber(c))
                    data += EncodeNumber(c) + 1;
                else
                    data += EncodeLetter(c) + 11;
                c = d;
                d = Next(s, ref i);
            }

            data *= 10;
            data += EncodeNumber(c);

            data *= 26;
            data += EncodeLetter(d);

            data *= 26;
            c = Next(s, ref i);
            data += EncodeLetter(c);

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