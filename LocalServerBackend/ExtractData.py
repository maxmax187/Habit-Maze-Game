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


def export_rounds():
    """Export rounds table."""
    df_rounds = pd.read_sql(
        "SELECT * FROM rounds ORDER BY participantEmail, day, round",
        engine
    )

    df_rounds.to_csv(
        os.path.join(output_dir, f"{timestamp}_rounds.csv"),
        index=False
    )


def export_round_logs():
    """Export round logs with round metadata."""
    df_logs = pd.read_sql(
        """
        SELECT r.participantEmail, r.day, r.round, r.phase, rl.t, rl.d
        FROM roundLogs rl
        JOIN rounds r ON r.id = rl.roundId
        ORDER BY r.participantEmail, r.day, r.round, rl.t
        """,
        engine
    )

    df_logs.to_csv(
        os.path.join(output_dir, f"{timestamp}_round_logs.csv"),
        index=False
    )


def export_habit_survey_data():
    """Export habit survey responses."""
    df_habit_survey = pd.read_sql(
        """
        SELECT
            participantEmail,
            day,
            srbai1,
            srbai2,
            srbai3,
            srbai4
        FROM habitSurvey
        ORDER BY participantEmail, day
        """,
        engine
    )

    df_habit_survey.to_csv(
        os.path.join(output_dir, f"{timestamp}_habit_survey.csv"),
        index=False
    )

def export_participants():
    """Export all participant data."""
    df_participants = pd.read_sql(
        "SELECT * FROM participants ORDER BY email",
        engine
    )

    df_participants.to_csv(
        os.path.join(output_dir, f"{timestamp}_participants.csv"),
        index=False
    )


# Run exports
# export_rounds()
# export_round_logs()
# export_habit_survey_data()
export_participants()

engine.dispose()
print(f"Done — files saved to {output_dir}")

