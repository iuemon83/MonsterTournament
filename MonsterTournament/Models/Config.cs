namespace MonsterTournament.Models
{
    public static class Config
    {
        public static readonly (int X, int Y) ImageSize = (15, 15);
        public static readonly int MaxImageFileSizeInBytes = 10_000_000;
        public static readonly int MaxCardFileSizeInBytes = 30_000_000;
    }
}
