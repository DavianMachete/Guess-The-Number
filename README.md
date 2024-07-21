<h1 align="center">Guess The Number</h1>


<h2 align="center">Game Description</h2>
Welcome to an exciting multiplayer guessing game! This is a simple multiplayer game using Unity and Microsoft Orleans. 2 players in the room need to guess a number, and whoever is closer to the Croupier (server) number gets the points.

<h2 align="center">How It Works</h2>

   1. Authorization: New players must be authorized upon connecting to the server.
   2. Queueing: Players start by joining a queue.
   3. Matchmaking: Once two players are queued, they enter a room.
   4. Round Start: The server picks a number between 0 and 100.
   5. Guessing: Each player picks a number between 0 and 100 and submits it via the console.
   6. Scoring: The player whose guess is closest to the server's number earns a point.
   7. Winning: The first player to reach 5 points wins the game.

   * Statistics: Win/loss statistics for each player are stored using the Microsoft Orleans persistence system.


---
<h2 align="left">Gameplay in macOS terminal, without Unity Engine</h2>

https://github.com/user-attachments/assets/b2c44782-7234-4a51-aaed-25f685eb908d

<h2 align="left">Building and running the game without Unity Engine</h2>

To download and run the game, follow these steps:

1. Download the repozitory.
2. In the command terminal navigate to the folder that holds the unzipped game codes.
3. Navigate to the "Guess-The-Number-Server" folder.
4. Run the server by the following command:

   ``` bash
   dotnet run --project Server
   ```

   (You should see the server startup and eventually print the line `Press Enter to terminate...`.)
5. In a separate terminal, execute the following to start the client and play the game:

   ``` bash
   dotnet run --project Client
   ```

6. Repeat the 5th step for multiple clients
