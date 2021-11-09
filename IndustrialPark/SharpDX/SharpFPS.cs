namespace IndustrialPark
{
    /// <summary>
    /// Helper to count frame per seconds
    /// </summary>
    public class SharpFPS
    {
        /// <summary>
        /// Last computed FPS
        /// </summary>
        public int FPS { get; private set; }

        private int n1;
        private long timeout;

        private System.Diagnostics.Stopwatch watch;

        /// <summary>
        /// Constructor
        /// </summary>
        public SharpFPS()
        {
            watch = new System.Diagnostics.Stopwatch();
            Reset();
        }

        /// <summary>
        /// Reset Counter
        /// </summary>
        public void Reset()
        {
            watch.Reset();
            n1 = 0;
            timeout = watch.ElapsedMilliseconds;
            FPS = 0;
            watch.Start();
        }

        /// <summary>
        /// Update counter. This must be executed every frame in render loop
        /// </summary>
        public void Update()
        {
            n1++;
            if (watch.ElapsedMilliseconds - timeout >= 1000)
            {
                FPS = n1;
                n1 = 0;
                timeout = watch.ElapsedMilliseconds;
            }
        }

    }
}
