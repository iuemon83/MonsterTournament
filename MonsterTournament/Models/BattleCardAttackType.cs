namespace MonsterTournament.Models
{
    public enum BattleCardAttackType
    {
        Other,
        Miss,
        Attack,
        Heal,
        ReAttack,
        BattleState,
        Special,
        Buff,
        Transform,
    }

    public static class BattleCardAttackTypeExtensions
    {
        public static string ToDisplayString(this BattleCardAttackType type)
        {
            return type switch
            {
                BattleCardAttackType.Other => "その他",
                BattleCardAttackType.Miss => "ミス",
                BattleCardAttackType.Attack => "攻撃",
                BattleCardAttackType.Heal => "回復",
                BattleCardAttackType.ReAttack => "再攻撃",
                BattleCardAttackType.BattleState => "状態異常",
                BattleCardAttackType.Special => "必殺技",
                BattleCardAttackType.Buff => "バフ",
                BattleCardAttackType.Transform => "変身",
                _ => "エラー",
            };
        }
    }
}
