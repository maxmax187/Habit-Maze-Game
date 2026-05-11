setup instructions local php server and database:
this folder should contain: index.php, DataBaseSetup.sql, .htaccess and .env file.
use the .example.env file as a template to create the .env file. only password likely needs to change.

first time (software setup):
install php thread-safe, put in program files and add to Path variable: 
https://youtu.be/2BiYxO1wszM?si=AV7zC03V9JJzWMRQ	
install MySQL (use development settings/profile) and add to Path variable:
https://youtu.be/hiS_mWZmmI0?si=JEQvRvAvvysTmIYj
save root password carefully


first time (database setup):
login to DB: 
mysql -u root -p
then fill in password

then setup the database and tables as follows:
CREATE DATABASE maze_study;
USE maze_study;
SOURCE DataBaseSetup.sql;

check if setup correctly:
SHOW TABLES;

setup php.ini by copying php.ini-development and renaming it to php.ini
-> uncomment extension=mysqli
-> uncomment extension_dir="ext"


always (how to use):
open PowerShell window in this folder by SHIFT-rightclicking in the folder and selecting "open powershell window here"

start MySQL (if start-on-windows-startup is not enabled):
Start-Service MySQL97
start php server: 
php -S localhost:8080 index.php

testing:
new entry test (posting to DB):
curl.exe -X POST "http://localhost:8080/api/addParticipant" `-H "Content-Type: application/json" `-d '{\"email\":\"test@test.com\",\"totalScore\":0,\"characterSelect\":0,\"trainingSeeds\":\"1,2\",\"testSeeds\":\"3,4\"}'

retrieving said entry:
curl.exe "http://localhost:8080/api/getParticipant?email=test@test.com"


or alternatively test by starting scene "Start" in unity editor and entering some email address, then progressing to the next scene, past the character select screen. At this point the unity editor console will either have shown an error or a debug log comment from databasehandler.cs.


Data extraction
install dependencies if needed (first time only)
pip install pandas python-dotenv sqlalchemy

use python file to bring rounds data to CSV files (will create a directory DataFiles containing the CSV files)
