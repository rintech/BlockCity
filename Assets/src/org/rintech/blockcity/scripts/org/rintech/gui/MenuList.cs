namespace org.rintech.blockcity.gui
{
    public class MenuList : MenuItem
    {
        public string[] list;
        public int select;
        public MenuList(string text, string[] list, int select) : base(text)
        {
            this.list = list;
            select = select < list.Length ? select : list.Length - 1;
            this.select = select < 0 ? 0 : select;
        }
    }
}
