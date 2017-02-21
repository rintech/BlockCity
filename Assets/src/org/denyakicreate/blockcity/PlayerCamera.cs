using UnityEngine;
using org.denyakicreate.blockcity.map;
using org.denyakicreate.blockcity.gui;

namespace org.denyakicreate.blockcity
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
        public static float cameraspeed = 100;//カメラの移動速度（後で設定から変更可能にする予定）
        public Block[][] provisionalblockmap = new Block[Map.mapsize][];//描画用のブロックのデータ
        private BlockRenderer[][] visibleblockmap = new BlockRenderer[Map.mapsize][];//ブロックレンダラーのデータ
        public Tool tool = Tool.none;
        private Camera playercamera;
        private int renderingchunk_x = int.MinValue;
        private int renderingchunk_y = int.MinValue;
        private int focus_x = int.MinValue;
        private int focus_y = int.MinValue;
        private int select_x = int.MinValue;
        private int select_y = int.MinValue;
        private int road_length = 0;
        private Map.Direction road_direction = Map.Direction.right;
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
                        Destroy(r[a].gameObject);
                    }
                    renderingchunk_x = int.MinValue;
                    renderingchunk_y = int.MinValue;
                }
                if (tool != Tool.road && select_x != int.MinValue)
                {
                    clearSelect();
                }

                //TODO 状態が変更されたブロックを削除

                if (GUIManager.buttonheight + GUIManager.buttonmargin * 2 < Input.mousePosition.y)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(playercamera.ScreenPointToRay(Input.mousePosition), out hit, Map.mapsize))
                    {
                        BlockRenderer blockrenderer = hit.transform.GetComponent<BlockRenderer>();
                        if (blockrenderer != null)
                        {
                            if (blockrenderer.x != focus_x || blockrenderer.y != focus_y)
                            {
                                setFocus(blockrenderer.x, blockrenderer.y);
                            }
                            switch (tool)
                            {
                                case Tool.road:
                                    if (Input.GetMouseButtonDown(0) && MapManager.playingmap.blockmap[blockrenderer.x + Map.mapsize / 2][blockrenderer.y + Map.mapsize / 2].id == Block.grass.id)
                                    {
                                        //TODO 影響するブロックの確認



                                        if (select_x == int.MinValue)
                                        {
                                            setSelect(blockrenderer.x, blockrenderer.y);
                                        }
                                        else
                                        {
                                            if (blockrenderer.x == select_x && blockrenderer.y == select_y)
                                            {
                                                clearSelect();
                                            }
                                            else
                                            {
                                                if (select_y == blockrenderer.y)
                                                {
                                                    if (0 < blockrenderer.x - select_x)
                                                    {
                                                        road_length = blockrenderer.x - select_x;
                                                        road_direction = Map.Direction.right;
                                                        BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
                                                        for (int a = 0; a < blockrenderers.Length; a++)
                                                        {
                                                            if (select_y == blockrenderers[a].y && select_x <= blockrenderers[a].x && blockrenderers[a].x <= blockrenderer.x)
                                                            {
                                                                Destroy(blockrenderers[a].gameObject);
                                                                Color c = color_plus;
                                                                if (select_x == blockrenderers[a].x && select_y == blockrenderers[a].y)
                                                                {
                                                                    c += color_select;
                                                                } else if (focus_x == blockrenderers[a].x && focus_y == blockrenderers[a].y)
                                                                {
                                                                    c += color_focus;
                                                                }
                                                                setBlockColor(BlockRenderer.spawnBlockRenderer(Block.road_x, blockrenderers[a].x, blockrenderers[a].y, false).gameObject, c);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        road_length = select_x - blockrenderer.x;
                                                        road_direction = Map.Direction.left;
                                                        BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
                                                        for (int a = 0; a < blockrenderers.Length; a++)
                                                        {
                                                            if (select_y == blockrenderers[a].y && select_x >= blockrenderers[a].x && blockrenderers[a].x >= blockrenderer.x)
                                                            {
                                                                Destroy(blockrenderers[a].gameObject);
                                                                Color c = color_plus;
                                                                if (select_x == blockrenderers[a].x && select_y == blockrenderers[a].y)
                                                                {
                                                                    c += color_select;
                                                                }
                                                                else if (focus_x == blockrenderers[a].x && focus_y == blockrenderers[a].y)
                                                                {
                                                                    c += color_focus;
                                                                }
                                                                setBlockColor(BlockRenderer.spawnBlockRenderer(Block.road_x, blockrenderers[a].x, blockrenderers[a].y, false).gameObject, c);
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (select_x == blockrenderer.x)
                                                {
                                                    if (0 < blockrenderer.y - select_y)
                                                    {
                                                        road_length = blockrenderer.y - select_y;
                                                        road_direction = Map.Direction.up;
                                                        BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
                                                        for (int a = 0; a < blockrenderers.Length; a++)
                                                        {
                                                            if (select_x == blockrenderers[a].x && select_y <= blockrenderers[a].y && blockrenderers[a].y <= blockrenderer.y)
                                                            {
                                                                Destroy(blockrenderers[a].gameObject);
                                                                Color c = color_plus;
                                                                if (select_x == blockrenderers[a].x && select_y == blockrenderers[a].y)
                                                                {
                                                                    c += color_select;
                                                                }
                                                                else if (focus_x == blockrenderers[a].x && focus_y == blockrenderers[a].y)
                                                                {
                                                                    c += color_focus;
                                                                }
                                                                setBlockColor(BlockRenderer.spawnBlockRenderer(Block.road_y, blockrenderers[a].x, blockrenderers[a].y, false).gameObject, c);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        road_length = select_y - blockrenderer.y;
                                                        road_direction = Map.Direction.down;
                                                        BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
                                                        for (int a = 0; a < blockrenderers.Length; a++)
                                                        {
                                                            if (select_x == blockrenderers[a].x && select_y >= blockrenderers[a].y && blockrenderers[a].y >= blockrenderer.y)
                                                            {
                                                                Destroy(blockrenderers[a].gameObject);
                                                                Color c = color_plus;
                                                                if (select_x == blockrenderers[a].x && select_y == blockrenderers[a].y)
                                                                {
                                                                    c += color_select;
                                                                }
                                                                else if (focus_x == blockrenderers[a].x && focus_y == blockrenderers[a].y)
                                                                {
                                                                    c += color_focus;
                                                                }
                                                                setBlockColor(BlockRenderer.spawnBlockRenderer(Block.road_y, blockrenderers[a].x, blockrenderers[a].y, false).gameObject, c);
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    setSelect(blockrenderer.x, blockrenderer.y);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case Tool.destroy:
                                    if (Input.GetMouseButtonDown(0))
                                    {
                                        if (blockrenderer.id == Block.tree.id || blockrenderer.id == Block.forest.id)
                                        {
                                            MapManager.playingmap.blockmap[Map.mapsize / 2 + blockrenderer.x][Map.mapsize / 2 + blockrenderer.y] = Block.grass;
                                            MapManager.playingmap.money += Map.tree_sale;
                                            SoundPlayer.play(new Vector3(blockrenderer.x + 0.5f, 0, blockrenderer.y + 0.5f), Main.main.audiovolume, AssetsManager.ac_bush);
                                        }
                                        else if (blockrenderer.id == Block.stone.id)
                                        {
                                            MapManager.playingmap.blockmap[Map.mapsize / 2 + blockrenderer.x][Map.mapsize / 2 + blockrenderer.y] = Block.grass;
                                            MapManager.playingmap.money += Map.stone_sale;
                                            SoundPlayer.play(new Vector3(blockrenderer.x + 0.5f, 0, blockrenderer.y + 0.5f), Main.main.audiovolume, AssetsManager.ac_bush);
                                        }
                                    }
                                    break;
                                default:
                                    break;
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
                Vector3 move = Vector3.zero;
                if (Input.GetKey(KeyCode.A))
                {
                    move += Vector3.left;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    move += Vector3.right;
                }
                if (Input.GetKey(KeyCode.W))
                {
                    move += Vector3.forward;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    move += Vector3.back;
                }
                Vector3 pos = move;
                pos *= Mathf.Max(1, Mathf.Min(Map.mapsize / 10, transform.position.y * 2)) * Time.deltaTime;
                pos += transform.position;
                if (pos.x < -Map.mapsize / 2 - 1)
                {
                    pos.x = -Map.mapsize / 2 - 1;
                }
                else if (pos.x > Map.mapsize / 2 + 1)
                {
                    pos.x = Map.mapsize / 2 + 1;
                }
                if (pos.y < 1)
                {
                    pos.y = 1;
                }
                else if (pos.y > Map.mapsize)
                {
                    pos.y = Map.mapsize;
                }
                if (pos.z < -Map.mapsize / 2 - 1)
                {
                    pos.z = -Map.mapsize / 2 - 1;
                }
                else if (pos.z > Map.mapsize / 2 + 1)
                {
                    pos.z = Map.mapsize / 2 + 1;
                }
                transform.position = pos;
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
        private void reloadColor()
        {

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
        private void removeFocusColor()
        {
            BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
            for (int a = 0; a < blockrenderers.Length; a++)
            {
                if (blockrenderers[a].x == focus_x && blockrenderers[a].y == focus_y)
                {
                    if (select_x == focus_x && select_y == focus_y)
                    {
                        setBlockColor(blockrenderers[a].gameObject, color_select);
                    }
                    else
                    {
                        Destroy(blockrenderers[a].gameObject);
                        BlockRenderer.spawnBlockRenderer(Block.getNewBlock(blockrenderers[a].id), blockrenderers[a].x, blockrenderers[a].y, false);
                    }
                }
            }
        }
        private void setFocus(int x, int y)
        {
            if (focus_x != x || focus_y != y)
            {
                removeFocusColor();
                focus_x = x;
                focus_y = y;
                BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
                for (int a = 0; a < blockrenderers.Length; a++)
                {
                    if (blockrenderers[a].x == focus_x && blockrenderers[a].y == focus_y)
                    {
                        if (select_x == focus_x && select_y == focus_y)
                        {
                            setBlockColor(blockrenderers[a].gameObject, color_focus + color_select);
                        }
                        else
                        {
                            setBlockColor(blockrenderers[a].gameObject, color_focus);
                        }
                    }
                }
            }
        }
        private void clearFocus()
        {
            if (focus_x != int.MinValue || focus_y != int.MinValue)
            {
                removeFocusColor();
                focus_x = int.MinValue;
                focus_y = int.MinValue;
            }
        }
        private void removeSelectColor()
        {
            BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
            for (int a = 0; a < blockrenderers.Length; a++)
            {
                if (blockrenderers[a].x == select_x && blockrenderers[a].y == select_y)
                {
                    if (select_x == focus_x && select_y == focus_y)
                    {
                        setBlockColor(blockrenderers[a].gameObject, color_focus);
                    }
                    else
                    {
                        bool b = true;
                        if (road_length != 0)
                        {
                            switch (road_direction)
                            {
                                case Map.Direction.right:
                                    if (select_y == blockrenderers[a].y && select_x <= blockrenderers[a].x && select_x + road_length >= blockrenderers[a].x)
                                    {
                                        
                                    }
                                    break;
                                case Map.Direction.left:
                                    if (select_y == blockrenderers[a].y && select_x >= blockrenderers[a].x && select_x + road_length <= blockrenderers[a].x)
                                    {

                                    }
                                    break;
                                case Map.Direction.up:
                                    if (select_x == blockrenderers[a].x && select_y <= blockrenderers[a].y && select_y + road_length >= blockrenderers[a].y)
                                    {

                                    }
                                    break;
                                case Map.Direction.down:
                                    if (select_x == blockrenderers[a].x && select_y >= blockrenderers[a].y && select_y + road_length <= blockrenderers[a].y)
                                    {

                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (b)
                        {
                            Destroy(blockrenderers[a].gameObject);
                            BlockRenderer.spawnBlockRenderer(Block.getNewBlock(blockrenderers[a].id), blockrenderers[a].x, blockrenderers[a].y, false);
                        }
                    }
                }
            }
        }
        private void setSelect(int x, int y)
        {
            if (select_x != x || select_y != y)
            {
                removeSelectColor();
                select_x = x;
                select_y = y;
                BlockRenderer[] blockrenderers = FindObjectsOfType<BlockRenderer>();
                for (int a = 0; a < blockrenderers.Length; a++)
                {
                    if (blockrenderers[a].x == select_x && blockrenderers[a].y == select_y)
                    {
                        if (select_x == focus_x && select_y == focus_y)
                        {
                            setBlockColor(blockrenderers[a].gameObject, color_focus + color_select);
                        }
                        else
                        {
                            setBlockColor(blockrenderers[a].gameObject, color_select);
                        }
                    }
                }
            }
        }
        private void clearSelect()
        {
            if (select_x != int.MinValue || select_y != int.MinValue)
            {
                removeSelectColor();
                select_x = int.MinValue;
                select_y = int.MinValue;
                road_length = 0;
                road_direction = Map.Direction.right;
            }
        }
        private void setRoad()
        {

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
                        Destroy(renderers[a].gameObject);
                    }
                }
                for (int cx = renderingchunk_x - BlockRenderer.renderingchunk_radius; cx <= renderingchunk_x + BlockRenderer.renderingchunk_radius; cx++)
                {
                    for (int cy = renderingchunk_y - BlockRenderer.renderingchunk_radius; cy <= renderingchunk_y + BlockRenderer.renderingchunk_radius; cy++)
                    {
                        if (((old_renderingchunk_x - BlockRenderer.renderingchunk_radius > cx || old_renderingchunk_x + BlockRenderer.renderingchunk_radius < cx) || (old_renderingchunk_y - BlockRenderer.renderingchunk_radius > cy || old_renderingchunk_y + BlockRenderer.renderingchunk_radius < cy)) &&
                            -Map.mapsize / 2 <= cx * BlockRenderer.chunksize && Map.mapsize / 2 > cx * BlockRenderer.chunksize && -Map.mapsize / 2 <= cy * BlockRenderer.chunksize && Map.mapsize / 2 > cy * BlockRenderer.chunksize)
                        {
                            for (int x = 0; x < BlockRenderer.chunksize; x++)
                            {
                                for (int y = 0; y < BlockRenderer.chunksize; y++)
                                {
                                    BlockRenderer blockrenderer = BlockRenderer.spawnBlockRenderer(MapManager.playingmap.blockmap[cx * BlockRenderer.chunksize + x + Map.mapsize / 2][cy * BlockRenderer.chunksize + y + Map.mapsize / 2], cx * BlockRenderer.chunksize + x, cy * BlockRenderer.chunksize + y, true);
                                    if (blockrenderer.x == focus_x && blockrenderer.y == focus_y)
                                    {
                                        if (select_x == focus_x && select_y == focus_y)
                                        {
                                            setBlockColor(blockrenderer.gameObject, color_focus + color_select);
                                        }
                                        else
                                        {
                                            setBlockColor(blockrenderer.gameObject, color_focus);
                                        }
                                    }
                                    else if (blockrenderer.x == select_x && blockrenderer.y == select_y)
                                    {
                                        if (select_x == focus_x && select_y == focus_y)
                                        {
                                            setBlockColor(blockrenderer.gameObject, color_focus + color_select);
                                        }
                                        else
                                        {
                                            setBlockColor(blockrenderer.gameObject, color_select);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
