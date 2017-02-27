using System;
using UnityEngine;
namespace org.rintech.blockcity.map
{
    public class BlockRenderer : MonoBehaviour
    {
        public const int chunksize = 16;
        public static int renderingchunk_radius = 1;
        public static BlockRenderer[][] blockrenderers = new BlockRenderer[Map.mapsize][];
        private bool initialized = false;
        private bool isprovisional = false;
        public int id
        {
            get;
            private set;
        }
        public int x
        {
            get;
            private set;
        }
        public int y
        {
            get;
            private set;
        }
        public static void initialize()
        {
            for (int a = 0; a < blockrenderers.Length; a++)
            {
                blockrenderers[a] = new BlockRenderer[Map.mapsize];
            }
        }
        void Start()
        {
        }
        void Update()
        {
            if (initialized)
            {
                Block provisionalblock = null;
                if (Main.main.playercamera == null ? isprovisional ? true : MapManager.playingmap.blockmap[x][y].id != id : (provisionalblock = Main.main.playercamera.provisionalblockmap[x][y]) == null ? isprovisional ? true : MapManager.playingmap.blockmap[x][y].id != id : isprovisional ? provisionalblock.id != id : true)
                {
                    reload();
                }
            }
        }
        public void init(int id, int x, int y, bool isprovisional)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.isprovisional = isprovisional;
            initialized = true;
        }
        public BlockRenderer reload()
        {
            Block provisionalblock = Main.main.playercamera == null ? null : Main.main.playercamera.provisionalblockmap[x][y];
            destroy();
            return spawnBlockRenderer(provisionalblock == null ? MapManager.playingmap.blockmap[x][y] : provisionalblock, x, y, provisionalblock != null);
        }
        public void destroy()
        {
            Destroy(gameObject);
            blockrenderers[x][y] = null;
        }
        public static BlockRenderer spawnBlockRenderer(Block block, int x, int y, bool isprovisional)
        {
            BlockRenderer blockrenderer = null;
            switch (block.id)
            {
                case 1:
                    blockrenderer = Main.main.blockrendererprefab_tree;
                    break;
                case 2:
                    blockrenderer = Main.main.blockrendererprefab_forest;
                    break;
                case 3:
                    blockrenderer = Main.main.blockrendererprefab_stone;
                    break;
                case 4:
                    blockrenderer = Main.main.blockrendererprefab_roadx;
                    break;
                case 5:
                    blockrenderer = Main.main.blockrendererprefab_roady;
                    break;
                case 6:
                    blockrenderer = Main.main.blockrendererprefab_roadlu;
                    break;
                case 7:
                    blockrenderer = Main.main.blockrendererprefab_roadru;
                    break;
                case 8:
                    blockrenderer = Main.main.blockrendererprefab_roadrd;
                    break;
                case 9:
                    blockrenderer = Main.main.blockrendererprefab_roadld;
                    break;
                case 10:
                    blockrenderer = Main.main.blockrendererprefab_roadlur;
                    break;
                case 11:
                    blockrenderer = Main.main.blockrendererprefab_roadrud;
                    break;
                case 12:
                    blockrenderer = Main.main.blockrendererprefab_roadldr;
                    break;
                case 13:
                    blockrenderer = Main.main.blockrendererprefab_roadlud;
                    break;
                case 14:
                    blockrenderer = Main.main.blockrendererprefab_roadc;
                    break;
                case 15:
                    blockrenderer = Main.main.blockrendererprefab_dirt;
                    break;
                default:
                    blockrenderer = Main.main.blockrendererprefab_grass;
                    break;
            }
            blockrenderer = Instantiate(blockrenderer);
            blockrenderer.gameObject.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
            blockrenderer.init(block.id, x, y, isprovisional);
            return blockrenderers[x][y] = blockrenderer;
        }
    }
}
