@echo off

title PHP server at localhost:8080

cd /d "%~dp0"

php -S localhost:8080 index.php

pause