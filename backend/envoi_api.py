from fastapi import FastAPI
from datetime import datetime, timezone
import random

app = FastAPI()

@app.get("/api/pidata/current")
def get_pi_data():
    now = datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%S.%f")[:-3] + "Z"
    
    return {
        "batchTimestamp": now,
        "piServer": "PIServer",
        "afDatabase": "BizDataRouterTest",
        "template": "TestDatarouterTemplate",
        "elementCount": 2,
        "elements": [
            {
                "element": "TestDatarouter",
                "attributeCount": 4,
                "attributes": [
                    {"name": "Atelier", "value": "TestDatarouter", "quality": "Good", "timestamp": now},
                    {"name": "Pressure", "value": round(14.0510154289897 + random.uniform(-0.5, 0.5), 13), "quality": "Good", "timestamp": now},
                    {"name": "ProcessTemp", "value": round(1.11178822680925 + random.uniform(-0.1, 0.1), 13), "quality": "Good", "timestamp": now},
                    {"name": "RandomValues", "value": round(70.4023494247358 + random.uniform(-5.0, 5.0), 13), "quality": "Good", "timestamp": now}
                ]
            },
            {
                "element": "TestDatarouter1",
                "attributeCount": 4,
                "attributes": [
                    {"name": "Atelier", "value": "TestDatarouter1_BroyeurCru", "quality": "Good", "timestamp": now},
                    {"name": "Pressure", "value": round(-1.91942945561345 + random.uniform(-0.5, 0.5), 13), "quality": "Good", "timestamp": now},
                    {"name": "ProcessTemp", "value": round(1.55874449925439 + random.uniform(-0.1, 0.1), 13), "quality": "Good", "timestamp": now},
                    {"name": "RandomValues", "value": round(43.9528850670685 + random.uniform(-5.0, 5.0), 13), "quality": "Good", "timestamp": now}
                ]
            }
        ]
    }