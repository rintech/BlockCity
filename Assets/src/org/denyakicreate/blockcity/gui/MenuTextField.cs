namespace org.denyakicreate.blockcity.gui
{
    public class MenuTextField : MenuItem
    {
        public string graytext;
        public string enter;
        public MenuTextField(string text, string graytext, string enter) : base(text)
        {
            this.graytext = graytext;
            this.enter = enter;
        }
    }
}
