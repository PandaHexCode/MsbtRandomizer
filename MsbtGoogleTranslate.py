import random
import sys
import traceback
from deep_translator import GoogleTranslator
import time
import re
from langdetect import detect

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
    "xh", "yi", "yo", "zu", "fr", "ur", "ay", "dv", "mni-Mtei", "lus", "gn"
]

def translate_to_random_language(text, count):
    dest_language = "de"
    if count != 4:
        dest_language = random.choice(all_languages)
    else:
        dest_language = random.choice(cuted_languages)

    translated_text = text
    try:
        translated_text = GoogleTranslator(source="auto", target=dest_language).translate(text)
    except Exception as ex:
        print(str(ex) + " " + dest_language)
        if str(ex).startswith("Server Error:"):
            print("\nEND Because of error, waiting to retry (Dont press any buttons)\n")
            time.sleep(180)
            return translate_to_random_language(text, count)
    return translated_text


def translate_to_message(text, dest):
    translated_text = text
    try:
        translated_text = GoogleTranslator(source="auto", target=dest).translate(text)

        for _ in range(4):
            try:
                detected_lang = detect(translated_text)
                if detected_lang.lower() == dest.lower():
                    break
                else:
                    print(f"Detected {detected_lang}, expected {dest}. Retrying...")
                    time.sleep(2)
                    translated_text = GoogleTranslator(source="auto", target=dest).translate(translated_text)
            except Exception as inner_ex:
                print(f"Langdetect error: {inner_ex}")
                break
    except Exception as ex:
        print(str(ex))

    return translated_text


def save_to_file(filename, fcontent):
    with open(filename, 'w', encoding='utf-8') as file:
        file.write(fcontent)


def translate_with_tags(line, translationsCount, outputLanguageCode):
    parts = re.split(r'(<[^>]+>)', line)  
    translated_parts = []

    for part in parts:
        if re.match(r'<[^>]+>', part): 
            translated_parts.append(part)
        else:
            text = part.strip()
            if text:
                for c in range(translationsCount):
                    text = translate_to_random_language(text, c)
                text = translate_to_message(text, outputLanguageCode)
            translated_parts.append(text)

    return ''.join(translated_parts)


try:
    path = str(sys.argv[1])
    finish_path = str(sys.argv[2])
    outputLanguageCode = str(sys.argv[3])
    progressPath = str(sys.argv[4])
    translationsCount = int(sys.argv[5])

    with open(path, 'r', encoding='utf-8') as file:
        content = file.read()

    lines = content.split('##!#')
    newContent = ""

    for idx, line in enumerate(lines):
        try:
            l = line.strip()
            if l:
                l = translate_with_tags(l, translationsCount, outputLanguageCode)
                newContent += l
            else:
                newContent += ""
        except Exception as ex:
            print(f"{traceback.format_exc().splitlines()[-1]}: {str(ex)}")
            newContent += line

        if idx < len(lines) - 1:
            newContent += "##!#"

        progress_msg = f"{idx} from {len(lines) - 1}"
        save_to_file(progressPath, progress_msg)

    save_to_file(finish_path, newContent)
    print(f"Translation finished. Saved to {finish_path}")

except Exception as ex:
    print("Error " + str(ex))
    while True:
        i = 0
