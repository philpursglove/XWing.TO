using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using XWingTO.Core;
using XWingTO.Core.Messages;
using XWingTO.Data;

namespace XWingTO.Functions
{
    public class ProcessGameScore
    {
        private readonly IRepository<Game, Guid> _gameRepository;
        private readonly IRepository<TournamentPlayer, Guid> _tournamentPlayerRepository;

        public ProcessGameScore(IRepository<Game, Guid> gameRepository, IRepository<TournamentPlayer, Guid> tournamentPlayerRepository)
        {
            _gameRepository = gameRepository;
            _tournamentPlayerRepository = tournamentPlayerRepository;
        }

        [Function("ProcessGameScore")]
        public async Task Run([QueueTrigger("gamescorequeue", Connection = "XWingTO.Queue")]string scoreMessage, ILogger log)
        {
            GameScoreMessage gameScoreMessage = JsonConvert.DeserializeObject<GameScoreMessage>(scoreMessage);

            Game game = await _gameRepository.Get(gameScoreMessage.GameId);

            TournamentPlayer player1 = await _tournamentPlayerRepository.Get(game.TournamentPlayer1Id);
            TournamentPlayer player2 = await _tournamentPlayerRepository.Get(game.TournamentPlayer2Id);

            if (gameScoreMessage.Player1Concede || gameScoreMessage.Player2Concede)
            {
                // Concession
                if (gameScoreMessage.Player1Concede)
                {
                    // Player2 +3 points, 20 mission points
                    player2.Points += 3;
                    player2.MissionPoints += 20;
                }
                else
                {
                    // Player1 +3 points, 20 mission points
                    player1.Points += 3;
                    player1.MissionPoints += 20;
                }
            }
            else
            {
                player1.MissionPoints += gameScoreMessage.Player1Points;
                player2.MissionPoints += gameScoreMessage.Player2Points;

                if (gameScoreMessage.Player1Points == gameScoreMessage.Player2Points)
                {
                    // Draw
                    player1.Points += 1;
                    player2.Points += 1;

                }
                else
                {
                    if (gameScoreMessage.Player1Points > gameScoreMessage.Player2Points)
                    {
                        // Player 1 win
                        player1.Points += 3;
                    }
                    else
                    {
                        // Player 2 win
                        player2.Points += 3;
                    }
                }
            }

            // Drops
            if (gameScoreMessage.Player1Drop)
            {
                player1.Dropped = true;
            }

            if (gameScoreMessage.Player2Drop)
            {
                player2.Dropped = true;
            }

            game.Player1MissionPoints = gameScoreMessage.Player1Points;
            game.Player2MissionPoints = gameScoreMessage.Player2Points;
            game.Turns = gameScoreMessage.Turns;
            game.OutOfTime = gameScoreMessage.OutOfTime;

            await _gameRepository.Update(game);

            await _tournamentPlayerRepository.Update(player1);
            await _tournamentPlayerRepository.Update(player2);
        }
    }
}
