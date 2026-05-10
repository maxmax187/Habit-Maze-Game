import mysql.connector
import pandas as pd
from dotenv import load_dotenv
import os
from datetime import datetime

load_dotenv()

conn = mysql.connector.connect(
    host=os.getenv("DB_HOST", "127.0.0.1"),
    user=os.getenv("DB_USER"),
    password=os.getenv("DB_PASSWORD"),
    database=os.getenv("DB_NAME"),
    port=int(os.getenv("DB_PORT", 3306))
)

os.makedirs("./DataFiles", exist_ok=True)
timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")

# Export rounds
df_rounds = pd.read_sql("SELECT * FROM rounds ORDER BY participantEmail, day, round", conn)
df_rounds.to_csv(f"./DataFiles/{timestamp}_rounds.csv", index=False)

# Export round logs
df_logs = pd.read_sql("""
    SELECT r.participantEmail, r.day, r.round, r.phase, rl.t, rl.d
    FROM roundLogs rl
    JOIN rounds r ON r.id = rl.roundId
    ORDER BY r.participantEmail, r.day, r.round, rl.t
""", conn)
df_logs.to_csv(f"./DataFiles/{timestamp}_round_logs.csv", index=False)

conn.close()
print(f"Done — files saved to ./DataFiles/{timestamp}_*.csv")