import React, { useEffect, useState } from "react";
import ReactFlow, { Background, Controls } from "reactflow";
import "reactflow/dist/style.css";
import axios from "axios";

const API_URL = "http://localhost:5028/api/attack/attack_paths";

function parsePathString(pathString) {
  // Replace unicode with > and split by '->'
  return pathString.replace(/\\u003E/g, ">").split(">").map(s => s.trim());
}

function buildGraphData(paths) {
  const nodes = {};
  const edges = [];
  let nodeId = 1;

  paths.forEach((p, idx) => {
    const steps = parsePathString(p.path);
    let prevNode = null;

    steps.forEach((step, i) => {
      if (!nodes[step]) {
        nodes[step] = {
          id: step,
          data: { label: step },
          position: { x: 150 * i, y: 100 * idx }
        };
      }
      if (prevNode) {
        edges.push({
          id: `${prevNode}-${step}-${idx}`,
          source: prevNode,
          target: step,
          animated: true,
          label: i === steps.length - 1 ? `Score: ${p.probability}` : undefined
        });
      }
      prevNode = step;
    });
  });

  return {
    nodes: Object.values(nodes),
    edges
  };
}

export default function AttackPathGraph() {
  const [graph, setGraph] = useState({ nodes: [], edges: [] });

  useEffect(() => {
    axios.get(API_URL).then(res => {
      setGraph(buildGraphData(res.data));
    });
  }, []);

  return (
    <div style={{ width: "100vw", height: "90vh" }}>
      <h2>Attack Path Graph</h2>
      <ReactFlow
        nodes={graph.nodes}
        edges={graph.edges}
        fitView
        style={{ background: "#f0f0f0" }}
      >
        <Background />
        <Controls />
      </ReactFlow>
    </div>
  );
}