namespace ZL.Cinematics
{
    public interface ICinematicAction
    {
        void Play();

        bool IsPlaying();

        void End();

        bool Ended();
    }
}