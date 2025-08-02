

public class Utility
{
    public class GameStateMessage
    {
        public float[] state;
        public bool collision;
        public bool foodEaten;
        public bool done;
        public int score;
    }

    public class ActionMessage 
    { 
        public int action; 
    }
}
