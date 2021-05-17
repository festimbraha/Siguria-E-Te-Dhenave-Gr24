using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace VigenereDecrypter
{
    public partial class Form1 : Form
    {

        public static string alpha = "abcçddheëfggjhijklllmnnjopqrrrsshtthuvxxhyzzh";


        public Form1()
        {
            InitializeComponent();
            encrypt_button.Enabled = false;
            decrypt_button.Enabled = false;
        }

        private void keylengthBtn_Click(object sender, EventArgs e)
        {
            string input = inputTextBox.Text.ToLower().Trim();
            bool check = input.Length == 0 ? false : true;
            if (check == false)
            {
                MessageBox.Show("Invalid Entry!");
            }
            else
            {
                keyLength(input);

            }

        }

        private void displayData(Dictionary<string, int> alphaFrequency)
        {

            chart1.ChartAreas[0].AxisX.LabelStyle.Interval = 1;

            Series S1 = new Series();

            var ordered = alphaFrequency.OrderBy(x => x.Key);

            foreach (var key in ordered)
            {
                S1.Points.AddXY(key.Key.ToString(), key.Value.ToString());

            }
            chart1.Series.Add(S1);
            chart1.Series[0].Color = Color.Gray;



        }

        public void keyLength(string input)
        {
            Dictionary<string, int> alphaFrequency = new Dictionary<string, int>()
            {
                {"a", 0}, {"d", 0}, {"f", 0}, {"i", 0}, {"ll", 0}, {"o", 0}, {"rr", 0}, {"th", 0}, {"xh", 0},
                {"b", 0}, {"dh", 0},{"g", 0}, {"j",0},  {"m", 0},  {"p", 0}, {"s", 0},  {"u", 0},  {"y", 0 },
                {"c", 0}, {"e", 0} ,{"gj", 0},{"k", 0}, {"n", 0},  {"q", 0}, {"sh", 0}, {"v",0},   {"z", 0},
                {"ç", 0}, {"ë", 0}, {"h",0} , {"l", 0}, {"nj", 0}, {"r", 0}, {"t", 0},  {"x", 0},  {"zh", 0}

            };

            List<int[]> list = new List<int[]>();

            string copy = input;
            int tmp = 0; // temporary count of coincidences
            int CI = 0; // largest count of coincidences
            int shift = 0; // shift with the largest CI
            int iter = 1; // temporal displacement

            while (iter < 20)
            {
                copy = input.Substring(0, (input.Length - iter));

                for (int j = 0; j < input.Length; j++)
                {

                    if (input[j] == copy[(j + iter) % copy.Length])
                    {
                        tmp += 1;
                    }
                    if (tmp >= CI && tmp != 35)
                    {
                        CI = tmp;
                        shift = iter;
                    }
                }
                list.Add(new int[] { iter, tmp });

                tmp = 0;
                iter++;

                foreach (char c in input)
                {
                    if (alphaFrequency.ContainsKey(c.ToString()))
                    {
                        alphaFrequency[c.ToString()] += 1;

                    }
                }
            }

            for (int i = 0; i < list.Count(); i++)
            {
                var x = list[i];
                this.dataGridView1.Rows.Add(x[0], x[1]);
            }
            largestCIBox.Text = CI.ToString();
            largestshiftBox.Text = shift.ToString();

            displayData(alphaFrequency); // goto function to show char frequency
            findkeyLetters(shift, input); // goto function to find whole key
        }


        public void findkeyLetters(int shift, string text)
        {
            List<double> big_dotproduct_list = new List<double>();

            Dictionary<string, int> cipherFrequency = new Dictionary<string, int>()

            {
                {"a", 0}, {"d", 0}, {"f", 0}, {"i", 0}, {"ll", 0}, {"o", 0}, {"rr", 0}, {"th", 0}, {"xh", 0},
                {"b", 0}, {"dh", 0},{"g", 0}, {"j",0},  {"m", 0},  {"p", 0}, {"s", 0},  {"u", 0},  {"y", 0 },
                {"c", 0}, {"e", 0} ,{"gj", 0},{"k", 0}, {"n", 0},  {"q", 0}, {"sh", 0}, {"v",0},   {"z", 0},
                {"ç", 0}, {"ë", 0}, {"h",0} , {"l", 0}, {"nj", 0}, {"r", 0}, {"t", 0},  {"x", 0},  {"zh", 0}
            };
            for (int off = 0; off <= shift - 1; off++)
            {

                for (int i = off; i < text.Length; i += shift)
                {
                    if (cipherFrequency.ContainsKey(text[i].ToString()))
                    {
                        cipherFrequency[text[i].ToString()]++;
                    }

                }

                int sum = 0;
                List<double> dot_product_list = new List<double>();

                var vector = cipherFrequency.OrderBy(key => key.Key);
                List<double> w = new List<double>();

                foreach (var x in vector)
                {
                    sum += cipherFrequency[x.Key]; // total number of letters counted
                }
                // Console.WriteLine(sum);

                foreach (var x in vector)
                {
                    if (cipherFrequency[x.Key] == 0)
                    {
                        w.Add(0);
                    }
                    else
                    {
                        w.Add((Convert.ToDouble(cipherFrequency[x.Key]) / sum));
                    }
                }

                sum = 0;
                double dot_product = 0;
                cipherFrequency = cipherFrequency.ToDictionary(p => p.Key, p => 0);

                for (int j = 0; j <= 35; j++)
                {
                    double[] x = new double[] { 0.0726, 0.0112, 0.0044, 0.0025, 0.0241, 0.00964, 0.1025, 0.1029, 0.00946, 0.00776, 0.00694, 0.0106, 0.0789, 0.02453, 0.0371, 0.0169, 0.00505, 0.039588, 0.06037, 0.00566, 0.0389, 0.0313, 0.0096, 0.0651, 0.0039, 0.0358, 0.0245, 0.0882, 0.00353, 0.0367, 0.0143, 0.00055, 0.00024, 0.00658, 0.00642, 0.00073 };// char freq vals   
                                   //              a       b       c       c      d        dh       e       e       f       g        gj        h      i          j      k        l      ll        m       n        nj       o        p       q       r      rr      s       sh       t       th      u        v       x        xh       y          z      zh
                    var a_j = ShiftRight(x, j);

                    for (int k = 0; k <= 35; k++)
                    {
                        dot_product += (w[k] * a_j.ElementAt(k));
                    }

                    dot_product_list.Add(Math.Round(dot_product, 4));
                    dot_product = 0;

                }
                big_dotproduct_list.Add(dot_product_list.IndexOf(dot_product_list.Max()));
            }

            string out_str = "";

            big_dotproduct_list.ForEach(x => out_str += alpha[(int)x]);
            keyTextBox.Text += out_str;
            decrypt(text, out_str);

        }

        public static int MathMod(int a, int b)
        {
            int c = ((a % b) + b) % b;
            return c;
        }
        public static IEnumerable<T> ShiftRight<T>(IList<T> values, int shift)
        {
            for (int index = 0; index < values.Count; index++)
            {
                yield return values[MathMod(index - shift, values.Count)];
            }
        }


        private void decrypt(string text, string key)
        {

            Dictionary<string, int> alpha = new Dictionary<string, int>()
            {
                {"a", 0}, {"d", 4}, {"f", 8},  {"i", 12}, {"ll", 16}, {"o", 20}, {"rr", 24}, {"th", 28}, {"xh", 32},
                {"b", 1}, {"dh", 5},{"g", 9},  {"j", 13},  {"m", 17}, {"p", 21}, {"s", 25},  {"u", 29},  {"y", 33},
                {"c", 2}, {"e", 6} ,{"gj", 10},{"k", 14}, {"n", 18},  {"q", 22}, {"sh", 26}, {"v",30},   {"z", 34},
                {"ç", 3}, {"ë", 7}, {"h", 11} ,{"l", 15}, {"nj", 19}, {"r", 23}, {"t", 27},  {"x", 31},  {"zh", 35}
            };
            text = text.ToLower();
            key = key.ToLower();
            string out_string = "";
            int start_key = 0;
            int key_val;
            int char_index;
            int next_char_index = 36;
            int e_val;
            int out_val;
            for (int i = 0; i < text.Length; i++)
            {
                if (start_key == key.Length)
                {
                    start_key = 0;
                }

                key_val = alpha[key[start_key].ToString()];

                char_index = alpha[text[i].ToString()];

                if (i < text.Length - 1)
                {
                    next_char_index = alpha[text[i + 1].ToString()];
                }

                if (char_index == 4 && next_char_index == 11)
                {
                    char_index = 5;
                    i++;
                }
                else if (char_index == 9 && next_char_index == 13)
                {
                    char_index = 10;
                    i++;
                }
                else if (char_index == 15 && next_char_index == 15)
                {
                    char_index = 16;
                    i++;
                }
                else if (char_index == 18 && next_char_index == 13)
                {
                    char_index = 19;
                    i++;
                }
                else if (char_index == 23 && next_char_index == 23)
                {
                    char_index = 24;
                    i++;
                }
                else if (char_index == 25 && next_char_index == 11)
                {
                    char_index = 26;
                    i++;
                }
                else if (char_index == 27 && next_char_index == 11)
                {
                    char_index = 28;
                    i++;
                }
                else if (char_index == 31 && next_char_index == 11)
                {
                    char_index = 32;
                    i++;
                }
                else if (char_index == 34 && next_char_index == 11)
                {
                    char_index = 35;
                    i++;
                }

                e_val = ((char_index - key_val));
                out_val = (e_val >= 0 ? e_val : (36 - (e_val * (-1))));

                out_string += alpha.First(x => x.Value == out_val).Key;

                start_key += 1;

            }

            inout_textBox.Clear();
            inout_textBox.Text = out_string;
            inout_textBox.Text += '\n' + key;
        }
        private void encrypt(string text, string key)
        {

            Dictionary<string, int> alpha = new Dictionary<string, int>()
            {
                {"a", 0}, {"d", 4}, {"f", 8},  {"i", 12}, {"ll", 16}, {"o", 20}, {"rr", 24}, {"th", 28}, {"xh", 32},
                {"b", 1}, {"dh", 5},{"g", 9},  {"j", 13},  {"m", 17}, {"p", 21}, {"s", 25},  {"u", 29},  {"y", 33},
                {"c", 2}, {"e", 6} ,{"gj", 10},{"k", 14}, {"n", 18},  {"q", 22}, {"sh", 26}, {"v",30},   {"z", 34},
                {"ç", 3}, {"ë", 7}, {"h", 11} ,{"l", 15}, {"nj", 19}, {"r", 23}, {"t", 27},  {"x", 31},  {"zh", 35}
            };
            text = text.ToLower();
            key = key.ToLower();
            string out_string = "";
            int start_key = 0;
            int key_val;
            int char_index;
            int next_char_index = 36;
            int e_val;
            for (int i = 0; i < text.Length; i++)
            {
                if (start_key == key.Length)
                {
                    start_key = 0;
                }

                key_val = alpha[key[start_key].ToString()];

                char_index = alpha[text[i].ToString()];
                    
                if (i < text.Length - 1)
                {
                    next_char_index = alpha[text[i + 1].ToString()];
                }

                if (char_index == 4 && next_char_index == 11)
                {
                    char_index = 5;
                    i++;
                }
                else if (char_index == 9 && next_char_index == 13)
                {
                    char_index = 10;
                    i++;
                }
                else if (char_index == 15 && next_char_index == 15)
                {
                    char_index = 16;
                    i++;
                }
                else if (char_index == 18 && next_char_index == 13)
                {
                    char_index = 19;
                    i++;
                }
                else if (char_index == 23 && next_char_index == 23)
                {
                    char_index = 24;
                    i++;
                }
                else if (char_index == 25 && next_char_index == 11)
                {
                    char_index = 26;
                    i++;
                }
                else if (char_index == 27 && next_char_index == 11)
                {
                    char_index = 28;
                    i++;
                }
                else if (char_index == 31 && next_char_index == 11)
                {
                    char_index = 32;
                    i++;
                }
                else if (char_index == 34 && next_char_index == 11)
                {
                    char_index = 35;
                    i++;
                }

                e_val = ((char_index + key_val) % 36);

                out_string += alpha.First(x => x.Value == e_val).Key;

                start_key += 1;

            }

            inout_textBox.Clear();
            inout_textBox.Text = out_string;
            inout_textBox.Text += '\n' + key;
        }


        private void encrypt_button_Click(object sender, EventArgs e)
        {
            string key = keyTextBox.Text.ToLower().Trim();
            string text = inout_textBox.Text.ToLower().Trim();
            bool check = text.Length == 0 || key.Length == 0 ? false : true;
            if (check == false)
            {
                MessageBox.Show("Invalid Entry!");
            }
            else
            {

                encrypt(text, key);
            }


        }

        private void decrypt_button_Click(object sender, EventArgs e)
        {
            string key = keyTextBox.Text.ToLower().Trim();
            string text = inout_textBox.Text.ToLower().Trim(); ;
            bool check = text.Length == 0 || key.Length == 0 ? false : true;
            if (check == false)
            {
                MessageBox.Show("Invalid Entry!");
            }
            else
            {
                decrypt(text, key);
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            encrypt_button.Enabled = true;
            decrypt_button.Enabled = true;
        }

        private void disable_radiobutton_CheckedChanged(object sender, EventArgs e)
        {
            encrypt_button.Enabled = false;
            decrypt_button.Enabled = false;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            keyTextBox.Clear();
            inout_textBox.Clear();

        }
    }
}
