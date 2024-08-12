namespace MonsterTournament.Models
{
    public enum AttackTarget
    {
        Self,
        Enemy,
        All,
    }

    public static class AttackTargetExtensions
    {
        public static string ToDisplayString(this AttackTarget type)
        {
            return type switch
            {
                AttackTarget.Self => "自分",
                AttackTarget.Enemy => "相手",
                AttackTarget.All => "お互い",
                _ => "エラー",
            };
        }
    }
}
