using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using org.rintech.blockcity.gui;
using org.rintech.blockcity.map;
using System.Collections;

namespace org.rintech.blockcity
{
    public class Main : MonoBehaviour
    {
        public const string ver = "0.002alpha";
        public const string welcomemsg = "BlockCityの世界へようこそ。詳しい情報は公式サイトをご覧下さい";

        public static Main main;

        private static AudioSource audiosource;

        private static string[] tips = new string[] { "メニューはEscキーで戻ることが出来ます", "ポーズメニューはEscキーで開きます", "F2でスクリーンショットを撮ることが出来ます", "保存したスクリーンショットは設定から見ることが出来ます", "ブロックを撤去するには撤去ツールを選択し撤去したいブロックをクリックします" };
        private static int tipsindex = 0;
        private static float tipstime = 0;

        public BlockRenderer blockrendererprefab_grass;
        public BlockRenderer blockrendererprefab_tree;
        public BlockRenderer blockrendererprefab_forest;
        public BlockRenderer blockrendererprefab_stone;
        public BlockRenderer blockrendererprefab_roadx;
        public BlockRenderer blockrendererprefab_roady;
        public BlockRenderer blockrendererprefab_roadlu;
        public BlockRenderer blockrendererprefab_roadru;
        public BlockRenderer blockrendererprefab_roadrd;
        public BlockRenderer blockrendererprefab_roadld;
        public BlockRenderer blockrendererprefab_roadlur;
        public BlockRenderer blockrendererprefab_roadrud;
        public BlockRenderer blockrendererprefab_roadldr;
        public BlockRenderer blockrendererprefab_roadlud;
        public BlockRenderer blockrendererprefab_roadc;
        public BlockRenderer blockrendererprefab_dirt;

        public string ssdirpath;
        public byte window = 0;
        public float audiovolume = 1;
        public Camera menucamera;
        public Texture background;
        public Texture darkbg;
        public Texture whitebg;
        public Texture yellowbg;
        public Texture redbg;
        public Texture logo_big;
        public Texture center;
        public Font font;
        public AudioClip ac_click;
        public AudioClip ac_msg;
        public AudioClip ac_bush;
        public PlayerCamera playercamera;

