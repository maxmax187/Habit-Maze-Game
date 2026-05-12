import React from "react";
import { StartExperimentButton } from "./StartExperimentButton";

function DevPage() {
  return (
    <div>
      <div className="logo-container">
        <img src="tue_logo.png" />
      </div>
      <h2>Dev Page</h2>
      If you are seeing this page you are in the wrong place.
      Developers only!
      {/* <StartExperimentButton link="/f8622112/devtestbuild" /> */}
      <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem", marginTop: "1rem" }}>
        <a href="/f8622112/devbuilds">Dev Builds</a>
        <a href="/f8622112/api/admin.php">DB Admin Panel</a>
        <a href="/f8622112/api/export.php">Export Data</a>
      </div>
    </div>
  );
}

export default DevPage;