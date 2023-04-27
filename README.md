```
Author:		Gloria Shin, Seoin Kim
Date:		19-Apr-2023
Course:		CS 3500, University of Utah, School of Computing
GitHub ID:	bestcreator01, seointhenerd
Repo:		https://github.com/uofu-cs3500-spring23/assignment-nine---web-server---sql-heavyeyebagsmadeby3500
Commit #:	32
Solution:	WebServer
Copyright:	CS 3500, Gloria Shin, and Seoin Kim - This work may not be copied for use
in Academic Coursework.
```

# Overview of the WebServer functionality

	This program allows anyone to use their web browser to see information about how players
	are doing when playing Agario on our client. 

	Necessary data received by the client is inserted to the SQL database server inside the
	Agario client project. 

	This is the following link of the Agario Client: https://github.com/uofu-cs3500-spring23/assignment8agario-northkoreannotsouth.git

# Time Expenditures:

	1. Assignment Nine - WebServer
		Predicted Hours:	15
		Actual Hours:		15

	Note: Our time estimates are getting better than before. We speculated the exact time for this assignment.

# Database Table Summary

	We created 6 tables: Game, Player, Mass, Time, LeaderBoard, HeartBeat

	Game - gameID, playerID, playerName, endTime
	Player - playerID, playerName
	Mass - gameID, playerID, Mass
	Heartbeat - playerID, heartbeat
	LeaderBoard - gameID, playerID, playerName, Rank, Mass

	All tables have the gameID, which is set in the Game table.
	Each table contains playerID so that they can be joined or merged in necessary situations.
	Contents of the LeaderBoard is based on these tables: Game, Player, and Mass. The rank
	is updated every time a new data is inserted.

	Non-standard pieces of data would be the heartbeat and start time just to let
	the clients know how long they survived and when they started the game.

# Extent of work

	1) Our web server has the ability to serve as a web server and return a basic welcome page.
	2) Our web server has the ability to return a web page with a chart of highscores,
	   more specifically scores with the players' rank.
	3) Our web server has the ability to take in a request to store a highscore.
	4) Our web server has the ability to return a web page showing a chart of scores(leaderboard)
	   and it also may display some random fun images in the fancy webpage.
	5) Our client code has the ability to contact the database and store information therein.

	Note: In terms of returning the scores of each player (ex. scores/Gloria) this returns the duration(heartbeat)
		  instead of date time.

# Evaluation on Time Expenditures

	I feel like the time expenditures were pretty accurate, but this assignment got much more time than I expected.
	I realized that I may have to do more reviews on labs and specifications.

	Estimating time expenditures can be a challenging task, as there are many variables that can affect the outcome. 
	It's important to break down larger tasks into smaller, more manageable ones and try to estimate the time required for each of them individually.
	It's also helpful to track your actual time expenditures and compare them with your estimates to see how accurate they were.

	To improve your abilities, it's important to practice regularly and seek feedback from others. 
	Consider taking on new challenges and learning new skills to expand your knowledge and expertise. 
	Additionally, try to reflect on your strengths and weaknesses and identify areas where you can improve. 
	With consistent effort and dedication, you can improve your abilities and become more proficient in your chosen field.	

# Examples of Good Software Practice (GSP)

	1. DRY (Don't Repeat Yourself)
		DRY is a principle that emphasizes the importance of avoiding redundancy in code.
		It suggests that each piece of information in a software system should have a single
		authoritative representation. You should write code that is concise, easy to read,
		and more maintainable.

	2. KISS (Keep It Simple, Stupid)
		KISS is a design principle that states that software should be simple, straightforward,
		and easy to understand. The idea behind KISS is that complex systems are more likely to
		fail, are harder to maintain, and are more difficult to understand and use. By keeping
		things simple, software is more likely to be correct, reliable, and user-friendly. The
		KISS principle encourages developers to look for the simplest solution to a problem and
		to avoid over-engineering.
		
	3. YAGNI (You Ain't Gonna Need It)
		YAGNI is a software development principle that states that a developer should not add
		functionality to a system unless it is immediately needed. The idea behind YAGNI is to 
		avoid over-engineering and to keep the system simple and focused. Developers can focus
		on delivering the most important functionality first, and only add additional functionality
		as it is needed.

# Branching

	1. Assignment Nine - 32 commits

	We worked on the project together, hence just used the main branch.

# Testing
	
	We did not do the testing with a MS test, but debugged manually through looking at the database and our web server.
	Console.WriteLine also helped a lot in terms of testing.

# Partnership

	Almost all code for this project was completed via pair programming, with each member contributing
	equally to the project. Therefore, there are no distinct sections of work that can be attributed to 
	any one member.

	However, each member brought their unique skill set to the project, which helped to ensure its success.
	Gloria was particularly adept at problem-solving and coming up with creative solutions to complex issues,
	while Seoin had a keen eye for detail and was able to cath error and bugs before they became a problem.

	Overall, this partnership was a great success, and both members worked collaboratively to ensure that the 
	project was completed to the highest standard possible.

# Best (Team) Practices

	One of the most effective practices that we employed in our partnership was assigning tasks based on
	each member's strengths and expertise. By doing so, we were able to work more efficiently and effectively,
	as each member was able to focus on tasks that they were particularly good at. For example, Gloria was 
	particularly adept at writing code, while Seoin was better at debugging and testing. By assigning tasks 
	in this way, we were able to make the coding process faster, as we were both able to work on tasks that 
	we were comfortable with and could complete quickly. Another effective practice that we employed was clear 
	and frequent communication. We made sure to have regular meetings and check-ins to discuss our progress 
	and any issues that we were facing. By doing so, we were able to catch potential problems early on and 
	address them before they became bigger issues. Additionally, we were able to give each other feedback 
	and suggestions, which helped to improve the quality of our code.

	An area of teamwork that Seoin could improve upon is conflict resolution. It's important to recognize that
	disagreements and conflicts can arise during any project, and it's essential to have strategies in place to
	address them. Seoin should be open to discussing different perspectives and finding solutions that work
	for both of them. Additionally, Gloria could to recognize the importance of addressing problems as soon as they 
	arise, rather than letting them fester and potentially grow into larger issues. Our team concluded that effective 
	communication is the most important factor when pair programming.