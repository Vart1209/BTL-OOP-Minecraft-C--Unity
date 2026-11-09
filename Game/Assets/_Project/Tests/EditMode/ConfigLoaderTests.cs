using System.IO;
using NUnit.Framework;
using Minecraft.Domain.Configuration;
using Minecraft.Domain.Contracts;
using Minecraft.Infrastructure.Config;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class ConfigLoaderTests
    {
        private string _tempDir;

        [SetUp]
        public void SetUp()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "MinecraftConfigTests_" + Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDir);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }

        [Test]
        public void LoadConfig_ValidJsonFile_LoadsAllParametersCorrectly()
        {
            string filePath = Path.Combine(_tempDir, "config.json");
            string validJson = @"{
                ""world"": {
                    ""chunkWidth"": 16,
                    ""chunkHeight"": 64,
                    ""renderDistanceInChunks"": 6,
                    ""defaultSeed"": 12345
                },
                ""player"": {
                    ""maxHealth"": 20,
                    ""maxHunger"": 20,
                    ""reachDistance"": 5.0,
                    ""walkSpeed"": 5.5,
                    ""jumpForce"": 7.0
                },
                ""resourcePack"": {
                    ""activePack"": ""CustomPack"",
                    ""availablePacks"": [""default"", ""CustomPack""],
                    ""tileSize"": 32
                }
            }";
            File.WriteAllText(filePath, validJson);

            IConfigLoader loader = new ConfigLoader();
            GameConfigDto config = loader.LoadConfig(filePath);

            Assert.IsNotNull(config);
            Assert.AreEqual(16, config.World.ChunkWidth);
            Assert.AreEqual(64, config.World.ChunkHeight);
            Assert.AreEqual(6, config.World.RenderDistanceInChunks);
            Assert.AreEqual(12345, config.World.DefaultSeed);

            Assert.AreEqual(20, config.Player.MaxHealth);
            Assert.AreEqual(5.0f, config.Player.ReachDistance);
            Assert.AreEqual(5.5f, config.Player.WalkSpeed);

            Assert.AreEqual("CustomPack", config.ResourcePack.ActivePack);
            Assert.AreEqual(32, config.ResourcePack.TileSize);
        }

        [Test]
        public void LoadConfig_MissingFile_FallsBackToDefaultWithoutCrashing()
        {
            string nonExistentPath = Path.Combine(_tempDir, "does_not_exist.json");
            IConfigLoader loader = new ConfigLoader();

            // Must catch FileNotFoundException and fallback gracefully (Tiêu chí 19)
            GameConfigDto config = loader.LoadConfig(nonExistentPath);

            Assert.IsNotNull(config);
            Assert.AreEqual(16, config.World.ChunkWidth);
            Assert.AreEqual(64, config.World.ChunkHeight);
            Assert.AreEqual(4, config.World.RenderDistanceInChunks);
            Assert.IsNotNull(loader.LastWarning);
            Assert.IsTrue(loader.LastWarning.Contains("không tồn tại") || loader.LastWarning.Contains("mặc định"));
        }

        [Test]
        public void LoadConfig_CorruptedJsonFile_FallsBackToDefaultWithoutCrashing()
        {
            string corruptPath = Path.Combine(_tempDir, "corrupt.json");
            File.WriteAllText(corruptPath, "{ broken json content: 12345 %%% }");

            IConfigLoader loader = new ConfigLoader();

            // Must catch JsonException and fallback gracefully (Tiêu chí 19)
            GameConfigDto config = loader.LoadConfig(corruptPath);

            Assert.IsNotNull(config);
            Assert.AreEqual(20, config.Player.MaxHealth);
            Assert.IsNotNull(loader.LastWarning);
        }
    }
}
