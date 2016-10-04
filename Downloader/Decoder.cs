using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace StreamSearch.Downloader
{
    public class Decoder
    {
        private const int END_OF_INPUT = -1;

        private static readonly char[] arrChrs = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/' };
        private static readonly IDictionary<char, int> reversegetFChars = new Dictionary<char, int>();

        private int getFCount;
        private string getFStr;

        static Decoder()
        {
            for (var i = 0; i < arrChrs.Length; i++)
            {
                var chr = arrChrs[i];

                reversegetFChars.Add(chr, i);
            }
        }

        public string Decode(string encoded)
        {
            var regex = new Regex("document.write\\(doit\\('(.*)'\\)\\);");
            var match = regex.Match(encoded);
            var group = match.Groups[1];

            return Unescape(GetF(GetF(group.Value)));
        }

        private void Reset(string e)
        {
            getFStr = e;
            getFCount = 0;
        }

        private int ReadReverseGetF()
        {
            if (string.IsNullOrEmpty(getFStr))
            {
                return END_OF_INPUT;
            }

            while (true)
            {
                if (getFCount >= getFStr.Length)
                {
                    return END_OF_INPUT;
                }

                var e = getFStr[getFCount];

                getFCount++;

                int fChar;

                if (reversegetFChars.TryGetValue(e, out fChar) && fChar > 0)
                {
                    return fChar;
                }

                if (e == 'A')
                {
                    return 0;
                }

                return END_OF_INPUT;
            }
        }

        private int ReadGetF()
        {
            if (string.IsNullOrEmpty(getFStr))
            {
                return END_OF_INPUT;
            }

            if (getFCount >= getFStr.Length)
            {
                return END_OF_INPUT;
            }

            //var e = getFStr.charCodeAt(getFCount) & 255;
            var e = (int)Char.GetNumericValue(getFStr, getFCount) & 255;

            getFCount++;

            return e;
        }

        private string GetF(string e)
        {
            Reset(e);

            var t = "";
            var r = false;
            var n = new int[4];

            while (!r && (n[0] = ReadReverseGetF()) != END_OF_INPUT && (n[1] = ReadReverseGetF()) != END_OF_INPUT)
            {
                n[2] = ReadReverseGetF();
                n[3] = ReadReverseGetF();

                t += N2S(n[0] << 2 & 255 | n[1] >> 4);

                if (n[2] != END_OF_INPUT)
                {
                    t += N2S(n[1] << 4 & 255 | n[2] >> 2);

                    if (n[3] != END_OF_INPUT)
                    {
                        t += N2S(n[2] << 6 & 255 | n[3]);
                    }
                    else
                    {
                        r = true;
                    }
                }
                else
                {
                    r = true;
                }
            }

            return t;
        }

        private string N2S(int e)
        {
            var s = e.ToString("x");

            if (s.Length == 1)
            {
                s = "0" + s;
            }

            s = "%" + s;

            return Unescape(s);
        }

        private string Unescape(string input)
        {
            var unescaped = Uri.UnescapeDataString(input);

            return unescaped;
            //return Uri.UnescapeDataString(input);
        }
    }
}
