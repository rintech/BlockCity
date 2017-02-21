namespace org.denyakicreate.blockcity.gui
{
    public class MenuItem
    {
        public string text;
        public bool clicked = false;
        public MenuItem[] initems;
        public MenuItem(string text)
        {
            this.text = text;
            initems = new MenuItem[0];
        }
        public MenuItem(MenuItem[] initems)
        {
            this.text = "";
            this.initems = initems;
        }
    }
}
