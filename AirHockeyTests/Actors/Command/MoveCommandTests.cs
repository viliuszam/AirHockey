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