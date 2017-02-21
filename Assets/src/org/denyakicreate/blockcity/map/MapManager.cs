using System.IO;
using UnityEngine;
using org.densyakun.csvmanager;
namespace org.denyakicreate.blockcity.map
{
    public class MapManager
    {
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
                string csvpath = null;
                for (int b = 0; b < files.Length; b++)
                {
                    if (Path.GetFileName(files[b]).Equals("map.csv"))
                    {
                        csvpath = files[b];
                        break;
                    }
                }
                if (csvpath != null)
                {
                    /*string[][] data = CSV.AllRead(csvpath);
                    string mapver = null;
                    for (int b = 0; b < data.Length; b++)
                    {
                        if (1 < data[b].Length)
                        {
                            if (data[b][0].Equals("v"))
                            {
                                mapver = data[b][1];
                            }
                        }
                    }
                    if (mapver != null && isEnableMapVer(mapver))
                    {*/
                    string[] copy = new string[maplist.Length + 1];
                    for (int b = 0; b < maplist.Length; b++)
                    {
                        copy[b] = maplist[b];
                    }
                    copy[maplist.Length] = mapname;
                    maplist = copy;
                    //}
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
        public static Map openMap(string mapname)
        {
            string mapdir = Path.Combine(dir, mapname);
            if (Directory.Exists(mapdir))
            {
                string[] files = Directory.GetFiles(mapdir);
                string csvpath = null;
                for (int a = 0; a < files.Length; a++)
                {
                    if (Path.GetFileName(files[a]).Equals("map.csv"))
                    {
                        csvpath = files[a];
                        break;
                    }
                }
                if (csvpath != null)
                {
                    string[][] data = CSV.AllRead(csvpath);
                    string mapver = null;
                    Block[][] blockmap = new Block[Map.mapsize][];
                    long money = Map.startmoney;
                    for (int x = 0; x < blockmap.Length; x++)
                    {
                        Block[] linearblocks = new Block[Map.mapsize];
                        for (int y = 0; y < linearblocks.Length; y++)
                        {
                            linearblocks[y] = Block.grass;
                        }
                        blockmap[x] = linearblocks;
                    }
                    for (int a = 0; a < data.Length; a++)
                    {
                        if (2 <= data[a].Length)
                        {
                            if (data[a][0].Equals(""))
                            {
                                if (4 <= data[a].Length)
                                {
                                    blockmap[int.Parse(data[a][2])][int.Parse(data[a][3])] = Block.getNewBlock(int.Parse(data[a][1]));
                                    //Debug.Log(a + "_" + data[a].Length);
                                    /*if (6 <= data[a].Length)
                                    {
                                        blockmap[int.Parse(data[a][2])][int.Parse(data[a][3])].metadata = new Metadata(CSV.StringtoArray(data[a][4]), CSV.StringtoArray(data[a][5]));
                                    }*/
                                }
                            }
                            else if (data[a][0].Equals("v"))
                            {
                                if (!isEnableMapVer(mapver = data[a][1]))
                                {
                                    break;
                                }
                            }
                            else if (data[a][0].Equals("m"))
                            {
                                money = long.Parse(data[a][1]);
                            }
                        }
                    }
                    if (mapver != null && isEnableMapVer(mapver))
                    {
                        playingmap = new Map(mapver, mapname, blockmap, money);
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
            string mapfile = Path.Combine(mapdir, "map.csv");
            int a = 0;
            for (int x = 0; x < Map.mapsize; x++)
            {
                for (int y = 0; y < Map.mapsize; y++)
                {
                    if (map.blockmap[x][y].id != 0)
                    {
                        a++;
                    }
                }
            }
            string[][] data = new string[2 + a][];
            data[0] = new string[] { "v", map.ver };
            data[1] = new string[] { "m", map.money.ToString() };
            a = 0;
            for (int x = 0; x < Map.mapsize; x++)
            {
                for (int y = 0; y < Map.mapsize; y++)
                {
                    if (map.blockmap[x][y].id != 0)
                    {
                        if (map.blockmap[x][y].metadata.keys.Length == 0)
                        {
                            data[2 + a] = new string[] { "", map.blockmap[x][y].id.ToString(), x.ToString(), y.ToString() };
                        }
                        else
                        {
                            data[2 + a] = new string[] { "", map.blockmap[x][y].id.ToString(), x.ToString(), y.ToString(), CSV.ArrayToString(map.blockmap[x][y].metadata.keys), CSV.ArrayToString(map.blockmap[x][y].metadata.values) };
                        }
                        a++;
                    }
                }
            }
            CSV.AllWrite(mapfile, data);
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
            /*byte b = 0;
            byte c = 0;
            for (int d = a.Length - 1; 0 <= d; d--)
            {
                b++;
                if (a[d].Equals('1'))
                {
                    if (b == 1 || b == 5)
                    {
                        str = "一" + str;
                    }
                }
                else if (a[d].Equals('2'))
                {
                    str = "二" + str;
                }
                else if (a[d].Equals('3'))
                {
                    str = "三" + str;
                }
                else if (a[d].Equals('4'))
                {
                    str = "四" + str;
                }
                else if (a[d].Equals('5'))
                {
                    str = "五" + str;
                }
                else if (a[d].Equals('6'))
                {
                    str = "六" + str;
                }
                else if (a[d].Equals('7'))
                {
                    str = "七" + str;
                }
                else if (a[d].Equals('8'))
                {
                    str = "八" + str;
                }
                else if (a[d].Equals('9'))
                {
                    str = "九" + str;
                }
                switch (b)
                {
                    case 2:
                        str = "十" + str; 
                        break;
                    case 3:
                        str = "百" + str;
                        break;
                    case 4:
                        str = "千" + str;
                        break;
                    case 5:
                        c++;
                        switch (c)
                        {
                            case 1:
                                str = "万" + str;
                                break;
                            case 2:
                                str = "億" + str;
                                break;
                            case 3:
                                str = "兆" + str;
                                break;
                            case 4:
                                str = "京" + str;
                                break;
                            case 5:
                                str = "垓" + str;
                                break;
                            case 6:
                                str = "𥝱" + str;
                                break;
                            case 7:
                                str = "穣" + str;
                                break;
                            case 8:
                                str = "溝" + str;
                                break;
                            case 9:
                                str = "澗" + str;
                                break;
                            case 10:
                                str = "正" + str;
                                break;
                            case 11:
                                str = "載" + str;
                                break;
                            case 12:
                                str = "極" + str;
                                break;
                            case 13:
                                str = "恒河沙" + str;
                                break;
                            case 14:
                                str = "阿僧祇" + str;
                                break;
                            case 15:
                                str = "那由他" + str;
                                break;
                            case 16:
                                str = "不可思議" + str;
                                break;
                            case 17:
                                str = "無量大数" + str;
                                break;
                            default:
                                break;
                        }
                        b = 0;
                        break;
                    default:
                        break;
                }
            }*/
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
                }
                b--;
            }
            return str + "円</color>";
        }
    }
}
