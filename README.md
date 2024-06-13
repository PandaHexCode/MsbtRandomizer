# Msbt Randomizer

Tested with Paper Mario: The Thousand Year Door (Remake)

MSBT files are files that contains games text used in Nintendo games.
The tool can translate most of the text to random languages and then translate it back to english/or any other language.

## How?
With [CLMS](https://github.com/KillzXGaming/CLMS) the program can open .msbt files and edit it,
so the program opens the .msbt file then it creates a temp file so that a python program (converted to exe) can
translate it to random languages and back with [deep-translator](https://pypi.org/project/deep-translator/)
I use c# and python and not only C# because I can't find any package for C# that doesn't require a API-key.

## Setup

1. **Download** the newest release from the Releases tab and unzip it.
2. **Open** the `MSBTRando.exe` file.
   - If it crashes make sure you have "[dotnet 8.0 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)"
     installed for your platform, if it still doesn't start please report the error.
3. **Select** a language folder from your game `romfs` (you need to have a dump of your game `romfs`).
4. **Write** your desired output language code:
   - "en" for English
   - "de" for German
   - etc.
5. **Press** "Start".
6. **Press** "Start" for the file you want to auto-translate.
   - **Note:** Avoid starting too many file edits at once, as this can lead to Google timing you out for minutes or even days.
7. **Wait** for the process to complete. This can take 30 minutes to 1 hour or more.
8. Once finished, you should have an output folder with every `.msbt` file in it, located in the same path as your `MSBTRando.exe`.
9. **Load** the output folder as a mod (with the correct path).

**Note:** Keep an eye on the console for any errors because it's only a test release.

Have fun!

## Why?
Just for fun it can lead to very funny results.
