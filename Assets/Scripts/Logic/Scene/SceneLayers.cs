namespace Logic.Scene
{
    public static class SceneLayers
    {
        public const int Floor = 3, Wall = 6;

        public const int
            WallMask = 1 << Wall,
            FloorMask = 1 << Floor;
    }
}	