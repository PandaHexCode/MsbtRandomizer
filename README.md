# Msbt Randomizer

Tested with Paper Mario: The Thousand Year Door (Remake)

MSBT files are files that contains games text used in Nintendo games.
The tool can translate most of the text to random languages and then translate it back to english/or any other language.

## Why?
Just for fun it can lead to very funny results.

## How?
With [CLMS](https://github.com/KillzXGaming/CLMS) the program can open .msbt files and edit it,
so the program opens the .msbt file then it creates a temp file so that a python program (converted to exe) can
translate it to random languages and bakc with [Googletrans]([https://github.com/KillzXGaming/CLMS](https://pypi.org/project/googletrans/))
I use c# and python and not only C# because I can't find any package for C# that doesn't require a API-key.

## Setup
Download the newest release from the Releases tab and unzip it.
Open the MSBTRando.exe file (If it crashes you need to install dotnet8)
Select a language folder from your game romfs(You need to have a dump of your game romfs)
Write your end output language code (That language that should be the output language) ("en" = English, "de" = German, ...) 
Press "Start"
And now you need to wait, that Process can take 30minutes- 1hour+
If it is finish you should have a output folder with every .msbt in it in the path where your MSBTRando.exe is.
You need to load that as a mod(With the right path).
Have fun!
