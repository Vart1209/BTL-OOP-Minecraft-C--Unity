using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Minecraft.Application.World;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.ResourcePacks;
using Minecraft.Infrastructure.ResourcePacks;

namespace Minecraft.Tests.EditMode
{
    /// <summary>
    /// Bộ kiểm thử đơn vị cho Phân hệ Gói Tài nguyên (Resource Pack System — Tuần T4).
    /// Kiểm tra: Thực thể <see cref="ResourcePackManifest"/>, Chiến lược <see cref="BuiltInAtlasProvider"/>,
    /// <see cref="FolderResourcePackProvider"/> (đọc thư mục, xử lý ngoại lệ <c>try-catch</c>) và <see cref="ResourcePackService"/> (Observer Pattern).
    /// </summary>
    [TestFixture]
    public sealed class ResourcePackTests
    {
        private string _tempRootDir;

        [SetUp]
        public void SetUp()
        {
            _tempRootDir = Path.Combine(Path.GetTempPath(), "MinecraftTests_ResourcePacks_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempRootDir);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempRootDir))
            {
                Directory.Delete(_tempRootDir, recursive: true);
            }
        }

        [Test]
        public void ResourcePackManifest_ValidatesInput_AndStoresImmutableProperties()
        {
            var textures = new Dictionary<BlockId, string>
            {
                { BlockId.Stone, "textures/block/stone.png" },
                { BlockId.Dirt, "textures/block/dirt.png" }
            };

            var manifest = new ResourcePackManifest(
                packId: "default",
                displayName: "Default Pack",
                description: "Gói mặc định 16x16",
                packFormat: 15,
                tileResolution: 16,
                blockTexturePaths: textures);

            Assert.AreEqual("default", manifest.PackId);
            Assert.AreEqual("Default Pack", manifest.DisplayName);
            Assert.AreEqual("Gói mặc định 16x16", manifest.Description);
            Assert.AreEqual(15, manifest.PackFormat);
            Assert.AreEqual(16, manifest.TileResolution);
            Assert.AreEqual(2, manifest.BlockTexturePaths.Count);
            Assert.AreEqual("textures/block/stone.png", manifest.BlockTexturePaths[BlockId.Stone]);

            Assert.Throws<ArgumentException>(() =>
                new ResourcePackManifest("", "Name", "Desc", 15, 16, textures));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ResourcePackManifest("default", "Name", "Desc", 15, 0, textures));
        }

        [Test]
        public void BuiltInAtlasProvider_ReturnsDefaultPackWith16xResolutionAndAllBlocks()
        {
            ITextureAtlasProvider provider = new BuiltInAtlasProvider();

            IReadOnlyList<string> available = provider.GetAvailablePacks();
            Assert.IsNotEmpty(available);
            Assert.Contains("default", (System.Collections.ICollection)available);

            ResourcePackManifest manifest = provider.LoadPack("default");
            Assert.IsNotNull(manifest);
            Assert.AreEqual("default", manifest.PackId);
            Assert.AreEqual(16, manifest.TileResolution);
            Assert.IsTrue(manifest.BlockTexturePaths.ContainsKey(BlockId.Stone));
            Assert.IsTrue(manifest.BlockTexturePaths.ContainsKey(BlockId.Dirt));
            Assert.IsTrue(manifest.BlockTexturePaths.ContainsKey(BlockId.Grass));
            Assert.IsTrue(manifest.BlockTexturePaths.ContainsKey(BlockId.Planks));
            Assert.IsTrue(manifest.BlockTexturePaths.ContainsKey(BlockId.Cobblestone));
            Assert.IsTrue(manifest.BlockTexturePaths.ContainsKey(BlockId.Bedrock));
            Assert.IsTrue(manifest.BlockTexturePaths.ContainsKey(BlockId.Sand));
            Assert.IsTrue(manifest.BlockTexturePaths.ContainsKey(BlockId.Wood));
            Assert.IsTrue(manifest.BlockTexturePaths.ContainsKey(BlockId.Leaves));
            Assert.IsTrue(manifest.BlockTexturePaths.ContainsKey(BlockId.CraftingTable));
        }

        [Test]
        public void FolderResourcePackProvider_ReadsValidPackMcmeta_AndDetectsPngFiles()
        {
            string packDir = Path.Combine(_tempRootDir, "OptimumRealism_64x");
            string blockTexturesDir = Path.Combine(packDir, "assets", "minecraft", "textures", "block");
            Directory.CreateDirectory(blockTexturesDir);

            string mcmetaJson = @"{
              ""pack"": {
                ""pack_format"": 80,
                ""description"": ""An fps-friendly realistic resource pack.""
              }
            }";
            File.WriteAllText(Path.Combine(packDir, "pack.mcmeta"), mcmetaJson);

