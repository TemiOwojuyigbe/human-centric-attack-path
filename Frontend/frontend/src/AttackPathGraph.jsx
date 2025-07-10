import React from "react";
import ReactFlow, { Background, Controls, MarkerType } from "reactflow";
import "reactflow/dist/style.css";

// Helper to assign node type
const getNodeType = (label) => {
  if (label === "Attacker") return "attacker";
  if (
    label.includes("Office") ||
    label.includes("Room") ||
    label.includes("Lobby") ||
    label.includes("Area") ||
    label.includes("Department") ||
    label.includes("Suite") ||
    label.includes("Lab")
  )
    return "location";
  if (
    label.includes("Database") ||
    label.includes("Directory") ||
    label.includes("System") ||
    label.includes("Repository") ||
    label.includes("Infrastructure") ||
    label.includes("Communications") ||
    label.includes("Server") ||
    label.includes("Application")
  )
    return "asset";
  return "user";
};

const nodeStyles = {
  attacker: {
    background: "#fff",
    color: "#222",
    borderRadius: 6,
    border: "1px solid #aaa",
    fontFamily: 'Segoe UI, Arial, sans-serif',
    fontWeight: 600
  },
  user: {
    background: "#fff",
    color: "#222",
    borderRadius: 6,
    border: "1px solid #aaa",
    fontFamily: 'Segoe UI, Arial, sans-serif',
    fontWeight: 600
  },
  location: {
    background: "#fff",
    color: "#222",
    borderRadius: 6,
    border: "1px solid #aaa",
    fontFamily: 'Segoe UI, Arial, sans-serif',
    fontWeight: 600
  },
  asset: {
    background: "#fff",
    color: "#222",
    borderRadius: 6,
    border: "1px solid #aaa",
    fontFamily: 'Segoe UI, Arial, sans-serif',
    fontWeight: 600
  }
};

const UserNode = ({ label, vuln }) => (
  <div title={typeof vuln === 'number' && !isNaN(vuln) ? `Vulnerability: ${vuln.toFixed(2)}` : undefined}>
    {label} {typeof vuln === 'number' && !isNaN(vuln) ? <span style={{ fontSize: 12, color: '#b71c1c' }}>(Vuln: {vuln.toFixed(2)})</span> : null}
  </div>
);

function parsePathString(pathString) {
  return pathString.replace(/\u003E/g, ">")
    .split("->")
    .map(s => s.trim());
}

export default function AttackPathGraph({ paths, allUsers }) {
  if (!paths || paths.length === 0) return <div>No paths to display.</div>;

  // Build unique nodes and edges for the attack graph (from all paths)
  const nodeMap = new Map();
  const edgeSet = new Set();
  let nodes = [];
  let edges = [];
  let rowMap = {};
  let yStep = 120;
  let xStep = 220;

  // 1. Build nodes and edges from all paths
  paths.forEach((p) => {
    const steps = parsePathString(p.path || p.Path);
    steps.forEach((label, idx) => {
      if (!nodeMap.has(label)) {
        const type = getNodeType(label);
        let vuln = undefined;
        const vulnMatch = label.match(/Vuln: ([0-9.]+)/);
        if (vulnMatch) vuln = Number(vulnMatch[1]);
        if (!rowMap[idx]) rowMap[idx] = 0;
        nodeMap.set(label, true);
        nodes.push({
          id: label,
          data: {
            label: type === "user" ? <UserNode label={label} vuln={vuln} /> : label
          },
          position: { x: idx * xStep, y: rowMap[idx] * yStep },
          style: nodeStyles[type],
          type
        });
        rowMap[idx]++;
      }
      if (idx > 0) {
        const prev = steps[idx - 1];
        const edgeId = `${prev}->${label}`;
        if (!edgeSet.has(edgeId)) {
          edgeSet.add(edgeId);
          edges.push({
            id: edgeId,
            source: prev,
            target: label,
            markerEnd: { type: MarkerType.ArrowClosed },
            animated: false
          });
        }
      }
    });
  });

  // (Removed: Add isolated user nodes)

  return (
    <div style={{ width: "100vw", height: "90vh" }}>
      <h2 style={{ fontFamily: 'Segoe UI, Arial, sans-serif', fontWeight: 600 }}>Attack Path Graph</h2>
      <ReactFlow
        nodes={nodes}
        edges={edges}
        fitView
        style={{ background: "#fff" }}
        nodesDraggable={false}
        nodesConnectable={false}
        elementsSelectable={false}
      >
        <Background />
        <Controls />
      </ReactFlow>
    </div>
  );
}