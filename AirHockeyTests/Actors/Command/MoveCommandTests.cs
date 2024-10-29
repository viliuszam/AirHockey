using NUnit.Framework;
using AirHockey.Actors.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Command.Tests
{
    [TestFixture]
    public class MoveCommandTests
    {
        private Entity _entity;
        private Queue<(float X, float Y, float timestamp)> _positionHistory;
        private MoveCommand _moveCommand;

        [SetUp]
        public void Setup()
        {
            _entity = new Puck { X = 0, Y = 0 };
            _positionHistory = new Queue<(float X, float Y, float timestamp)>();
            _moveCommand = new MoveCommand(_entity, _positionHistory);
        }

        [Test]
        public void ExecuteTest_HistoryAdded()
        {
            _entity.X = 10;
            _entity.Y = 20;

            _moveCommand.Execute();

            Assert.AreEqual(1, _positionHistory.Count, "Position history should have 1 entry after Execute.");
            var position = _positionHistory.Peek();
            Assert.AreEqual(10, position.X, "X position in history should match entity's X position at execution time.");
            Assert.AreEqual(20, position.Y, "Y position in history should match entity's Y position at execution time.");
        }

        [Test]
        public void UndoTest_RemovesOldPositions()
        {
            // Arrange: Add entries with timestamps beyond the TimeWindow threshold
            float currentTime = _moveCommand.GetCurrentTime();
            _positionHistory.Enqueue((0, 0, currentTime - 10)); // Older than TimeWindow
            _positionHistory.Enqueue((5, 5, currentTime - 6));  // Just outside TimeWindow
            _positionHistory.Enqueue((10, 10, currentTime - 1)); // Within TimeWindow

            _moveCommand.Undo();

            // Act: Only entries within the TimeWindow should remain
            Assert.AreEqual(1, _positionHistory.Count, "Only entries within the TimeWindow should remain after Undo.");
            var remainingPosition = _positionHistory.Peek();
            Assert.AreEqual(10, remainingPosition.X, "Remaining X position should match last position within TimeWindow.");
            Assert.AreEqual(10, remainingPosition.Y, "Remaining Y position should match last position within TimeWindow.");

            // Check that entity's position is set to last available position in history
            Assert.AreEqual(10, _entity.X, "Entity's X position should revert to the last recorded X within TimeWindow.");
            Assert.AreEqual(10, _entity.Y, "Entity's Y position should revert to the last recorded Y within TimeWindow.");
        }

        [Test]
        public void GetEntityTest_ReturnsEntity()
        {
            var result = _moveCommand.getEntity();
            Assert.AreSame(_entity, result, "getEntity should return the initialized entity instance.");
        }

        [Test]
        public void UndoTest_PositionReverted()
        {
            _entity.X = 5;
            _entity.Y = 5;
            _moveCommand.Execute();

            _entity.X = 10;
            _entity.Y = 10;
            _moveCommand.Execute();

            _moveCommand.Undo();

            Assert.AreEqual(5, _entity.X, "Entity X position should revert to the last recorded position after Undo.");
            Assert.AreEqual(5, _entity.Y, "Entity Y position should revert to the last recorded position after Undo.");
        }

    }
}