            // Tạo các file png mẫu
            File.WriteAllBytes(Path.Combine(blockTexturesDir, "stone.png"), new byte[] { 137, 80, 78, 71 });
            File.WriteAllBytes(Path.Combine(blockTexturesDir, "dirt.png"), new byte[] { 137, 80, 78, 71 });
            File.WriteAllBytes(Path.Combine(blockTexturesDir, "grass_block_top.png"), new byte[] { 137, 80, 78, 71 });
            File.WriteAllBytes(Path.Combine(blockTexturesDir, "oak_planks.png"), new byte[] { 137, 80, 78, 71 });

            var provider = new FolderResourcePackProvider(_tempRootDir, new BuiltInAtlasProvider());
            IReadOnlyList<string> packs = provider.GetAvailablePacks();

            Assert.Contains("OptimumRealism_64x", (System.Collections.ICollection)packs);

            ResourcePackManifest manifest = provider.LoadPack("OptimumRealism_64x");
            Assert.AreEqual("OptimumRealism_64x", manifest.PackId);
            Assert.AreEqual(80, manifest.PackFormat);
            Assert.AreEqual(64, manifest.TileResolution);
            Assert.AreEqual("An fps-friendly realistic resource pack.", manifest.Description);
            Assert.IsTrue(manifest.BlockTexturePaths[BlockId.Stone].EndsWith("stone.png", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(manifest.BlockTexturePaths[BlockId.Grass].EndsWith("grass_block_top.png", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void FolderResourcePackProvider_HandlesCorruptedMcmetaAndMissingFolderGracefully()
        {
            // Trường hợp 1: pack.mcmeta bị lỗi cú pháp JSON
            string corruptedPackDir = Path.Combine(_tempRootDir, "BrokenPack_64x");
            Directory.CreateDirectory(corruptedPackDir);
            File.WriteAllText(Path.Combine(corruptedPackDir, "pack.mcmeta"), "{ invalid json !!! ");

            var provider = new FolderResourcePackProvider(_tempRootDir, new BuiltInAtlasProvider());
            ResourcePackManifest brokenManifest = null;

            Assert.DoesNotThrow(() =>
            {
                brokenManifest = provider.LoadPack("BrokenPack_64x");
            });

            Assert.IsNotNull(brokenManifest);
            Assert.AreEqual("BrokenPack_64x", brokenManifest.PackId);
            Assert.IsNotEmpty(brokenManifest.Description);

            // Trường hợp 2: Thư mục gốc hoặc gói không tồn tại -> fallback về BuiltInAtlasProvider an toàn
            var missingRootProvider = new FolderResourcePackProvider(Path.Combine(_tempRootDir, "NonExistentRoot"), new BuiltInAtlasProvider());
            ResourcePackManifest fallbackManifest = null;

            Assert.DoesNotThrow(() =>
            {
                fallbackManifest = missingRootProvider.LoadPack("NonExistentPack");
            });

            Assert.IsNotNull(fallbackManifest);
            Assert.AreEqual("default", fallbackManifest.PackId);
            Assert.AreEqual(16, fallbackManifest.TileResolution);
        }

        [Test]
        public void ResourcePackService_SwitchPack_UpdatesCurrentPack_AndFiresObserverEvent()
        {
            string packDir = Path.Combine(_tempRootDir, "OptimumRealism_64x");
            string blockTexturesDir = Path.Combine(packDir, "assets", "minecraft", "textures", "block");
            Directory.CreateDirectory(blockTexturesDir);
            File.WriteAllText(Path.Combine(packDir, "pack.mcmeta"), @"{""pack"":{""pack_format"":80,""description"":""64x Pack""}}");
            File.WriteAllBytes(Path.Combine(blockTexturesDir, "stone.png"), new byte[] { 1, 2, 3 });

            ITextureAtlasProvider provider = new FolderResourcePackProvider(_tempRootDir, new BuiltInAtlasProvider());
            var service = new ResourcePackService(provider);

            ResourcePackManifest notifiedPack = null;
            int eventCount = 0;
            service.ResourcePackChanged += manifest =>
            {
                notifiedPack = manifest;
                eventCount++;
            };

            bool switched = service.SwitchPack("OptimumRealism_64x");

            Assert.IsTrue(switched);
            Assert.AreEqual(1, eventCount);
            Assert.IsNotNull(service.CurrentPack);
            Assert.AreEqual("OptimumRealism_64x", service.CurrentPack.PackId);
            Assert.AreSame(service.CurrentPack, notifiedPack);
        }
    }
}
