namespace BattleMath
{
    struct vector2 
    { 
        vector2(decimal x, decimal y)
        {
            this.x = x;
            this.y = y;
        }
        public decimal x; 
        public decimal y;
        static public vector2 plus(vector2 left, vector2 right) 
        {
            return vector2(left.x + right.x, left.y + right.y);
        }
        static public vector2 mines(vector2 left, vector2 right) 
        {
            return vector2(left.x - right.x, left.y - right.y);
        }
        static public vector2 mutiply(vector2 left, decimal mutiplier) 
        {
            return vector2(left.x * mutiplier, left.y * mutiplier);
        }
        static public vector2 devide(vector2 left, decimal devider) 
        {
            return vector2(left.x/devider, left.y/devider);
        }
    }
}
