using System.Collections.Generic;
using MagicEscape.Core;
using Xunit;

namespace MagicEscape.Core.Tests
{
    public class ProgressionModelTests
    {
        [Fact]
        public void Initial_states_match_spec()
        {
            var m = new ProgressionModel();
            Assert.Equal(GameFlowState.Title, m.Flow);
            Assert.Equal(PuzzleState.Active, m.GetState(PuzzleId.Rune));
            Assert.Equal(PuzzleState.Locked, m.GetState(PuzzleId.Potion));
            Assert.Equal(PuzzleState.Locked, m.GetState(PuzzleId.Seal));
            Assert.False(m.ExitOpen);
        }

        [Fact]
        public void Potion_cannot_be_solved_before_rune()
        {
            var m = new ProgressionModel();
            Assert.False(m.MarkSolved(PuzzleId.Potion));
            Assert.False(m.MarkSolved(PuzzleId.Seal));
            Assert.Equal(PuzzleState.Locked, m.GetState(PuzzleId.Potion));
        }

        [Fact]
        public void Puzzles_unlock_in_order_and_open_exit()
        {
            var m = new ProgressionModel();
            var log = new List<string>();
            m.PuzzleStateChanged += (id, s) => log.Add(id + ":" + s);
            bool exit = false;
            m.ExitOpened += () => exit = true;

            Assert.True(m.MarkSolved(PuzzleId.Rune));
            Assert.Equal(PuzzleState.Active, m.GetState(PuzzleId.Potion));
            Assert.Equal(PuzzleState.Locked, m.GetState(PuzzleId.Seal));
            Assert.True(m.MarkSolved(PuzzleId.Potion));
            Assert.Equal(PuzzleState.Active, m.GetState(PuzzleId.Seal));
            Assert.True(m.MarkSolved(PuzzleId.Seal));
            Assert.True(m.ExitOpen);
            Assert.True(exit);
            Assert.Equal(3, m.SolvedCount);
            Assert.Equal(new[]
            {
                "Rune:Solved", "Potion:Active", "Potion:Solved", "Seal:Active", "Seal:Solved"
            }, log);
        }

        [Fact]
        public void Solving_twice_is_ignored()
        {
            var m = new ProgressionModel();
            Assert.True(m.MarkSolved(PuzzleId.Rune));
            Assert.False(m.MarkSolved(PuzzleId.Rune));
        }

        [Fact]
        public void Escape_only_when_exit_open_and_playing()
        {
            var m = new ProgressionModel();
            m.StartGame();
            Assert.False(m.Escape());
            m.MarkSolved(PuzzleId.Rune);
            m.MarkSolved(PuzzleId.Potion);
            m.MarkSolved(PuzzleId.Seal);
            m.Pause();
            Assert.False(m.Escape());
            m.Resume();
            Assert.True(m.Escape());
            Assert.Equal(GameFlowState.Cleared, m.Flow);
        }

        [Fact]
        public void Flow_transitions_are_guarded()
        {
            var m = new ProgressionModel();
            Assert.False(m.Pause());
            Assert.False(m.Resume());
            Assert.True(m.StartGame());
            Assert.False(m.StartGame());
            Assert.True(m.Pause());
            Assert.True(m.Resume());
        }

        [Fact]
        public void Time_accumulates_only_while_playing()
        {
            var m = new ProgressionModel();
            m.Tick(5f);
            Assert.Equal(0f, m.ElapsedSeconds);
            m.StartGame();
            m.Tick(2f);
            m.Pause();
            m.Tick(10f);
            m.Resume();
            m.Tick(1f);
            m.Tick(-3f);
            Assert.Equal(3f, m.ElapsedSeconds);
        }
    }

    public class InventoryModelTests
    {
        [Fact]
        public void Add_until_full_then_refuse()
        {
            var inv = new InventoryModel(2);
            Assert.True(inv.TryAdd("a"));
            Assert.True(inv.TryAdd("b"));
            Assert.True(inv.IsFull);
            Assert.False(inv.TryAdd("c"));
            Assert.Equal(2, inv.Count);
        }

