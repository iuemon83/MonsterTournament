namespace MonsterTournament.Models
{
    public class BattleCardAttackStateMachine(BattleCardAttack source)
    {
        public BattleCardAttack Def { get; set; } = source;
        public BattleCardAttack? StatusDef { get; set; }
        public BattleCardAttack? Special { get; set; }
        public int DiffValue { get; set; }

        public string Name => Special?.Name ?? Def.Name;
        public BattleCardAttackType Type => this.Special == null ? Def.Type : this.Special.Type;

        public (BattleCardAttack, int) GetAttack()
        {
            return (this.Special ?? this.Def, this.DiffValue);
        }

        public (BattleCardAttack?, int) GetStatusAttack()
        {
            return (this.StatusDef, 0);
        }
    }
}
