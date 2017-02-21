using UnityEngine;
namespace org.denyakicreate.blockcity.map
{
    public class BlockRenderer : MonoBehaviour
    {
        public const int chunksize = 16;
        public static int renderingchunk_radius = 1;
        private bool initialized = false;
        //private bool updatable = false;
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
        /*public bool isUpdatable()
        {
            return updatable;
        }*/
        void Start()
        {
        }
        void Update()
        {
            if (/*updatable && */initialized)
            {
                int px = Map.mapsize / 2 + x;
                int py = Map.mapsize / 2 + y;
                Block provisionalblock = null;
                Block block = null;
                if (Main.main.playercamera == null ? true : (provisionalblock = Main.main.playercamera.provisionalblockmap[px][py]) == null ? (block = MapManager.playingmap.blockmap[px][py]).id != id : provisionalblock.id != id)
                {
                    Destroy(gameObject);
                    if (Main.main.playercamera != null)
                    {
                        spawnBlockRenderer(provisionalblock == null ? block : provisionalblock, x, y, true);
                    }
                }
            }
        }
        public void init(int id, int x, int y, bool updatable)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            //this.updatable = updatable;
            initialized = true;
        }
        public static BlockRenderer spawnBlockRenderer(Block block, int x, int y, bool updatable)
        {
            BlockRenderer blockrenderer = null;
            switch (block.id)
            {
                case 1:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_tree);
                    break;
                case 2:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_forest);
                    break;
                case 3:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_stone);
                    break;
                case 4:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_roadx);
                    break;
                case 5:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_roady);
                    break;
                case 6:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_roadlu);
                    break;
                case 7:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_roadru);
                    break;
                case 8:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_roadrd);
                    break;
                case 9:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_roadld);
                    break;
                case 10:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_roadlur);
                    break;
                case 11:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_roadrud);
                    break;
                case 12:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_roadldr);
                    break;
                case 13:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_roadlud);
                    break;
                case 14:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_roadc);
                    break;
                default:
                    blockrenderer = Instantiate(Main.main.blockrendererprefab_grass);
                    break;
            }
            blockrenderer.gameObject.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
            blockrenderer.init(block.id, x, y, updatable);
            return blockrenderer;
        }
    }
}
