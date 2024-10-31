using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using AirHockey.Services;
using AirHockey.Managers;
using Microsoft.Extensions.Logging;
using Moq;
using AirHockey.Analytics;
using AirHockey.Strategies;

namespace AirHockeyTests.Integration
{
    [TestFixture]
    public class GameHubIntegrationTests : IAsyncDisposable
    {
        private TestServer _testServer;
        private HubConnection _player1Connection;
        private HubConnection _player2Connection;
        private const string TEST_ROOM = "TEST123";
        private List<string> _receivedMessages;
        private Dictionary<string, object> _player1ReceivedData;
        private Dictionary<string, object> _player2ReceivedData;

        class StubGameAnalytics : IGameAnalytics
        {
            public List<string> LoggedEvents { get; } = new List<string>();

            public void LogEvent(string roomCode, string eventName, Dictionary<string, object> eventData)
            {
                LoggedEvents.Add($"Room ({roomCode}):" + eventName + "- " + eventData);
            }
        }
        public class GameHubDriver
        {
            private readonly HubConnection _connection;

            public GameHubDriver(HubConnection connection)
            {
                _connection = connection;
            }

            public Task CreateRoom(string roomCode, string nickname)
            {
                return _connection.InvokeAsync("CreateRoom", roomCode, nickname);
            }

            public Task JoinRoom(string roomCode, string nickname)
            {
                return _connection.InvokeAsync("JoinRoom", roomCode, nickname);
            }

            public Task UpdateInput(string roomCode, string playerId, bool up, bool down, bool left, bool right, bool attack)
            {
                return _connection.InvokeAsync("UpdateInput", roomCode, playerId, up, down, left, right, attack);
            }
        }

        private GameHubDriver _player1Driver;
        private GameHubDriver _player2Driver;

        private StubGameAnalytics _stubGameAnalytics;

