namespace Sherlock.ProtoActor
{
    public static class SherlockSettings
    {
        private static bool _enabled;

        public static void Enable() => _enabled = true;
        public static bool Enabled => _enabled;
    }
}