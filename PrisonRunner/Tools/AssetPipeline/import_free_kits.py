"""Import only vendor model/texture/license files, never third-party code."""
from pathlib import Path
import hashlib
import json
import zipfile

root = Path(__file__).resolve().parents[2]
downloads = root / "References" / "AssetDownloads"
destination = root / "Unity" / "Assets" / "External" / "Staging" / "Kenney"
record = []
for archive in sorted(downloads.glob("*.zip")):
    kit = archive.stem
    count = 0
    with zipfile.ZipFile(archive) as source:
        for entry in source.infolist():
            relative = Path(entry.filename)
            if relative.is_absolute() or ".." in relative.parts:
                raise ValueError("Invalid archive path")
            suffix = relative.suffix.lower()
            model = suffix == ".fbx"
            texture = suffix == ".png" and any("texture" in part.lower() for part in relative.parts)
            license_file = "license" in relative.name.lower() and suffix == ".txt"
            if not (model or texture or license_file):
                continue
            target = destination / kit / relative
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_bytes(source.read(entry))
            if model:
                count += 1
    record.append({"kit": kit, "source": f"https://kenney.nl/assets/{kit}",
                   "license": "CC0", "models": count,
                   "archive_sha256": hashlib.sha256(archive.read_bytes()).hexdigest()})
    print(kit, count, "FBX models imported")
(root / "docs" / "FREE_ASSETS_IMPORTED.json").write_text(json.dumps(record, indent=2), encoding="utf-8")
