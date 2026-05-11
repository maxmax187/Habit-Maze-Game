import React from "react";

function Overview() {
  return (
    <div>
      <div className="logo-container">
        <img src="tue_logo.png" />
      </div>
      <h2>Overview</h2>
      <>
        <p style={{ fontSize: "20px", marginBottom: "28px" }}>
          This experiment has ended. However, you can still access the
          experiment here for educational purposes. Please note that your data
          is no longer persisted or uploaded to TUe servers, therefore some
          functions like scoring or character selection might not work as
          intended. Please use the menu below to navigate to different days of
          the experiment.
        </p>
        <p style={{ fontSize: "20px", marginBottom: "28px" }}>
          For questions or remarks, please contact{" "}
          <a href="mailto:p.h.a.evers@student.tue.nl">
            p.h.a.evers@student.tue.nl
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
        </ul>
      </>
    </div>
  );
}

export default Overview;