        [Fact]
        public void Rejects_null_or_empty()
        {
            var inv = new InventoryModel();
            Assert.False(inv.TryAdd(null));
            Assert.False(inv.TryAdd(""));
            Assert.Equal(8, inv.Capacity);
        }

        [Fact]
        public void Selected_item_follows_selection()
        {
            var inv = new InventoryModel();
            Assert.Null(inv.SelectedItemId);
            inv.TryAdd("a");
            inv.TryAdd("b");
            Assert.Equal("a", inv.SelectedItemId);
            inv.Select(1);
            Assert.Equal("b", inv.SelectedItemId);
            inv.Select(5);
            Assert.Null(inv.SelectedItemId);
            inv.Select(99);
            Assert.Equal(5, inv.SelectedIndex);
        }

        [Fact]
        public void Removing_selected_last_item_moves_selection_back()
        {
            var inv = new InventoryModel();
            inv.TryAdd("a");
            inv.TryAdd("b");
            inv.Select(1);
            Assert.True(inv.Remove("b"));
            Assert.Equal(0, inv.SelectedIndex);
            Assert.Equal("a", inv.SelectedItemId);
            Assert.False(inv.Remove("zzz"));
        }

        [Fact]
        public void Wheel_selection_wraps()
        {
            var inv = new InventoryModel(3);
            inv.SelectNext(-1);
            Assert.Equal(2, inv.SelectedIndex);
            inv.SelectNext(1);
            Assert.Equal(0, inv.SelectedIndex);
        }

        [Fact]
        public void Changed_event_fires()
        {
            var inv = new InventoryModel();
            int n = 0;
            inv.Changed += () => n++;
            inv.TryAdd("a");
            inv.Select(1);
            inv.Remove("a");
            inv.Clear();
            Assert.Equal(4, n);
        }
    }

    /// <summary>퍼즐 3종을 ProgressionModel로 묶은 전체 탈출 시나리오.</summary>
    public class FullRunTests
    {
        [Fact]
        public void Whole_escape_flow()
        {
            var prog = new ProgressionModel();
            var inv = new InventoryModel();
            var rune = RunePuzzleLogic.CreateDefault();
            var potion = PotionPuzzleLogic.CreateDefault();
            var seal = new SealPuzzleLogic();

            prog.PuzzleStateChanged += (id, s) =>
            {
                if (s != PuzzleState.Active) return;
                if (id == PuzzleId.Potion) potion.Activate();
                if (id == PuzzleId.Seal) seal.Activate();
            };
            prog.StartGame();
            rune.Activate();

            foreach (var r in RunePuzzleLogic.DefaultAnswer) rune.Press(r);
            Assert.True(rune.IsSolved);
            prog.MarkSolved(PuzzleId.Rune);
            inv.TryAdd(ItemIds.CrystalSapphire);

            foreach (var id in PotionPuzzleLogic.DefaultRecipe)
            {
                inv.TryAdd(id);
                Assert.True(PotionPuzzleLogic.ConsumesItem(potion.Add(id)));
                inv.Remove(id);
            }
            Assert.True(potion.IsSolved);
            prog.MarkSolved(PuzzleId.Potion);
            inv.TryAdd(ItemIds.PotionRevelation);
            inv.TryAdd(ItemIds.CrystalRuby);

            Assert.Equal(SealActionResult.Revealed, seal.UseOnInscription(ItemIds.PotionRevelation));
            inv.Remove(ItemIds.PotionRevelation);
            seal.InsertCrystal(SocketSide.Left, ItemIds.CrystalSapphire);
            inv.Remove(ItemIds.CrystalSapphire);
            seal.InsertCrystal(SocketSide.Right, ItemIds.CrystalRuby);
            inv.Remove(ItemIds.CrystalRuby);
            Assert.Equal(SealActionResult.Released, seal.ActivateCore());
            prog.MarkSolved(PuzzleId.Seal);

            Assert.True(prog.Escape());
            Assert.Equal(0, inv.Count);
        }
    }
}
