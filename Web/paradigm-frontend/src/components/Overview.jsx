import React from "react";

function Overview() {
  return (
    <div>
      <div className="logo-container">
        <img src="tue_logo.png" />
      </div>
      <h2>Overview</h2>
      <>
        <p style={{ fontSize: "16px", marginBottom: "12px" }}>
          Welcome to the Bachelor End Project study of Max te Brake and Mare Hulshof.
          Thank you for participating.
        </p>
        <p style={{ fontSize: "16px", marginBottom: "12px" }}>
          Here you will find the game you will be playing over the course of the study.
          Please complete the game for Day 1 through Day 3 on three consecutive days 
          (ideally at a similar time each day). Do not leave any gap days between sessions.
        </p>
        <p style={{ fontSize: "16px", marginBottom: "4px" }}>
          Note that there are some minor differences in game structure between days, so the expected duration may vary slightly.
          Based on our own experience, we expect that you will spend roughly:
        </p>
        <ul style={{ fontSize: "14px", marginBottom: "12px" }}>
          <li>1 hour on Day 1</li>
          <li>45 minutes on Day 2</li>
          <li>1 hour and 20 minutes on Day 3 (including survey)</li>
        </ul>
        <p style={{ fontSize: "16px", marginBottom: "28px" }}>
          After completing the Day 3 game, please don't forget to fill in the final survey via the LimeSurvey link below. 
        </p>
        <p style={{ fontSize: "16px", marginBottom: "28px" }}>
          The compensation for your participation in this study will be arranged some time after May 24th, the end date of this study.
          For questions or remarks, please contact{" "}
          <a href="mailto:m.a.j.t.brake@student.tue.nl">
            m.a.j.t.brake@student.tue.nl
          </a>{" "}
          or{" "}
          <a href="mailto:m.hulshof@student.tue.nl">
            m.hulshof@student.tue.nl
          </a>
          .
        </p>
        <ul style={{ fontSize: "20px", marginBottom: "28px" }}>
          <li>
            <a href="/f8622112/day1">Day 1</a>
          </li>
          <li>
            <a href="/f8622112/day2">Day 2</a>
          </li>
          <li>
            <a href="/f8622112/day3">Day 3</a>
          </li>
          <li>
            <a href="https://youtu.be/dQw4w9WgXcQ?si=NhMtoN9YBiqklc7q">LimeSurvey Link</a>
          </li>
        </ul>
      </>
    </div>
  );
}

export default Overview;
