using System;
using MagicEscape.Core;
using Xunit;

namespace MagicEscape.Core.Tests
{
    public class RunePuzzleLogicTests
    {
        private static RunePuzzleLogic Active()
        {
            var p = RunePuzzleLogic.CreateDefault();
            p.Activate();
            return p;
        }

        [Fact]
        public void Inactive_puzzle_ignores_input()
        {
            var p = RunePuzzleLogic.CreateDefault();
            Assert.Equal(RuneInputResult.Ignored, p.Press(RuneId.Earth));
            Assert.Equal(0, p.ProgressCount);
        }

        [Fact]
        public void Correct_sequence_solves()
        {
            var p = Active();
            bool solved = false;
            p.Solved += () => solved = true;
            Assert.Equal(RuneInputResult.Progress, p.Press(RuneId.Earth));
            Assert.Equal(RuneInputResult.Progress, p.Press(RuneId.Water));
            Assert.Equal(RuneInputResult.Progress, p.Press(RuneId.Fire));
            Assert.Equal(RuneInputResult.Solved, p.Press(RuneId.Light));
            Assert.True(p.IsSolved);
            Assert.True(solved);
            Assert.Equal(RuneInputResult.Ignored, p.Press(RuneId.Earth));
        }

        [Fact]
        public void Wrong_rune_resets_progress_and_counts_fail()
        {
            var p = Active();
            p.Press(RuneId.Earth);
            p.Press(RuneId.Water);
            Assert.Equal(RuneInputResult.Wrong, p.Press(RuneId.Shadow));
            Assert.Equal(0, p.ProgressCount);
            Assert.Equal(1, p.FailCount);
        }

        [Fact]
        public void After_wrong_must_restart_from_first_rune()
        {
            var p = Active();
            p.Press(RuneId.Earth);
            p.Press(RuneId.Wind);
            Assert.Equal(RuneInputResult.Wrong, p.Press(RuneId.Water));
            Assert.Equal(RuneInputResult.Progress, p.Press(RuneId.Earth));
        }

        [Fact]
        public void Hint_appears_after_three_fails()
        {
            var p = Active();
            p.Press(RuneId.Wind);
            p.Press(RuneId.Wind);
            Assert.False(p.ShouldShowHint);
            p.Press(RuneId.Wind);
            Assert.True(p.ShouldShowHint);
        }

        [Fact]
        public void Default_answer_matches_spec()
        {
            Assert.Equal(new[] { RuneId.Earth, RuneId.Water, RuneId.Fire, RuneId.Light }, RunePuzzleLogic.DefaultAnswer);
        }

