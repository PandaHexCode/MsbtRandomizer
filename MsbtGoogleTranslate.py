import random
import sys
import traceback
from deep_translator import GoogleTranslator
import time

all_languages = [
    "af", "sq", "am", "ar", "hy", "as", "ay", "az", "bm", "eu", "be", "bn", "bho", "bs", "bg", "ca", 
    "ceb", "ny", "zh-CN", "zh-TW", "co", "hr", "cs", "da", "dv", "doi", "nl", "en", "eo", "et", "ee", 
    "tl", "fi", "fr", "fy", "gl", "ka", "de", "el", "gn", "gu", "ht", "ha", "haw", "iw", "hi", "hmn", 
    "hu", "is", "ig", "ilo", "id", "ga", "it", "ja", "jw", "kn", "kk", "km", "rw", "gom", "ko", "kri", 
    "ku", "ckb", "ky", "lo", "la", "lv", "ln", "lt", "lg", "lb", "mk", "mai", "mg", "ms", "ml", "mt", 
    "mi", "mr", "mni-Mtei", "lus", "mn", "my", "ne", "no", "or", "om", "ps", "fa", "pl", "pt", "pa", 
    "qu", "ro", "ru", "sm", "sa", "gd", "nso", "sr", "st", "sn", "sd", "si", "sk", "sl", "so", "es", 
    "su", "sw", "sv", "tg", "ta", "tt", "te", "th", "ti", "ts", "tr", "tk", "ak", "uk", "ur", "ug", 
    "uz", "vi", "cy", "xh", "yi", "yo", "zu"
]

cuted_languages = [
    "hy", "as", "ay", "bm", "eu", "be", "bho", "bg", "ceb", "ny", "zh-CN", "zh-TW", "dv", 
    "doi", "et", "ee", "tl", "fy", "ka", "gn", "hi", "hmn", "ig", "ilo", "jw", "kk", "km", 
    "rw", "gom", "kri", "ku", "ckb", "ky", "lo", "lv", "ln", "lg", "mai", "mg", "mni-Mtei", 
    "lus", "mn", "my", "ne", "or", "om", "ps", "qu", "sm", "sa", "gd", "nso", "st", "sn", 
    "sd", "si", "so", "su", "tg", "ta", "tt", "te", "th", "ti", "ts", "tk", "ak", "ug", 
    "xh", "yi", "yo", "zu", "fr", "ur", "ay", "dv", "mni-Mtei", "lus", "gn", "ay", "dv", 
    "mni-Mtei", "lus", "gn"#Copied hard languages multiple times
]

content = ""
args = sys.argv

def translate_to_random_language(text):
    dest_language = random.choice(cuted_languages)
    translated_text = text
    try:
        #print ("TTT" + translated_text)
        translated_text = GoogleTranslator(source="auto", target=dest_language).translate(text)
    except Exception as ex:
        print(str(ex) + " " + dest_language)
        if(str(ex).startswith("Server Error:")):
            print("\nEND Because of error, waiting to retry(Dont press any buttons)\n")
            time.sleep(180)
            return translate_to_random_language(text)
            
    return translated_text

def translate_to_message(text, dest):
    translated_text = text
    try:
        translated_text = GoogleTranslator(source="auto", target=dest).translate(text)
    except Exception as ex:
        print(str(ex))
    return translated_text

def save_to_file(filename, fcontent):
    with open(filename, 'w', encoding='utf-8') as file:
        file.write(fcontent)

try:

    path = str(args[1])

    with open(path, 'r', encoding='utf-8') as file:
        content = file.read()

    lines = content.split('##!#')

    i = 0

    outputLanguageCode = str(args[3])
    linesCount = str(len(lines) - 1)
    progressPath = str(args[4])
    translationsCount = int(args[5])
    newContent = ""

    for line in lines:
        try:
            l = line
            for k in range(translationsCount):
                l =  str(translate_to_random_language(str(l)))
            l = translate_to_message(l, outputLanguageCode)
            newContent = newContent + l + "##!#"
            m = f"{i} from {linesCount}"
            save_to_file(progressPath, str(m))
        except Exception as ex:
            print(f"{traceback.format_exc().splitlines()[-1]}: {str(ex)}")
            continue
        i = i + 1

    save_to_file(str(args[2]), newContent)
except Exception as ex:
     print(str(ex))
     while True:
         i =  0
