import React from "react";
import { useState } from "react";
import { DayInfo } from "./DayInfo";
import { StartExperimentButton } from "./StartExperimentButton";

function ConsentForm() {
  const [consent, setConsent] = useState("");
  const [consentChoice, setConsentChoice] = useState("agree");

  const tableStyles = {
    borderCollapse: "collapse",
    borderSpacing: 0,
  };

  const cellStyles = {
    borderColor: "black",
    borderStyle: "solid",
    borderWidth: "1px",
    fontSize: "14px",
    overflow: "hidden",
    padding: "10px 5px",
    wordBreak: "normal",
  };

  const italicCellStyles = {
    ...cellStyles,
    color: "#000000",
    textAlign: "left",
    verticalAlign: "top",
    fontStyle: "italic",
  };

  const handleContinue = (event) => {
    event.preventDefault();
    if (consent === "agree") {
      setConsentChoice("agree");
    } else if (consent === "disagree") {
      setConsentChoice("disagree");
    }
  };

  return (
    <div>
      <div className="logo-container">
        <img src="tue_logo.png" />
      </div>

      {consentChoice === "empty" && (
        <div>
          <h1>
            Consent Form and Information for Experiment “A maze game to study
            game engagement and human motivation”
          </h1>
          <div className="c-container">
            <h2>1. Introduction</h2>
            <p>
              You have been invited to take part in research project “A maze
              game to study game engagement and human motivation”, because you
              responded to an invite through the participant database or were
              personally recruited.
            </p>
            <p>
              Participation in this research project is voluntary: the decision
              to take part is up to you. Before you decide to participate we
              would like to ask you to read the following information, so that
              you know what the research project is about, what we expect from
              you and how we deal with processing your personal data. Based on
              this information you can indicate via the consent declaration
              whether you consent to take part in this research project and the
              processing of your personal data.
            </p>
            <h2>2. Purpose of the research</h2>
            <p>
              This research project will be managed by Paul Evers, under the
              supervision of Dr. Chao Zhang and Dr. Max Birk. The purpose of
              this research project is to study how engagement influences
              motivation and decision-making in the context of a maze game.{" "}
            </p>

            <h2>3. Controller in the sense of the GDPR</h2>
            <p>
              TU/e is responsible for processing your personal data within the
              scope of the research. The contact details of TU/e are:
            </p>
            <p>Technische Universiteit Eindhoven</p>
            <p>De Groene Loper 3</p>
            <p>5612 AE Eindhoven </p>

            <h2>4. What will taking part in the research project involve?</h2>
            <p>
              You will be taking part in a research project in which we will
              gather information by:{" "}
            </p>
            <ul>
              <li>
                Asking you to fill in various online questionnaires during a
                period of 3 days, 6 times about behavior in the experiment and
                your experience with it.
              </li>
              <li>
                An experiment in which realtime game data and behavioral data is
                collected that includes character information, player position,
                player choices, and player performance.
              </li>
            </ul>
            <p>
              Participation will take approximately 30 minutes each day for 3
              consecutive days. For your participation in this research project
              you will receive a compensation of €20,- for completing all 3 days
              as a sign of our appreciation. If you are among the top 5
              performing participants, another €5,- will be rewarded. In case of
              dropping out before the end of the experiment, you will be paid
              for your efforts accordingly.
            </p>

            <h2>5. Potential risks and inconveniences </h2>
            <p>
              Your participation in this research project does not involve any
              physical, legal or economic risks. You do not have to answer
              questions which you do not wish to answer. Your participation is
              voluntary. This means that you may end your participation at any
              moment you choose by letting the researcher know this. You do not
              have to explain why you decided to end your participation in the
              research project. None of this will have any negative consequences
              for you whatsoever.
            </p>

            <h2>6. What personal data from you do we gather and process? </h2>
            <p>
              Within the framework of the research project we process the
              folllowing personal data:
            </p>
            <table style={tableStyles}>
              <thead>
                <tr>
                  <th style={italicCellStyles}>Category</th>
                  <th style={italicCellStyles}>Personal data</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td style={italicCellStyles}>Contact data</td>
                  <td style={italicCellStyles}>Name, e-mail</td>
                </tr>
                <tr>
                  <td style={italicCellStyles}>Banking data</td>
                  <td style={italicCellStyles}>
                    Banking information to pay out the participation reward.
                  </td>
                </tr>
                <tr>
                  <td style={italicCellStyles}>Game &amp; Behavioral data</td>
                  <td style={italicCellStyles}>
                    Character information, player position, player choices, and
                    player performance.
                  </td>
                </tr>
              </tbody>
            </table>

            <h2>7. Confidentiality, use, and storage of data</h2>
            <p>
              We will do everything we can to protect your privacy as best as
              possible. The research results that will be published will not in
              any way contain confidential information or personal data from or
              about you through which anyone can recognize you, unless in our
              consent form you have explicitly given your consent for mentioning
              your name, for example in a quote.{" "}
            </p>
            <p>
              The personal data that were gathered via the online web-app and
              surveys and other documents within the framework of this research
              project, will be stored on an encrypted server of the Human
              Technology Interaction group. The raw and processed research data
              will be retained for a period of 10 years, with the exception of
              banking details which will be deleted after the payments has been
              completed. The research data will, if necessary (e.g. for a check
              on scientific integrity) and only in anonymous form be made
              available to persons outside the research group.{" "}
            </p>

            <h2>8. Withdrawing your consent and contact details</h2>
            <p>
              Participation in this research project is entirely voluntary. You
              may end your participation in the research project at any moment,
              or withdraw your consent to using your data for the research,
              without specifying any reason. Ending your participation will have
              no disadvantageous consequences for you or for any compensation
              you may already have received.{" "}
            </p>
            <p>
              Do you wish to end the research, or do you have any questions
              and/or complaints? Then please contact the researcher via{" "}
              <a href="mailto:p.h.a.evers@student.tue.nl">
                p.h.a.evers@student.tue.nl
              </a>
              .
            </p>

            <h2>9. Legal ground for processing your personal data</h2>
            <p>The legal basis upon which we process your data is consent. </p>
            <p>
              This research project was assessed and approved by the ethical
              review committee of the HTI group of the Eindhoven University of
              Technology.
            </p>

            <h2>Consent form for participation</h2>
            <p>By signing this consent form I acknowledge the following: </p>
            <ol>
              <li>
                I am sufficiently informed about the research project through a
                separate information sheet. I have read the information sheet
                and have had the opportunity to ask questions. These questions
                have been answered satisfactorily.{" "}
              </li>
              <li>
                I take part in this research project voluntarily. There is no
                explicit or implicit pressure for me to take part in this
                research project. It is clear to me that I can end participation
                in this research project at any moment, without giving any
                reason. I do not have to answer a question if I do not wish to
                do so.{" "}
              </li>

              <p>
                Furthermore, I consent to the following parts of the research
                project:
              </p>
              <li>
                I consent to processing my personal data gathered during the
                research in the way described in the information sheet.
              </li>
            </ol>
          </div>

          <div className="form-container">
            <form onSubmit={handleContinue}>
              <div>
                <input
                  type="radio"
                  id="agree"
                  name="consent"
                  value="agree"
                  checked={consent === "agree"}
                  onChange={() => setConsent("agree")}
                />
                <label htmlFor="agree">I consent</label>
              </div>
              <div>
                <input
                  type="radio"
                  id="disagree"
                  name="consent"
                  value="disagree"
                  checked={consent === "disagree"}
                  onChange={() => setConsent("disagree")}
                />
                <label htmlFor="disagree">I do not consent</label>
              </div>
              <button type="submit" disabled={consent === ""}>
                Continue
              </button>
            </form>
          </div>
        </div>
      )}

      {consentChoice === "disagree" && (
        <div>You are not able to participate. Thank you for your time.</div>
      )}

      {consentChoice === "agree" && (
        <div>
          <h2>Day 1</h2>
          <DayInfo />
          <StartExperimentButton link="/f8622112/experiment1" />
        </div>
      )}
    </div>
  );
}

export default ConsentForm;
