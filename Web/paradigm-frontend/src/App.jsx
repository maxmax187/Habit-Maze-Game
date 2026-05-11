import "./App.css";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Day1 from "./components/Day1";
import Day2 from "./components/Day2";
import Day3 from "./components/Day3";
import Overview from "./components/Overview";
import DevPage from "./components/DevPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Overview />} />
        <Route path="/f8622112" element={<Overview />} />
        <Route path="/f8622112/day1" element={<Day1 />} />
        <Route path="/f8622112/day2" element={<Day2 />} />
        <Route path="/f8622112/day3" element={<Day3 />} />
        <Route path="/f8622112/devpage" element={<DevPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
