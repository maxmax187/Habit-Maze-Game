import React, { useState } from "react";
import { StartExperimentButton } from "./StartExperimentButton";

const DEV_PASSWORD = "letmetest";

function DevBuildsPage() {
  const [input, setInput] = useState("");
  const [authed, setAuthed] = useState(false);
  const [error, setError] = useState("");

  function handleSubmit(e) {
    e.preventDefault();
    if (input === DEV_PASSWORD) {
      setAuthed(true);
      setError("");
    } else {
      setError("Incorrect password.");
    }
  }

  if (!authed) {
    return (
      <div>
        <div className="logo-container">
          <img src="tue_logo.png" />
        </div>
        <h2>Dev Builds</h2>
        <form onSubmit={handleSubmit} style={{ display: "flex", flexDirection: "column", gap: "0.5rem", maxWidth: "300px" }}>
          <label style={{ fontSize: "0.85rem" }}>Password</label>
          <input
            type="password"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            autoFocus
            style={{ padding: "0.5rem", fontSize: "1rem" }}
          />
          {error && <span style={{ color: "red", fontSize: "0.8rem" }}>{error}</span>}
          <button type="submit" style={{ padding: "0.5rem", cursor: "pointer" }}>
            Enter
          </button>
        </form>
      </div>
    );
  }

  return (
    <div>
      <div className="logo-container">
        <img src="tue_logo.png" />
      </div>
      <h2>Dev Builds</h2>
      <p style={{ marginBottom: "1rem", fontSize: "0.9rem" }}>
        Developer access only.
      </p>
      <p style={{ marginBottom: "1rem", fontSize: "0.9rem" }}>
        <u><strong>DO NOT RUN GAMES DURING STUDY, IT WILL INSERT DATA INTO DB AND MESS UP DATA ANALYSIS</strong></u>
      </p>
      <div style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
        <div>
          <p style={{ marginBottom: "0.25rem" }}>Day 1 Dev Build</p>
          <StartExperimentButton link="/f8622112/devbuild1" />
        </div>
        <div>
          <p style={{ marginBottom: "0.25rem" }}>Day 2 Dev Build</p>
          <StartExperimentButton link="/f8622112/devbuild2" />
        </div>
        <div>
          <p style={{ marginBottom: "0.25rem" }}>Day 3 Dev Build</p>
          <StartExperimentButton link="/f8622112/devbuild3" />
        </div>
      </div>
    </div>
  );
}

export default DevBuildsPage;