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
    </div>
  );
}

export default Day2;
