using System;

namespace org.rintech.blockcity.map
{
    [Serializable]
    public class Map
    {
        public const string latestver = "0.1";//最新（現在）のマップバージョン名。マップのバージョンを保存するために必要
        public const int mapsize = 1024;//マップのサイズ。mapsize x mapsizeとなる。数字は必ず16（レンダリングチャンクの大きさ）で割れる数にすること
        public const long startmoney = 500L * 10000 * 10000;//初期の所持金
        public const long tree_sale = 12000;//木の値段
        public const long stone_sale = 1000;//石の値段
        public string ver
        {
            private set;
            get;
        }//マップのバージョン
        public string mapname
        {
            private set;
            get;
        }//マップ名
        public Block[][] blockmap;//ブロックのデータ（blockmap[x][y]）
        public long money;//所持金
        public string oldver
        {
            private set;
            get;
        }//マップの旧バージョン(読み込み時のバージョン)
        public long starttime
        {
            private set;
            get;
        }//マップの開始時間
        public long playtime;//マップのプレイ時間
        public long time;//マップの時間
        public Map(string ver, string mapname)
        {
            oldver = ver;
            this.ver = latestver;
            this.mapname = mapname;
            blockmap = new Block[mapsize][];
            for (int a = 0; a < blockmap.Length; a++)
            {
                Block[] b = new Block[mapsize];
                for (int c = 0; c < b.Length; c++)
                {
                    switch (UnityEngine.Random.Range(0, 1024))
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
                                    if (0 < a + d && a + d < mapsize && 0 < c + e && c + e < mapsize && UnityEngine.Random.Range(0, 8) == 0)
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
            starttime = DateTime.Now.Ticks;
            playtime = 0;
            time = 0;
        }
        public Map(string ver, string mapname, Block[][] blockmap, long money,long starttime, long playtime, long time)
        {
            oldver = ver;
            this.ver = latestver;
            this.mapname = mapname;
            this.blockmap = blockmap;
            this.money = money;
            this.starttime = starttime;
            this.playtime = playtime;
            this.time = time;
        }
        public bool destroyBlock(int x, int y)
        {
            if (blockmap[x][y].id != Block.dirt.id)
            {
                if (blockmap[x][y].id == Block.tree.id || blockmap[x][y].id == Block.forest.id)
                {
                    blockmap[x][y] = Block.grass;
                }
                else if (blockmap[x][y].id == Block.stone.id)
                {
                    blockmap[x][y] = Block.grass;
                }
                else
                {
                    blockmap[x][y] = Block.dirt;
                }
                return true;
            }
            return false;
        }
        public void roadConstruction(int x, int y, int length, Direction direction)
        {
            buildTraffic(x, y, length, direction, Block.road_x, Block.road_y, Block.road_lu, Block.road_ru, Block.road_rd, Block.road_ld, Block.road_lur, Block.road_rud, Block.road_ldr, Block.road_lud, Block.road_c);
        }
        public void buildTraffic(int x, int y, int length, Direction direction, Block tx, Block ty, Block lu, Block ru, Block rd, Block ld, Block lur, Block rud, Block ldr, Block lud, Block c)
        {
            switch (direction)
            {
                case Direction.up:
                    for (int a = y; a < y + length; a++)
                    {
                        if (roadIsConstructible(blockmap[x][a].id))
                        {
                            if (blockmap[x][a].id == ld.id)
                            {
                                if (a != y + length - 1)
                                {
                                    blockmap[x][a] = lud;
                                }
                            }
                            else if (blockmap[x][a].id == lu.id)
                            {
                                if (a != y)
                                {
                                    blockmap[x][a] = lud;
                                }
                            }
                            else if (blockmap[x][a].id == ldr.id)
                            {
                                if (a != y + length - 1)
                                {
                                    blockmap[x][a] = c;
                                }
                            }
                            else if (blockmap[x][a].id == lur.id)
                            {
                                if (a != y)
                                {
                                    blockmap[x][a] = c;
                                }
                            }
                            else if (blockmap[x][a].id == tx.id)
                            {
                                if (a == y)
                                {
                                    bool l = false;
                                    if (0 < x && (blockmap[x - 1][a].id == tx.id || blockmap[x - 1][a].id == ldr.id || blockmap[x - 1][a].id == lur.id || blockmap[x - 1][a].id == rd.id || blockmap[x - 1][a].id == ru.id || blockmap[x - 1][a].id == rud.id || blockmap[x - 1][a].id == c.id))
                                    {
                                        l = true;
                                    }
                                    bool r = false;
                                    if (x < mapsize - 1 && (blockmap[x + 1][a].id == tx.id || blockmap[x + 1][a].id == ld.id || blockmap[x + 1][a].id == ldr.id || blockmap[x + 1][a].id == lu.id || blockmap[x + 1][a].id == lud.id || blockmap[x + 1][a].id == lur.id || blockmap[x + 1][a].id == c.id))
                                    {
                                        r = true;
                                    }
                                    if (l == r)
                                    {
                                        blockmap[x][a] = lur;
                                    }
                                    else
                                    {
                                        if (l)
                                        {
                                            blockmap[x][a] = lu;
                                        }
                                        else
                                        {
                                            blockmap[x][a] = ru;
                                        }
                                    }
                                }
                                else if (a == y + length - 1)
                                {
                                    bool l = false;
                                    if (0 < x && (blockmap[x - 1][a].id == tx.id || blockmap[x - 1][a].id == ldr.id || blockmap[x - 1][a].id == lur.id || blockmap[x - 1][a].id == rd.id || blockmap[x - 1][a].id == ru.id || blockmap[x - 1][a].id == rud.id || blockmap[x - 1][a].id == c.id))
                                    {
                                        l = true;
                                    }
                                    bool r = false;
                                    if (x < mapsize - 1 && (blockmap[x + 1][a].id == tx.id || blockmap[x + 1][a].id == ld.id || blockmap[x + 1][a].id == ldr.id || blockmap[x + 1][a].id == lu.id || blockmap[x + 1][a].id == lud.id || blockmap[x + 1][a].id == lur.id || blockmap[x + 1][a].id == c.id))
                                    {
                                        r = true;
                                    }
                                    if (l == r)
                                    {
                                        blockmap[x][a] = ldr;
                                    }
                                    else
                                    {
                                        if (l)
                                        {
                                            blockmap[x][a] = ld;
                                        }
                                        else
                                        {
                                            blockmap[x][a] = rd;
                                        }
                                    }
                                }
                                else
                                {
                                    bool l = false;
                                    if (0 < x && (blockmap[x - 1][a].id == tx.id || blockmap[x - 1][a].id == ldr.id || blockmap[x - 1][a].id == lur.id || blockmap[x - 1][a].id == rd.id || blockmap[x - 1][a].id == ru.id || blockmap[x - 1][a].id == rud.id || blockmap[x - 1][a].id == c.id))
                                    {
                                        l = true;
                                    }
                                    bool r = false;
                                    if (x < mapsize - 1 && (blockmap[x + 1][a].id == tx.id || blockmap[x + 1][a].id == ld.id || blockmap[x + 1][a].id == ldr.id || blockmap[x + 1][a].id == lu.id || blockmap[x + 1][a].id == lud.id || blockmap[x + 1][a].id == lur.id || blockmap[x + 1][a].id == c.id))
                                    {
                                        r = true;
                                    }
                                    if (l == r)
                                    {
                                        blockmap[x][a] = c;
                                    }
                                    else
                                    {
                                        if (l)
                                        {
                                            blockmap[x][a] = lud;
                                        }
                                        else
                                        {
                                            blockmap[x][a] = rud;
                                        }
                                    }
                                }
                            }
                            else if (blockmap[x][a].id == rd.id)
                            {
                                if (a != y + length - 1)
                                {
                                    blockmap[x][a] = rud;
                                }
                            }
                            else if (blockmap[x][a].id == ru.id)
                            {
                                if (a != y)
                                {
                                    blockmap[x][a] = rud;
                                }
                            }
                            else if(blockmap[x][a].id != lud.id && blockmap[x][a].id != rud.id && blockmap[x][a].id != c.id)
                            {
                                blockmap[x][a] = ty;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case Direction.down:
                    for (int a = y; a > y - length; a--)
                    {
                        if (roadIsConstructible(blockmap[x][a].id))
                        {
                            if (blockmap[x][a].id == ld.id)
                            {
                                if (a != y)
                                {
                                    blockmap[x][a] = lud;
                                }
                            }
                            else if (blockmap[x][a].id == lu.id)
                            {
                                if (a != y - length + 1)
                                {
                                    blockmap[x][a] = lud;
                                }
                            }
                            else if (blockmap[x][a].id == ldr.id)
                            {
                                if (a != y)
                                {
                                    blockmap[x][a] = c;
                                }
                            }
                            else if (blockmap[x][a].id == lur.id)
                            {
                                if (a != y - length + 1)
                                {
                                    blockmap[x][a] = c;
                                }
                            }
                            else if (blockmap[x][a].id == tx.id)
                            {
                                if (a == y)
                                {
                                    bool l = false;
                                    if (0 < x && (blockmap[x - 1][a].id == tx.id || blockmap[x - 1][a].id == ldr.id || blockmap[x - 1][a].id == lur.id || blockmap[x - 1][a].id == rd.id || blockmap[x - 1][a].id == ru.id || blockmap[x - 1][a].id == rud.id || blockmap[x - 1][a].id == c.id))
                                    {
                                        l = true;
                                    }
                                    bool r = false;
                                    if (x < mapsize - 1 && (blockmap[x + 1][a].id == tx.id || blockmap[x + 1][a].id == ld.id || blockmap[x + 1][a].id == ldr.id || blockmap[x + 1][a].id == lu.id || blockmap[x + 1][a].id == lud.id || blockmap[x + 1][a].id == lur.id || blockmap[x + 1][a].id == c.id))
                                    {
                                        r = true;
                                    }
                                    if (l == r)
                                    {
                                        blockmap[x][a] = ldr;
                                    }
                                    else
                                    {
                                        if (l)
                                        {
                                            blockmap[x][a] = ld;
                                        }
                                        else
                                        {
                                            blockmap[x][a] = rd;
                                        }
                                    }
                                }
                                else if (a == y - length + 1)
                                {
                                    bool l = false;
                                    if (0 < x && (blockmap[x - 1][a].id == tx.id || blockmap[x - 1][a].id == ldr.id || blockmap[x - 1][a].id == lur.id || blockmap[x - 1][a].id == rd.id || blockmap[x - 1][a].id == ru.id || blockmap[x - 1][a].id == rud.id || blockmap[x - 1][a].id == c.id))
                                    {
                                        l = true;
                                    }
                                    bool r = false;
                                    if (x < mapsize - 1 && (blockmap[x + 1][a].id == tx.id || blockmap[x + 1][a].id == ld.id || blockmap[x + 1][a].id == ldr.id || blockmap[x + 1][a].id == lu.id || blockmap[x + 1][a].id == lud.id || blockmap[x + 1][a].id == lur.id || blockmap[x + 1][a].id == c.id))
                                    {
                                        r = true;
                                    }
                                    if (l == r)
                                    {
                                        blockmap[x][a] = lur;
                                    }
                                    else
                                    {
                                        if (l)
                                        {
                                            blockmap[x][a] = lu;
                                        }
                                        else
                                        {
                                            blockmap[x][a] = ru;
                                        }
                                    }
                                }
                                else
                                {
                                    bool l = false;
                                    if (0 < x && (blockmap[x - 1][a].id == tx.id || blockmap[x - 1][a].id == ldr.id || blockmap[x - 1][a].id == lur.id || blockmap[x - 1][a].id == rd.id || blockmap[x - 1][a].id == ru.id || blockmap[x - 1][a].id == rud.id || blockmap[x - 1][a].id == c.id))
                                    {
                                        l = true;
                                    }
                                    bool r = false;
                                    if (x < mapsize - 1 && (blockmap[x + 1][a].id == tx.id || blockmap[x + 1][a].id == ld.id || blockmap[x + 1][a].id == ldr.id || blockmap[x + 1][a].id == lu.id || blockmap[x + 1][a].id == lud.id || blockmap[x + 1][a].id == lur.id || blockmap[x + 1][a].id == c.id))
                                    {
                                        r = true;
                                    }
                                    if (l == r)
                                    {
                                        blockmap[x][a] = c;
                                    }
                                    else
                                    {
                                        if (l)
                                        {
                                            blockmap[x][a] = lud;
                                        }
                                        else
                                        {
                                            blockmap[x][a] = rud;
                                        }
                                    }
                                }
                            }
                            else if (blockmap[x][a].id == rd.id)
                            {
                                if (a != y)
                                {
                                    blockmap[x][a] = rud;
                                }
                            }
                            else if (blockmap[x][a].id == ru.id)
                            {
                                if (a != y - length + 1)
                                {
                                    blockmap[x][a] = rud;
                                }
                            }
                            else if (blockmap[x][a].id != lud.id && blockmap[x][a].id != rud.id && blockmap[x][a].id != c.id)
                            {
                                blockmap[x][a] = ty;
                            }
                        }
                    }
                    break;
                case Direction.left:
                    for (int a = x; a > x - length; a--)
                    {
                        if (roadIsConstructible(blockmap[a][y].id))
                        {
                            if (blockmap[a][y].id == ld.id)
                            {
                                if (a != x)
                                {
                                    blockmap[a][y] = ldr;
                                }
                            }
                            else if (blockmap[a][y].id == lu.id)
                            {
                                if (a != x)
                                {
                                    blockmap[a][y] = lur;
                                }
                            }
                            else if (blockmap[a][y].id == lud.id)
                            {
                                if (a != x)
                                {
                                    blockmap[a][y] = c;
                                }
                            }
                            else if (blockmap[a][y].id == ty.id)
                            {
                                if (a == x)
                                {
                                    bool d = false;
                                    if (0 < y && (blockmap[a][y - 1].id == ty.id || blockmap[a][y - 1].id == c.id || blockmap[a][y - 1].id == lu.id || blockmap[a][y - 1].id == lud.id || blockmap[a][y - 1].id == lur.id || blockmap[a][y - 1].id == ru.id || blockmap[a][y - 1].id == rud.id))
                                    {
                                        d = true;
                                    }
                                    bool u = false;
                                    if (y < mapsize - 1 && (blockmap[a][y + 1].id == ty.id || blockmap[a][y + 1].id == c.id || blockmap[a][y + 1].id == ld.id || blockmap[a][y + 1].id == ldr.id || blockmap[a][y + 1].id == lud.id || blockmap[a][y + 1].id == rd.id || blockmap[a][y + 1].id == rud.id))
                                    {
                                        u = true;
                                    }
                                    if (d == u)
                                    {
                                        blockmap[a][y] = lud;
                                    }
                                    else
                                    {
                                        if (d)
                                        {
                                            blockmap[a][y] = ld;
                                        }
                                        else
                                        {
                                            blockmap[a][y] = lu;
                                        }
                                    }
                                }
                                else if (a == x - length + 1)
                                {
                                    bool d = false;
                                    if (0 < y && (blockmap[a][y - 1].id == ty.id || blockmap[a][y - 1].id == c.id || blockmap[a][y - 1].id == lu.id || blockmap[a][y - 1].id == lud.id || blockmap[a][y - 1].id == lur.id || blockmap[a][y - 1].id == ru.id || blockmap[a][y - 1].id == rud.id))
                                    {
                                        d = true;
                                    }
                                    bool u = false;
                                    if (y < mapsize - 1 && (blockmap[a][y + 1].id == ty.id || blockmap[a][y + 1].id == c.id || blockmap[a][y + 1].id == ld.id || blockmap[a][y + 1].id == ldr.id || blockmap[a][y + 1].id == lud.id || blockmap[a][y + 1].id == rd.id || blockmap[a][y + 1].id == rud.id))
                                    {
                                        u = true;
                                    }
                                    if (d == u)
                                    {
                                        blockmap[a][y] = rud;
                                    }
                                    else
                                    {
                                        if (d)
                                        {
                                            blockmap[a][y] = rd;
                                        }
                                        else
                                        {
                                            blockmap[a][y] = ru;
                                        }
                                    }
                                }
                                else
                                {
                                    bool d = false;
                                    if (0 < y && (blockmap[a][y - 1].id == ty.id || blockmap[a][y - 1].id == c.id || blockmap[a][y - 1].id == lu.id || blockmap[a][y - 1].id == lud.id || blockmap[a][y - 1].id == lur.id || blockmap[a][y - 1].id == ru.id || blockmap[a][y - 1].id == rud.id))
                                    {
                                        d = true;
                                    }
                                    bool u = false;
                                    if (y < mapsize - 1 && (blockmap[a][y + 1].id == ty.id || blockmap[a][y + 1].id == c.id || blockmap[a][y + 1].id == ld.id || blockmap[a][y + 1].id == ldr.id || blockmap[a][y + 1].id == lud.id || blockmap[a][y + 1].id == rd.id || blockmap[a][y + 1].id == rud.id))
                                    {
                                        u = true;
                                    }
                                    if (d == u)
                                    {
                                        blockmap[a][y] = c;
                                    }
                                    else
                                    {
                                        if (d)
                                        {
                                            blockmap[a][y] = ldr;
                                        }
                                        else
                                        {
                                            blockmap[a][y] = lur;
                                        }
                                    }
                                }
                            }
                            else if (blockmap[a][y].id == rd.id)
                            {
                                if (a != x - length + 1)
                                {
                                    blockmap[a][y] = ldr;
                                }
                            }
                            else if (blockmap[a][y].id == ru.id)
                            {
                                if (a != x - length + 1)
                                {
                                    blockmap[a][y] = lur;
                                }
                            }
                            else if (blockmap[a][y].id == rud.id)
                            {
                                if (a != x - length + 1)
                                {
                                    blockmap[a][y] = c;
                                }
                            }
                            else if (blockmap[a][y].id != ldr.id && blockmap[a][y].id != lur.id && blockmap[a][y].id != c.id)
                            {
                                blockmap[a][y] = tx;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case Direction.right:
                    for (int a = x; a < x + length; a++)
                    {
                        if (roadIsConstructible(blockmap[a][y].id))
                        {
                            if (blockmap[a][y].id == ld.id)
                            {
                                if (a != x + length - 1)
                                {
                                    blockmap[a][y] = ldr;
                                }
                            }
                            else if (blockmap[a][y].id == lu.id)
                            {
                                if (a != x + length - 1)
                                {
                                    blockmap[a][y] = lur;
                                }
                            }
                            else if (blockmap[a][y].id == lud.id)
                            {
                                if (a != x + length - 1)
                                {
                                    blockmap[a][y] = c;
                                }
                            }
                            else if (blockmap[a][y].id == ty.id)
                            {
                                if (a == x)
                                {
                                    bool d = false;
                                    if (0 < y && (blockmap[a][y - 1].id == ty.id || blockmap[a][y - 1].id == c.id || blockmap[a][y - 1].id == lu.id || blockmap[a][y - 1].id == lud.id || blockmap[a][y - 1].id == lur.id || blockmap[a][y - 1].id == ru.id || blockmap[a][y - 1].id == rud.id))
                                    {
                                        d = true;
                                    }
                                    bool u = false;
                                    if (y < mapsize - 1 && (blockmap[a][y + 1].id == ty.id || blockmap[a][y + 1].id == c.id || blockmap[a][y + 1].id == ld.id || blockmap[a][y + 1].id == ldr.id || blockmap[a][y + 1].id == lud.id || blockmap[a][y + 1].id == rd.id || blockmap[a][y + 1].id == rud.id))
                                    {
                                        u = true;
                                    }
                                    if (d == u)
                                    {
                                        blockmap[a][y] = rud;
                                    }
                                    else
                                    {
                                        if (d)
                                        {
                                            blockmap[a][y] = rd;
                                        }
                                        else
                                        {
                                            blockmap[a][y] = ru;
                                        }
                                    }
                                }
                                else if (a == x + length - 1)
                                {
                                    bool d = false;
                                    if (0 < y && (blockmap[a][y - 1].id == ty.id || blockmap[a][y - 1].id == c.id || blockmap[a][y - 1].id == lu.id || blockmap[a][y - 1].id == lud.id || blockmap[a][y - 1].id == lur.id || blockmap[a][y - 1].id == ru.id || blockmap[a][y - 1].id == rud.id))
                                    {
                                        d = true;
                                    }
                                    bool u = false;
                                    if (y < mapsize - 1 && (blockmap[a][y + 1].id == ty.id || blockmap[a][y + 1].id == c.id || blockmap[a][y + 1].id == ld.id || blockmap[a][y + 1].id == ldr.id || blockmap[a][y + 1].id == lud.id || blockmap[a][y + 1].id == rd.id || blockmap[a][y + 1].id == rud.id))
                                    {
                                        u = true;
                                    }
                                    if (d == u)
                                    {
                                        blockmap[a][y] = lud;
                                    }
                                    else
                                    {
                                        if (d)
                                        {
                                            blockmap[a][y] = ld;
                                        }
                                        else
                                        {
                                            blockmap[a][y] = lu;
                                        }
                                    }
                                }
                                else
                                {
                                    bool d = false;
                                    if (0 < y && (blockmap[a][y - 1].id == ty.id || blockmap[a][y - 1].id == c.id || blockmap[a][y - 1].id == lu.id || blockmap[a][y - 1].id == lud.id || blockmap[a][y - 1].id == lur.id || blockmap[a][y - 1].id == ru.id || blockmap[a][y - 1].id == rud.id))
                                    {
                                        d = true;
                                    }
                                    bool u = false;
                                    if (y < mapsize - 1 && (blockmap[a][y + 1].id == ty.id || blockmap[a][y + 1].id == c.id || blockmap[a][y + 1].id == ld.id || blockmap[a][y + 1].id == ldr.id || blockmap[a][y + 1].id == lud.id || blockmap[a][y + 1].id == rd.id || blockmap[a][y + 1].id == rud.id))
                                    {
                                        u = true;
                                    }
                                    if (d == u)
                                    {
                                        blockmap[a][y] = c;
                                    }
                                    else
                                    {
                                        if (d)
                                        {
                                            blockmap[a][y] = ldr;
                                        }
                                        else
                                        {
                                            blockmap[a][y] = lur;
                                        }
                                    }
                                }
                            }
                            else if (blockmap[a][y].id == rd.id)
                            {
                                if (a != x)
                                {
                                    blockmap[a][y] = ldr;
                                }
                            }
                            else if (blockmap[a][y].id == ru.id)
                            {
                                if (a != x)
                                {
                                    blockmap[a][y] = lur;
                                }
                            }
                            else if (blockmap[a][y].id == rud.id)
                            {
                                if (a != x)
                                {
                                    blockmap[a][y] = c;
                                }
                            }
                            else if (blockmap[a][y].id != ldr.id && blockmap[a][y].id != lur.id && blockmap[a][y].id != c.id)
                            {
                                blockmap[a][y] = tx;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public static bool breakable(int id)
        {
            return id != Block.dirt.id;
        }
        public static bool roadIsConstructible(int id)
        {
            return id == Block.grass.id || id == Block.dirt.id || id == Block.road_ld.id || id == Block.road_ldr.id || id == Block.road_lu.id || id == Block.road_lud.id || id == Block.road_lur.id || id == Block.road_rd.id || id == Block.road_ru.id || id == Block.road_rud.id || id == Block.road_x.id || id == Block.road_y.id || id == Block.road_c.id;
        }
    }
}
