using System.IO;
namespace org.densyakun.csvmanager
{
    public static class CSV
    {
        public static void AllWrite(string path, string[][] datas)
        {
            FileStream fs = File.Create(path);
            StreamWriter w = new StreamWriter(fs);
            for (int a = 0; a < datas.Length; a++)
            {
                for (int b = 0; b < datas[a].Length; b++)
                {
                    if (b != 0)
                    {
                        w.Write(", ");
                    }
                    w.Write(datas[a][b].Trim());
                }
                w.Write("\n");
            }
            w.Close();
            fs.Close();
        }
        public static string[][] AllRead(string path)
        {
            if (File.Exists(path))
            {
                int a = 0;
                StreamReader r = new StreamReader(path);
                if (r != null)
                {
                    while (!r.EndOfStream && (r.ReadLine() != null))
                    {
                        a++;
                    }
                    r.Close();
                }
                string[][] d = new string[a][];
                a = 0;
                r = new StreamReader(path);
                if (r != null)
                {
                    for (int l = 0; !r.EndOfStream; l++)
                    {
                        string line = r.ReadLine();
                        char[] h = line.ToCharArray();
                        int b = 0;
                        for (int c = 0; c < line.Length; c++)
                        {
                            if (h[c].Equals('['))
                            {
                                b++;
                            }
                            else if (h[c].Equals(']'))
                            {
                                b--;
                            }
                            else if ((b == 0) && h[c].Equals(','))
                            {
                                a++;
                            }
                        }
                        a++;
                        string[] f = new string[a];
                        a = 0;
                        b = 0;
                        int e = 0;
                        for (int c = 0; c < line.Length; c++)
                        {
                            if (h[c].Equals('['))
                            {
                                b++;
                            }
                            else if (h[c].Equals(']'))
                            {
                                b--;
                            }
                            else if ((b == 0) && h[c].Equals(','))
                            {
                                f[a] = line.Substring(e, c - e).Trim();
                                a++;
                                e = c + 1;
                            }
                        }
                        f[a] = line.Substring(e).Trim();
                        d[l] = f;
                    }
                    r.Close();
                }
                return d;
            }
            return new string[0][];
        }
        public static string ArrayToString(string[] array)
        {
            if (array != null)
            {
                string a = "[";
                for (int b = 0; b < array.Length; b++)
                {
                    if (0 < b)
                    {
                        a += ", ";
                    }
                    a += array[b];
                }
                a += "]";
                return a;
            }
            return "";
        }
        public static string[] StringtoArray(string str)
        {
            int s = 0;
            if (2 <= str.Length)
            {
                str = str.Substring(1, str.Length - 2);
                int b = 0;
                int e = 0;
                for (int c = 0; c < str.Length; c++)
                {
                    char d = str.ToCharArray()[c];
                    if (d.Equals('['))
                    {
                        b++;
                    }
                    else if (d.Equals(']'))
                    {
                        b--;
                    }
                    else if ((b == 0) && d.Equals(','))
                    {
                        s++;
                        e = c + 1;
                    }
                }
                s++;
            }
            string[] a = new string[s];
            s = 0;
            if (2 <= str.Length)
            {
                str = str.Substring(1, str.Length - 2);
                int b = 0;
                int e = 0;
                for (int c = 0; c < str.Length; c++)
                {
                    char d = str.ToCharArray()[c];
                    if (d.Equals('['))
                    {
                        b++;
                    }
                    else if (d.Equals(']'))
                    {
                        b--;
                    }
                    else if ((b == 0) && d.Equals(','))
                    {
                        a[s] = str.Substring(e, c - e);
                        s++;
                        e = c + 1;
                    }
                }
                a[s] = str.Substring(e);
                for (int c = 0; c < a.Length; c++)
                {
                    a[c] = a[c].Trim();
                }
            }
            return a;
        }
        public static bool isArray(string str)
        {
            if (0 < str.Length)
            {
                return str.ToCharArray()[0].Equals('[') && str.ToCharArray()[str.Length - 1].Equals(']');
            }
            return false;
        }
    }
}
