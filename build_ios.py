#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
build_ios.py
Tu dong dong goi va tao chu ky so CodeResources chuan cho DragonBoy_Mod_iOS.ipa
Ho tro cai dat tren iPhone / iPad qua Sideloadly, 3uTools, TrollStore, AltStore, Scarlet.
"""

import os
import sys
import shutil
import zipfile
import hashlib
import plistlib
from PIL import Image

try:
    if sys.stdout.encoding != 'utf-8':
        sys.stdout.reconfigure(encoding='utf-8')
except Exception:
    pass

BASE_DIR = r"c:\ModNRO"
IOS_BUILDS_DIR = os.path.join(BASE_DIR, "02_iOS_Builds")
SOURCE_IPA = os.path.join(IOS_BUILDS_DIR, "(iPhone) MOD_DP_246.ipa")
TEMP_DIR = os.path.join(IOS_BUILDS_DIR, ".temp_ios")
PAYLOAD_DIR = os.path.join(TEMP_DIR, "Payload")
APP_DIR = os.path.join(PAYLOAD_DIR, "MODDP246.app")
OUTPUT_IPA = os.path.join(IOS_BUILDS_DIR, "DragonBoy_Mod_iOS.ipa")
DESKTOP_IPA = r"C:\Users\PhamTriHien\Desktop\DragonBoy_Mod_iOS.ipa"
LOGO_PNG = os.path.join(BASE_DIR, "DragonBoy_Net8_Native", "custom_logo.png")

def main():
    print("======================================================================")
    print("  DONG GOI BUILD IOS IPA - DRAGONBOY MOD TRIHIENKUN")
    print("======================================================================")

    if not os.path.exists(SOURCE_IPA):
        print(f"[LOI] Khong tim thay source IPA: {SOURCE_IPA}")
        sys.exit(1)

    print("\n[1/5] Giai nen source IPA...")
    if os.path.exists(TEMP_DIR):
        shutil.rmtree(TEMP_DIR)
    os.makedirs(TEMP_DIR, exist_ok=True)

    with zipfile.ZipFile(SOURCE_IPA, 'r') as z:
        z.extractall(TEMP_DIR)

    if not os.path.exists(APP_DIR):
        print(f"[LOI] Khong tim thay thu muc app: {APP_DIR}")
        sys.exit(1)

    print("\n[2/5] Cap nhat Info.plist thuong hieu DragonBoy TriHienKun...")
    plist_path = os.path.join(APP_DIR, "Info.plist")
    if os.path.exists(plist_path):
        with open(plist_path, "rb") as f:
            info = plistlib.load(f)

        info["CFBundleDisplayName"] = "DragonBoy TriHienKun"
        info["CFBundleName"] = "DragonBoyTriHienKun"
        info["CFBundleShortVersionString"] = "2.5.0"
        info["CFBundleVersion"] = "2.5.0"

        with open(plist_path, "wb") as f:
            plistlib.dump(info, f)
        print(f"  + Display Name: {info['CFBundleDisplayName']}")
        print(f"  + Version: {info['CFBundleShortVersionString']}")

    print("\n[3/5] Dong bo Icon TriHienKun cho iPhone va iPad...")
    if os.path.exists(LOGO_PNG):
        try:
            with Image.open(LOGO_PNG) as img:
                # iPhone 120x120
                img.resize((120, 120), Image.Resampling.LANCZOS).save(os.path.join(APP_DIR, "AppIcon60x60@2x.png"), "PNG")
                # iPad 152x152
                img.resize((152, 152), Image.Resampling.LANCZOS).save(os.path.join(APP_DIR, "AppIcon76x76@2x~ipad.png"), "PNG")
            print("  + Da tao icon sac net tu custom_logo.png")
        except Exception as e:
            print(f"  - Bo qua cap nhat icon ({e})")

    print("\n[4/5] Tinh toan ma bam CodeResources (SHA-1 va SHA-256)...")
    files_dict = {}
    files2_dict = {}

    for root, dirs, files in os.walk(APP_DIR):
        for file in files:
            full_path = os.path.join(root, file)
            rel_path = os.path.relpath(full_path, APP_DIR).replace("\\", "/")

            if rel_path.startswith("_CodeSignature"):
                continue

            with open(full_path, "rb") as f:
                content = f.read()

            sha1 = hashlib.sha1(content).digest()
            sha256 = hashlib.sha256(content).digest()

            files_dict[rel_path] = sha1
            files2_dict[rel_path] = {
                "hash": sha1,
                "hash2": sha256
            }

    code_resources = {
        "files": files_dict,
        "files2": files2_dict,
        "rules": {
            "^.*": True,
            "^.*\\.lproj/": {"optional": True, "weight": 1000.0},
            "^.*\\.lproj/locversion\\.plist$": {"omit": True, "weight": 1100.0},
            "^Info\\.plist$": {"omit": True, "weight": 20.0},
            "^PkgInfo$": {"omit": True, "weight": 20.0}
        },
        "rules2": {
            ".*": True,
            "^.*\\.lproj/": {"optional": True, "weight": 1000.0},
            "^.*\\.lproj/locversion\\.plist$": {"omit": True, "weight": 1100.0},
            "^Info\\.plist$": {"omit": True, "weight": 20.0},
            "^PkgInfo$": {"omit": True, "weight": 20.0}
        }
    }

    cr_dir = os.path.join(APP_DIR, "_CodeSignature")
    os.makedirs(cr_dir, exist_ok=True)
    cr_path = os.path.join(cr_dir, "CodeResources")
    with open(cr_path, "wb") as f:
        plistlib.dump(code_resources, f)
    print(f"  + Da tinh toan ma bam {len(files_dict)} files vao CodeResources")

    print("\n[5/5] Nen goi Payload thanh DragonBoy_Mod_iOS.ipa...")
    if os.path.exists(OUTPUT_IPA):
        os.remove(OUTPUT_IPA)

    with zipfile.ZipFile(OUTPUT_IPA, 'w', zipfile.ZIP_DEFLATED) as zip_out:
        for root, dirs, files in os.walk(TEMP_DIR):
            for file in files:
                full_path = os.path.join(root, file)
                rel_path = os.path.relpath(full_path, TEMP_DIR).replace("\\", "/")
                zip_out.write(full_path, rel_path)

    shutil.copyfile(OUTPUT_IPA, DESKTOP_IPA)
    ipa_size_mb = os.path.getsize(OUTPUT_IPA) / (1024 * 1024)

    if os.path.exists(TEMP_DIR):
        shutil.rmtree(TEMP_DIR)

    print("\n======================================================================")
    print("BUILD IOS IPA THANH CONG 100%!")
    print(f"  + Tep IPA pipeline: {OUTPUT_IPA} ({ipa_size_mb:.2f} MB)")
    print(f"  + Tep IPA Desktop : {DESKTOP_IPA}")
    print("======================================================================")

if __name__ == "__main__":
    main()
