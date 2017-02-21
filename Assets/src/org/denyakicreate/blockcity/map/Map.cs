using UnityEngine;

namespace org.denyakicreate.blockcity.map
{
    public class Map
    {
        public const string latestver = "0.1";//最新（現在）のマップバージョン名。マップのバージョンを保存するために必要
        public const int mapsize = 1024;//マップのサイズ。mapsize x mapsizeとなる。数字は必ず16（レンダリングチャンクの大きさ）で割れる数にすること
        public const long startmoney = 500L * 10000 * 10000;//初期の所持金
        public const long tree_sale = 12000;//木の値段
        public const long stone_sale = 1000;//石の値段
        public string ver;//マップのバージョン
        public string mapname;//マップ名
        public Block[][] blockmap;//ブロックのデータ（blockmap[x][y]となる。使用する際はブロックの座標がマイナスになるため、配列を呼び出すためにはxとyをそれぞれmapsize / 2で足すこと。マップの大きさが偶数であるため、マップの中心は左上とする）
        public long money;//所持金
        public enum Direction//上下左右の向き情報。道路などに使用
        {
            up, down, left, right
        }
        public Map(string ver, string mapname)
        {
            this.ver = ver;
            this.mapname = mapname;
            blockmap = new Block[mapsize][];
            for (int a = 0; a < blockmap.Length; a++)
            {
                Block[] b = new Block[mapsize];
                for (int c = 0; c < b.Length; c++)
                {
                    switch (Random.Range(0, 1024))
                    {
                        case 0:
                            b[c] = Block.tree;
                            break;
                        case 1:
                            b[c] = Block.stone;
                            break;
                        default:
                            b[c] = Block.grass;
                            break;
                    }
                }
                blockmap[a] = b;
            }
            for (int z = 0; z < 10; z++)
            {
                for (int a = 0; a < blockmap.Length; a++)
                {
                    for (int c = 0; c < blockmap[a].Length; c++)
                    {
                        if (blockmap[a][c].id == Block.tree.id)
                        {
                            for (int d = -1; d <= 1; d++)
                            {
                                for (int e = -1; e <= 1; e++)
                                {
                                    if (0 < a + d && a + d < mapsize && 0 < c + e && c + e < mapsize && Random.Range(0, 8) == 0)
                                    {
                                        if (blockmap[a + d][c + e].id == Block.grass.id)
                                        {
                                            blockmap[a + d][c + e] = Block.tree;
                                        }
                                        else if (blockmap[a + d][c + e].id == Block.tree.id)
                                        {
                                            blockmap[a + d][c + e] = Block.forest;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            money = startmoney;
        }
        public Map(string ver, string mapname, Block[][] blockmap, long money)
        {
            this.ver = ver;
            this.mapname = mapname;
            this.blockmap = blockmap;
            this.money = money;
        }
    }
}
