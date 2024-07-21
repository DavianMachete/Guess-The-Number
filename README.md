
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
