namespace ZombieHouseDefense.core
{
    public interface IPauseService
    {
        event System.Action Paused;
        event System.Action Resumed;
        bool IsPaused { get; }
        void Pause();
        void Resume();
    }

    public static class Pause
    {
        public static IPauseService Instance { get; set; }
    }
}