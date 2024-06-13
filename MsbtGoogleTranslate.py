import random
from googletrans import Translator
import concurrent.futures
import os
import sys
import traceback
from deep_translator import GoogleTranslator

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

translator = Translator()

def translate_to_random_language(text):
    dest_language = random.choice(all_languages)
    translated_text = text
    try:
        #print ("TTT" + translated_text)
        t = GoogleTranslator(source="auto", target=dest_language).translate(text)
        translated_text = t
    except Exception as ex:
        print(str(ex) + " " + dest_language)
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
    args = sys.argv

    path = str(args[1])

    with open(path, 'r', encoding='utf-8') as file:
        content = file.read()

    lines = content.split('##!#')

    content = ""

    i = 0
    for line in lines:
        try:
            l = line
            for k in range(5):
                t = str(translate_to_random_language(str(l)))
                l = t
                #print(l)
            l = translate_to_message(l, str(args[3]))

            content = content + l + "##!#"
            m = str(i) + " from " + str(len(lines))
            save_to_file(str(args[4]), str(m))
            #print(m)
        except Exception as ex:
            print(f"{traceback.format_exc().splitlines()[-1]}: {str(ex)}")
            continue
        i = i + 1

    save_to_file(str(args[2]), content)
except Exception as ex:
     print(str(ex))
     while True:
         i =  0