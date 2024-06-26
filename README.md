# Msbt Randomizer (Msbt Google translator)

Automatically creates google translated mods (Like Paper Mario Remake but google translated) for many languages! (English, German, ...)

![Screenshot](GoogleTranslated.png)

Tested with Paper Mario: The Thousand Year Door (Remake)

MSBT files are files that contains games text used in Nintendo games.
The tool can translate most of the text to random languages and then translate it back to english/or any other language.

![Screenshot](Preview1.PNG)

## How?

With [CLMS](https://github.com/KillzXGaming/CLMS), the program can open and edit `.msbt` files.

1. The C# program opens the `.msbt` file.
2. A temporary file is created for a Python script (converted to an executable (Located at program-path/resources/)) to translate the content to random languages and back using [deep-translator](https://pypi.org/project/deep-translator/).
3. The C# program converts the new temporary file back to `.msbt` format.

I use both C# and Python because I couldn't find a C# package that doesn't require an API key.

## Setup

**Video-Tutorial**: [Video](https://www.youtube.com/watch?v=gsKVmAUBY0c)

1. **Download** the newest release from the Releases tab and unzip it.
2. **Open** the `MSBTRando.exe` file.
   - If it crashes make sure you have "[dotnet 8.0 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)"
     installed for your platform, if it still doesn't start please report the error.
3. **Select** a language folder from your game `romfs` (you need to have a dump of your game `romfs`).
4. **Write** your desired output language code:
   - "en" for English
   - "de" for German
   - etc.
5. **Press** "Open".
6. **Press** "Start" for the file you want to auto-translate.
   - **Note:** Avoid starting too many file edits at once(Try it maybe with 10-14), as this can lead to Google timing you out for minutes or even days.
   - Use the "Start as next" button so that it automatically start translating the file after one of the already translated is finished
7. **Wait** for the process to complete. This can take 2hours to 4 hours or more.
8. Once finished, you should have an output folder with every `.msbt` file in it, located in the same path as your `MSBTRando.exe`.
9. **Load** the output folder as a mod (with the correct path).

**Note:** Keep an eye on the console for any errors because it's only a test release.

Have fun!

## Known Issues

- **Translation Errors**: Sometimes words or sentences in other languages are not translated back to your output language correctly.
- **UI Glitches**: Occasionally, the UI may appear broken, such as the health display for Mario & party.
- **Message Merging**: Sometimes messages are incorrectly merged into one. For example, "Hello World (New TextBox), Hello again" might appear as "Hello World, Hello again (One TextBox)".
- **Double Press Required**: Occasionally, you may need to press a button twice to proceed to the next message.
- **Empty Messages**: There may be instances where messages appear empty.
- **Crashes**: While unlikely, the game can crash. If you experience a crash, particularly during chapter intro scenes, you will need to regenerate the `ui.msbt` file.

**Please report any new errors you encounter so I can work on fixing them.**

**Note**: Despite these issues, I believe you can still have a fun time with it.

## Why?
Just for fun it can lead to very funny results.
