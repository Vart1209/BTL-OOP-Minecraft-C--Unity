using System;
using System.Collections.Generic;
using NUnit.Framework;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Tests
{
    /// <summary>
    /// Bộ kiểm thử tự động (Unit Tests) cho các hợp đồng dùng chung tầng Domain (Tuần T0).
    /// Đáp ứng Tiêu chí 18 trong barem [S2]: Kiểm thử tự động tách biệt hoàn toàn với UnityEngine.
    /// </summary>
    [TestFixture]
    public class DomainContractsTests
    {
        [Test]
        public void ChunkCoord_EqualityAndHashCode_WorksForDictionaryKey()
        {
            ChunkCoord c1 = new ChunkCoord(-2, 5);
            ChunkCoord c2 = new ChunkCoord(-2, 5);
            ChunkCoord c3 = new ChunkCoord(2, 5);

            Assert.That(c1, Is.EqualTo(c2));
            Assert.That(c1 == c2, Is.True);
            Assert.That(c1 != c3, Is.True);
            Assert.That(c1.GetHashCode(), Is.EqualTo(c2.GetHashCode()));
        }

        [Test]
        public void BlockPos_ArithmeticAndOffset_ReturnsCorrectCoordinates()
        {
            BlockPos origin = new BlockPos(10, 64, -5);
            BlockPos delta = new BlockPos(2, -4, 5);

            Assert.That(origin + delta, Is.EqualTo(new BlockPos(12, 60, 0)));
            Assert.That(origin - delta, Is.EqualTo(new BlockPos(8, 68, -10)));
            Assert.That(origin.Offset(0, 1, 0), Is.EqualTo(new BlockPos(10, 65, -5)));
        }

        [Test]
        public void BlockPos_ToChunkCoord_HandlesPositiveAndNegativeCoordinatesCorrectly()
        {
            // Tọa độ dương: (0..15) thuộc chunk 0, 16 thuộc chunk 1
            Assert.That(new BlockPos(0, 10, 15).ToChunkCoord(16), Is.EqualTo(new ChunkCoord(0, 0)));
            Assert.That(new BlockPos(16, 10, 31).ToChunkCoord(16), Is.EqualTo(new ChunkCoord(1, 1)));

            // Tọa độ âm: -1 phải thuộc chunk -1 (không phải chunk 0!), -16 thuộc chunk -1, -17 thuộc chunk -2
            Assert.That(new BlockPos(-1, 10, -16).ToChunkCoord(16), Is.EqualTo(new ChunkCoord(-1, -1)));
            Assert.That(new BlockPos(-17, 10, -1).ToChunkCoord(16), Is.EqualTo(new ChunkCoord(-2, -1)));
        }

        [Test]
        public void BlockPos_ToLocalPos_HandlesNegativeCoordinatesWithoutOutOfBoundsIndex()
        {
            // Tọa độ âm: -1 trong chunk kích thước 16 phải ra vị trí cục bộ là 15, -16 ra 0
            BlockPos negPos = new BlockPos(-1, 25, -16);
            BlockPos local = negPos.ToLocalPos(16);

            Assert.That(local, Is.EqualTo(new BlockPos(15, 25, 0)));
        }

        [Test]
        public void BlockPos_InvalidChunkWidth_ThrowsArgumentOutOfRangeException()
        {
            BlockPos pos = new BlockPos(1, 2, 3);
            Assert.Throws<ArgumentOutOfRangeException>(() => pos.ToChunkCoord(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => pos.ToLocalPos(-16));
        }

        [Test]
        public void BlockIdAndItemId_SharedContractIds_AreAlignedForPlaceableBlocks()
        {
            // Air và None đều bằng 0 để khởi tạo mảng mặc định là không khí / ô trống
            Assert.That((int)BlockId.Air, Is.EqualTo(0));
            Assert.That((int)ItemId.None, Is.EqualTo(0));

            // Các khối đặt được có mã số tương đồng giữa BlockId và ItemId
            Assert.That((int)BlockId.Bedrock, Is.EqualTo((int)ItemId.Bedrock));
            Assert.That((int)BlockId.Stone, Is.EqualTo((int)ItemId.Stone));
            Assert.That((int)BlockId.Dirt, Is.EqualTo((int)ItemId.Dirt));
            Assert.That((int)BlockId.Grass, Is.EqualTo((int)ItemId.Grass));
            Assert.That((int)BlockId.Wood, Is.EqualTo((int)ItemId.Wood));
            Assert.That((int)BlockId.Planks, Is.EqualTo((int)ItemId.Planks));
            Assert.That((int)BlockId.CraftingTable, Is.EqualTo((int)ItemId.CraftingTable));

            // Công cụ và thức ăn nằm ở dải ID riêng biệt (>= 100 và >= 200)
            Assert.That((int)ItemId.WoodenPickaxe, Is.GreaterThanOrEqualTo(100));
            Assert.That((int)ItemId.Apple, Is.GreaterThanOrEqualTo(200));
        }

        [Test]
        public void BlockChangedEventArgs_StoresPositionAndTransitionCorrectly()
        {
            BlockPos pos = new BlockPos(4, 12, -8);
            BlockChangedEventArgs args = new BlockChangedEventArgs(pos, BlockId.Stone, BlockId.Air);

            Assert.That(args.Position, Is.EqualTo(pos));
            Assert.That(args.OldBlock, Is.EqualTo(BlockId.Stone));
            Assert.That(args.NewBlock, Is.EqualTo(BlockId.Air));
            Assert.That(args.HasChanged, Is.True);
        }

        [Test]
        public void WorldReaderAndWriterContract_EnforcesHeightBoundsAndRaisesBlockChangedEvent()
        {
            FakeContractWorld world = new FakeContractWorld(maxHeight: 64);
            List<BlockChangedEventArgs> receivedEvents = new List<BlockChangedEventArgs>();
            world.BlockChanged += args => receivedEvents.Add(args);

            // Quy tắc R3: Tọa độ Y ngoài giới hạn [0, MaxHeight) bị từ chối
            BlockPos belowZero = new BlockPos(0, -1, 0);
            BlockPos aboveMax = new BlockPos(0, 64, 0);
            Assert.That(world.IsInBounds(belowZero), Is.False);
            Assert.That(world.IsInBounds(aboveMax), Is.False);
            Assert.That(world.SetBlock(belowZero, BlockId.Stone), Is.False);
            Assert.That(world.SetBlock(aboveMax, BlockId.Stone), Is.False);
            Assert.That(receivedEvents.Count, Is.EqualTo(0));

            // Đặt block hợp lệ: trả về true, đọc lại đúng block và phát sự kiện Observer
            BlockPos validPos = new BlockPos(5, 10, 5);
            Assert.That(world.IsInBounds(validPos), Is.True);
            Assert.That(world.SetBlock(validPos, BlockId.Grass), Is.True);
            Assert.That(world.GetBlock(validPos), Is.EqualTo(BlockId.Grass));
            Assert.That(receivedEvents.Count, Is.EqualTo(1));
            Assert.That(receivedEvents[0].Position, Is.EqualTo(validPos));
            Assert.That(receivedEvents[0].OldBlock, Is.EqualTo(BlockId.Air));
            Assert.That(receivedEvents[0].NewBlock, Is.EqualTo(BlockId.Grass));

            // Đặt lại cùng loại block không phát sự kiện thừa
            Assert.That(world.SetBlock(validPos, BlockId.Grass), Is.False);
            Assert.That(receivedEvents.Count, Is.EqualTo(1));
        }

        [Test]
        public void TerrainGeneratorContract_StrategyPattern_GeneratesLayersByHeight()
        {
            ITerrainGenerator generator = new FakeFlatTerrainGenerator(surfaceHeight: 4);

            Assert.That(generator.GetBlockAt(new BlockPos(0, 0, 0), seed: 123), Is.EqualTo(BlockId.Bedrock));
            Assert.That(generator.GetBlockAt(new BlockPos(10, 3, -5), seed: 123), Is.EqualTo(BlockId.Dirt));
            Assert.That(generator.GetBlockAt(new BlockPos(10, 4, -5), seed: 123), Is.EqualTo(BlockId.Grass));
            Assert.That(generator.GetBlockAt(new BlockPos(10, 5, -5), seed: 123), Is.EqualTo(BlockId.Air));
        }

        [Test]
        public void GameModeContract_StatePattern_EnforcesActorPermissionsBetweenSurvivalAndCreative()
        {
            IGameMode survival = new FakeSurvivalMode();
            IGameMode creative = new FakeCreativeMode();

            // Quy tắc R1 & R12: Phân quyền tác nhân (Survival vs Creative)
            Assert.That(survival.CanTakeDamage, Is.True);
            Assert.That(survival.ConsumesItemsOnPlace, Is.True);
            Assert.That(survival.InstantBreak, Is.False);
            Assert.That(survival.CanFly, Is.False);
            Assert.That(survival.CanBreakBlock(BlockId.Bedrock), Is.False);
            Assert.That(survival.CanBreakBlock(BlockId.Stone), Is.True);
            Assert.That(survival.CanBreakBlock(BlockId.Air), Is.False);

            Assert.That(creative.CanTakeDamage, Is.False);
            Assert.That(creative.ConsumesItemsOnPlace, Is.False);
            Assert.That(creative.InstantBreak, Is.True);
            Assert.That(creative.CanFly, Is.True);
            Assert.That(creative.CanBreakBlock(BlockId.Bedrock), Is.True);
            Assert.That(creative.CanBreakBlock(BlockId.Air), Is.False);
        }

        private sealed class FakeContractWorld : IWorldReader, IWorldWriter
        {
            private readonly Dictionary<BlockPos, BlockId> _blocks = new Dictionary<BlockPos, BlockId>();
            private readonly int _maxHeight;

            public int MaxHeight => _maxHeight;

            public event Action<BlockChangedEventArgs> BlockChanged;

            public FakeContractWorld(int maxHeight)
            {
                _maxHeight = maxHeight;
            }

            public bool IsInBounds(BlockPos position)
            {
                return position.Y >= 0 && position.Y < _maxHeight;
            }

            public BlockId GetBlock(BlockPos position)
            {
                if (!IsInBounds(position))
                {
                    return BlockId.Air;
                }

                return _blocks.TryGetValue(position, out BlockId id) ? id : BlockId.Air;
            }

            public bool SetBlock(BlockPos position, BlockId blockId)
            {
                if (!IsInBounds(position))
                {
                    return false;
                }

                BlockId oldBlock = GetBlock(position);
                if (oldBlock == blockId)
                {
                    return false;
                }

                _blocks[position] = blockId;
                BlockChanged?.Invoke(new BlockChangedEventArgs(position, oldBlock, blockId));
                return true;
            }
        }

        private sealed class FakeFlatTerrainGenerator : ITerrainGenerator
        {
            private readonly int _surfaceHeight;

            public FakeFlatTerrainGenerator(int surfaceHeight)
            {
                _surfaceHeight = surfaceHeight;
            }

            public BlockId GetBlockAt(BlockPos worldPosition, int seed)
            {
                if (worldPosition.Y == 0) return BlockId.Bedrock;
                if (worldPosition.Y < _surfaceHeight) return BlockId.Dirt;
                if (worldPosition.Y == _surfaceHeight) return BlockId.Grass;
                return BlockId.Air;
            }
        }

        private sealed class FakeSurvivalMode : IGameMode
        {
            public string ModeName => "Survival";
            public bool CanTakeDamage => true;
            public bool ConsumesItemsOnPlace => true;
            public bool InstantBreak => false;
            public bool CanFly => false;

            public bool CanBreakBlock(BlockId blockId)
            {
                return blockId != BlockId.Air && blockId != BlockId.Bedrock;
            }
        }

        private sealed class FakeCreativeMode : IGameMode
        {
            public string ModeName => "Creative";
            public bool CanTakeDamage => false;
            public bool ConsumesItemsOnPlace => false;
            public bool InstantBreak => true;
            public bool CanFly => true;

            public bool CanBreakBlock(BlockId blockId)
            {
                return blockId != BlockId.Air;
            }
        }
    }
}
