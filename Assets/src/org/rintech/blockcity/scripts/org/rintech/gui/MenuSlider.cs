namespace org.rintech.blockcity.gui
{
    public class MenuSlider : MenuItem
    {
        public float minvalue;
        public float maxvalue;
        public float value;
        public MenuSlider(string text, float minvalue, float maxvalue, float value) : base(text)
        {
            this.minvalue = minvalue;
            this.maxvalue = maxvalue;
            this.value = value;
        }
    }
}
