using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace org.rintech.blockcity.map
{
    public class MapManager
    {
        public const string mapfilename = "map.bin";
        public static string dir;//マップファイルを格納するフォルダ
        public static string[] enablemapvers;//使用可能なマップのバージョン
        public static string[] randommapnames;//マップ名のサンプル（マップ新規作成時に使用）
        public static int randommapnamesindex;//マップ名のサンプルを参照する際に使用するインデックス。参照するたびに値をランダムに変更する
        public static Map playingmap;//使用中のマップ
        public static void init()
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir = Path.Combine(Application.persistentDataPath, "maps"));
            }
            enablemapvers = new string[] { "0.1" };
            randommapnames = new string[] {"green city", "new bridge", "blue forest", "thousand leaf",
                "stone river", "fortune island", "mountain hand", "northeast",
                "red castle", "new lodge" , "east capital", "small mountain",
            "white river", "white stone", "old river", "plateau",
            "long hill", "long field", "sakura city", "big ship",
            "east city", "west city", "south city", "north city",
            "central city", "forest city", "black stone", "sunset beach",
            "blue leaf", "chestnut field", "pine island", "harbor mountain",
            "east river", "west river", "south river", "north river",
            "pine mountain", "pine village", "pine river", "pine hill",
            "pine beach", "mountain hill", "big river", "big mountain" };
            randommapnamesindex = Random.Range(0, randommapnames.Length);
        }
        public static string[] getMapList()
        {
            string[] maplist = new string[0];
            string[] mapdirs = Directory.GetDirectories(dir);
            for (int a = 0; a < mapdirs.Length; a++)
            {
                string mapname = new DirectoryInfo(mapdirs[a]).Name;
                string[] files = Directory.GetFiles(Path.Combine(dir, mapname));
                string datpath = null;
                for (int b = 0; b < files.Length; b++)
                {
                    if (Path.GetFileName(files[b]).Equals(mapfilename))
                    {
                        datpath = files[b];
                        break;
                    }
                }
                if (datpath != null)
                {
                    string[] copy = new string[maplist.Length + 1];
                    for (int b = 0; b < maplist.Length; b++)
                    {
                        copy[b] = maplist[b];
                    }
                    copy[maplist.Length] = mapname;
                    maplist = copy;
                }
            }
            return maplist;
        }
        public static bool createNewMap(string mapname)
        {
            if (Directory.Exists(Path.Combine(dir, mapname)))
            {
                return false;
            }
            saveMap(new Map(Map.latestver, mapname));
            return true;
        }
        public static bool isEnableMapVer(string mapver)
        {
            for (int a = 0; a < enablemapvers.Length; a++)
            {
                if (enablemapvers[a].Equals(mapver))
                {
                    return true;
                }
            }
            return false;
        }
        public static void RandomMapNameChange()
        {
            randommapnamesindex = Random.Range(0, randommapnames.Length);
        }
        public static string RandomMapName()
        {
            if (0 < randommapnames.Length)
            {
                string str = randommapnames[randommapnamesindex];
                if (Directory.Exists(Path.Combine(dir, str)))
                {
                    RandomMapNameChange();
                    return RandomMapName();
                }
                else
                {
                    return str;
                }
            }
            return "";
        }
        //データ:
        //ブロック: (empty), id, x, y or (empty), id, x, y, [metadata_keys], [metadata_values]
        //バージョン: v, バージョン
        //財布の金額: m, 金額
        //マップの開始時間: st, 時間
        //マップのプレイ時間: pt, 時間
        //マップの時間: t, 時間
        public static Map openMap(string mapname)
        {
            string mapdir = Path.Combine(dir, mapname);
            if (Directory.Exists(mapdir))
            {
                string[] files = Directory.GetFiles(mapdir);
                string datpath = null;
                for (int a = 0; a < files.Length; a++)
                {
                    if (Path.GetFileName(files[a]).Equals(mapfilename))
                    {
                        datpath = files[a];
                        break;
                    }
                }
                if (datpath != null)
                {
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(Path.Combine(mapdir, mapfilename), FileMode.Open, FileAccess.Read, FileShare.Read);
                    playingmap = (Map)formatter.Deserialize(stream);
                    stream.Close();
                    if (isEnableMapVer(playingmap.ver))
                    {
                        return playingmap;
                    }
                }
            }
            return null;
        }
        public static void saveMap(Map map)
        {
            string mapdir = Path.Combine(dir, map.mapname);
            Directory.CreateDirectory(mapdir);
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(mapdir, mapfilename), FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, map);
            stream.Close();
            Debug.Log("マップをセーブしました: " + map.mapname);
        }
        public static void closeMap()
        {
            if (playingmap != null)
            {
                BlockRenderer[] renderers = GameObject.FindObjectsOfType<BlockRenderer>();
                for (int a = 0; a < renderers.Length; a++)
                {
                    Object.Destroy(renderers[a].gameObject);
                }
                playingmap = null;
            }
        }
        public static string getMoneyString(long money)
        {
            if (money == 0)
            {
                return "0円";
            }
            char[] a = money.ToString().ToCharArray();
            string str = money <= 10000 * 10000 ? "<color=red>" : "<color=white>";
            int b = a.Length;
            while (4 < b)
            {
                b -= 4;
            }
            bool c = false;
            bool d = false;
            for (int e = 0; e < a.Length; e++)
            {
                if (a[e].Equals('1'))
                {
                    if (1 < a.Length && a.Length < 5)
                    {
                        str += "一";
                        c = true;
                        d = true;
                    }
                }
                else if (a[e].Equals('2'))
                {
                    str += "二";
                    c = true;
                    d = true;
                }
                else if (a[e].Equals('3'))
                {
                    str += "三";
                    c = true;
                    d = true;
                }
                else if (a[e].Equals('4'))
                {
                    str += "四";
                    c = true;
                    d = true;
                }
                else if (a[e].Equals('5'))
                {
                    str += "五";
                    c = true;
                    d = true;
                }
                else if (a[e].Equals('6'))
                {
                    str += "六";
                    c = true;
                    d = true;
                }
                else if (a[e].Equals('7'))
                {
                    str += "七";
                    c = true;
                    d = true;
                }
                else if (a[e].Equals('8'))
                {
                    str += "八";
                    c = true;
                    d = true;
                }
                else if (a[e].Equals('9'))
                {
                    str += "九";
                    c = true;
                    d = true;
                }
                if (c)
                {
                    if (b == 2)
                    {
                        str += "十";
                        c = false;
                    }
                    else if (b == 3)
                    {
                        str += "百";
                        c = false;
                    }
                    else if (b == 4)
                    {
                        str += "千";
                        c = false;
                    }
                }
                if (d)
                {
                    if (e == a.Length - 5)
                    {
                        str += "万";
                        b = 5;
                        c = false;
                        d = false;
                    }
                    else if (e == a.Length - 9)
                    {
                        str += "億";
                        b = 5;
                        c = false;
                        d = false;
                    }
                    else if (e == a.Length - 13)
                    {
                        str += "兆";
                        b = 5;
                        c = false;
                        d = false;
                    }
                    else if (e == a.Length - 17)
                    {
                        str += "京";
                        b = 5;
                        c = false;
                        d = false;
                    }
                    else if (e == a.Length - 21)
                    {
                        str += "垓";
                        b = 5;
                        c = false;
                        d = false;
                    }
                }
                b--;
            }
            return str + "円</color>";
        }
    }
}
