import React from "react";
import axios from "axios";
import AttackPathGraph from "./AttackPathGraph";

function App() {
  const [paths, setPaths] = React.useState([]);
  const [loading, setLoading] = React.useState(false);

  // Fetch paths on load
  React.useEffect(() => {
    fetchPaths();
  }, []);

  const fetchPaths = async () => {
    setLoading(true);
    const res = await axios.get("http://localhost:5028/api/attack/top_paths");
    console.log(res.data);
    setPaths(res.data);
    setLoading(false);
  };

  const trainAlice = async () => {
    try {
      const res = await axios.post("http://localhost:5028/api/attack/toggle_training", { user_id: "U1001" });
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <div>
      <h1>Social-Engineering Attack-Path Simulator</h1>
      <button onClick={fetchPaths}>Fetch Paths</button>
      <button onClick={trainAlice}>Train Alice</button>
      <AttackPathGraph />
    </div>
  );
}

export default App;