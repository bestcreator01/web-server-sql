using Communications;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace WebServer
{
    /// <summary>
    /// Author:     Seoin Kim and Gloria Shin
    /// Partner:    Seoin Kim and Gloria Shin
    /// Date:       19-Apr-2023
    /// Course:     CS 3500, University of Utah, School of Computing
    /// Copyright:  CS 3500, Gloria Shin, and Seoin Kim - This work may not 
    /// be copied for use in Academic Courswork.
    /// 
    /// We, Seoin Kim and Gloria Shin, certify that we wrote this code from scratch and did not copy it in part or whole from another source. 
    /// All references used in the completion of the assignments are cited in my README file.
    /// 
    /// File Contents
    /// 
    ///     This class contains the implementation of a WebServer.
    ///     
    /// </summary>
    public class WebServer
    {
        /* FIELDS */

        /// <summary>
        /// keep track of how many requests have come in.  Just used
        /// for display purposes.
        /// </summary>
        static private int counter = 0;

        /// <summary>
        ///     The information necessary for the program to connec to the Database.
        /// </summary>
        public static readonly string connectionString;

        /// <summary>
        ///     The name of a player.
        /// </summary>
        private static string playerName = "";

        /// <summary>
        ///     Default constructor - sets up the SQL connection.
        /// </summary>
        static WebServer()
        {
            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<WebServer>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("WebServerSecrets");

            connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = SelectedSecrets["server_name"],
                InitialCatalog = SelectedSecrets["DBName"],
                UserID = SelectedSecrets["UserID"],
                Password = SelectedSecrets["DBPassword"],
                Encrypt = false
            }.ConnectionString;
        }

        /// <summary>
        ///     Main function of this program.
        /// </summary>
        /// <param name="args"> command line arguments </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Web Server running!");
            Console.WriteLine(connectionString);

            Networking webServer = new(NullLogger.Instance, OnClientConnect, OnDisconnect, OnMessage, '\n');
            webServer.WaitForClients(11001, true);

            Console.ReadLine();
        }


        /* HEADER */

        /// <summary>
        /// Create the HTTP message header, containing items such as
        /// the "HTTP/1.1 200 OK" message.
        /// 
        /// See: https://www.tutorialspoint.com/http/http_responses.htm
        /// 
        /// Warning, don't forget that there have to be new messages at the
        /// end of this message!
        /// </summary>
        /// <param name="length"> how big a message are we sending</param>
        /// <param name="type"> usually html, but could be css</param>
        /// <returns>returns a string with the message header</returns>
        private static string BuildHTTPResponseHeader(int length, string type = "text/html")
        {
            try
            {
                return $@"
HTTP/1.1 200 OK
Date: {DateTime.Now}
Server: localhost:11001
Last-Modified: Fri, 21 Apr 2023 17:46:35 GMT
Content-Length: {length}
Content-Type: {type}
Connection: Closed
";
            }
            catch
            {
                return $@"
HTTP/1.1 404 Not Found
Date: {DateTime.Now}
Server: localhost:11001
Content-Length: {length}
Content-Type: {type}
Connection: Closed
";
            }
        }

        /* BODY */

        /// <summary>
        ///   Create a web page!  The body of the returned message is the web page
        ///   "code" itself. Usually this would start with the doctype tag followed by the HTML element.  Take a look at:
        ///   https://www.sitepoint.com/a-basic-html5-template/
        /// </summary>
        /// <returns> A string the represents a web page.</returns>
        private static string BuildHTTPBody()
        {
            return $@"
<html>
<head>
<meta charset='UTF-8'>
<title>My Web Page</title>
</head>
<body>
<h1>My {counter} Times Heading</h1>
<p>I am Seoin and Gloria is next to me. How are you?</p>
</body>
</html>
";
        }

        /// <summary>
        ///      Builds the body of an HTTP response containing the high scores.
        /// </summary>
        /// <returns> The body of the HTTP response containing the high scores. </returns>
        private static string BuildHTTPHighScoresBody()
        {
            // TODO

            //CreateDBTablesPage();

            string playersTable = BuildPlayersTable();

            return $@"
<html>
<head>
<meta charset='UTF-8'>
<title>My Web Page</title>
</head>
<body>
<h1>My {counter} Times Heading</h1>
<p>I am Seoin and Gloria is next to me. I'm Good.</p>

{playersTable}

</body>
</html>
";
        }

        /// <summary>
        ///     Builds the HTTP response body for retrieveing scores for a 
        ///     a particular player.
        /// </summary>
        /// <returns> The HTTP response body for retrieving scores for a particular player. </returns>
        private static string BuildHTTPScoreForParticularPlayerBody()
        {
            if (DoesPlayerNameExist("Game", playerName))
            {
                string body = "";

                // Using playerName on the field, get a score of the particular player from DB.
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Execute the SQL query using the established connection.
                    string query = $"SELECT HeartBeat, MAX(Mass) FROM Mass WHERE playerName = '{playerName}'"; // TODO - modify to a high score for each game

                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    StringBuilder bodyBuilder = new StringBuilder();
                    bodyBuilder.Append("<html>");
                    bodyBuilder.Append($"<h1>{playerName}'s Scores</h1>");
                    bodyBuilder.Append("<table>");
                    bodyBuilder.Append("<tr><th>Time</th><th>High Score</th></tr>");

                    while (reader.Read())
                    {
                        int heartbeat = reader.GetInt32(0); // ?
                        int highScore = reader.GetInt32(1); // ?
                        bodyBuilder.Append($"<tr><td>{heartbeat}</td><td>{highScore}</td></tr>");
                    }

                    bodyBuilder.Append("</table>");
                    bodyBuilder.Append("</html");

                    body = bodyBuilder.ToString();
                }

                return body;
            }
            else
            {
                // Return a error body since there is no such a player.
                return BuildHTTPErrorBody();
            }

        }

        /// <summary>
        ///     Constructs the HTML body of a page to display a message 
        ///     indicating successful insertion of data into the Scores table.
        /// </summary>
        /// <returns> the HTML response body for an insert page. </returns>
        private static string BuildHTTPInsertDataBody()
        {
            return $@"
<html>
<head>
<meta charset='UTF-8'>
<title> Inserted data into the Scores table successfully! </title>
</head>
<body>
<h1> You want to go to the main page? </h1>
<a href='http://localhost:11001'> Main Page </a>
</body>
</html>
";
        }

        /// <summary>
        ///     Checks if a targeting player name exists in the Game table.
        /// </summary>
        /// <returns> true if the player name exists in the Game table. </returns>
        private static bool DoesPlayerNameExist(string tableName, string playerName)
        {
            bool playerExists = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT COUNT(*) FROM {tableName} WHERE playerName = '{playerName}'";

                SqlCommand command = new SqlCommand(query, connection);
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    playerExists = true;
                }
            }

            return playerExists;
        }

        /// <summary>
        ///     Builds the HTTP response body for creating a new score entry.
        /// </summary>
        /// <returns> The HTTP response body for creating a new score entry. </returns>
        private static string BuildHTTPCreateBody()
        {
            return $@"
<html>
<head>
<meta charset='UTF-8'>
<title> Created database tables successfully! </title>
</head>
<body>
<h1> You want to go to the main page? </h1>
<a href='http://localhost:11001'> Main Page </a>
</body>
</html>
";
        }

        /// <summary>
        ///     Builds the body of an HTTP response containing an error message.
        /// </summary>
        /// <returns> The body of the HTTP response containing an error message. </returns>
        private static string BuildHTTPErrorBody()
        {
            return $@"
<html>
<head>
<meta charset='UTF-8'>
<title> Page Not Found </title>
</head>
<body>
<h1> This webpage does not exist </h1>
<p> You want to go to the main page? </p>
<a href='http://localhost:11001'> Main Page </a>
</body>
</html>
";
        }

        /* PAGES */

        /// <summary> 
        ///     Builds the Main page.
        ///
        ///  The Header must follow the header protocol.
        ///  The body should follow the HTML doc protocol.
        /// </summary>
        /// <returns> the complete HTTP message</returns>
        private static string BuildMainPage()
        {
            return BuildPage(BuildHTTPBody());
        }

        /// <summary>
        ///     Builds the High Scores page.
        /// </summary>
        /// <returns> the complete HTTP message for Scores page </returns>
        private static string BuildHighScoresPage()
        {
            return BuildPage(BuildHTTPHighScoresBody());
        }

        /// <summary>
        ///     Builds the page where you query the DB for all scores associated 
        ///     with the given name and return an HTML web page with a summary
        ///     of the time and high score for each game played by the player.
        /// </summary>
        /// <returns> the complete HTTP message for Scores For Particular Player page </returns>
        private static string BuildScoresForParticularPlayerPage()
        {
            return BuildPage(BuildHTTPScoreForParticularPlayerBody());
        }

        /// <summary>
        ///     Builds an HTML web page with a form that allows the user to insert new data
        ///     into the Scores table of the database.
        /// </summary>
        /// <returns> the complete HTTP message for Insert page. </returns>
        private static string BuildInsertDataPage()
        {
            return BuildPage(BuildHTTPInsertDataBody());
        }

        /// <summary>
        ///     Builds the page where you can build the required database tables
        ///     and seed them with some dummy data.
        /// </summary>
        /// <returns> the complete HTTP message for Create page </returns>
        private static string BuildCreatePage()
        {
            return BuildPage(BuildHTTPCreateBody());
        }

        /// <summary>
        ///     Builds the page where you show an error message on the webpage.
        /// </summary>
        /// <returns> the complete HTTP message for Error page </returns>
        private static string BuildErrorPage()
        {
            return BuildPage(BuildHTTPErrorBody());
        }

        /// <summary>
        ///     Helper method to build a complete HTTP response page by 
        ///     combining the HTTP response header and the page body.
        ///     
        ///     Creates a message message string to send back to the connecting
        ///     program (i.e., the web browser).  The string is of the form:
        /// 
        ///     HTTP Header
        ///     [new message]
        ///     HTTP Body
        /// </summary>
        /// <param name="body"> The body content of the page. </param>
        /// <returns> A complete HTTP response page as a string. </returns>
        private static string BuildPage(string body)
        {
            string header = BuildHTTPResponseHeader(body.Length);

            return header + "\r\n" + body;
        }

        /* CALLBACK */

        /// <summary>
        /// Basic connect handler - i.e., a browser has connected!
        /// Print an information message
        /// </summary>
        /// <param name="channel"> the Networking connection</param>

        internal static void OnClientConnect(Networking channel)
        {
            Console.WriteLine($"A client is connected!: {channel.ID} ");
        }

        /// <summary>
        ///   <para>
        ///     When a request comes in (from a browser) this method will
        ///     be called by the Networking code.  Each message of the HTTP request
        ///     will come as a separate message.  The "message" we are interested in
        ///     is a PUT or GET request.  
        ///   </para>
        ///   <para>
        ///     The following body are actionable:
        ///   </para>
        ///   <para>
        ///      get highscore - respond with a highscore page
        ///   </para>
        ///   <para>
        ///      get favicon - don't do anything (we don't support this)
        ///   </para>
        ///   <para>
        ///      get scores/name - along with a name, respond with a list of scores for the particular user
        ///   <para>
        ///      get scores/name/highmass/highrank/startime/endtime - insert the appropriate data
        ///      into the database.
        ///   </para>
        ///   </para>
        ///   <para>
        ///     create - contact the DB and create the required tables and seed them with some dummy data
        ///   </para>
        ///   <para>
        ///     get index (or "", or "/") - send a happy home page back
        ///   </para>
        ///   <para>
        ///     get css/styles.css?v=1.0  - send your sites css file data back
        ///   </para>
        ///   <para>
        ///     otherwise send a page not found error
        ///   </para>
        ///   <para>
        ///     Warning: when you send a message, the web browser is going to expect the message to
        ///     be message by message (new message separated) but we use new message as a special character in our
        ///     networking object.  Thus, you have to send _every message of your response_ as a new Send message.
        ///   </para>
        /// </summary>
        /// <param name="network_message_state"> provided by the Networking code, contains socket and message</param>
        internal static void OnMessage(Networking channel, string message)
        {
            if (message.Contains("index.html") || message.Contains("/ HTTP") || message.Contains("index")) // Main Page
            {
                counter++;
                SendResponseMessage(BuildMainPage(), channel);
            }
            else if (message.Contains("favicon"))   // Do nothing.
            {

            }
            else if (message.Contains("highscores")) // High Scores Page
            {
                SendResponseMessage(BuildHighScoresPage(), channel);
            }
            else if (message.Contains("scores")) // Scores for Particular Player Page
            {
                playerName = ParsePlayerName(message);

                // Check if the name is contained in message.
                if (message.Contains(playerName))
                {
                    SendResponseMessage(BuildScoresForParticularPlayerPage(), channel);
                }
                else
                {
                    // Show a message that there is no player whose name is playerName.
                    SendResponseMessage(BuildErrorPage(), channel);
                }
            }
            else if (message.Contains("scores/")) // Insert into Database Endpoint
            {
                var (playerName, highMass, highRank, startTime, endTime) = ParseInformationOfPlayer(message);

                // TODO - "Starttime and endtime should be in milliseconds since the start of the Unix epic."

                try
                {
                    bool playerExists = DoesPlayerNameExist("Scores", playerName);

                    // Check if player exists in the database.
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query;

                        if (playerExists)
                        {
                            // Player exists, update their data.
                            query = $"UPDATE Scores SET highMass = '{highMass}', highRank = '{highRank}', startTime = '{startTime}', endTime = '{endTime}' WHERE playerName = '{playerName}'";
                        }
                        else
                        {
                            // Player does not exist, insert new data.
                            query = $"INSERT INTO Scores (playerName, highMass, highRank, startTime, endTime) VALUES ('{playerName}', '{highMass}', '{highRank}', '{startTime}', '{endTime}')";
                        }

                        // Execute the SQL query
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error occured when inserting data: {e.Message}");
                }

                SendResponseMessage(BuildInsertDataPage(), channel);
            }
            else if (message.Contains("create")) // Create
            {
                CreateTablesOrDoNothing(channel);

                SeedDummyData();

                SendResponseMessage(BuildCreatePage(), channel);
            }
            else if (message.Contains("fancy")) // Fancy Page
            {
                // OPTIONAL - A pretty HTML table with the data.
                // Make sure to include this in your README.
            }
            else // Any other links that we do not support
            {
                SendResponseMessage(BuildErrorPage(), channel);
            }

            // Disconnects every time it receives a message.
            channel.Disconnect();

            Console.WriteLine(message);
        }

        /// <summary>
        ///     Checks if the required database tables exist,
        ///     creates them if they don't.
        /// </summary>
        /// <param name="channel"> the networking channel to use for sending the response. </param>
        private static void CreateTablesOrDoNothing(Networking channel)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the DB tables already exist.. ERROR - TODO
                    string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Table1' OR TABLE_NAME = 'Table2'";
                    SqlCommand command = new SqlCommand(query, connection);

                    int tableCount = (int)command.ExecuteScalar();

                    if (tableCount == 6) // If all tables exist, do nothing.
                    {
                        Console.WriteLine("Tables already exist.");
                    }
                    else // If not, create new DB tables.
                    {
                        CreateDBTablesPage();

                        Console.WriteLine("Created new DB tables just now.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured: {e.Message}");
            }
        }

        /// <summary>
        ///     Inserts some dummy data into the tables
        ///     for testing purposes.
        /// </summary>
        private static void SeedDummyData()
        {
            // TODO
        }

        /// <summary>
        ///     Parses the player name from a message from a request message.
        /// </summary>
        /// <param name="message"> A request message from a webpage. </param>
        /// <returns> The player name </returns>
        private static string ParsePlayerName(string message)
        {
            // Get the name followed by "scores/"
            string playerName = "";
            string pattern = @"\/scores/\/\w+";

            Match match = Regex.Match(message, pattern);
            if (match.Success)
            {
                playerName = match.Groups[1].Value;
            }

            return playerName;
        }

        /// <summary>
        ///     Parses the information of a specific player from a message from a request message.
        /// </summary>
        /// <param name="message"> A request message from a webpage. </param>
        /// <returns></returns>
        private static (string playerName, string highMass, string highRank, string startTime, string endTime)
                        ParseInformationOfPlayer(string message)
        {
            string pattern = @"scores\/([^\/]*)\/([^\/]*)\/([^\/]*)\/([^\/]*)\/([^\/]*)";
            Match match = Regex.Match(message, pattern);

            if (match.Success)
            {
                string playerName = match.Groups[1].Value;
                string highMass = match.Groups[2].Value;
                string highRank = match.Groups[3].Value;
                string startTime = match.Groups[4].Value;
                string endTime = match.Groups[5].Value;
                Console.WriteLine($"Name: {playerName}\nHighmass: {highMass}\nHighrank: {highRank}\nStarttime: {startTime}\nEndtime: {endTime}");

                return (playerName, highMass, highRank, startTime, endTime);
            }
            else
            {
                Console.WriteLine("No match found.");
                return ("", "", "", "", "");
            }
        }

        /// <summary>
        ///     Sends a response message to the specified channel, 
        ///     based on the body of the message to be displayed.
        /// </summary>
        /// <param name="body">The body of the message to be displayed.</param>
        /// <param name="channel"> The Networking object representing the channel 
        /// to which the response message should be sent. </param>
        private static void SendResponseMessage(string body, Networking channel)
        {
            string[] messages = body.Split('\n');

            foreach (string message in messages)
            {
                channel.Send(message);
            }
        }

        /// <summary>
        ///     This is called when a network connection is lost or disconnected.
        /// </summary>
        /// <param name="channel"></param>
        internal static void OnDisconnect(Networking channel)
        {
            Console.WriteLine($"Goodbye {channel.ID}");
        }

        /* SQL */

        /// <summary>
        /// Handle some CSS to make our pages beautiful
        /// </summary>
        /// <returns>HTTP Response Header with CSS file contents added</returns>
        private static string SendCSSResponse()
        {
            throw new NotSupportedException("read the css file from the solution folder, build an http response, and return this string");
            //Note: for starters, simply return a static hand written css string from right here (don't do file reading)
        }

        /// <summary>
        ///     TODO
        /// </summary>
        /// <returns></returns>
        private static string BuildPlayersTable()
        {
            // Connect to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query the Players table for all rows
                string query = "SELECT Game.gameID, Game.playerName, Game.endTime, Mass.Mass" +
                    "FROM Game g" +
                    "JOIN Mass m ON g.gameID = m.gameID" +
                    "JOIN Player p ON p.playerName = g.playerName" +
                    "WHERE p.playerName = 'given_name'" +
                    "ORDER BY g.gameID;";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Create an HTML table to display the results
                        StringBuilder html = new StringBuilder();
                        html.Append("<table>");
                        html.Append("<tr><th>Name</th><th>Id</th></tr>");

                        // Loop through the rows and add them to the table
                        while (reader.Read())
                        {
                            html.Append("<tr>");
                            html.Append($"<td>{reader.GetString(0)}</td>");
                            html.Append($"<td>{reader.GetInt32(1)}</td>");
                            html.Append("</tr>");
                        }

                        html.Append("</table>");

                        return html.ToString();
                    }
                }
            }
        }

        /// <summary>
        ///    (1) Instruct the DB to seed itself (build tables, add data)
        ///    (2) Report to the web browser on the success
        /// </summary>
        /// <returns> the HTTP message header followed by some informative information</returns>
        private static string CreateDBTablesPage()
        {
            CreateTableGame();
            CreateTableHeartbeat();
            CreateTableLeaderboard();
            CreateTableMass();
            CreateTablePlayer();
            CreateTableTime();
            return "";
        }

        /// <summary>
        ///     Creates the "Game" table in the database.
        /// </summary>
        private static void CreateTableGame()
        {
            string query = @"
            CREATE TABLE Game (
                playerID INT NOT NULL,
                playerName VARCHAR(50) NOT NULL,
                endTime DATETIME NOT NULL,
                gameID INT PRIMARY KEY NOT NULL
            )";

            CreateTable(query);
        }

        /// <summary>
        ///     Creates the "Heartbeat" table in the database.
        /// </summary>
        private static void CreateTableHeartbeat()
        {
            string query = @"
            CREATE TABLE Heartbeat (
                HeartBeat INT NOT NULL,
                playerID INT NOT NULL,
                playerName VARCHAR(50) NOT NULL,
                gameID INT NOT NULL
            )";
            CreateTable(query);
        }

        /// <summary>
        ///     Creates the "Leaderboard" table in the database.
        /// </summary>
        private static void CreateTableLeaderboard()
        {
            string query = @"
            CREATE TABLE LeaderBoard (
                Rank INT NOT NULL,
                playerName VARCHAR(50) NOT NULL,
                playerID INT NOT NULL,
                score INT NOT NULL,
                gameID INT NOT NULL
            )";
            CreateTable(query);
        }


        /// <summary>
        ///     Creates the "Mass" table in the database.
        /// </summary>
        private static void CreateTableMass()
        {
            string query = @"
            CREATE TABLE LeaderBoard (
                Mass INT NOT NULL,
                playerID INT NOT NULL,
                gameID INT NOT NULL
            )";
            CreateTable(query);
        }

        /// <summary>
        ///     Creates the "Player" table in the database.
        /// </summary>
        private static void CreateTablePlayer()
        {
            string query = @"
            CREATE TABLE Player (
                playerID INT NOT NULL,
                playerName VARCHAR(50) NOT NULL
            )";
            CreateTable(query);
        }

        /// <summary>
        ///     Creates the "Time" table in the database.
        /// </summary>
        private static void CreateTableTime()
        {
            string query = @"
            CREATE TABLE LeaderBoard (
                startTime DATETIME NOT NULL,
                playerID INT NOT NULL,
                playerName VARCHAR(50) NOT NULL,
                gameID INT NOT NULL
            )";
            CreateTable(query);
        }

        /// <summary>
        ///     Helper method to create a table in SSMS.
        /// </summary>
        /// <param name="query"> a query for creating a specific table. </param>
        private static void CreateTable(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
