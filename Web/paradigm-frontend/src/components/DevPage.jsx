import React from "react";
import { DayInfo } from "./DayInfo";
import { StartExperimentButton } from "./StartExperimentButton";

function Day2() {
  return (
    <div>
      <div className="logo-container">
        <img src="tue_logo.png" />
      </div>
      <h2>Dev Page</h2>
      If you are seeing this page you are in the wrong place.
      Developers only!
      <StartExperimentButton link="/f8622112/devtestbuild" />
      <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem", marginTop: "1rem" }}>
        <a href="https://htionline.tue.nl/f8622112/api/admin.php" target="_blank" rel="noreferrer">
          DB Admin
        </a>
        <a href="https://htionline.tue.nl/f8622112/api/export.php" target="_blank" rel="noreferrer">
          Export Data
        </a>
      </div>
    </div>
  );
}

export default Day2;