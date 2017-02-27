using UnityEngine;
using org.rintech.blockcity.map;
using org.rintech.blockcity.gui;

namespace org.rintech.blockcity
{
    public class PlayerCamera : MonoBehaviour
    {
        public enum Tool//ツール。プレイ時には画面下から選択して使用
        {
            none, road, destroy
        }
        public static Color color_focus = Color.green;
        public static Color color_select = Color.yellow;
        public static Color color_plus = Color.green + Color.blue;
        public static Color color_remove = Color.red;
        public const float speed = 100;
        public const float fast = 300;
        public Block[][] provisionalblockmap = new Block[Map.mapsize][];//描画用のブロックのデータ
        private BlockRenderer[][] visibleblockmap = new BlockRenderer[Map.mapsize][];//ブロックレンダラーのデータ
        private Tool tool = Tool.none;
        private Camera playercamera;
        private int renderingchunk_x = int.MinValue;
        private int renderingchunk_y = int.MinValue;
        private int focus_x = int.MinValue;
        private int focus_y = int.MinValue;
        private int select_x = int.MinValue;
        private int select_y = int.MinValue;
        private int road_length = 0;
        private Direction road_direction = Direction.right;
        void Awake()
        {
            for (int a = 0; a < provisionalblockmap.Length; a++)
            {
                provisionalblockmap[a] = new Block[Map.mapsize];
            }
            for (int a = 0; a < visibleblockmap.Length; a++)
            {
                visibleblockmap[a] = new BlockRenderer[Map.mapsize];
            }
            BlockRenderer.initialize();
        }
        void Start()
        {
            transform.position = new Vector3(0, 6, 0);
            transform.eulerAngles = new Vector3(45, -45, 0);
            playercamera = gameObject.AddComponent<Camera>();
            playercamera.orthographic = true;
            playercamera.orthographicSize = 4;
            gameObject.AddComponent<GUILayer>();
            gameObject.AddComponent<AudioSource>();
            gameObject.AddComponent<FlareLayer>();
            Main.main.playercamera = this;
        }
        void Update()
        {
            if (Main.main.window == 30)
            {
                if (Input.GetKey(KeyCode.F5))
                {
                    BlockRenderer[] r = FindObjectsOfType<BlockRenderer>();
                    for (int a = 0; a < r.Length; a++)
                    {
                        r[a].destroy();
                    }
                    renderingchunk_x = int.MinValue;
                    renderingchunk_y = int.MinValue;
                }
                else
                {
                    if (GUIManager.buttonheight + GUIManager.buttonmargin * 2 < Input.mousePosition.y)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(playercamera.ScreenPointToRay(Input.mousePosition), out hit, Map.mapsize))
                        {
                            BlockRenderer blockrenderer = hit.transform.GetComponent<BlockRenderer>();
                            if (blockrenderer != null)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    if (blockrenderer.x == select_x && blockrenderer.y == select_y)
                                    {
                                        clearSelect();
                                    }
                                    else
                                    {
                                        setSelect(blockrenderer);
                                        clearFocus();
                                    }
                                }
                                if (blockrenderer.x != focus_x || blockrenderer.y != focus_y)
                                {
                                    setFocus(blockrenderer);
                                }
                                if (tool == Tool.destroy)
                                {
                                    if (Input.GetMouseButtonDown(0))
                                    {
                                        clearSelect();
                                        int id = MapManager.playingmap.blockmap[blockrenderer.x][blockrenderer.y].id;
                                        if (MapManager.playingmap.destroyBlock(blockrenderer.x, blockrenderer.y))
                                        {
                                            if (id == Block.tree.id || id == Block.forest.id)
                                            {
                                                MapManager.playingmap.money += Map.tree_sale;
                                            }
                                            else if (id == Block.stone.id)
                                            {
                                                MapManager.playingmap.money += Map.stone_sale;
                                            }
                                            SoundPlayer.play(new Vector3(blockrenderer.x + 0.5f, 0, blockrenderer.y + 0.5f), Main.main.audiovolume, AssetsManager.ac_bush);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                clearFocus();
                            }
                        }
                        else
                        {
                            clearFocus();
                        }
                    }
                    else
                    {
                        clearFocus();
                    }
                }
                if (tool == Tool.road && Input.GetMouseButtonUp(0))
                {
                    clearProvisionalBlocks();
                    if (0 < road_length)
                    {
                        MapManager.playingmap.roadConstruction(select_x, select_y, road_length, road_direction);
                    }
                    clearSelect();
                }
                Vector3 pos = Vector3.zero;
                if (Input.GetKey(KeyCode.A))
                {
                    pos += Vector3.left;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    pos += Vector3.right;
                }
                if (Input.GetKey(KeyCode.W))
                {
                    pos += Vector3.forward;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    pos += Vector3.back;
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    pos *= 5;
                }
                pos *= Mathf.Max(1, Mathf.Min(Map.mapsize / 10, transform.position.y * 2)) * Time.deltaTime;
                pos += transform.position;
                if (pos.x < -1)
                {
                    pos.x = -1;
                }
                else if (pos.x > Map.mapsize + 1)
                {
                    pos.x = Map.mapsize + 1;
                }
                if (pos.y < 1)
                {
                    pos.y = 1;
                }
                else if (pos.y > Map.mapsize)
                {
                    pos.y = Map.mapsize;
                }
                if (pos.z < -1)
                {
                    pos.z = -1;
                }
                else if (pos.z > Map.mapsize + 1)
                {
                    pos.z = Map.mapsize + 1;
                }
                transform.position = pos;
            }
            else if (tool == Tool.road)
            {
                clearProvisionalBlocks();
                clearSelect();
            }
            reloadBlockRendering();
        }
        public void OnGUI()
        {
            if (Main.main.window == 30)
            {
                if (focus_x != int.MinValue)
                {
                    GUIManager.label("座標 x: " + focus_x + " y: " + focus_y, GUIManager.Position.RightTop);
                }
                switch (tool)
                {
                    case Tool.road:
                        GUIManager.label("道路の長さ: " + road_length, GUIManager.Position.RightTop);
                        GUIManager.label("道路の向き: " + road_direction, GUIManager.Position.RightTop);
                        break;
                    default:
                        break;
                }
            }
        }
        public void clearProvisionalBlocks()
        {
            for (int a = 0; a < provisionalblockmap.Length; a++)
            {
                for (int b = 0; b < provisionalblockmap[a].Length; b++)
                {
                    provisionalblockmap[a][b] = null;
                }
            }
        }
        public void setTool(Tool tool)
        {
            this.tool = tool;
            clearSelect();
            clearProvisionalBlocks();
        }
        public Tool getTool()
        {
            return tool;
        }
        private void reloadColor(BlockRenderer blockrenderer)
        {
            reloadColor(blockrenderer, true);
        }
        private void reloadColor(BlockRenderer blockrenderer, bool respawn)
        {
            Color color = Color.black;
            if (provisionalblockmap[blockrenderer.x][blockrenderer.y] != null)
            {
                color += color_plus;
            }
            if (blockrenderer.x == focus_x && blockrenderer.y == focus_y)
            {
                color += color_focus;
            }
            if (blockrenderer.x == select_x && blockrenderer.y == select_y)
            {
                color += color_select;
            }
            if (color == Color.black)
            {
                if (respawn)
                {
                    blockrenderer.reload();
                }
            }
            else
            {
                setBlockColor(blockrenderer.gameObject, color);
            }
        }
        private void setBlockColor(GameObject obj, Color color)
        {
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
            Transform[] childs = obj.GetComponentsInChildren<Transform>();
            for (int a = 1; a < childs.Length; a++)
            {
                setBlockColor(childs[a].gameObject, color);
            }
        }
        private void setFocus(BlockRenderer blockrenderer)
        {
            int old_focus_x = focus_x;
            int old_focus_y = focus_y;
            focus_x = blockrenderer.x;
            focus_y = blockrenderer.y;
            if (tool == Tool.road)
            {
                if (select_y == blockrenderer.y)
                {
                    clearProvisionalBlocks();
                    road_length = 0;
                    if (blockrenderer.x == select_x)
                    {
                        if (Map.roadIsConstructible(MapManager.playingmap.blockmap[select_x][select_y].id)) {
                            road_length = 1;
                            if (road_direction == Direction.left || road_direction == Direction.right)
                            {
                                provisionalblockmap[blockrenderer.x][blockrenderer.y] = Block.road_x;
                            }
                            else
                            {
                                provisionalblockmap[blockrenderer.x][blockrenderer.y] = Block.road_y;
                            }
                            reloadColor(blockrenderer.reload());
                        }
                    }
                    else if (0 < blockrenderer.x - select_x)
                    {
                        for (int a = select_x; a <= blockrenderer.x; a++)
                        {
                            if (Map.roadIsConstructible(MapManager.playingmap.blockmap[a][select_y].id))
                            {
                                road_length++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        road_direction = Direction.right;
                        BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
                        for (int a = 0; a < blockrenderers.Length; a++)
                        {
                            if (select_y == blockrenderers[a].y && select_x <= blockrenderers[a].x && blockrenderers[a].x < select_x + road_length)
                            {
                                provisionalblockmap[blockrenderers[a].x][blockrenderers[a].y] = Block.road_x;
                                reloadColor(blockrenderers[a].reload());
                            }
                        }
                    }
                    else
                    {
                        for (int a = select_x; a >= blockrenderer.x; a--)
                        {
                            if (Map.roadIsConstructible(MapManager.playingmap.blockmap[a][select_y].id))
                            {
                                road_length++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        road_direction = Direction.left;
                        BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
                        for (int a = 0; a < blockrenderers.Length; a++)
                        {
                            if (select_y == blockrenderers[a].y && select_x >= blockrenderers[a].x && blockrenderers[a].x > select_x - road_length)
                            {
                                provisionalblockmap[blockrenderers[a].x][blockrenderers[a].y] = Block.road_x;
                                reloadColor(blockrenderers[a].reload());
                            }
                        }
                    }
                }
                else if (select_x == blockrenderer.x)
                {
                    clearProvisionalBlocks();
                    road_length = 0;
                    if (blockrenderer.y == select_y)
                    {
                        if (Map.roadIsConstructible(MapManager.playingmap.blockmap[select_x][select_y].id))
                        {
                            road_length = 1;
                            if (road_direction == Direction.left || road_direction == Direction.right)
                            {
                                provisionalblockmap[blockrenderer.x][blockrenderer.y] = Block.road_x;
                            }
                            reloadColor(blockrenderer.reload());
                        }
                    }
                    else if (0 < blockrenderer.y - select_y)
                    {
                        for (int a = select_y; a <= blockrenderer.y; a++)
                        {
                            if (Map.roadIsConstructible(MapManager.playingmap.blockmap[select_x][a].id))
                            {
                                road_length++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        road_direction = Direction.up;
                        BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
                        for (int a = 0; a < blockrenderers.Length; a++)
                        {
                            if (select_x == blockrenderers[a].x && select_y <= blockrenderers[a].y && blockrenderers[a].y < select_y + road_length)
                            {
                                provisionalblockmap[blockrenderers[a].x][blockrenderers[a].y] = Block.road_y;
                                reloadColor(blockrenderers[a].reload());
                            }
                        }
                    }
                    else
                    {
                        for (int a = select_y; a >= blockrenderer.y; a--)
                        {
                            if (Map.roadIsConstructible(MapManager.playingmap.blockmap[select_x][a].id))
                            {
                                road_length++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        road_direction = Direction.down;
                        BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
                        for (int a = 0; a < blockrenderers.Length; a++)
                        {
                            if (select_x == blockrenderers[a].x && select_y >= blockrenderers[a].y && blockrenderers[a].y > select_y - road_length)
                            {
                                provisionalblockmap[blockrenderers[a].x][blockrenderers[a].y] = Block.road_y;
                                reloadColor(blockrenderers[a].reload());
                            }
                        }
                    }
                }
            }
            if ((old_focus_x != int.MinValue || old_focus_y != int.MinValue) && BlockRenderer.blockrenderers[old_focus_x][old_focus_y] != null)
            {
                reloadColor(BlockRenderer.blockrenderers[old_focus_x][old_focus_y]);
            }
            reloadColor(blockrenderer);
        }
        private void clearFocus()
        {
            int old_focus_x = focus_x;
            int old_focus_y = focus_y;
            focus_x = int.MinValue;
            focus_y = int.MinValue;
            if ((old_focus_x != int.MinValue || old_focus_y != int.MinValue) && BlockRenderer.blockrenderers[old_focus_x][old_focus_y] != null)
            {
                reloadColor(BlockRenderer.blockrenderers[old_focus_x][old_focus_y]);
            }
        }
        private void setSelect(BlockRenderer blockrenderer)
        {
            int old_select_x = select_x;
            int old_select_y = select_y;
            select_x = blockrenderer.x;
            select_y = blockrenderer.y;
            if ((old_select_x != int.MinValue || old_select_y != int.MinValue) && BlockRenderer.blockrenderers[old_select_x][old_select_y] != null)
            {
                reloadColor(BlockRenderer.blockrenderers[old_select_x][old_select_y]);
            }
            reloadColor(blockrenderer);
        }
        private void clearSelect()
        {
            int old_select_x = select_x;
            int old_select_y = select_y;
            select_x = int.MinValue;
            select_y = int.MinValue;
            road_length = 0;
            if ((old_select_x != int.MinValue || old_select_y != int.MinValue) && BlockRenderer.blockrenderers[old_select_x][old_select_y] != null)
            {
                reloadColor(BlockRenderer.blockrenderers[old_select_x][old_select_y]);
            }
        }
        private void reloadBlockRendering()
        {
            BlockRenderer[] renderers = FindObjectsOfType<BlockRenderer>();
            int old_renderingchunk_x = renderingchunk_x;
            int old_renderingchunk_y = renderingchunk_y;
            renderingchunk_x = Mathf.FloorToInt(transform.position.x / BlockRenderer.chunksize);
            renderingchunk_y = Mathf.FloorToInt(transform.position.z / BlockRenderer.chunksize);
            if (old_renderingchunk_x != renderingchunk_x || old_renderingchunk_y != renderingchunk_y || renderers.Length == 0)
            {
                for (int a = 0; a < renderers.Length; a++)
                {
                    int chunkx = Mathf.FloorToInt((float) renderers[a].x / BlockRenderer.chunksize);
                    int chunky = Mathf.FloorToInt((float) renderers[a].y / BlockRenderer.chunksize);
                    if (renderingchunk_x - BlockRenderer.renderingchunk_radius > chunkx ||
                        renderingchunk_x + BlockRenderer.renderingchunk_radius < chunkx ||
                        renderingchunk_y - BlockRenderer.renderingchunk_radius > chunky ||
                        renderingchunk_y + BlockRenderer.renderingchunk_radius < chunky)
                    {
                        renderers[a].destroy();
                    }
                }
                for (int cx = renderingchunk_x - BlockRenderer.renderingchunk_radius; cx <= renderingchunk_x + BlockRenderer.renderingchunk_radius; cx++)
                {
                    for (int cy = renderingchunk_y - BlockRenderer.renderingchunk_radius; cy <= renderingchunk_y + BlockRenderer.renderingchunk_radius; cy++)
                    {
                        if (((old_renderingchunk_x - BlockRenderer.renderingchunk_radius > cx || old_renderingchunk_x + BlockRenderer.renderingchunk_radius < cx) || (old_renderingchunk_y - BlockRenderer.renderingchunk_radius > cy || old_renderingchunk_y + BlockRenderer.renderingchunk_radius < cy)) &&
                            0 <= cx * BlockRenderer.chunksize && Map.mapsize > cx * BlockRenderer.chunksize && 0 <= cy * BlockRenderer.chunksize && Map.mapsize > cy * BlockRenderer.chunksize)
                        {
                            for (int x = 0; x < BlockRenderer.chunksize; x++)
                            {
                                for (int y = 0; y < BlockRenderer.chunksize; y++)
                                {
                                    BlockRenderer blockrenderer = BlockRenderer.spawnBlockRenderer(MapManager.playingmap.blockmap[cx * BlockRenderer.chunksize + x][cy * BlockRenderer.chunksize + y], cx * BlockRenderer.chunksize + x, cy * BlockRenderer.chunksize + y, true);
                                    reloadColor(blockrenderer, false);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
