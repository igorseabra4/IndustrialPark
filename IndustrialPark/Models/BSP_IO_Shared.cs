namespace IndustrialPark.Models
{
    public static class BSP_IO_Shared
    {
        public static int scoobyRenderWareVersion => 0x00000310;
        public static int bfbbRenderWareVersion => 0x1003FFFF; // fix
        public static int tssmRenderWareVersion => 0x1400FFFF;

        public static int modelRenderWareVersion(HipHopFile.Game game)
        {
            if (game == HipHopFile.Game.Scooby)
                return scoobyRenderWareVersion;
            return tssmRenderWareVersion;
        }
    }
}
