from datetime import datetime
import urllib.request
import base64
import json
import time
import os
import requests
import sys

webui_server_url = 'http://127.0.0.1:7860'
text_url = "http://127.0.0.1:5000/v1/chat/completions"
headers = {"Content-Type": "application/json"}

out_dir = 'Images'
os.makedirs(out_dir, exist_ok=True)


def encode_file_to_base64(path):
    with open(path, 'rb') as file:
        return base64.b64encode(file.read()).decode('utf-8')


def decode_and_save_base64(base64_str, save_path):
    with open(save_path, "wb") as file:
        file.write(base64.b64decode(base64_str))


def call_api(api_endpoint, payload):
    data = json.dumps(payload).encode('utf-8')
    request = urllib.request.Request(f'{webui_server_url}/{api_endpoint}', headers={'Content-Type': 'application/json'}, data=data, )
    response = urllib.request.urlopen(request)
    return json.loads(response.read().decode('utf-8'))


def call_txt2img_api(payload, name):
    response = call_api('sdapi/v1/txt2img', payload)
    for index, image in enumerate(response.get('images')):
        save_path = os.path.join(out_dir, name + '.png')
        decode_and_save_base64(image, save_path)


if __name__ == '__main__':
    sys.stdout.reconfigure(encoding='utf-8')
    if sys.argv[1] == "org":
        user_message = "Provide a short description for an emblem of a gang called " + sys.argv[2] + " for image generating ai. Their gang color is " + sys.argv[
            3] + ". Make it somewhat abstract or stylised. The background must be black. Say nothing before or after the description."
        data = {"mode": "instruct", "character": "Assistant", "messages": [{"role": "user", "content": user_message}]}
        response = requests.post(text_url, headers=headers, json=data, verify=False)
        descr = response.json()['choices'][0]['message']['content']
        gang_name = sys.argv[2].replace('_', ' ')
        print(descr)

        payload = {"prompt": "gang emblem of " + sys.argv[2] + ", ((black background)), " + descr, "negative_prompt": "1girl, 1boy, man, woman, person, gray background, watermark",
            "seed": -1, "steps": 20, "width": 512, "height": 512, "cfg_scale": 2, "sampler_name": "LCM", "n_iter": 1, "batch_size": 1, }
        call_txt2img_api(payload, gang_name)
    else:
        if len(sys.argv) < 7:
            user_message = "Provide a name and surname for a modern day " + sys.argv[1] + " " + sys.argv[2] + " " + sys.argv[3] + " " + sys.argv[
                4] + ". Adding a nickname or a middle name is optional, but only rarely. Make the name interesting. Say nothing before or after the name and surname."
            data = {"mode": "instruct", "character": "Assistant", "messages": [{"role": "user", "content": user_message}]}
            response = requests.post(text_url, headers=headers, json=data, verify=False)
            name = response.json()['choices'][0]['message']['content']
        else:
            name = sys.argv[6]
        name = name.replace('_', ' ')
        name = name.replace('"', '~')
        name = "".join(x for x in name if (x.isalnum() or x in "._- "))
        print(name)

        user_message_universal = " Mention age, race, hair color, haircut, emotion, background scenery, fatness, clothes, facial hair if any. The face may be clean-shaven - specify it in the description. Type of clothes depends on being poor or rich. Don't mention boots, pants or trousers, focus on the upper body and face. Say nothing before or after the description."

        if sys.argv[4] == "gangster":
            direct_prompt = " wearing (" + sys.argv[5] + ") clothes , "
            user_message = "Provide a short description of a modern day " + sys.argv[1] + " " + sys.argv[2] + " " + sys.argv[3] + " " + sys.argv[4] + " wearing " + sys.argv[
                5] + " - the gang color - for image generating ai." + user_message_universal
        elif sys.argv[4] == "policeman":
            direct_prompt = " wearing (" + sys.argv[5] + ") police uniform , "
            user_message = "Provide a short description of a modern day " + sys.argv[1] + " " + sys.argv[2] + " " + sys.argv[3] + " " + sys.argv[4] + " wearing a " + sys.argv[
                5] + " police uniform for image generating ai." + user_message_universal
        else:
            direct_prompt = ""
            user_message = "Provide a short description of a modern day " + sys.argv[1] + " " + sys.argv[2] + " " + sys.argv[3] + " " + sys.argv[
                4] + " for image generating ai." + user_message_universal
        data = {"mode": "instruct", "character": "Assistant", "messages": [{"role": "user", "content": user_message}]}
        response = requests.post(text_url, headers=headers, json=data, verify=False)
        assistant_message = response.json()['choices'][0]['message']['content']
        print(assistant_message)

        payload = {"prompt": "(face close-up), SFW, " + sys.argv[1] + ", " + sys.argv[2] + ", " + sys.argv[3] + ", " + sys.argv[4] + direct_prompt + assistant_message,
            "negative_prompt": "watermark, cleavage, breasts, NSFW, abs, abdomen, toples, naked", "seed": -1, "steps": 20, "width": 512, "height": 512, "cfg_scale": 2, "sampler_name": "LCM", "n_iter": 1,
            "batch_size": 1, }
        call_txt2img_api(payload, name)