        /*private bool loading = false;
        private bool loaded = false;*/
        private bool firststart = false;
        private int mapselect = 0;
        private string mapnameenter = "";
        private bool statusinfo = false;
        private string username = "";
        private float lastmsgtime = 0;
        private byte window_1 = 1;
        private AudioSource menusource;
        void Awake()
        {
            main = this;
            ssdirpath = Path.Combine(Application.persistentDataPath, "screenshots");
            tipsindex = UnityEngine.Random.Range(0, tips.Length);
        }
        void Start()
        {
            AssetsManager.background = background;
            AssetsManager.darkbg = darkbg;
            AssetsManager.whitebg = whitebg;
            AssetsManager.yellowbg = yellowbg;
            AssetsManager.redbg = redbg;
            AssetsManager.logo_big = logo_big;
            AssetsManager.center = center;
            AssetsManager.font = font;
            AssetsManager.ac_click = ac_click;
            AssetsManager.ac_msg = ac_msg;
            AssetsManager.ac_bush = ac_bush;

            string str = PlayerPrefs.GetString("time-firstplay", null);
            if (str == null || str.Length == 0)
            {
                firststart = true;
                PlayerPrefs.SetString("time-firstplay", (TimeManager.firstplay = DateTime.Now.Ticks).ToString());
            }
            else
            {
                TimeManager.firstplay = long.Parse(str);
            }

            str = PlayerPrefs.GetString("time-totalplaytime", null);
            if (str != null && str.Length != 0)
            {
                TimeManager.totalplaytime = long.Parse(str);
            }
            username = PlayerPrefs.GetString("username", "");

            GUIManager.init();
            MapManager.init();

            GameObject obj = new GameObject("MenuSource");
            (menusource = obj.AddComponent<AudioSource>()).volume = (audiovolume = PlayerPrefs.GetFloat("volume", 1));
            obj.AddComponent<AudioListener>();
        }
        void Update()
        {
            PlayerPrefs.SetString("time-totalplaytime", (TimeManager.totalplaytime += (long)(Time.deltaTime * 1000 * 10000)).ToString());
            if (MapManager.playingmap != null)
            {
                MapManager.playingmap.playtime += (long)(Time.deltaTime * 1000 * 10000);
                if (window == 30)
                {
                    MapManager.playingmap.time += (long)(Time.deltaTime * 1000 * 10000);
                }
            }
            if (Input.GetKeyDown(KeyCode.F1))
            {
                statusinfo = !statusinfo;
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                Directory.CreateDirectory(ssdirpath);
                ScreenCapture.CaptureScreenshot(Path.Combine(ssdirpath, DateTime.Now.Ticks.ToString()) + ".png");
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (window == 1)
                {
                    Application.Quit();
                }
                else if (window == 2)
                {
                    if (MapManager.playingmap != null)
                    {
                        window = 31;
                    }
                    else
                    {
                        window = 1;
                    }
                }
                else if (window == 3)
                {
                    window = 1;
                }
                else if (window == 10)
                {
                    window = 1;
                }
                else if (window == 11)
                {
                    window = window_1;
                }
                else if (window == 30)
                {
                    window = 31;
                }
                else if (window == 31)
                {
                    window = 30;
                }
                else if (window == 32)
                {
                    window = 31;
                }
                else if (window == 33)
                {
                    window = 1;
                }
            }
            if (menucamera.enabled = MapManager.playingmap == null)
            {
                playercamera = null;
                audiosource = menusource;
                menusource.GetComponent<AudioListener>().enabled = true;
                menusource.enabled = true;
            }
            else if (playercamera != null)
            {
                menusource.GetComponent<AudioListener>().enabled = false;
                menusource.enabled = false;
                (audiosource = playercamera.GetComponent<AudioSource>()).volume = audiovolume;
                if (playercamera.GetComponent<AudioListener>() == null)
                {
                    playercamera.gameObject.AddComponent<AudioListener>();
                }
                if (window == 30)
                {
                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        playSound(ac_click);
                        playercamera.setTool(playercamera.getTool() == PlayerCamera.Tool.destroy ? PlayerCamera.Tool.none : PlayerCamera.Tool.destroy);
                    }
                    else if (Input.GetKeyDown(KeyCode.R))
                    {
                        playSound(ac_click);
                        playercamera.setTool(playercamera.getTool() == PlayerCamera.Tool.road ? PlayerCamera.Tool.none : PlayerCamera.Tool.road);
                    }
                }
            }
        }
        void OnGUI()
        {
            GUIManager.reset();
            /*
            Window:
            0: 初期状態
		    1: タイトル
		    2: タイトル>設定
            3: タイトル>設定>クレジット

		    10: タイトル>ゲームを始める
            11: マップ生成
            19: タイトル>ゲームを始める>マップロード中...
            
            30: ゲーム中
            31: ポーズメニュー
            32: ポーズメニュー>タイトルへ戻る
            33: ポーズメニュー>タイトルへ戻る>最後に一言
            */
            Cursor.visible = window != 30;
            if (playercamera == null)
            {
                GUIManager.background();
            }
            switch (window)
            {
                case 0:
                    window = 1;
                    break;
                case 1:
                    GUIManager.dark();
                    GUIManager.titlebardark();
                    GUIManager.logo_big();
                    GUIManager.label("ver: " + ver, GUIManager.Position.LeftBottom);
                    if (statusinfo)
                    {
                        GUIManager.label("初回起動: " + firststart, GUIManager.Position.LeftBottom);
                        GUIManager.label("初回起動時刻: " + TimeManager.convpoint(new DateTime(TimeManager.firstplay), TimeManager.ConvPointType.day), GUIManager.Position.LeftBottom);
                        GUIManager.label("起動時間: " + TimeManager.convinterval(new TimeSpan((long)(Time.fixedTime * 1000 * 1000 * 10))), GUIManager.Position.LeftBottom);
                        GUIManager.label("総計起動時間: " + TimeManager.convinterval(new TimeSpan(TimeManager.totalplaytime)), GUIManager.Position.LeftBottom);
                    }
                    if (firststart)
                    {
                        GUIManager.label(welcomemsg, GUIManager.Position.TitleBar);
                    }
                    else
                    {
                        GUIManager.label("tips: " + gettips(), GUIManager.Position.TitleBar);
                    }
                    MenuTextField _0username = new MenuTextField("プレイヤー名(2~16文字)", "プレイヤー名を入力して下さい", username);
                    MenuItem[] _1menu = new MenuItem[] { _0username, new MenuButton("ゲームを始める"), new MenuButton("公式サイトへ"), new MenuItem(new MenuItem[] { new MenuButton("設定"), new MenuButton("ゲームを終了") }) };
                    GUIManager.menu_center(_1menu);
                    PlayerPrefs.SetString("username", username = _0username.enter);
                    if (_1menu[1].clicked)
                    {
                        playSound(ac_click);
                        if (username.Length < 2 || 16 < username.Length)
                        {
                            GUIManager.setmsg("プレイヤー名を正しく入力して下さい");
                        }
                        else
                        {
                            window = 10;
                            window_1 = 10;
                        }
                    }
                    else if (_1menu[2].clicked)
                    {
                        playSound(ac_click);
                        Application.OpenURL("https://denyakicreate.github.io/blockcity/");
                    }
                    else if (_1menu[3].initems[0].clicked)
                    {
                        playSound(ac_click);
                        window = 2;
                    }
                    else if (_1menu[3].initems[1].clicked)
                    {
                        playSound(ac_click);
                        Application.Quit();
                    }
                    break;
                case 2:
                    GUIManager.dark();
                    GUIManager.titlebardark();
                    GUIManager.label("設定", GUIManager.Position.TitleBar);
                    MenuSlider _2audiovolume = new MenuSlider("音量", 0, 1, audiovolume);
                    MenuItem[] _2menu = new MenuItem[] { _2audiovolume, new MenuItem(new MenuItem[] { new MenuButton("キャンセル"), new MenuButton("音声再生"), new MenuButton("決定") }), new MenuButton("スクリーンショットフォルダを開く"), new MenuButton("クレジット") };
                    GUIManager.menu_center(_2menu);
                    PlayerPrefs.SetFloat("volume", audiosource.volume = (menusource.volume = (audiovolume = _2audiovolume.value)));
                    if (_2menu[1].initems[0].clicked)
                    {
                        playSound(ac_click);
                        if (MapManager.playingmap != null)
                        {
                            window = 31;
                        }
                        else
                        {
                            window = 1;
                        }
                    }
                    else if (_2menu[1].initems[1].clicked)
                    {
                        switch (UnityEngine.Random.Range(0, 2))
                        {
                            case 1:
                                playSound(ac_msg);
                                break;
                            default:
                                playSound(ac_click);
                                break;
                        }
                    }
                    else if (_2menu[1].initems[2].clicked)
                    {
                        playSound(ac_click);
                        if (MapManager.playingmap != null)
                        {
                            window = 31;
                        }
                        else
                        {
                            window = 1;
                        }
                    }
                    else if (_2menu[2].clicked)
                    {
                        playSound(ac_click);
                        Process.Start(ssdirpath);
                    }
                    else if (_2menu[3].clicked)
                    {
                        playSound(ac_click);
                        window = 3;
                    }
                    break;
                case 3:
                    GUIManager.dark();
                    GUIManager.titlebardark();
                    GUIManager.label("設定>クレジット", GUIManager.Position.TitleBar);
                    MenuItem[] _3menu = new MenuItem[] { new MenuButton("thank you for playing!") };
                    GUIManager.menu_center(new MenuItem[] { new MenuItem("Block Cityはまちづくりサンドボックスゲームです。"), new MenuItem(""), new MenuItem("プログラマー: "), new MenuItem("電車君"), new MenuItem(""), new MenuItem("画像作成: "), new MenuItem("電車君"), new MenuItem("yakiniki"), new MenuItem(""), new MenuItem("フォント: "), new MenuItem("M+ FONTS"), new MenuItem("") });
                    GUIManager.menu_bottom(_3menu);
                    GUIManager.label("フリー効果音素材 くらげ工匠", GUIManager.Position.RightBottom);
                    GUIManager.label("©効果音ラボ", GUIManager.Position.RightBottom);
                    GUIManager.label("音楽・効果音: ", GUIManager.Position.RightBottom);
                    if (_3menu[0].clicked)
                    {
                        playSound(ac_click);
                        window = 1;
                    }
                    break;
                case 10:
                    GUIManager.dark();
                    GUIManager.titlebardark();
                    GUIManager.label("ゲームを始める", GUIManager.Position.TitleBar);
                    MenuList _10mapselect = new MenuList("マップ選択", MapManager.getMapList(), mapselect);
                    MenuItem[] _10menu = new MenuItem[] { _10mapselect, new MenuItem(new MenuItem[] { new MenuButton("戻る"), new MenuButton("マップ生成") }), new MenuItem(new MenuItem[] { new MenuButton("フォルダを開く"), new MenuButton("始める") }) };
                    GUIManager.menu_center(_10menu);
                    mapselect = _10mapselect.select;
                    if (_10menu[1].initems[0].clicked)
                    {
                        playSound(ac_click);
                        window = 1;
                    }
                    else if (_10menu[1].initems[1].clicked)
                    {
                        playSound(ac_click);
                        MapManager.RandomMapNameChange();
                        mapnameenter = MapManager.RandomMapName();
                        window = 11;
                    }
                    else if (_10menu[2].initems[0].clicked)
                    {
                        playSound(ac_click);
                        Process.Start(MapManager.dir);
                    }
                    else if (_10menu[2].initems[1].clicked)
                    {
                        playSound(ac_click);
                        window = 19;
                    }
                    break;
                case 11:
                    GUIManager.dark();
                    GUIManager.titlebardark();
                    GUIManager.label("マップ生成", GUIManager.Position.TitleBar);
                    MenuTextField _11mapname = new MenuTextField("マップ名", "", mapnameenter);
                    MenuItem[] _11menu = new MenuItem[] { _11mapname, new MenuItem(new MenuItem[] { new MenuButton("戻る"), new MenuButton("生成") }) };
                    GUIManager.menu_center(_11menu);
                    mapnameenter = _11mapname.enter;
                    if (_11menu[1].initems[0].clicked)
                    {
                        playSound(ac_click);
                        window = window_1;
                    }
                    else if (_11menu[1].initems[1].clicked)
                    {
                        playSound(ac_click);
                        if (MapManager.createNewMap(mapnameenter))
                        {
                            GUIManager.setmsg("マップ\"" + mapnameenter + "\"を生成しました");
                            window = window_1;
                        }
                        else
                        {
                            GUIManager.setmsg("マップ\"" + mapnameenter + "\"はすでに存在します", GUIManager.MessageType.CAUTION);
                        }
                    }
                    break;
                case 19:
                    GUIManager.dark();
                    GUIManager.titlebardark();
                    GUIManager.label("マップロード中...", GUIManager.Position.TitleBar);
                    GUIManager.menu_center(new string[] { gettips() });
                    if (MapManager.getMapList().Length <= mapselect)
                    {
                        GUIManager.setmsg("マップがありません");
                        window = 10;
                    }
                    else
                    {
                        MapManager.openMap(MapManager.getMapList()[mapselect]);
                        playercamera = new GameObject("PlayerCamera").AddComponent<PlayerCamera>();
                        window = 30;
                    }
                    break;
                case 30:
                    GUIManager.titlebardark();
                    GUIManager.label("所持金: " + MapManager.getMoneyString(MapManager.playingmap.money), GUIManager.Position.TitleBar);
                    GUIManager.label("マップの時間: " + TimeManager.convpoint(new DateTime(MapManager.playingmap.time), TimeManager.ConvPointType.sec), GUIManager.Position.RightTop);
                    MenuItem[] _30menu = new MenuItem[] { new MenuItem(new MenuItem[] { new MenuButton("<color=" + (playercamera.getTool() == PlayerCamera.Tool.road ? "yellow" : "white") + ">道路</color>"), new MenuButton("<color=" + (playercamera.getTool() == PlayerCamera.Tool.destroy ? "yellow" : "white") + ">撤去</color>") }) };
                    GUIManager.menu_bottom(_30menu);
                    if (_30menu[0].initems[0].clicked)
                    {
                        playSound(ac_click);
                        playercamera.setTool(playercamera.getTool() == PlayerCamera.Tool.road ? PlayerCamera.Tool.none : PlayerCamera.Tool.road);
                    }
                    else if (_30menu[0].initems[1].clicked)
                    {
                        playSound(ac_click);
                        playercamera.setTool(playercamera.getTool() == PlayerCamera.Tool.destroy ? PlayerCamera.Tool.none : PlayerCamera.Tool.destroy);
                    }
                    GUI.DrawTexture(new Rect(Input.mousePosition.x - AssetsManager.center.width / 2, Screen.height - Input.mousePosition.y - AssetsManager.center.height / 2, AssetsManager.center.width, AssetsManager.center.height), AssetsManager.center);
                    break;
                case 31:
                    GUIManager.dark();
                    GUIManager.titlebardark();
                    GUIManager.label("tips: " + gettips(), GUIManager.Position.LeftBottom);
                    if (MapManager.playingmap != null)
                    {
                        GUIManager.label("マップの時間: " + TimeManager.convpoint(new DateTime(MapManager.playingmap.time), TimeManager.ConvPointType.sec), GUIManager.Position.RightTop);
                        GUIManager.label("所持金: " + MapManager.getMoneyString(MapManager.playingmap.money), GUIManager.Position.RightTop);
                        GUIManager.label("マップの開始時間: " + TimeManager.convpoint(new DateTime(MapManager.playingmap.starttime), TimeManager.ConvPointType.sec), GUIManager.Position.RightBottom);
                        GUIManager.label("マップのプレイ時間: " + TimeManager.convinterval(new TimeSpan(MapManager.playingmap.playtime)), GUIManager.Position.RightBottom);
                    }
                    GUIManager.label("ポーズメニュー", GUIManager.Position.TitleBar);
                    MenuItem[] _31menu = new MenuItem[] { new MenuButton("設定"), new MenuButton("保存"), new MenuButton("タイトルへ戻る") };
                    GUIManager.menu_center(_31menu);
                    if (_31menu[0].clicked)
                    {
                        playSound(ac_click);
                        window = 2;
                    }
                    else if (_31menu[1].clicked)
                    {
                        playSound(ac_click);
                        MapManager.saveMap(MapManager.playingmap);
                        window = 30;
                        GUIManager.setmsg("セーブしました");
                    }
                    else if (_31menu[2].clicked)
                    {
                        playSound(ac_click);
                        window = 32;
                    }
                    break;
                case 32:
                    GUIManager.dark();
                    GUIManager.titlebardark();
                    GUIManager.label("ポーズメニュー>タイトルへ戻る", GUIManager.Position.TitleBar);
                    MenuItem[] _33menu = new MenuItem[] { new MenuItem("セーブされてない情報は削除されます"), new MenuItem(new MenuItem[] { new MenuButton("はい"), new MenuButton("いいえ") }) };
                    GUIManager.menu_center(_33menu);
                    if (_33menu[1].initems[0].clicked)
                    {
                        playSound(ac_click);
                        Destroy(playercamera.gameObject);
                        MapManager.closeMap();
                        lastmsgtime = 3;
                        window = 33;
                    }
                    else if (_33menu[1].initems[1].clicked)
                    {
                        playSound(ac_click);
                        window = 31;
                    }
                    break;
                case 33:
                    GUIManager.dark();
                    GUIManager.titlebardark();
                    string text = "終了しています...";
                    if (Time.smoothDeltaTime >= 30 * 60)
                    {
                        text = "お疲れ様でした。";
                    }
                    GUIManager.menu_center(new MenuItem[] { new MenuItem(text) });
                    if (lastmsgtime <= 0)
                    {
                        window = 1;
                    }
                    else
                    {
                        lastmsgtime -= Time.deltaTime;
                    }
                    break;
                default:
                    break;
            }
            if (statusinfo)
            {
                GUIManager.label("FPS: " + 1 / Time.smoothDeltaTime, GUIManager.Position.LeftTop);
                GUIManager.label("Window: " + window, GUIManager.Position.LeftTop);
            }
            GUIManager.flush();
            if (tipstime < 10)
            {
                tipstime += Time.deltaTime;
            }
            else
            {
                tipstime = 0;
                if ((tipsindex += 1) >= tips.Length)
                {
                    tipsindex = 0;
                }
            }
        }
        void OnApplicationFocus(bool focusStatus)
        {
            if (window == 30)
            {
                window = 31;
            }
        }
        void OnApplicationPause(bool pauseStatus)
        {
            if (window == 30)
            {
                window = 31;
            }
        }
        public static void playSound(AudioClip clip)
        {
            audiosource.clip = clip;
            audiosource.Play();
        }
        public static string gettips()
        {
            return tips[tipsindex];
        }
    }
}
