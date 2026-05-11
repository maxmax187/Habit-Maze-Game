import React from "react";
import { DayInfo } from "./DayInfo";
import { StartExperimentButton } from "./StartExperimentButton";

function Day3() {
  return (
    <div>
      <div className="logo-container">
        <img src="tue_logo.png" />
      </div>
      <h2>Day 3</h2>
      <DayInfo />
      <StartExperimentButton link="/f8622112/experiment3" />
    </div>
  );
}

export default Day3;
