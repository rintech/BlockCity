namespace org.denyakicreate.blockcity.map
{
    public class Metadata
    {
        public string[] keys;
        public string[] values;
        public Metadata()
        {
            keys = new string[0];
            values = new string[0];
        }
        public Metadata(string[] keys, string[] values)
        {
            this.keys = keys;
            this.values = values;
        }
    }
}
