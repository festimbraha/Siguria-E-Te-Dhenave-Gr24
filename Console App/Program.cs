using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vignere
{
    class Program
    {

        public static string alphabet = "abcçdαeëfgßhijklπmnσopqrµsτtδuvxφyzε";
        static void Main(string[] args)
        {
            // Kodi qe na mundeson input me te gjate se 254 karaktere
            byte[] inputBuffer = new byte[1024];
            Stream inputStream = Console.OpenStandardInput(inputBuffer.Length);
            Console.SetIn(new StreamReader(inputStream, Console.InputEncoding, false, inputBuffer.Length));

            // Marrim tekstin shqip pjese pjese dhe celesin
            Console.WriteLine("Shkruaj pjesen e pare te tekstit: ");
            string text_1 = Console.ReadLine();
            Console.WriteLine("Shkruaj pjesen e dyte te tekstit: ");
            string text_2 = Console.ReadLine();
            Console.WriteLine("Shkruaj pjesen e trete te tekstit: ");
            string text_3 = Console.ReadLine();
            string plaintext_doubles = text_1 + text_2 + text_3;
            Console.WriteLine("Shkruaj celesin: ");
            string key = Console.ReadLine();

            // Zevendesojme shkronjat e dyfishta
            string plaintext = removeDoubles(plaintext_doubles);

            string ciphertext = encrypt(plaintext, key);
            Console.WriteLine("Teksti enkriptuar eshte: " + ciphertext);


            // Kthejme shkronjat e dyfishta
            string decrypted = addDoubles(decrypt(ciphertext, key));
            Console.WriteLine("Teksti dekriptuar eshte: " + decrypted);


            // Marrim gjatesine e celesit si string dhe e kthejme ne int
            Console.WriteLine("Shkruaj gjatesine e celesit: ");
            string str_key_length = Console.ReadLine();
            int key_length = Convert.ToInt32(str_key_length);

            string celesi = addDoubles(findkeyLetters(key_length, ciphertext));
            Console.WriteLine("Celesi eshte: " + celesi);

        }

        // Funksioni qe zevendeson shkronjat e dyfishta
        public static string removeDoubles(string text)
        {
            string plaintext = "";
            text = text.Replace(" ", String.Empty);
            text = text.Replace(".", String.Empty);
            text = text.Replace(",", String.Empty);
            text = text.ToLower();

            for (int i = 0; i < text.Length; i++)
            {
                if (i < text.Length - 1)
                {
                    // Gjejme ku ka "dh" dhe e zevendesojme me "α"
                    if (text[i].ToString() == "d" && text[i + 1].ToString() == "h")
                    {
                        plaintext += "α";
                        i++; // Kapercejme nje shkronje (pasi qe eshte pjese e dyfishes)
                    }
                    // Gjejme ku ka "gj" dhe e zevendesojme me "ß"
                    else if (text[i].ToString() == "g" && text[i + 1].ToString() == "j")
                    {
                        plaintext += "ß";
                        i++;
                    }
                    // Gjejme ku ka "ll" dhe e zevendesojme me "π"
                    else if (text[i].ToString() == "l" && text[i + 1].ToString() == "l")
                    {
                        plaintext += "π";
                        i++;
                    }
                    // Gjejme ku ka "nj" dhe e zevendesojme me "σ"
                    else if (text[i].ToString() == "n" && text[i + 1].ToString() == "j")
                    {
                        plaintext += "σ";
                        i++;
                    }
                    // Gjejme ku ka "rr" dhe e zevendesojme me "µ"
                    else if (text[i].ToString() == "r" && text[i + 1].ToString() == "r")
                    {
                        plaintext += "µ";
                        i++;
                    }
                    // Gjejme ku ka "sh" dhe e zevendesojme me "τ"
                    else if (text[i].ToString() == "s" && text[i + 1].ToString() == "h")
                    {
                        plaintext += "τ";
                        i++;
                    }
                    // Gjejme ku ka "th" dhe e zevendesojme me "δ"
                    else if (text[i].ToString() == "t" && text[i + 1].ToString() == "h")
                    {
                        plaintext += "δ";
                        i++;
                    }
                    // Gjejme ku ka "xh" dhe e zevendesojme me "φ"
                    else if (text[i].ToString() == "x" && text[i + 1].ToString() == "h")
                    {
                        plaintext += "φ";
                        i++;
                    }
                    // Gjejme ku ka "zh" dhe e zevendesojme me "ε"
                    else if (text[i].ToString() == "z" && text[i + 1].ToString() == "h")
                    {
                        plaintext += "ε";
                        i++;
                    }
                    else
                    {
                        plaintext += text[i];
                    }
                }
                // Shkronja e fundit eshte gjithmone njeshe
                else if (i == text.Length - 1)
                {
                    plaintext += text[i];
                }
            }
            // Kthejme teksin me shkronjat e zevendesuara
            return plaintext;
        }

        public static String decrypt(string ciphertext, string key)
        {

            Dictionary<string, int> alphabet = new Dictionary<string, int>()
            {
                {"a", 0}, {"d", 4}, {"f", 8},  {"i", 12}, {"π", 16}, {"o", 20}, {"µ", 24}, {"δ", 28}, {"φ", 32},
                {"b", 1}, {"α", 5},{"g", 9},  {"j", 13},  {"m", 17}, {"p", 21}, {"s", 25},  {"u", 29},  {"y", 33},
                {"c", 2}, {"e", 6} ,{"ß", 10},{"k", 14}, {"n", 18},  {"q", 22}, {"τ", 26}, {"v",30},   {"z", 34},
                {"ç", 3}, {"ë", 7}, {"h", 11} ,{"l", 15}, {"σ", 19}, {"r", 23}, {"t", 27},  {"x", 31},  {"ε", 35}
            };

            key = key.ToLower();
            string decrypted = "";
            int key_position = 0;
            int key_index;     // Indeksi celesit
            int char_index;    // Indeksi shkronjes
            int d_index;       // Indeksi shkronjes se dekriptuar (positiv ose negativ)
            int out_index;     // Indeksi shkronjes se dekriptuar (positiv)

            for (int i = 0; i < ciphertext.Length; i++)
            {
                // Perserisim celesin nese mberrin ne fund
                if (key_position == key.Length)
                {
                    key_position = 0;
                }

                // Gjejme karakterin e celesit
                key_index = alphabet[key[key_position].ToString()];
                // Gjejme indeksin e shkronjes
                char_index = alphabet[ciphertext[i].ToString()];
                // Gjejme indeksin e shkronjes se enkriptuar
                d_index = ((char_index - key_index) % 36);
                // Kontrollojme nese indeksi ka vlere pozitive
                out_index = (d_index >= 0 ? d_index : (36 - (d_index * (-1))));
                // Shtojme shkronjen e dekriptuar te decrypted
                decrypted += alphabet.First(x => x.Value == out_index).Key;
                // Kalojme te shkronja tjeter e celesit
                key_position += 1;
            }

            // Kthejme tekstin e dekriptuar
            return decrypted;
        }

        public static String encrypt(string plaintext, string key)
        {

            Dictionary<string, int> alphabet = new Dictionary<string, int>()
            {
                {"a", 0}, {"d", 4}, {"f", 8},  {"i", 12}, {"π", 16}, {"o", 20}, {"µ", 24}, {"δ", 28}, {"φ", 32},
                {"b", 1}, {"α", 5},{"g", 9},  {"j", 13},  {"m", 17}, {"p", 21}, {"s", 25},  {"u", 29},  {"y", 33},
                {"c", 2}, {"e", 6} ,{"ß", 10},{"k", 14}, {"n", 18},  {"q", 22}, {"τ", 26}, {"v",30},   {"z", 34},
                {"ç", 3}, {"ë", 7}, {"h", 11} ,{"l", 15}, {"σ", 19}, {"r", 23}, {"t", 27},  {"x", 31},  {"ε", 35}
            };

            key = key.ToLower();
            string ciphertext = "";
            int key_position = 0;
            int key_index;     // Indeksi celesit
            int char_index;    // Indeksi shkronjes
            int e_index;       // Indeksi shkronjes se enkriptuar

            for (int i = 0; i < plaintext.Length; i++)
            {
                // Perserisim celesin nese mberrin ne fund
                if (key_position == key.Length)
                {
                    key_position = 0;
                }

                // Gjejme karakterin e celesit
                key_index = alphabet[key[key_position].ToString()];
                // Gjejme indeksin e shkronjes
                char_index = alphabet[plaintext[i].ToString()];
                // Gjejme indeksin e shkronjes se enkriptuar
                e_index = ((char_index + key_index) % 36);
                // Shtojme shkronjen e enkriptuar tek cipherteksti
                ciphertext += alphabet.First(x => x.Value == e_index).Key;
                // Kalojme te shkronja tjeter e celesit
                key_position += 1;
            }

            // Kthejme ciphertextin
            return ciphertext;
        }

        public static String addDoubles(string text)
        {
            // Stringu qe ka shronja dyshe
            string withDoubles = "";

            for (int i = 0; i < text.Length; i++)
            {
                // Gjejme ku ka "α" dhe e zevendesojme me "dh"
                if (text[i].ToString() == "α")
                {
                    withDoubles += "dh";
                }
                // Gjejme ku ka "ß" dhe e zevendesojme me "gj"
                else if (text[i].ToString() == "ß")
                {
                    withDoubles += "gj";
                }
                // Gjejme ku ka "π" dhe e zevendesojme me ll"
                else if (text[i].ToString() == "π")
                {
                    withDoubles += "ll";
                }
                // Gjejme ku ka "σ" dhe e zevendesojme me "nj"
                else if (text[i].ToString() == "σ")
                {
                    withDoubles += "nj";
                }
                // Gjejme ku ka "µ" dhe e zevendesojme me "rr"
                else if (text[i].ToString() == "µ")
                {
                    withDoubles += "rr";
                }
                // Gjejme ku ka "τ" dhe e zevendesojme me "sh"
                else if (text[i].ToString() == "τ")
                {
                    withDoubles += "sh";
                }
                // Gjejme ku ka "δ" dhe e zevendesojme me "th"
                else if (text[i].ToString() == "δ")
                {
                    withDoubles += "th";
                }
                // Gjejme ku ka "φ" dhe e zevendesojme me "xh"
                else if (text[i].ToString() == "φ")
                {
                    withDoubles += "xh";
                }
                // Gjejme ku ka "ε" dhe e zevendesojme me "zh"
                else if (text[i].ToString() == "ε")
                {
                    withDoubles += "zh";
                }
                else
                {
                    withDoubles += text[i];
                }
            }

            // Kthejme tekstin me shkronja te dyfishta
            return withDoubles;
        }

        public static string findkeyLetters(int key_length, string ciphertext)
        {
            List<double> big_dotproduct_list = new List<double>();

            Dictionary<string, int> cipherFrequency = new Dictionary<string, int>()

            {
                {"a", 0}, {"d", 0}, {"f", 0},  {"i", 0}, {"π", 0}, {"o", 0}, {"µ", 0}, {"δ", 0}, {"φ", 0},
                {"b", 0}, {"α", 0},{"g", 0},  {"j", 0},  {"m", 0}, {"p", 0}, {"s", 0},  {"u", 0},  {"y", 0},
                {"c", 0}, {"e", 0} ,{"ß", 0},{"k", 0}, {"n", 0},  {"q", 0}, {"τ", 0}, {"v",0},   {"z", 0},
                {"ç", 0}, {"ë", 0}, {"h", 0} ,{"l", 0}, {"σ", 0}, {"r", 0}, {"t", 0},  {"x", 0},  {"ε", 0}
            };
            // Perseritet shift-here (sa gjatesia e celesit)
            for (int off = 0; off < key_length; off++)
            {
                // Per secilen  shkronje te nje grupi (psh shkronjen 0, 3, 6 nese shifti eshte 3)
                for (int i = off; i < ciphertext.Length; i += key_length)
                {
                    if (cipherFrequency.ContainsKey(ciphertext[i].ToString()))
                    {
                        cipherFrequency[ciphertext[i].ToString()]++;
                    }
                }

                int sum = 0;
                List<double> dot_product_list = new List<double>();

                // Tek vector rendisim shkronjat e cipherFreq nga me e shpeshta
                var vector = cipherFrequency.OrderBy(key => key.Key);
                // Tek w do ruajme perqindjen e perseritjes se shkronjave
                List<double> w = new List<double>();

                foreach (var x in vector)
                {
                    // Totali i shkronjave te numeruara ne ate grup
                    sum += cipherFrequency[x.Key];
                }

                foreach (var x in vector)
                {
                    // Nese nuk shfaqet nje shkronje shtoja 0
                    if (cipherFrequency[x.Key] == 0)
                    {
                        w.Add(0);
                    }
                    // Nese shfaqet shkronja shtoja perqindjen e pjesmarrjes ne totalin e shkronjave
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
                    // Frekuenca e karaktereve ne cfaredo teksti shqip
                    double[] x = new double[] { 0.0726, 0.0112, 0.0044, 0.0025, 0.0241, 0.0964, 0.1025, 0.1029, 0.00946, 0.00776, 0.00694, 0.0106, 0.0789, 0.02453, 0.0371, 0.0169, 0.00505, 0.039588, 0.0603, 0.00566, 0.0389, 0.0313, 0.0096, 0.0651, 0.0039, 0.0358, 0.0245, 0.0882, 0.00353, 0.0367, 0.0143, 0.00055, 0.00024, 0.0065, 0.00642, 0.00073 };

                    var a_j = ShiftRight(x, j);

                    for (int k = 0; k <= 35; k++)
                    {
                        dot_product += (w[k] * a_j.ElementAt(k));
                    }

                    dot_product_list.Add(Math.Round(dot_product, 4)); // Rrumbullaksojme 4 shifra pas pikes
                    dot_product = 0;

                }
                big_dotproduct_list.Add(dot_product_list.IndexOf(dot_product_list.Max()));
            }

            string out_str = "";

            big_dotproduct_list.ForEach(x => out_str += alphabet[(int)x]);

            return out_str;

            //decrypt(ciphertext, out_str);

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

    }
}
