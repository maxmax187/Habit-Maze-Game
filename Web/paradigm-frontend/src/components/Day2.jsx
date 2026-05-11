import React from "react";
import { DayInfo } from "./DayInfo";
import { StartExperimentButton } from "./StartExperimentButton";

function Day2() {
  return (
    <div>
      <div className="logo-container">
        <img src="tue_logo.png" />
      </div>
      <h2>Day 2</h2>
      <DayInfo />
      <StartExperimentButton link="/f8622112/experiment2" />
    </div>
  );
}

export default Day2;
