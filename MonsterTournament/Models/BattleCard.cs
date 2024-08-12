using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace MonsterTournament.Client.Models
{
    public static class Config
    {
        public static readonly (int X, int Y) ImageSize = (250, 250);
        public static readonly int MaxImageFileSizeInBytes = 2_000_000;
        public static readonly int MaxCardFileSizeInBytes = 2_500_000;
    }

    public class AudioSrc
    {
        public static readonly string Heal = "/audio/heal.mp3";
        public static readonly string Damage = "/audio/damage.mp3";
        public static readonly string Death = "/audio/death.mp3";
        public static readonly string Action = "/audio/action.mp3";
        public static readonly string ActionRoulette = "/audio/action_roulette.mp3";
        public static readonly string Buff = "/audio/buff.mp3";
    }

    public class BattleCard
    {
        public string Name { get; set; } = "";
        public string FlavorText { get; set; } = "";
        public int Hp { get; set; }
        public string ImageFileName { get; set; } = "";
        public BattleCardAttack[] Attacks { get; set; } = [];
    }

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

    public class BattleCardAttack
    {
        public BattleCardAttackType Type { get; set; }
        public AttackTarget Target { get; set; }
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public int IntValue { get; set; } = 0;
    }

    public class BattleState
    {
        public string Name { get; set; } = "";
        public BattleCardAttack[] Attacks { get; set; } = [];
    }

    public class SpecialAttack
    {
        public string Name { get; set; } = "";
        public BattleCardAttack[] Attacks { get; set; } = [];
    }

    public record MonsterTounamentFile(string FileName, byte[] Bytes);

    public class BattleCardLoader
    {
        public static readonly string FileExtension = ".mtbc";
        public static readonly string JsonFileName = "card.json";

        public async Task<MonsterTounamentFile> Save(BattleCard model, byte[] imageBytes)
        {
            var fileStream = new MemoryStream();

            // disposeしないとzipファイルに書き込まれない
            using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                var cardEntry = zip.CreateEntry(JsonFileName);
                using (var cardStream = cardEntry.Open())
                {
                    await JsonSerializer.SerializeAsync(cardStream, model);
                }

                if (imageBytes.Length != 0)
                {
                    var entry = zip.CreateEntry(model.ImageFileName);
                    using var stream = entry.Open();
                    await stream.WriteAsync(imageBytes);
                }
            }

            return new($"{model.Name}{FileExtension}", fileStream.ToArray());
        }

        public async Task<(BattleCard?, byte[] image)> Load(byte[] bytes)
        {
            using var archive = new ZipArchive(new MemoryStream(bytes));

            BattleCard? cardOrNull = null;
            foreach (var entry in archive.Entries)
            {
                Console.WriteLine(entry.Name);

                if (entry.Name == JsonFileName)
                {
                    using var stream = entry.Open();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    Console.WriteLine(json);

                    cardOrNull = JsonSerializer.Deserialize<BattleCard>(json);
                    break;
                }
            }

            if (cardOrNull == null)
            {
                //TODO : カードの読み込みに失敗 error handling
                Console.WriteLine("カードの読み込みに失敗");
                return default;
            }

            var imageMs = new MemoryStream();
            foreach (var entry in archive.Entries)
            {
                if (entry.Name == cardOrNull.ImageFileName)
                {
                    Console.WriteLine("image found");

                    using var stream = entry.Open();
                    await stream.CopyToAsync(imageMs);

                    break;
                }
            }

            return (cardOrNull, imageMs.ToArray());
        }
    }

    public class BattleStateLoader
    {
        public static readonly string FileExtension = ".mtbs";
        public static readonly string JsonFileName = "state.json";

        public async Task<MonsterTounamentFile> Save(BattleState model)
        {
            var fileStream = new MemoryStream();

            // disposeしないとzipファイルに書き込まれない
            using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                var cardEntry = zip.CreateEntry(JsonFileName);
                using (var cardStream = cardEntry.Open())
                {
                    await JsonSerializer.SerializeAsync(cardStream, model);
                }
            }

            return new($"{model.Name}{FileExtension}", fileStream.ToArray());
        }

        public async Task<BattleState?> Load(byte[] bytes)
        {
            using var archive = new ZipArchive(new MemoryStream(bytes));

            BattleState? stateOrNull = null;
            foreach (var entry in archive.Entries)
            {
                Console.WriteLine(entry.Name);

                if (entry.Name == JsonFileName)
                {
                    using var stream = entry.Open();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    Console.WriteLine(json);

                    stateOrNull = JsonSerializer.Deserialize<BattleState>(json);
                    break;
                }
            }

            if (stateOrNull == null)
            {
                //TODO : カードの読み込みに失敗 error handling
                Console.WriteLine("状態の読み込みに失敗");
                return default;
            }

            return stateOrNull;
        }
    }

    public class SpecialAttackLoader
    {
        public static readonly string FileExtension = ".mtsa";
        public static readonly string JsonFileName = "special.json";

        public async Task<MonsterTounamentFile> Save(SpecialAttack model)
        {
            var fileStream = new MemoryStream();

            // disposeしないとzipファイルに書き込まれない
            using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                var cardEntry = zip.CreateEntry(JsonFileName);
                using (var cardStream = cardEntry.Open())
                {
                    await JsonSerializer.SerializeAsync(cardStream, model);
                }
            }

            return new($"{model.Name}{FileExtension}", fileStream.ToArray());
        }

        public async Task<SpecialAttack?> Load(byte[] bytes)
        {
            using var archive = new ZipArchive(new MemoryStream(bytes));

            SpecialAttack? stateOrNull = null;
            foreach (var entry in archive.Entries)
            {
                Console.WriteLine(entry.Name);

                if (entry.Name == JsonFileName)
                {
                    using var stream = entry.Open();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    Console.WriteLine(json);

                    stateOrNull = JsonSerializer.Deserialize<SpecialAttack>(json);
                    break;
                }
            }

            if (stateOrNull == null)
            {
                //TODO : カードの読み込みに失敗 error handling
                Console.WriteLine("状態の読み込みに失敗");
                return default;
            }

            return stateOrNull;
        }
    }
}
