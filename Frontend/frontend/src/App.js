import React from "react";
import axios from "axios";
function App() {
  const [paths, setPaths] = React.useState([]);
  const [loading, setLoading] = React.useState(false);

  // Fetch paths on load
  React.useEffect(() => {
    fetchPaths();
  }, []);

  const fetchPaths = async () => {
    setLoading(true);
    const res = await axios.get("http://localhost:5000/api/attack/top_paths");
    setPaths(res.data);
    setLoading(false);
  };

  const trainAlice = async () => {
    await axios.post("http://localhost:5000/api/attack/toggle_training", { user_id: "U1001" });
    fetchPaths();
  };

  return (
    <div>
      <h1>Social-Engineering Attack-Path Simulator</h1>
      <button onClick={fetchPaths}>Fetch Paths</button>
      <button onClick={trainAlice}>Train Alice</button>
      {/* Render chart and table here */}
    </div>
  );
}

export default App;