        [SetUp]
        public async Task Setup()
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddControllersWithViews();
                    services.AddSignalR();
                    services.AddSingleton<IGameAnalytics, StubGameAnalytics>();
                    //services.AddSingleton<IGameAnalytics>(sp => new FileLoggerAdapter("./"));
                    services.AddSingleton<GameService>();
                    services.AddSingleton<ICollision, BaseCollision>();
                    services.AddSingleton<ICollision, WallCollision>();
                    services.AddSingleton<ICollision, QuickCollision>();
                    services.AddSingleton<ICollision, TeleportCollision>();
                    services.AddSingleton<ICollision, BouncyCollision>();
                    services.AddSingleton<ICollision, ScrolingCollision>();
                    services.AddRazorPages();
                })
                .Configure(app =>
                {

                    app.UseHttpsRedirection();
                    app.UseStaticFiles();
                    app.UseRouting();
                    app.UseAuthorization();
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHub<GameHub>("/gameHub");
                    });

                });

            _testServer = new TestServer(webHostBuilder);

            _stubGameAnalytics = _testServer.Services.GetService<IGameAnalytics>() as StubGameAnalytics;

            Assert.NotNull(_stubGameAnalytics, "Analytics could not be retrieved.");

            _player1Connection = new HubConnectionBuilder()
                .WithUrl($"{_testServer.BaseAddress}gameHub", options =>
                {
                    options.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
                })
                .Build();

            _player2Connection = new HubConnectionBuilder()
                .WithUrl($"{_testServer.BaseAddress}gameHub", options =>
                {
                    options.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
                })
                .Build();

            _receivedMessages = new List<string>();
            _player1ReceivedData = new Dictionary<string, object>();
            _player2ReceivedData = new Dictionary<string, object>();

            _player1Connection.On<string, string>("AssignPlayer", (playerId, nickname) =>
            {
                _player1ReceivedData["PlayerId"] = playerId;
                _player1ReceivedData["Nickname"] = nickname;
            });

            _player1Connection.On("WaitingForPlayer", () =>
            {
                _receivedMessages.Add("Player1WaitingForPlayer");
            });

            _player1Connection.On<string, string, int, int>("StartGame",
                (player1Nickname, player2Nickname, player1Score, player2Score) =>
                {
                    _player1ReceivedData["GameStarted"] = true;
                    _player1ReceivedData["Player1Score"] = player1Score;
                    _player1ReceivedData["Player2Score"] = player2Score;
                });

            _player2Connection.On<string, string>("AssignPlayer", (playerId, nickname) =>
            {
                _player2ReceivedData["PlayerId"] = playerId;
                _player2ReceivedData["Nickname"] = nickname;
            });

            _player2Connection.On<string, string, int, int>("StartGame",
                (player1Nickname, player2Nickname, player1Score, player2Score) =>
                {
                    _player2ReceivedData["GameStarted"] = true;
                    _player2ReceivedData["Player1Score"] = player1Score;
                    _player2ReceivedData["Player2Score"] = player2Score;
                });

            _player1Connection.On<string>("RoomCreationFailed", (message) =>
            {
                _player1ReceivedData["CreationFailed"] = message;
            });

            _player1Connection.On<string>("RoomNotFound", (message) =>
            {
                _player1ReceivedData["NotFound"] = message;
            });

            _player2Connection.On<string>("RoomCreationFailed", (message) =>
            {
                _player2ReceivedData["CreationFailed"] = message;
            });

            _player2Connection.On<string>("RoomNotFound", (message) =>
            {
                _player2ReceivedData["NotFound"] = message;
            });

            _player1Driver = new GameHubDriver(_player1Connection);
            _player2Driver = new GameHubDriver(_player2Connection);

            await _player1Connection.StartAsync();
            await _player2Connection.StartAsync();

            Assert.That(_player1Connection.State, Is.EqualTo(HubConnectionState.Connected));
            Assert.That(_player2Connection.State, Is.EqualTo(HubConnectionState.Connected));
        }

        [TearDown]
        public async Task TearDown()
        {
            await _player1Connection.DisposeAsync();
            await _player2Connection.DisposeAsync();
            _testServer.Dispose();
        }

        [Test]
        public async Task Goal_Scored_WhenPuckIsTeleportedToGoal_LogsGoalEvent()
        {
            await _player1Driver.CreateRoom(TEST_ROOM, "Player1");
            await _player2Driver.JoinRoom(TEST_ROOM, "Player2");
            var player2Id = _player2ReceivedData["PlayerId"].ToString();

            var room = GameSessionManager.Instance.GetRoom(TEST_ROOM);
            var puck = room.Puck;

            // set puck in goal
            puck.X = 10;
            puck.Y = 240;

            // send some updates
            for (int i = 0; i < 50; i++)
            {
                await _player1Driver.UpdateInput(TEST_ROOM, player2Id, true, false, true, false, false);
            }

            Assert.That(_stubGameAnalytics.LoggedEvents, Has.Some.Contains("GoalScored"));
        }

        [SetUp]
        public void TestSetup()
        {
            _receivedMessages.Clear();
            _player1ReceivedData.Clear();
            _player2ReceivedData.Clear();
            GameSessionManager.Instance.ActiveRooms.Clear();
        }

        [Test]
        public async Task CreateRoom_WhenRoomDoesNotExist_CreatesRoomSuccessfully()
        {
            await _player1Driver.CreateRoom(TEST_ROOM, "Player1");

            Assert.That(_receivedMessages, Contains.Item("Player1WaitingForPlayer"));
            Assert.That(_player1ReceivedData.ContainsKey("PlayerId"), Is.True);
            Assert.That(GameSessionManager.Instance.RoomExists(TEST_ROOM), Is.True);
        }

        [Test]
        public async Task CreateRoom_WhenRoomExists_FailsToCreateRoom()
        {
            await _player2Driver.CreateRoom(TEST_ROOM, "Player2");
            await _player1Driver.CreateRoom(TEST_ROOM, "Player1");

            await Task.Delay(100);

            Assert.That(_player1ReceivedData["CreationFailed"], Is.EqualTo("Room " + TEST_ROOM + " already exists."));
        }

        [Test]
        public async Task JoinRoom_WhenRoomExistsAndNotFull_JoinsSuccessfully()
        {
            await _player1Driver.CreateRoom(TEST_ROOM, "Player1");
            await _player2Driver.JoinRoom(TEST_ROOM, "Player2");

            var room = GameSessionManager.Instance.GetRoom(TEST_ROOM);
            Assert.That(room.Players.Count, Is.EqualTo(2));
            Assert.That(_player1ReceivedData["GameStarted"], Is.True);
            Assert.That(_player2ReceivedData["GameStarted"], Is.True);
        }

        [Test]
        public async Task JoinRoom_WhenRoomDoesNotExist_FailsToJoin()
        {
            await _player1Driver.JoinRoom("NONEXISTENT", "Player1");
            await Task.Delay(100);
            Assert.That(_player1ReceivedData["NotFound"], Is.EqualTo("The room you are trying to join (NONEXISTENT) does not exist."));
        }

        [Test]
        public async Task UpdateInput_SendsPlayerMovementCorrectly()
        {
            await _player1Driver.CreateRoom(TEST_ROOM, "Player1");
            await _player2Driver.JoinRoom(TEST_ROOM, "Player2");
            var player1Id = _player1ReceivedData["PlayerId"].ToString();

            await _player1Driver.UpdateInput(TEST_ROOM, player1Id, true, false, false, true, false);

            var room = GameSessionManager.Instance.GetRoom(TEST_ROOM);
            var player = room.GetPlayerById(player1Id);
            Assert.That(player, Is.Not.Null);
        }

        [Test]
        public async Task PlayerDisconnect_RemovesPlayerAndNotifiesOthers()
        {
            await _player1Driver.CreateRoom(TEST_ROOM, "Player1");
            await _player2Driver.JoinRoom(TEST_ROOM, "Player2");

            bool player2ReceivedDisconnectMessage = false;
            _player2Connection.On<string>("PlayerDisconnected", (message) =>
            {
                player2ReceivedDisconnectMessage = true;
            });

            // simulate disconnection
            await _player1Connection.StopAsync();
            await Task.Delay(100);
            await _player1Connection.StartAsync();

            Assert.That(player2ReceivedDisconnectMessage, Is.True);
            Assert.That(GameSessionManager.Instance.RoomExists(TEST_ROOM), Is.False);
        }

        [Test]
        public async Task PlayerMovement_ConfinedWithinBounds()
        {
            await _player1Driver.CreateRoom(TEST_ROOM, "Player1");
            await _player2Driver.JoinRoom(TEST_ROOM, "Player2");
            var player1Id = _player1ReceivedData["PlayerId"].ToString();

            for (int i = 0; i < 2000; i++)
            {
                await _player1Driver.UpdateInput(TEST_ROOM, player1Id, true, false, false, false, false);
            }

            var room = GameSessionManager.Instance.GetRoom(TEST_ROOM);
            var player = room.GetPlayerById(player1Id);

            const float MIN_X = 0f;
            const float MAX_X = 855f;
            const float MIN_Y = 0f;
            const float MAX_Y = 541f;
            Assert.That(player.X, Is.InRange(MIN_X, MAX_X));
            Assert.That(player.Y, Is.InRange(MIN_Y, MAX_Y));
        }

        public async ValueTask DisposeAsync()
        {
            if (_player1Connection != null)
            {
                await _player1Connection.DisposeAsync();
            }
            if (_player2Connection != null)
            {
                await _player2Connection.DisposeAsync();
            }
            if (_testServer != null)
            {
                _testServer.Dispose();
            }
        }
    }
}
