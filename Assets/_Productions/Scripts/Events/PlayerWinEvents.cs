namespace CapsaBanting
{
    public struct PLayerWinEvent
    {
        public int indexPlayer;

        public PLayerWinEvent(int index)
        {
            indexPlayer = index;
        }

        private static PLayerWinEvent gameEvent;

        public static void Trigger(int index)
        {
            gameEvent.indexPlayer = index;
            EventManager.TriggerEvent(gameEvent);
        }
    }
}