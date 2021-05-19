/********************************************
 * Author: Yao Zhou
 * Reference :- http://www.cs.ucf.edu/~dmarino/progcontests/mysols/worldfinals/2018/f.java
 * *****************************************/

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Go_with_the_Flow
{
    class VM : INotifyPropertyChanged
    {

        #region Property Change
        public event PropertyChangedEventHandler PropertyChanged;

        private void propChange([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region Data

        int numlocation;
        public static int n;
        public static int[] each_word_len;

        private string paragraph = "";

        public string Paragraph { get { return paragraph; } set { paragraph = value; propChange(); } }

        private string original = "";

        public string Original { get { return original; } set { original = value; propChange(); } }

        private string input = "";

        public string Input { get { return input; } set { input = value; propChange(); } }

        int numwords;

        public int NumWords { get { return numwords; } set { numwords = value; propChange(); } }

        int linewidth;

        public int LineWidth { get { return linewidth; } set { linewidth = value; propChange(); } }

        int riverlength;

        public int RiverLength { get { return riverlength; } set { riverlength = value; propChange(); } }


        #endregion

        #region Singleton

        private static VM vm;
        public static VM Instance { get { vm ??= new VM(); return vm; } }

        private VM()
        {

        }

        #endregion

        public void OpenFile()
        {
            Paragraph = "";

            numlocation = Input.IndexOf(" ");
            NumWords = Convert.ToInt32(Input.Substring(0, numlocation));

            Original = Input.Substring(numlocation + 1);
            Paragraph = Original;
        }

        public void RiverCheck()
        {
            n = NumWords;
            each_word_len = new int[n];
            string[] words = Original.Split(' ');
            int sum = 0, max = 0;
            for (int i = 0; i < n; i++)
            {
                each_word_len[i] = words[i].Length;
                sum += each_word_len[i];
                max = Math.Max(max, each_word_len[i]);
            }

            // Put all the words in one line.

            int res = -1, resw = 0;

            // Brute force over all widths, update when an answer is strictly better.
            for (int w = max; w <= sum + n - 1; w++)
            {
                int tmp = Check(w);
                if (tmp > res)
                {
                    res = tmp;
                    resw = w;
                }
            }
            LineWidth = resw;
            RiverLength = res;
        }

        public int FindLocation(int i, int w, int[] r, int[] c)
        {
            return w * r[i] + c[i];
        }

        public int Check(int w)
        {
            // (r[i], c[i]) will store location of space after ith word.
            int[] r = new int[n];
            int[] c = new int[n];

            int cR = 0, cC = -1;

            int i;
            // Place each word.
            for (i = 0; i < n; i++)
            {
                cC += (each_word_len[i] + 1);

                // Wrap around.
                if (cC > w)
                {
                    r[i] = r[i - 1] + 1;
                    c[i] = each_word_len[i];
                    cR = r[i];
                    cC = c[i];
                }

                // Same line.
                else
                {
                    r[i] = cR;
                    c[i] = cC;
                }
            }

            // dp[i] stores max river ending at space after word represented by len[i].
            int[] dp = new int[n];

            // Init DP table for first row.
            i = 0;
            int res = 0;
            while (i < n && r[i] == 0)
            {
                // Usual case, another word on this line.
                if (i + 1 < n && r[i + 1] == 0) { dp[i] = 1; res = 1; }
                i++;
            }

            // Now we are on row two. Use a look back pointer to make the DP efficient.
            int pre = 0;
            for (; i < n - 1; i++)
            {
                // Last item on line with trailing spaces, can't be part of any river.
                if (r[i + 1] > r[i]) continue;

                // I can always start a river here.
                dp[i] = 1;

                // Where I am now.
                int curI = FindLocation(i, w, r, c);

                // Advance my ptr.
                while (c[pre] == w || FindLocation(pre, w, r, c) < curI - w - 1)
                {
                    pre++;
                }

                // Before me.
                int prevI = FindLocation(pre, w, r, c);

                // Up and to the left; update and advance to next spot.
                if (prevI == curI - w - 1)
                {
                    dp[i] = Math.Max(dp[i], dp[pre] + 1);
                    pre++;
                    prevI = FindLocation(pre, w, r, c);
                }

                // Directly above.
                else if (prevI == curI - w)
                {
                    dp[i] = Math.Max(dp[i], dp[pre] + 1);
                }

                // Check left and right separately so we can consider both left and right...
                if (prevI == curI - w + 1)
                {
                    dp[i] = Math.Max(dp[i], dp[pre] + 1);
                }

                // Update best.
                res = Math.Max(res, dp[i]);
            }
            return res;
        }
    }
}
