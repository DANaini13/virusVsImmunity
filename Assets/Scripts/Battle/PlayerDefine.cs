using BattleMath;

abstract class Player 
{
    protected vector2 location;
    protected decimal speed;
    public abstract void move(vector2 direction);
    public abstract void implementTask(int key);
} 
class Macrophages: Player
{
    public override void implementTask(int key)
    {
    }

    public override void move(vector2 direction)
    {
        location = vector2.plus(location, vector2.mutiply(direction, speed));
    }
}
