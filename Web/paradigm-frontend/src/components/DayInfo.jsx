import React from "react";

export function DayInfo() {
  return (
    <>
      <p style={{ fontSize: "20px", marginBottom: "28px" }}>
        <strong>Important Note!</strong> You can only participate on a computer
        with keyboard input. If you are on a mobile phone, tablet, etc. please
        switch to a laptop or desktop computer.
      </p>
      <ul>
        <li>
          <strong>
            A stable constant connection to the internet is required!
          </strong>
        </li>
        <li>
          Please try to participate in the experiment at a set time each day,
          e.g., 18:30 each day.
        </li>
        <li>
          Your participation will be registered at the end of the experiment
          each day. <strong><u>Do not close out of the game until you are instructed to do so.</u></strong>
        </li>
        {/* <li>
          For further questions please email:{" "}
          <a href="mailto:m.a.j.t.brake@student.tue.nl">
            m.a.j.t.brake@student.tue.nl
          </a>{" "}
          or{" "}
          <a href="mailto:m.hulshof@student.tue.nl">
            m.hulshof@student.tue.nl
          </a>
          .
        </li> */}
      </ul>
    </>
  );
}
