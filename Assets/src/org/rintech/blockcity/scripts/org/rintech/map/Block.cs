using System;

namespace org.rintech.blockcity.map
{
    [Serializable]
    public class Block
    {
        public static Block grass = new Block(0);
        public static Block tree = new Block(1);
        public static Block forest = new Block(2);
        public static Block stone = new Block(3);
        public static Block road_x = new Block(4);
        public static Block road_y = new Block(5);
        public static Block road_lu = new Block(6);
        public static Block road_ru = new Block(7);
        public static Block road_rd = new Block(8);
        public static Block road_ld = new Block(9);
        public static Block road_lur = new Block(10);
        public static Block road_rud = new Block(11);
        public static Block road_ldr = new Block(12);
        public static Block road_lud = new Block(13);
        public static Block road_c = new Block(14);
        public static Block dirt = new Block(15);
        public int id
        {
            private set;
            get;
        }
        public Metadata metadata;
        public Block(int id) : this(id, new Metadata())
        {
        }
        public Block(int id, Metadata metadata)
        {
            this.id = id;
            this.metadata = metadata;
        }
        public static Block getNewBlock(int id)
        {
            switch (id)
            {
                case 1:
                    return tree;
                case 2:
                    return forest;
                case 3:
                    return stone;
                case 4:
                    return road_x;
                case 5:
                    return road_y;
                case 6:
                    return road_lu;
                case 7:
                    return road_ru;
                case 8:
                    return road_rd;
                case 9:
                    return road_ld;
                case 10:
                    return road_lur;
                case 11:
                    return road_rud;
                case 12:
                    return road_ldr;
                case 13:
                    return road_lud;
                case 14:
                    return road_c;
                case 15:
                    return dirt;
                default:
                    return grass;
            }
        }
    }
}
