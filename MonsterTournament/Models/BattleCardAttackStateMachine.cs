namespace MonsterTournament.Models
{
    public class BattleCardAttackStateMachine
    {
        private static string BuildDescription(BattleCardAttack a, int diffValue)
        {
            var intValueWithBuff = Math.Max(0, a.IntValue + diffValue);

            return a.Type switch
            {
                BattleCardAttackType.Miss => "ミス",
                BattleCardAttackType.Attack
                    => $"{(a.Target != AttackTarget.Enemy ? $"{a.Target.ToDisplayString()}に" : "")}{intValueWithBuff}ダメージ",
                BattleCardAttackType.Heal
                    => $"{(a.Target != AttackTarget.Self ? $"{a.Target.ToDisplayString()}を" : "")}{intValueWithBuff}回復",
                BattleCardAttackType.ReAttack
                    => $"もう一度・{(a.Target != AttackTarget.Enemy ? $"{a.Target.ToDisplayString()}に" : "")}{intValueWithBuff}",
                BattleCardAttackType.Buff
                    => $"{(a.Target != AttackTarget.Self ? $"{a.Target.ToDisplayString()}の" : "")}攻撃を{a.IntValue:+#;-#;0}",
                BattleCardAttackType.BattleState
                    => $"{(a.Target != AttackTarget.Enemy ? $"{a.Target.ToDisplayString()}を" : "")}状態:{a.Value}",
                BattleCardAttackType.Special => $"必殺技:{a.Value}",
                BattleCardAttackType.Transform => $"{a.Value}に変身",
                _ => a.Value,
            };
        }

        public BattleCardAttack Def { get; set; }
        public BattleCardAttack? StatusDef { get; set; }
        public BattleCardAttack? Special { get; set; }
        public int DiffValue { get; set; }

        public BattleCardAttackStateMachine(BattleCardAttack source)
        {
            Def = source;
        }

        public string Name => Def.Name;
        public BattleCardAttackType Type => this.Special == null ? Def.Type : this.Special.Type;

        public string Description => BuildDescription(this.Def, this.DiffValue);
        public string StatusDescription => this.StatusDef == null ? "" : BuildDescription(this.StatusDef, 0);
        public string SpecialName => Special?.Name ?? "";
        public string SpecialAttackDescription => this.Special == null ? "" : BuildDescription(this.Special, this.DiffValue);
    }
}
