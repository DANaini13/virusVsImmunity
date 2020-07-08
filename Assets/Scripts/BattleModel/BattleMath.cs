using UnityEngine;
namespace BattleMath
{
    struct vector2 
    { 
        public vector2(decimal x, decimal y)
        {
            this.x = x;
            this.y = y;
        }
        public decimal x; 
        public decimal y;
        static public vector2 plus(vector2 left, vector2 right) 
        {
            return new vector2(left.x + right.x, left.y + right.y);
        }
        static public vector2 mines(vector2 left, vector2 right) 
        {
            return new vector2(left.x - right.x, left.y - right.y);
        }
        static public vector2 mutiply(vector2 left, decimal mutiplier) 
        {
            return new vector2(left.x * mutiplier, left.y * mutiplier);
        }
        static public vector2 devide(vector2 left, decimal devider) 
        {
            return new vector2(left.x/devider, left.y/devider);
        }
        static public vector2 fromString(string str)
        {
            string[] parts = str.Split(new char[] { '-' });
            decimal x = decimal.Parse(parts[0]);
            decimal y = decimal.Parse(parts[1]);
            return new vector2(x, y);
        }
        public string ToString()
        {
            return x + "-" + y;
        }
        public Vector2 toVec2() 
        {
            return new Vector2((float) x, (float) y);
        }
    }
}