        [Fact]
        public void Null_or_empty_answer_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RunePuzzleLogic(null));
            Assert.Throws<ArgumentException>(() => new RunePuzzleLogic(new RuneId[0]));
        }
    }

    public class PotionPuzzleLogicTests
    {
        private static PotionPuzzleLogic Active()
        {
            var p = PotionPuzzleLogic.CreateDefault();
            p.Activate();
            return p;
        }

        [Fact]
        public void Locked_cauldron_returns_NotActive_without_consuming()
        {
            var p = PotionPuzzleLogic.CreateDefault();
            var r = p.Add(ItemIds.Moonpetal);
            Assert.Equal(PotionInputResult.NotActive, r);
            Assert.False(PotionPuzzleLogic.ConsumesItem(r));
            Assert.Equal(0, p.FailCount);
        }

        [Fact]
        public void Correct_recipe_solves()
        {
            var p = Active();
            Assert.Equal(PotionInputResult.Accepted, p.Add(ItemIds.Moonpetal));
            Assert.Equal(PotionInputResult.Accepted, p.Add(ItemIds.SalamanderScale));
            Assert.Equal(PotionInputResult.Solved, p.Add(ItemIds.CrystalDust));
            Assert.True(p.IsSolved);
            Assert.Equal(PotionInputResult.NotActive, p.Add(ItemIds.Moonpetal));
        }

        [Fact]
        public void Non_ingredient_is_not_consumed_and_not_a_fail()
        {
            var p = Active();
            var r = p.Add(ItemIds.CrystalSapphire);
            Assert.Equal(PotionInputResult.NotIngredient, r);
            Assert.False(PotionPuzzleLogic.ConsumesItem(r));
            Assert.Equal(0, p.FailCount);
            Assert.Equal(PotionInputResult.NotIngredient, p.Add(null));
        }

        [Fact]
        public void Wrong_order_returns_whole_batch_for_respawn()
        {
            var p = Active();
            p.Add(ItemIds.Moonpetal);
            var r = p.Add(ItemIds.CrystalDust);
            Assert.Equal(PotionInputResult.Wrong, r);
            Assert.True(PotionPuzzleLogic.ConsumesItem(r));
            Assert.Equal(new[] { ItemIds.Moonpetal, ItemIds.CrystalDust }, p.LastFailedBatch);
            Assert.Equal(0, p.ProgressCount);
            Assert.Equal(1, p.FailCount);
        }

        [Fact]
        public void Toadstool_is_always_wrong_and_respawned()
        {
            var p = Active();
            Assert.Equal(PotionInputResult.Wrong, p.Add(ItemIds.Toadstool));
            Assert.Equal(new[] { ItemIds.Toadstool }, p.LastFailedBatch);
        }

        [Fact]
        public void Hint_after_two_fails_and_retry_still_possible()
        {
            var p = Active();
            p.Add(ItemIds.Toadstool);
            Assert.False(p.ShouldShowHint);
            p.Add(ItemIds.SalamanderScale);
            Assert.True(p.ShouldShowHint);
            p.Add(ItemIds.Moonpetal);
            p.Add(ItemIds.SalamanderScale);
            Assert.Equal(PotionInputResult.Solved, p.Add(ItemIds.CrystalDust));
        }
    }

    public class SealPuzzleLogicTests
    {
        private static SealPuzzleLogic Revealed()
        {
            var s = new SealPuzzleLogic();
            s.Activate();
            s.UseOnInscription(ItemIds.PotionRevelation);
            return s;
        }

        [Fact]
        public void Dormant_device_reports_dormant_everywhere()
        {
            var s = new SealPuzzleLogic();
            Assert.Equal(SealActionResult.Dormant, s.UseOnInscription(ItemIds.PotionRevelation));
            Assert.Equal(SealActionResult.Dormant, s.InsertCrystal(SocketSide.Left, ItemIds.CrystalSapphire));
            Assert.Equal(SealActionResult.Dormant, s.ActivateCore());
            Assert.Equal(SealStage.Dormant, s.Stage);
        }

        [Fact]
        public void Inscription_requires_potion()
        {
            var s = new SealPuzzleLogic();
            s.Activate();
            Assert.Equal(SealStage.AwaitingReveal, s.Stage);
            Assert.Equal(SealActionResult.InvalidItem, s.UseOnInscription(ItemIds.CrystalRuby));
            Assert.Equal(SealActionResult.InvalidItem, s.UseOnInscription(null));
            var r = s.UseOnInscription(ItemIds.PotionRevelation);
            Assert.Equal(SealActionResult.Revealed, r);
            Assert.True(SealPuzzleLogic.ConsumesItem(r));
            Assert.Equal(SealStage.AwaitingCrystals, s.Stage);
        }

        [Fact]
        public void Sockets_unavailable_before_reveal()
        {
            var s = new SealPuzzleLogic();
            s.Activate();
            Assert.Equal(SealActionResult.NotAvailable, s.InsertCrystal(SocketSide.Left, ItemIds.CrystalSapphire));
        }

        [Fact]
        public void Wrong_socket_is_rejected_without_consuming()
        {
            var s = Revealed();
            var r = s.InsertCrystal(SocketSide.Left, ItemIds.CrystalRuby);
            Assert.Equal(SealActionResult.Rejected, r);
            Assert.False(SealPuzzleLogic.ConsumesItem(r));
            Assert.False(s.IsSocketFilled(SocketSide.Left));
            Assert.Equal(1, s.FailCount);
        }

        [Fact]
        public void Non_crystal_item_is_invalid_not_fail()
        {
            var s = Revealed();
            Assert.Equal(SealActionResult.InvalidItem, s.InsertCrystal(SocketSide.Right, ItemIds.Moonpetal));
            Assert.Equal(0, s.FailCount);
        }

        [Fact]
        public void Full_sequence_releases_seal()
        {
            var s = Revealed();
            Assert.Equal(SealActionResult.NotAvailable, s.ActivateCore());
            Assert.Equal(SealActionResult.CrystalPlaced, s.InsertCrystal(SocketSide.Right, ItemIds.CrystalRuby));
            Assert.Equal(SealActionResult.AlreadyFilled, s.InsertCrystal(SocketSide.Right, ItemIds.CrystalRuby));
            Assert.Equal(SealActionResult.CrystalPlaced, s.InsertCrystal(SocketSide.Left, ItemIds.CrystalSapphire));
            Assert.Equal(SealStage.ReadyToRelease, s.Stage);
            Assert.Equal(SealActionResult.Released, s.ActivateCore());
            Assert.True(s.IsReleased);
        }

        [Fact]
        public void Hint_after_two_rejections()
        {
            var s = Revealed();
            s.InsertCrystal(SocketSide.Left, ItemIds.CrystalRuby);
            Assert.False(s.ShouldShowHint);
            s.InsertCrystal(SocketSide.Right, ItemIds.CrystalSapphire);
            Assert.True(s.ShouldShowHint);
        }
    }
}
