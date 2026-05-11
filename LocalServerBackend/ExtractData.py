import pandas as pd
from dotenv import load_dotenv
from sqlalchemy import create_engine
import os
from datetime import datetime

# Load .env from the script's own directory
script_dir = os.path.dirname(os.path.abspath(__file__))
load_dotenv(os.path.join(script_dir, ".env"))

engine = create_engine(
    f"mysql+mysqlconnector://{os.getenv('DB_USER')}:{os.getenv('DB_PASSWORD')}"
    f"@{os.getenv('DB_HOST', '127.0.0.1')}:{os.getenv('DB_PORT', 3306)}"
    f"/{os.getenv('DB_NAME')}"
)

output_dir = os.path.join(script_dir, "DataFiles")
os.makedirs(output_dir, exist_ok=True)
timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")

# Export rounds
df_rounds = pd.read_sql("SELECT * FROM rounds ORDER BY participantEmail, day, round", engine)
df_rounds.to_csv(os.path.join(output_dir, f"{timestamp}_rounds.csv"), index=False)

# Export round logs
df_logs = pd.read_sql("""
    SELECT r.participantEmail, r.day, r.round, r.phase, rl.t, rl.d
    FROM roundLogs rl
    JOIN rounds r ON r.id = rl.roundId
    ORDER BY r.participantEmail, r.day, r.round, rl.t
""", engine)
df_logs.to_csv(os.path.join(output_dir, f"{timestamp}_round_logs.csv"), index=False)

engine.dispose()
print(f"Done — files saved to {output_dir}")