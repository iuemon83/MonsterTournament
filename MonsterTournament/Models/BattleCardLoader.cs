using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace MonsterTournament.Models
{
    public class BattleCardLoader
    {
        public static readonly string FileExtension = ".mtbc";
        public static readonly string JsonFileName = "card.json";

        public async Task<MonsterTounamentFile> Save(BattleCard model, Dictionary<string, byte[]> imagesByName)
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

                foreach (var (name, imageBytes) in imagesByName)
                {
                    if (imageBytes.Length != 0)
                    {
                        var entry = zip.CreateEntry(name);
                        using var stream = entry.Open();
                        await stream.WriteAsync(imageBytes);
                    }
                }
            }

            return new($"{model.Main.Name}{FileExtension}", fileStream.ToArray());
        }

        public async Task<(BattleCard?, Dictionary<string, byte[]> Images)> Load(byte[] bytes)
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

            var imagesByName = new Dictionary<string, byte[]>();
            var imageNames = new[] { cardOrNull.Main.ImageFileName }
                .Concat(cardOrNull.Transforms.Select(t => t.ImageFileName))
                .ToArray();
            foreach (var entry in archive.Entries)
            {
                foreach (var name in imageNames)
                {
                    if (entry.Name == name)
                    {
                        Console.WriteLine("transform image found");

                        var imageMs = new MemoryStream();
                        using var stream = entry.Open();
                        await stream.CopyToAsync(imageMs);
                        imagesByName.Add(name, imageMs.ToArray());
                    }
                }
            }

            return (cardOrNull, imagesByName);
        }
    }
}
