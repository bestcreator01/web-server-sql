using Communications;
using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

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
        public static void Main(string[] args)
        {
            Networking webServer = new(NullLogger.Instance, OnClientConnect, OnDisconnect, OnMessage, '\n');

            webServer.WaitForClients(11001, true);
        }

        /// <summary>
        /// keep track of how many requests have come in.  Just used
        /// for display purposes.
        /// </summary>
        static private int counter = 1;

        /// <summary>
        /// Basic connect handler - i.e., a browser has connected!
        /// Print an information message
        /// </summary>
        /// <param name="channel"> the Networking connection</param>

        internal static void OnClientConnect(Networking channel)
        {
            throw new NotImplementedException("Print something about a connection happening");
        }

        /// <summary>
        /// Create the HTTP response header, containing items such as
        /// the "HTTP/1.1 200 OK" line.
        /// 
        /// See: https://www.tutorialspoint.com/http/http_responses.htm
        /// 
        /// Warning, don't forget that there have to be new lines at the
        /// end of this message!
        /// </summary>
        /// <param name="length"> how big a message are we sending</param>
        /// <param name="type"> usually html, but could be css</param>
        /// <returns>returns a string with the response header</returns>
        private static string BuildHTTPResponseHeader(int length, string type = "text/html")
        {
            throw new NotImplementedException("Modify this to return an actual HTTP protocol message");
        }

        /// <summary>
        ///   Create a web page!  The body of the returned message is the web page
        ///   "code" itself. Usually this would start with the doctype tag followed by the HTML element.  Take a look at:
        ///   https://www.sitepoint.com/a-basic-html5-template/
        /// </summary>
        /// <returns> A string the represents a web page.</returns>
        private static string BuildHTTPBody()
        {
            // FIXME: this should be a complete web page.
            return $@"
            <h1>hello world{counter}</h1>
            <a href='localhost:11000'>Reload</a> 
            <br/>how are you...";
        }

        /// <summary>
        /// Create a response message string to send back to the connecting
        /// program (i.e., the web browser).  The string is of the form:
        /// 
        ///   HTTP Header
        ///   [new line]
        ///   HTTP Body
        ///  
        ///  The Header must follow the header protocol.
        ///  The body should follow the HTML doc protocol.
        /// </summary>
        /// <returns> the complete HTTP response</returns>
        private static string BuildMainPage()
        {
            string message = BuildHTTPBody();
            string header = BuildHTTPResponseHeader(message.Length);

            return header + message;
        }

        /// <summary>
        ///   <para>
        ///     When a request comes in (from a browser) this method will
        ///     be called by the Networking code.  Each line of the HTTP request
        ///     will come as a separate message.  The "line" we are interested in
        ///     is a PUT or GET request.  
        ///   </para>
        ///   <para>
        ///     The following messages are actionable:
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
        ///     Warning: when you send a response, the web browser is going to expect the message to
        ///     be line by line (new line separated) but we use new line as a special character in our
        ///     networking object.  Thus, you have to send _every line of your response_ as a new Send message.
        ///   </para>
        /// </summary>
        /// <param name="network_message_state"> provided by the Networking code, contains socket and message</param>
        internal static void OnMessage(Networking channel, string message)
        {
            string header = $@"


            ";

            string body = $@"
            
            ";

            channel.Send(header);
            channel.Send("");
            channel.Send(body);

            Console.WriteLine(message);
        }

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
        /// <returns> the HTTP response header followed by some informative information</returns>
        private static string CreateDBTablesPage()
        {
            throw new NotImplementedException("create the database tables by 'talking' with the DB server and then return an informative web page");
        }

        /// <summary>
        ///     TODO
        /// </summary>
        /// <param name="channel"></param>
        internal static void OnDisconnect(Networking channel)
        {
            Debug.WriteLine($"Goodbye {channel.RemoteAddressPort}");
        }

    }
}
