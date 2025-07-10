import React, { useState, useEffect } from "react";
import axios from "axios";
import AttackPathGraph from "./AttackPathGraph";
import "reactflow/dist/style.css";

const profiles = [
  { label: "Stealthy", value: "Stealthy" },
  { label: "Aggressive", value: "Aggressive" },
  { label: "Opportunistic", value: "Opportunistic" }
];

function App() {
  const [allPaths, setAllPaths] = useState([]);
  const [selectedProfile, setSelectedProfile] = useState(profiles[0].value);
  const [showAll, setShowAll] = useState(true);
  const [allUsers, setAllUsers] = useState([]);
  const [showVulnTable, setShowVulnTable] = useState(false);
  const [userVulns, setUserVulns] = useState([]);

  useEffect(() => {
    if (showAll) {
      axios.get(`http://localhost:5028/api/attack/attack_paths?profile=${selectedProfile}`)
        .then(res => setAllPaths(res.data));
    } else {
      axios.get(`http://localhost:5028/api/attack/top_paths?profile=${selectedProfile}&topN=5`)
        .then(res => setAllPaths(res.data));
    }
    // Fetch all users for isolated nodes
    axios.get(`http://localhost:5028/api/attack/test_vulnerability`)
      .then(res => setAllUsers(res.data));
  }, [selectedProfile, showAll]);

  const fetchUserVulns = async () => {
    const res = await axios.get("http://localhost:5028/api/attack/user_vulnerabilities");
    setUserVulns(res.data);
    setShowVulnTable(v => !v);
  };

  return (
    <div>
      <h1 style={{ fontFamily: 'Segoe UI, Arial, sans-serif', fontWeight: 700 }}>Social-Engineering Attack-Path Simulator</h1>
      <select value={selectedProfile} onChange={e => setSelectedProfile(e.target.value)}>
        {profiles.map(p => <option key={p.value} value={p.value}>{p.label}</option>)}
      </select>
      <button onClick={() => setShowAll(true)} style={{ marginRight: 8 }}>
        Show All Paths
      </button>
      <button onClick={() => setShowAll(false)}>
        Show Best Path
      </button>
      <button onClick={fetchUserVulns} style={{ marginLeft: 16 }}>
        {showVulnTable ? "Hide" : "Show"} All User Vulnerabilities
      </button>
      {showVulnTable && (
        <div style={{ margin: '20px 0' }}>
          <table style={{ borderCollapse: 'collapse', width: '100%', maxWidth: 600 }}>
            <thead>
              <tr>
                <th style={{ border: '1px solid #aaa', padding: 8 }}>User</th>
                <th style={{ border: '1px solid #aaa', padding: 8 }}>Vulnerability Score</th>
              </tr>
            </thead>
            <tbody>
              {userVulns.map(u => (
                <tr key={u.user_id}>
                  <td style={{ border: '1px solid #aaa', padding: 8 }}>{u.name}</td>
                  <td style={{ border: '1px solid #aaa', padding: 8 }}>
                    {typeof u.vulnerability_score === 'number' && !isNaN(u.vulnerability_score)
                      ? u.vulnerability_score.toFixed(2)
                      : 'N/A'}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      <AttackPathGraph paths={allPaths} allUsers={allUsers} />
    </div>
  );
}

export default App;