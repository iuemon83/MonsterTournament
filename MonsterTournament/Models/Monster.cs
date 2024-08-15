namespace MonsterTournament.Models
{
    public class Monster
    {
        public static Monster Empty => new()
        {
            Name = "新規の変身",
            Attacks = Enumerable.Range(1, 6).Select(i => new BattleCardAttack()).ToArray()
        };

        public string Name { get; set; } = "";
        public string FlavorText { get; set; } = "";
        public int Hp { get; set; }
        public string ImageFileName { get; set; } = "";
        public BattleCardAttack[] Attacks { get; set; } = [];
    }
}
