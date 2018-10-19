namespace MyApplication
{
    using Lite;
    using Lite.Caching;

    public class MyGameCache : RoomCacheBase
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static readonly MyGameCache Instance = new MyGameCache();

        protected override Room CreateRoom(string roomId, params object[] args)
        {
            return new MyGame(roomId, this);
        }
    }
}
