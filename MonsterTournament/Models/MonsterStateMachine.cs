using Microsoft.AspNetCore.Components;

namespace MonsterTournament.Models
{
    public class MonsterStateMachine
    {
        public Monster Def { get; set; }
        public int CurrentHp { get; set; }
        public int DiffValue { get; set; }
        public BattleCardAttackStateMachine[] Attacks { get; set; } = [];
        public BattleState? battleStateOrNull;
        public SpecialAttack? specialAttackOrNull;

        public bool IsAlive => CurrentHp > 0;

        public MarkupString FlavorText
            => (MarkupString)System.Web.HttpUtility.HtmlEncode(Def.FlavorText).Replace("\n", "<br />");

        public bool BattleStateAttached => battleStateOrNull != null;
        public string BattleStateName => battleStateOrNull?.Name ?? "なし";

        public bool SpecialAttackAttached => specialAttackOrNull != null;
        public string SpecialName => specialAttackOrNull?.Name ?? "なし";

        public MonsterStateMachine(Monster source)
        {
            Def = source;
            CurrentHp = source.Hp;
            DiffValue = 0;
            Attacks = source.Attacks.Select(a => new BattleCardAttackStateMachine(a)).ToArray();
        }

        public MonsterStateMachine(Monster source, MonsterStateMachine baseCard) : this(source)
        {
            // 現在の状態を引き継ぐ
            this.CurrentHp = Math.Min(this.Def.Hp, baseCard.CurrentHp);
            this.Buff(baseCard.DiffValue);
            if (baseCard.battleStateOrNull != null)
            {
                this.AttacheBattleState(baseCard.battleStateOrNull);
            }
        }

        public void AttacheBattleState(BattleState state)
        {
            battleStateOrNull = state;
            foreach (var (s, a) in state.Attacks.Zip(Attacks))
            {
                a.StatusDef = s;
            }
        }

        public void DettacheBattleState()
        {
            battleStateOrNull = null;
            foreach (var a in Attacks)
            {
                a.StatusDef = null;
            }
        }

        public void AttacheSpecial(SpecialAttack special)
        {
            this.specialAttackOrNull = special;
            foreach (var (s, a) in special.Attacks.Zip(Attacks))
            {
                a.Special = s;
            }
        }

        public void DettacheSpecial()
        {
            this.specialAttackOrNull = null;
            foreach (var a in Attacks)
            {
                a.Special = null;
            }
        }

        public int AddHp()
        {
            var heal = Math.Min(10, Def.Hp - CurrentHp);
            this.CurrentHp += heal;

            return heal;
        }

        public int SubHp()
        {
            var damage = Math.Min(this.CurrentHp, 10);
            this.CurrentHp -= damage;

            return damage;
        }

        public void Buff()
        {
            this.Buff(10);
        }

        public void Debuff()
        {
            this.Buff(-10);
        }

        public void Buff(int x)
        {
            this.DiffValue += x;
            foreach (var a in Attacks)
            {
                a.DiffValue += x;
            }
        }

        public int NextAttackIndex() => Random.Shared.Next(this.Attacks.Length - 1);
    }
}
