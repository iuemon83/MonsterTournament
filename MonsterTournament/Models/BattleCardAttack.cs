namespace MonsterTournament.Models
{
    public class BattleCardAttack
    {
        public BattleCardAttackType Type { get; set; }
        public AttackTarget Target { get; set; }
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public int IntValue { get; set; } = 0;
    }
}
