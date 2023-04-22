using Communications;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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
        ///     Default constructor - sets up the SQL connection.
        /// </summary>
        static WebServer()
        {
            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<WebServer>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("WebserverSecrets");

            connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = SelectedSecrets["sever_name"],
                InitialCatalog = "cs3500",
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

        /* HEADER AND BODY*/
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

        /// <summary>
        ///   Create a web page!  The body of the returned message is the web page
        ///   "code" itself. Usually this would start with the doctype tag followed by the HTML element.  Take a look at:
        ///   https://www.sitepoint.com/a-basic-html5-template/
        /// </summary>
        /// <returns> A string the represents a web page.</returns>
        private static string BuildHTTPBody()
        {
            counter++;

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
        /// 
        /// </summary>
        /// <returns></returns>
        private static string BuildHTTPBodyHighScores()
        {
            // TODO
            return "";
        }

        /* PAGES */
        /// <summary>
        /// Create a message message string to send back to the connecting
        /// program (i.e., the web browser).  The string is of the form:
        /// 
        ///   HTTP Header
        ///   [new message]
        ///   HTTP Body
        ///  
        ///  The Header must follow the header protocol.
        ///  The body should follow the HTML doc protocol.
        /// </summary>
        /// <returns> the complete HTTP message</returns>
        private static string BuildMainPage()
        {
            string message = BuildHTTPBody();
            string header = BuildHTTPResponseHeader(message.Length);

            return header + "\r\n" + message;
        }

        /// <summary>
        /// Create a message message string to send back to the connecting
        /// program (i.e., the web browser).  The string is of the form:
        /// 
        ///   HTTP Header
        ///   [new message]
        ///   HTTP Body
        ///   
        /// This method is to build the High Scores page.
        /// </summary>
        /// <returns> the complete HTTP message for Scores Page </returns>
        private static string BuildHighScoresPage()
        {
            string message = BuildHTTPBodyHighScores();
            string header = BuildHTTPResponseHeader(message.Length);

            return header + "\r\n" + message;
        }

        /// <summary>
        /// Create a message message string to send back to the connecting
        /// program (i.e., the web browser).  The string is of the form:
        /// 
        ///   HTTP Header
        ///   [new message]
        ///   HTTP Body
        ///   
        /// This method is to build the page where you query the DB for all scores
        /// associated with the given name and return an HTML web page with a summary
        /// of the time and high score for each game played by the player.
        /// </summary>
        /// <returns> the complete HTTP message for Scores For Particular Player Page </returns>
        private static string BuildScoresForParticularPlayerPage()
        {
            // TODO
            // if message contains Gloria...
            // if message contains Seoin...
            // if message contains Jim...

            string message = ""; // TODO
            string header = BuildHTTPResponseHeader(message.Length);

            return header + "\r\n" + message;
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
            // Return an HTML page listing the top score for each player in the database.
            // "score" means the highest mass achieved by the player.
            if (message.Contains("highscores")) // High Scores Page
            {
                SendResponseMessage(BuildHighScoresPage(), channel);
            }
            else if (message.Contains("scores")) // Scores for Particular Player Page
            {
                string playerName = GetPlayerNameFollowedByScores(message);

                // Check if the name is contained in message.
                if (message.Contains(playerName))
                {

                } else
                {
                    // TODO - Show a message that there is no player whose name is playerName.
                }
            }
            else if (message.Contains("fancy"))
            {
                // OPTIONAL
                // A pretty HTML table with the data.
                // TODO - make sure to include this in your README.
            }
            else
            {
                // Main Page
                SendResponseMessage(BuildMainPage(), channel);
            }

            channel.Disconnect();

            Console.WriteLine(message);
        }

        /// <summary>
        ///     Gets the player name followed by the string "/scores"
        /// </summary>
        /// <param name="message"> The message to search in. </param>
        /// <returns> The player name </returns>
        private static string GetPlayerNameFollowedByScores(string message)
        {
            // Get the name followed by "scores/"
            string playerName = "";
            string pattern = @"\/scores/\/\w+";

            Match match = Regex.Match(message, pattern);
            if (match.Success)
            {
                playerName = match.Value.Split('/').Last();
            }

            return playerName;
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

        ///// <summary>
        /////     Try to add a row to the database table
        /////     
        ///// </summary>
        //private static void AddClients()
        //{
        //    Console.WriteLine("Can we add a row?");
        //    try
        //    {
        //        using SqlConnection con = new SqlConnection(connectionString);

        //        con.Open();

        //        using SqlCommand command = new SqlCommand("TODO");
        //        using SqlDataReader reader = command.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            Console.WriteLine("{0} {1}",
        //                reader.GetInt32(0), reader.GetString(1));
        //        }
        //    }
        //    catch (SqlException exception)
        //    {
        //        Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
        //    }
        //}

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
        ///    (1) Instruct the DB to seed itself (build tables, add data)
        ///    (2) Report to the web browser on the success
        /// </summary>
        /// <returns> the HTTP message header followed by some informative information</returns>
        private static string CreateDBTablesPage()
        {
            try
            {
                // Connect to the database
                string connectionString = "Data Source=cs3500.eng.utah.edu,14330;Initial Catalog=agario_db;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create the tables
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Players (
                        Name VARCHAR(50) NOT NULL,
                        Id INT NOT NULL,
                        PRIMARY KEY (Name, Id)
                    );
                    CREATE TABLE IF NOT EXISTS Games (
                        Id INT NOT NULL,
                        PlayerName VARCHAR(50) NOT NULL,
                        StartTime DATETIME NOT NULL,
                        EndTime DATETIME,
                        Size INT NOT NULL,
                        Rank INT NOT NULL,
                        PRIMARY KEY (Id, PlayerName),
                        FOREIGN KEY (PlayerName, Id) REFERENCES Players(Name, Id)
                    );
                ";
                        command.ExecuteNonQuery();
                    }

                    // Insert some data into the tables
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"
                    INSERT INTO Players (Name, Id) VALUES
                        ('Alice', 1),
                        ('Bob', 2),
                        ('Charlie', 3),
                        ('David', 4),
                        ('Eve', 5);
                    INSERT INTO Games (Id, PlayerName, StartTime, EndTime, Size, Rank) VALUES
                        (1, 'Alice', '2023-04-21 10:00:00', '2023-04-21 10:10:00', 100, 1),
                        (1, 'Bob', '2023-04-21 10:00:00', '2023-04-21 10:05:00', 80, 2),
                        (1, 'Charlie', '2023-04-21 10:00:00', '2023-04-21 10:08:00', 90, 3),
                        (2, 'Alice', '2023-04-22 15:00:00', '2023-04-22 15:20:00', 120, 1),
                        (2, 'Bob', '2023-04-22 15:00:00', '2023-04-22 15:05:00', 60, 2),
                        (2, 'David', '2023-04-22 15:00:00', '2023-04-22 15:12:00', 80, 3);
                ";
                        command.ExecuteNonQuery();
                    }

                    // Close the connection
                    connection.Close();
                }

                // Return a success message
                return "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n" +
                       "Database tables and data have been successfully created.";
            }
            catch (Exception ex)
            {
                // Return an error message
                return "HTTP/1.1 500 Internal Server Error\r\nContent-Type: text/plain\r\n\r\n" +
                       "An error occurred while creating the database tables and data: " + ex.Message;
            }
        }
    }
}
