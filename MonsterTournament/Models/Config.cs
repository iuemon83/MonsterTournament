namespace MonsterTournament.Models
{
    public static class Config
    {
        public static readonly (int X, int Y) ImageSize = (15, 15);
        public static readonly int MaxImageFileSizeInBytes = 2_000_000;
        public static readonly int MaxCardFileSizeInBytes = 2_500_000;
    }
}
