using UnityEngine;
namespace org.rintech.blockcity.gui
{
    public class GUIManager
    {
        public const int buttonwidth = 256;
        public const int buttonheight = 24;
        public const byte buttonmargin = 4;
        private static bool _logo_big = false;
        private static string msg = "";
        private static float msgtime = 0;
        private static MessageType msgtype = MessageType.NORMAL;
        private static GUIStyle fontstyle = new GUIStyle();
        private static byte label_lefttop = 0;
        private static byte label_righttop = 0;
        private static byte label_leftbottom = 0;
        private static byte label_rightbottom = 0;
        public static void init()
        {
            fontstyle.richText = true;
            fontstyle.normal.textColor = Color.white;
            fontstyle.alignment = TextAnchor.MiddleCenter;
            fontstyle.font = AssetsManager.font;
        }
        public static void reset()
        {
            _logo_big = false;
            label_lefttop = 0;
            label_righttop = 0;
            label_leftbottom = 0;
            label_rightbottom = 0;
        }
        public static void background()
        {
            int left = Screen.width / 2;
            int up = Screen.height / 2;
            while (0 < left)
            {
                left -= AssetsManager.background.width;
            }
            while (0 < up)
            {
                up -= AssetsManager.background.height;
            }
            for (int x = 0; x * AssetsManager.background.width - AssetsManager.background.width < Screen.width; x++)
            {
                for (int y = 0; y * AssetsManager.background.height - AssetsManager.background.height < Screen.height; y++)
                {
                    GUI.DrawTexture(new Rect(left + x * AssetsManager.background.width, up + y * AssetsManager.background.height, AssetsManager.background.width, AssetsManager.background.height), AssetsManager.background);
                }
            }
        }
        public static void dark()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), AssetsManager.darkbg);
        }
        public static void titlebardark()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, buttonheight), AssetsManager.darkbg);
        }
        public static void logo_big()
        {
            _logo_big = true;
            GUI.DrawTexture(new Rect(0, buttonheight, Screen.width, Screen.height / 2 - buttonheight), AssetsManager.logo_big, ScaleMode.ScaleToFit);
        }
        public static void menu_center(MenuItem[] items)
        {
            int y;
            if (_logo_big)
            {
                y = Screen.height * 3 / 4;
            }
            else
            {
                y = Screen.height / 2;
            }
            int itemsheight = buttonheight * items.Length + buttonmargin * 2 + buttonmargin * (items.Length - 1);
            GUI.DrawTexture(new Rect(Screen.width / 2 - buttonwidth / 2 - buttonmargin, y - itemsheight / 2, buttonwidth + buttonmargin * 2, buttonheight * items.Length + buttonmargin * 2 + buttonmargin * (items.Length - 1)), AssetsManager.whitebg);
            for (int a = 0; a < items.Length; a++)
            {
                __zzz(items[a], 
                    Screen.width / 2 - buttonwidth / 2, 
                    y - itemsheight / 2 + buttonmargin + buttonheight * a + buttonmargin * a, 
                    buttonwidth, buttonheight);
            }
        }
        public static void menu_bottom(MenuItem[] items)
        {
            int itemsheight = buttonheight * items.Length + buttonmargin * 2 + buttonmargin * (items.Length - 1);
            GUI.DrawTexture(new Rect(0, Screen.height - itemsheight, Screen.width, itemsheight), AssetsManager.darkbg);
            GUI.DrawTexture(new Rect(Screen.width / 2 - buttonwidth / 2 - buttonmargin, Screen.height - itemsheight, buttonwidth + buttonmargin * 2, buttonheight * items.Length + buttonmargin * 2 + buttonmargin * (items.Length - 1)), AssetsManager.whitebg);
            for (int a = 0; a < items.Length; a++)
            {
                __zzz(items[a],
                    Screen.width / 2 - buttonwidth / 2,
                    Screen.height - itemsheight + buttonmargin + buttonheight * a + buttonmargin * a,
                    buttonwidth, buttonheight);
            }
        }
        private static void __zzz(MenuItem item, int x, int y, int width, int height)
        {
            if (0 < item.initems.Length)
            {
                for (int a = 0; a < item.initems.Length; a++)
                {
                    __zzz(item.initems[a],
                        x + (width + buttonmargin * 2) * a / item.initems.Length,
                        y,
                        (width + buttonmargin * 2) / item.initems.Length - buttonmargin * 2,
                        height);
                }
            }
            else
            {
                if (item is MenuList)
                {
                    GUIStyle labelstyle = new GUIStyle(fontstyle);
                    GUI.DrawTexture(new Rect(x, y, width, height), AssetsManager.darkbg);
                    if (0 < ((MenuList)item).list.Length)
                    {
                        if (GUI.Button(new Rect(x, y, buttonheight, height), "<", fontstyle))
                        {
                            Main.playSound(AssetsManager.ac_click);
                            ((MenuList)item).select--;
                        }
                        else if (GUI.Button(new Rect(x + width - buttonheight, y, buttonheight, height), ">", fontstyle))
                        {
                            Main.playSound(AssetsManager.ac_click);
                            ((MenuList)item).select++;
                        }
                        if (((MenuList)item).select < 0)
                        {
                            ((MenuList)item).select = ((MenuList)item).list.Length - 1;
                        }
                        else if (((MenuList)item).list.Length <= ((MenuList)item).select)
                        {
                            ((MenuList)item).select = 0;
                        }
                        GUI.Label(new Rect(x + buttonheight, y, width - buttonheight * 2, height), ((MenuList)item).list[((MenuList)item).select], fontstyle);
                        labelstyle.normal.textColor = Color.white;
                        labelstyle.alignment = TextAnchor.MiddleRight;
                        GUI.Label(new Rect(x - buttonwidth, y, width, height), item.text + "(" + (((MenuList)item).select + 1) + "/" + ((MenuList)item).list.Length + "): ", labelstyle);
                    }
                    else
                    {
                        labelstyle.normal.textColor = Color.gray;
                        GUI.Label(new Rect(x + buttonheight, y, width - buttonheight * 2, height), "(なし)", labelstyle);
                        labelstyle.normal.textColor = Color.white;
                        labelstyle.alignment = TextAnchor.MiddleRight;
                        GUI.Label(new Rect(x - buttonwidth, y, width, height), item.text + ": ", labelstyle);
                    }
                }
                else if (item is MenuButton)
                {
                    GUI.DrawTexture(new Rect(x, y, width, height), AssetsManager.darkbg);
                    item.clicked = GUI.Button(new Rect(x, y, width, height), item.text, fontstyle);
                }
                else if (item is MenuTextField)
                {
                    GUI.DrawTexture(new Rect(x, y, width, height), AssetsManager.darkbg);
                    GUIStyle labelstyle = new GUIStyle(fontstyle);
                    labelstyle.alignment = TextAnchor.MiddleRight;
                    GUI.Label(new Rect(x - buttonwidth, y, width, height), item.text + ": ", labelstyle);
                    labelstyle.alignment = TextAnchor.MiddleLeft;
                    if (((MenuTextField)item).enter.Length == 0)
                    {
                        labelstyle.normal.textColor = Color.gray;
                        GUI.Label(new Rect(x, y, width, height), ((MenuTextField)item).graytext, labelstyle);
                        labelstyle.normal.textColor = Color.white;
                    }
                    ((MenuTextField)item).enter = GUI.TextField(new Rect(x, y, width, height), ((MenuTextField)item).enter, labelstyle);
                }
                else if (item is MenuSlider)
                {
                    GUIStyle labelstyle = new GUIStyle(fontstyle);
                    labelstyle.alignment = TextAnchor.MiddleRight;
                    GUI.Label(new Rect(x - buttonwidth, y, width, height), item.text + ": ", labelstyle);
                    ((MenuSlider)item).value = GUI.HorizontalSlider(new Rect(x, y, width, height), ((MenuSlider)item).value, ((MenuSlider)item).minvalue, ((MenuSlider)item).maxvalue);
                }
                else
                {
                    GUI.Label(new Rect(x, y, width, height), item.text, fontstyle);
                }
            }
        }
        public static bool[] menu_center(string[] buttons)
        {
            int y;
            if (_logo_big)
            {
                y = Screen.height * 3 / 4;
            }
            else
            {
                y = Screen.height / 2;
            }
            int itemsheight = buttonheight * buttons.Length + buttonmargin * 2 + buttonmargin * (buttons.Length - 1);
            GUI.DrawTexture(new Rect(Screen.width / 2 - buttonwidth / 2 - buttonmargin, 
                y - itemsheight / 2, 
                buttonwidth + buttonmargin * 2, 
                buttonheight * buttons.Length + buttonmargin * 2 + buttonmargin * (buttons.Length - 1)), AssetsManager.whitebg);
            bool[] click = new bool[buttons.Length];
            for (int a = 0; a < buttons.Length; a++)
            {
                GUI.DrawTexture(new Rect(Screen.width / 2 - buttonwidth / 2, 
                    y - itemsheight / 2 + buttonmargin + buttonheight * a + buttonmargin * a, 
                    buttonwidth, 
                    buttonheight), AssetsManager.darkbg);
                click[a] = GUI.Button(new Rect(Screen.width / 2 - buttonwidth / 2, 
                    y - itemsheight / 2 + buttonmargin + buttonheight * a + buttonmargin * a, 
                    buttonwidth, 
                    buttonheight), buttons[a], fontstyle);
            }
            return click;
        }
        public enum MessageType
        {
            NORMAL, CAUTION, WARNING
        }
        public static void setmsg(string msg)
        {
            setmsg(msg, MessageType.NORMAL);
        }
        public static void setmsg(string msg, MessageType type)
        {
            GUIManager.msg = msg;
            msgtime = (msgtype = type) == MessageType.NORMAL ? 5 : 10;
            Main.playSound(AssetsManager.ac_msg);
        }
        public enum Position
        {
            TitleBar, LeftTop, RightTop, LeftBottom, RightBottom
        }
        public static void label(string text, Position pos)
        {
            GUIStyle labelstyle = new GUIStyle(fontstyle);
            int x = 0;
            int y = buttonheight;
            int width = Screen.width / 2;
            switch (pos)
            {
                case Position.TitleBar:
                    labelstyle.alignment = TextAnchor.MiddleLeft;
                    y = 0;
                    width = Screen.width;
                    break;
                case Position.LeftTop:
                    labelstyle.alignment = TextAnchor.MiddleLeft;
                    y += buttonheight * label_lefttop;
                    break;
                case Position.RightTop:
                    labelstyle.alignment = TextAnchor.MiddleRight;
                    x = Screen.width / 2;
                    y += buttonheight * label_righttop;
                    break;
                case Position.LeftBottom:
                    labelstyle.alignment = TextAnchor.MiddleLeft;
                    y = Screen.height - buttonheight - buttonheight * label_leftbottom;
                    break;
                case Position.RightBottom:
                    labelstyle.alignment = TextAnchor.MiddleRight;
                    x = Screen.width / 2;
                    y = Screen.height - buttonheight - buttonheight * label_rightbottom;
                    break;
                default:
                    break;
            }
            GUI.Label(new Rect(x, y, width, buttonheight), text, labelstyle);
            switch (pos)
            {
                case Position.LeftTop:
                    label_lefttop++;
                    break;
                case Position.RightTop:
                    label_righttop++;
                    break;
                case Position.LeftBottom:
                    label_leftbottom++;
                    break;
                case Position.RightBottom:
                    label_rightbottom++;
                    break;
                default:
                    break;
            }
        }
        public static void flush()
        {
            if (0 < msgtime)
            {
                msgtime -= Time.deltaTime;
                switch (msgtype)
                {
                    case MessageType.CAUTION:
                        GUI.DrawTexture(new Rect(0, 96, Screen.width, 24), AssetsManager.yellowbg);
                        break;
                    case MessageType.WARNING:
                        GUI.DrawTexture(new Rect(0, 96, Screen.width, 24), AssetsManager.redbg);
                        break;
                    default:
                        GUI.DrawTexture(new Rect(0, 96, Screen.width, 24), AssetsManager.darkbg);
                        break;
                }
                GUI.Label(new Rect(0, 96, Screen.width, 24), msg, fontstyle);
            }
        }
    }
}
