"use client";

import React, { useState, useEffect, useCallback } from "react";

interface AiParameterProposal {
  proposalId: string;
  stockId: string;
  parameterName: string;
  currentValue: string;
  proposedValue: string;
  confidenceScore: number;
  reasoning: string;
  status: string;
  createdAt: string;
}

interface Position {
  id: string;
  symbol: string;
  type: string;
  expiry: string;
  strike: number;
  qty: number;
  px: number;
  iv: string;
  pnl: number;
  delta: number;
  gamma: number;
  theta: number;
  itmStatus?: "ITM" | "OTM" | "ATM";
}

interface OrderLog {
  id: string;
  timestamp: string;
  symbol: string;
  type: string;
  strategy: string;
  qty: number;
  px: number;
  status: "FILLED" | "PENDING" | "CANCELLED";
}

interface AuditLog {
  timestamp: string;
  category: "SYSTEM" | "AUDIT" | "GATEWAY" | "RISK";
  message: string;
}

interface DocChapter {
  id: string;
  name: string;
  path: string;
  size: string;
  title: string;
  version: string;
  category: string;
  eosLayer: string;
  communication: string;
  aiAgent: string;
  owner: string;
  lastUpdated: string;
  conceptTitle: string;
  conceptContent: string;
  flowTitle: string;
  flowContent: string;
}

export default function PlatformDashboard() {
  // Navigation Tabs matching mockup
  const [activeTab, setActiveTab] = useState<"dashboard" | "portfolio" | "research" | "order-book" | "analytics" | "logs">("dashboard");

  // Real-time states
  const [proposals, setProposals] = useState<AiParameterProposal[]>([]);
  const [loadingProposals, setLoadingProposals] = useState(true);
  const [proposalError, setProposalError] = useState<string | null>(null);

  // Live PnL tick simulation
  const [livePnL, setLivePnL] = useState(2182450.0);
  const [deltaVal, setDeltaVal] = useState(0.18);
  const [sysTime, setSysTime] = useState("");

  // RAG Interactive Console
  const [ragQuery, setRagQuery] = useState("");
  const [ragAnswer, setRagAnswer] = useState<string | null>(null);
  const [ragSources, setRagSources] = useState<string[]>([]);
  const [ragLoading, setRagLoading] = useState(false);

  // Options Analytics Calculator States
  const [calcSpot, setCalcSpot] = useState(2450.0);
  const [calcStrike, setCalcStrike] = useState(2500.0);
  const [calcLowerStrike, setCalcLowerStrike] = useState(2400.0);
  const [calcUpperStrike, setCalcUpperStrike] = useState(2500.0);
  const [calcVol, setCalcVol] = useState(18.5);
  const [calcDays, setCalcDays] = useState(30);
  const [calcIsCall, setCalcIsCall] = useState(true);

  // Calculator Results
  const [bsmPob, setBsmPob] = useState<number | null>(64.2);
  const [touchPob, setTouchPob] = useState<number | null>(72.5);
  const [rangePob, setRangePob] = useState<number | null>(58.3);
  const [calcLoading, setCalcLoading] = useState(false);

  // Alerts & Messages
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  // Search & Filters on Option Positions
  const [posSearch, setPosSearch] = useState("");
  const [posFilter, setPosFilter] = useState<"ALL" | "CE" | "PE">("ALL");

  // Logs category filter
  const [logFilter, setLogFilter] = useState<"ALL" | "SYSTEM" | "RISK" | "GATEWAY">("ALL");

  // State to read/fetch raw master markdown file dynamically
  const [docMarkdown, setDocMarkdown] = useState<string>("");
  const [showFullDocReader, setShowFullDocReader] = useState<boolean>(false);

  // Documentation Interactive Reader Chapters Data
  const chapters: DocChapter[] = [
    {
      id: "exec",
      name: "01. Executive Summary & Vision",
      path: "/01_Executive_Summary/01_executive_summary.md",
      size: "7.3 KB",
      title: "Executive Summary — Comprehensive Documentation",
      version: "1.0.0",
      category: "Executive Summary & Vision",
      eosLayer: "All Layers (Holistic System)",
      communication: "SignalR Hub & Kafka Streams",
      aiAgent: "VIB (VYUH Intelligence Brain)",
      owner: "Chief Quantitative Architect",
      lastUpdated: "2026-07-15",
      conceptTitle: "1. Concept & Vision",
      conceptContent: "VYUH (Sanskrit: व्यूह, meaning 'strategic placement' or 'battle array') is a next-generation institutional portfolio intelligence and construction engine designed specifically for the Indian Stock Options (NSE F&O) Market. The platform operates as a quantitative decision-support system, aiming to deploy and manage a target capital of ₹25 Crores (INR 250,000,000) across a diversified portfolio of 70 to 80 liquid stocks.",
      flowTitle: "2. Core System Boundaries (What VYUH is NOT)",
      flowContent: "To maintain structural integrity and high performance, VYUH enforces strict boundaries. It does not perform execution or low-level market scanning directly. It acts as the central brain. Every 10 seconds, it normalizes spot, future, ATR, and IV profiles; calculates Touch, Range, and Historical probabilities; identifies optimal strikes; and generates signed execution packets for Vega."
    },
    {
      id: "brd",
      name: "02. Business Requirements (BRD)",
      path: "/02_Business_Requirements/02_business_requirements.md",
      size: "9.2 KB",
      title: "Business Requirements — Comprehensive Documentation",
      version: "1.0.0",
      category: "Business Requirements (BRD)",
      eosLayer: "All Layers",
      communication: "Narad Event Bus",
      aiAgent: "VIB",
      owner: "Product Owner / CIO",
      lastUpdated: "2026-07-15",
      conceptTitle: "1. Business Mandate and Thesis",
      conceptContent: "VYUH is designed to capture the Variance Risk Premium (VRP) in the Indian equity derivatives market (NSE F&O). By consistently writing options (selling volatility) across a highly diversified basket of underlyings, VYUH harvests theta decay. The target monthly yield is 1.5% to 2.0% net of transaction costs, with a maximum peak-to-trough drawdown capped at 8% at the portfolio level.",
      flowTitle: "2. Portfolio Allocation & Diversification",
      flowContent: "The ₹25 Crore capital is structured into Margin Capital (60% / ₹15 Crores), Liquid Buffer (30% / ₹7.5 Crores), and Drawdown Reserve (10% / ₹2.5 Crores). No single underlying can consume more than 2.0% of the total allocated margin. Total capital allocated to any single NSE sector must not exceed 15% of the portfolio."
    },
    {
      id: "arch",
      name: "04. System Architecture Blueprint",
      path: "/04_Architecture/04_architecture.md",
      size: "11.5 KB",
      title: "System Architecture — Comprehensive Documentation",
      version: "1.0.0",
      category: "System Architecture",
      eosLayer: "Layer 2 — Integration & Ingestion",
      communication: "gRPC / Apache Kafka",
      aiAgent: "DataSync",
      owner: "System Architect",
      lastUpdated: "2026-07-15",
      conceptTitle: "1. Technical Architecture Blueprint",
      conceptContent: "VYUH is structured as a high-throughput, low-latency microservices stack using .NET 9.0. It communicates asynchronously via Apache Kafka and SignalR with Vega OMS and Lakshmi feed handlers. It deploys modular scoring, strike intelligence, and portfolio selection engines in a clean separation of concerns.",
      flowTitle: "2. Performance & Low Latency",
      flowContent: "The complete loop cycle must execute in < 1,800 milliseconds under a sustained throughput of 15,000 ticks/sec. Hot-standby replication is maintained via active-passive PostgreSQL setups with automated pgvector database synchronization."
    }
  ];

  const [activeChapter, setActiveChapter] = useState<DocChapter>(chapters[0]);

  const showToast = (msg: string) => {
    setToastMessage(msg);
    setTimeout(() => setToastMessage(null), 4000);
  };

  // Option positions matching the mockup and document parameters exactly
  const [positions, setPositions] = useState<Position[]>([
    { id: "P1", symbol: "RELIANCE", type: "CE", expiry: "27 JUN 24", strike: 2800, qty: 12000, px: 85.50, iv: "28.5%", pnl: 112000, delta: 0.45, gamma: 0.08, theta: -2.1, itmStatus: "ITM" },
    { id: "P2", symbol: "TCS", type: "PE", expiry: "27 JUN 24", strike: 3900, qty: 8000, px: 115.20, iv: "25.1%", pnl: -45000, delta: -0.38, gamma: 0.06, theta: -1.8, itmStatus: "OTM" },
    { id: "P3", symbol: "HDFCBANK", type: "CE", expiry: "27 JUN 24", strike: 3900, qty: 8000, px: 115.20, iv: "25.1%", pnl: 11000, delta: 0.45, gamma: 0.08, theta: -2.6, itmStatus: "OTM" },
    { id: "P4", symbol: "INFY", type: "PE", expiry: "27 JUN 24", strike: 3900, qty: 8000, px: 115.20, iv: "25.5%", pnl: 900, delta: 0.38, gamma: 0.06, theta: -3.5, itmStatus: "ATM" }
  ]);

  // Order Book state matching audit database
  const [orders, setOrders] = useState<OrderLog[]>([
    { id: "ORD-2947", timestamp: "17:00:10 UTC", symbol: "RELIANCE", type: "CE", strategy: "Short Strangle", qty: 12000, px: 85.50, status: "FILLED" },
    { id: "ORD-2946", timestamp: "16:58:30 UTC", symbol: "TCS", type: "PE", strategy: "Short Straddle", qty: 8000, px: 115.20, status: "FILLED" },
    { id: "ORD-2945", timestamp: "16:55:12 UTC", symbol: "HDFCBANK", type: "CE", strategy: "Gamma Hedge", qty: 8000, px: 115.20, status: "FILLED" },
    { id: "ORD-2944", timestamp: "16:50:00 UTC", symbol: "INFY", type: "PE", strategy: "Iron Condor", qty: 8000, px: 115.20, status: "FILLED" }
  ]);

  // Live Audit Logs
  const [auditLogs, setAuditLogs] = useState<AuditLog[]>([
    { timestamp: "17:04:36 UTC", category: "AUDIT", message: "User navigated to Portfolio cockpit tab view." },
    { timestamp: "17:02:10 UTC", category: "GATEWAY", message: "Order execution log created for RELIANCE CE strike 2800." },
    { timestamp: "17:01:23 UTC", category: "AUDIT", message: "PM approved VIB rebalancing proposal #p-1." },
    { timestamp: "17:00:15 UTC", category: "SYSTEM", message: "SignalR connection established with Gateway Hub on port 5200." },
    { timestamp: "16:56:37 UTC", category: "RISK", message: "Value at Risk limit valuation completed. Risk index: 2.45% (Safe)." }
  ]);

  // Fetch proposals from API
  const fetchProposals = useCallback(async () => {
    const apiBase = process.env.NEXT_PUBLIC_OPTIMIZER_API_URL || "http://localhost:5206";
    try {
      setLoadingProposals(true);
      const res = await fetch(`${apiBase}/api/v1/optimizer/vib/proposals/pending`);
      if (!res.ok) {
        throw new Error(`Error ${res.status}: ${res.statusText}`);
      }
      const data = await res.json();
      setProposals(data);
      setProposalError(null);
    } catch (err: any) {
      console.warn("Optimizer API is offline. Using simulated proposals fallback.", err.message);
      setProposalError(err.message);
      setProposals([
        {
          proposalId: "p-1",
          stockId: "NIFTY",
          parameterName: "Gamma Hedge",
          currentValue: "NIFTY 23500 CE",
          proposedValue: "BUY",
          confidenceScore: 0.94,
          reasoning: "Dynamic sector relative strength indexes indicate structural expansion post earnings.",
          status: "PENDING",
          createdAt: new Date().toISOString()
        },
        {
          proposalId: "p-2",
          stockId: "SBIN",
          parameterName: "Theta Decay",
          currentValue: "SBIN 700 PE",
          proposedValue: "SELL",
          confidenceScore: 0.88,
          reasoning: "Abnormal retail derivative open interest flags tail-risk threshold breach warnings.",
          status: "PENDING",
          createdAt: new Date().toISOString()
        },
        {
          proposalId: "p-3",
          stockId: "WIPRO",
          parameterName: "Vol Arb",
          currentValue: "WIPRO 500 CE",
          proposedValue: "SELL",
          confidenceScore: 0.91,
          reasoning: "Vega volatility contraction favors immediate short entries.",
          status: "PENDING",
          createdAt: new Date().toISOString()
        }
      ]);
    } finally {
      setLoadingProposals(false);
    }
  }, []);

  // Respond to proposal handler
  const handleProposalResponse = async (id: string, approved: boolean) => {
    const apiBase = process.env.NEXT_PUBLIC_OPTIMIZER_API_URL || "http://localhost:5206";
    try {
      const res = await fetch(`${apiBase}/api/v1/optimizer/vib/proposals/${id}/respond`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({ approved })
      });

      if (!res.ok) {
        throw new Error(`Failed to update proposal: ${res.statusText}`);
      }

      showToast(`AI proposal successfully ${approved ? "APPROVED" : "REJECTED"} and config updated.`);
      
      setAuditLogs(prev => [
        { timestamp: new Date().toLocaleTimeString("en-US", { hour12: false }) + " UTC", category: "AUDIT", message: `VIB proposal #${id} manually ${approved ? "APPROVED" : "REJECTED"} by PM.` },
        ...prev
      ]);
    } catch (err: any) {
      showToast(`[Simulation Mode] Proposal ${approved ? "Approved" : "Rejected"} successfully.`);
      setProposals(prev => prev.filter(p => p.proposalId !== id));
      
      setAuditLogs(prev => [
        { timestamp: new Date().toLocaleTimeString("en-US", { hour12: false }) + " UTC", category: "AUDIT", message: `[Simulated] VIB proposal #${id} ${approved ? "APPROVED" : "REJECTED"} successfully.` },
        ...prev
      ]);
    } finally {
      fetchProposals();
    }
  };

  // Trigger RAG queries
  const handleRAGQuery = (queryText: string) => {
    setRagLoading(true);
    setRagAnswer(null);

    setTimeout(() => {
      const query = queryText.toLowerCase();
      if (query.includes("reliance") || query.includes("crude")) {
        setRagAnswer(
          "RELIANCE exhibits an inverse relationship to Brent Crude price spikes above $85. Historically, gross refining margins (GRM) expand, but overall volatility crush speed is delayed by 1.5 days due to retail speculation. The VIB recommendation engine suggests tightening straddle delta caps to 0.12 during these regimes."
        );
        setRagSources(["NSE Corp Actions Feed - 2026-Q1", "Sector Valuation Report (Energy)", "VIB Historic Regime Vol Archive"]);
      } else if (query.includes("volatility") || query.includes("crush") || query.includes("earnings")) {
        setRagAnswer(
          "Analysis of NIFTY 100 constituent post-earnings volatility profiles shows that 82% of constituent stocks crush over 75% of implied volatility (IV) within 45 minutes of market open post-earnings. High-beta names (e.g., metals) exhibit the fastest decay rates, averaging a 90% crush."
        );
        setRagSources(["Option Chain Snapshots Archive (2024-2025)", "VIB Post-Earnings Model Weights"]);
      } else {
        setRagAnswer(
          `VIB Search Result for "${queryText}": System scans indicate a consolidation pattern across primary indices. pairewise correlation coefficients stand at 0.58. Options open interest suggests a strong support boundary 3% below spot levels. Quantitative regime classification: Neutral Volatility.`
        );
        setRagSources(["Regime Classification Weights", "Live Market Index Order Book"]);
      }
      setRagLoading(false);
      
      setAuditLogs(prev => [
        { timestamp: new Date().toLocaleTimeString("en-US", { hour12: false }) + " UTC", category: "SYSTEM", message: `VIB RAG query processed: "${queryText}".` },
        ...prev
      ]);
    }, 1200);
  };

  const handleRAGQuerySubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!ragQuery.trim()) return;
    handleRAGQuery(ragQuery);
  };

  // Option Calculator Trigger API call
  const calculateOptionProbabilities = async () => {
    const apiBase = process.env.NEXT_PUBLIC_OPTIMIZER_API_URL || "http://localhost:5206";
    setCalcLoading(true);
    try {
      const bsmRes = await fetch(`${apiBase}/api/v1/optimizer/probability/bsm`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          spotPrice: calcSpot,
          strikePrice: calcStrike,
          volatility: calcVol,
          daysToExpiry: calcDays,
          riskFreeRate: 0.07,
          isCall: calcIsCall
        })
      });
      if (bsmRes.ok) {
        const bsmData = await bsmRes.json();
        setBsmPob(bsmData.probabilityOfItm * 100);
      }

      const touchRes = await fetch(`${apiBase}/api/v1/optimizer/probability/touch`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          spotPrice: calcSpot,
          strikePrice: calcStrike,
          volatility: calcVol,
          daysToExpiry: calcDays,
          riskFreeRate: 0.07,
          isCall: calcIsCall
        })
      });
      if (touchRes.ok) {
        const touchData = await touchRes.json();
        setTouchPob(touchData.probabilityOfTouch * 100);
      }

      const rangeRes = await fetch(`${apiBase}/api/v1/optimizer/probability/range`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          spotPrice: calcSpot,
          lowerStrike: calcLowerStrike,
          upperStrike: calcUpperStrike,
          volatility: calcVol,
          daysToExpiry: calcDays,
          riskFreeRate: 0.07
        })
      });
      if (rangeRes.ok) {
        const rangeData = await rangeRes.json();
        setRangePob(rangeData.probabilityOfRange * 100);
      }

      showToast("Calculated probabilities from Optimizer Math Engine.");
    } catch (err: any) {
      console.warn("Calculations failed or API offline. Falling back to local simulation.");
      setBsmPob(64.2);
      setTouchPob(72.5);
      setRangePob(58.3);
      showToast("Simulated math calculation results displayed.");
    } finally {
      setCalcLoading(false);
    }
  };

  // Custom premium interactive Markdown parser & HTML renderer function
  const renderMarkdown = (markdown: string) => {
    if (!markdown) return <p className="text-slate-500 font-mono text-xs">Loading complete documentation content...</p>;

    const lines = markdown.split("\n");
    const elements: React.ReactNode[] = [];
    let inCodeBlock = false;
    let codeBlockLines: string[] = [];
    let tableLines: string[] = [];
    let inTable = false;

    const flushTable = (key: number) => {
      if (tableLines.length === 0) return null;
      const rows = tableLines.map(line =>
        line.split("|").map(cell => cell.trim()).filter((_, i, arr) => i > 0 && i < arr.length - 1)
      );
      tableLines = [];
      inTable = false;
      
      const headers = rows[0] || [];
      const hasDivider = rows[1] && rows[1].every(cell => cell.startsWith(":") || cell.startsWith("-"));
      const dataRows = hasDivider ? rows.slice(2) : rows.slice(1);

      return (
        <div key={`table-${key}`} className="overflow-x-auto my-3 w-full">
          <table className="w-full text-left text-sm border-collapse border border-slate-900 font-mono">
            <thead>
              <tr className="bg-slate-950 text-slate-400 font-bold border-b border-slate-900 text-sm">
                {headers.map((h, i) => (
                  <th key={i} className="p-2.5 border-r border-slate-900">{h}</th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-900 text-slate-300 text-sm">
              {dataRows.map((row, rIdx) => (
                <tr key={rIdx} className="hover:bg-[#050813]/40">
                  {row.map((cell, cIdx) => (
                    <td key={cIdx} className="p-2.5 border-r border-slate-900">{cell}</td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      );
    };

    for (let i = 0; i < lines.length; i++) {
      const line = lines[i];

      // Code Block Checker
      if (line.trim().startsWith("```")) {
        if (inCodeBlock) {
          inCodeBlock = false;
          const codeText = codeBlockLines.join("\n");
          codeBlockLines = [];
          elements.push(
            <pre key={`code-${i}`} className="bg-slate-950 p-3 rounded border border-slate-900 text-sm text-[#38bdf8] font-mono overflow-x-auto my-3 leading-relaxed">
              <code>{codeText}</code>
            </pre>
          );
        } else {
          inCodeBlock = true;
        }
        continue;
      }

      if (inCodeBlock) {
        codeBlockLines.push(line);
        continue;
      }

      // Table Checker
      if (line.trim().startsWith("|") && line.trim().endsWith("|")) {
        inTable = true;
        tableLines.push(line);
        continue;
      } else if (inTable) {
        const tableElem = flushTable(i);
        if (tableElem) elements.push(tableElem);
      }

      // Heading Parser
      if (line.startsWith("# ")) {
        elements.push(
          <h1 key={i} className="text-[#f97316] font-bold text-lg tracking-wider uppercase border-l-4 border-orange-500 pl-3.5 my-4 font-mono">
            {line.substring(2)}
          </h1>
        );
      } else if (line.startsWith("## ")) {
        elements.push(
          <h2 key={i} className="text-cyan-400 font-bold text-base uppercase tracking-wide my-3 font-mono">
            {line.substring(3)}
          </h2>
        );
      } else if (line.startsWith("### ")) {
        elements.push(
          <h3 key={i} className="text-slate-200 text-sm font-bold tracking-wider my-2.5 font-mono uppercase">
            {line.substring(4)}
          </h3>
        );
      } else if (line.trim() === "---") {
        elements.push(<hr key={i} className="border-slate-900 my-3" />);
      } else if (line.trim().startsWith("> [!")) {
        const alertType = line.includes("IMPORTANT") ? "IMPORTANT" : line.includes("WARNING") ? "WARNING" : "NOTE";
        elements.push(
          <div key={i} className={`p-3 rounded border my-2 text-sm font-mono ${
            alertType === "IMPORTANT" ? "bg-amber-955/20 border-amber-900/40 text-amber-400" :
            alertType === "WARNING" ? "bg-rose-955/20 border-rose-900/40 text-rose-455" :
            "bg-blue-955/20 border-blue-900/40 text-blue-400"
          }`}>
            <strong>{alertType}:</strong> {lines[i+1]?.replace("> *", "")?.trim()}
          </div>
        );
        i++; // Skip descriptive next line
      } else if (line.trim()) {
        let processed = line;
        
        // Custom check for spec definitions like: * Purpose: ...
        const specRegex = /^\*\s+(Purpose|Inputs|Outputs|Process|Result|Rule\s+\d+):\s*(.*)$/i;
        const matchSpec = specRegex.exec(processed);
        
        if (matchSpec) {
          const type = matchSpec[1].toUpperCase();
          const desc = matchSpec[2];
          
          let typeColor = "bg-blue-950 text-blue-400 border-blue-900/40";
          if (type.includes("INPUT")) typeColor = "bg-emerald-950 text-emerald-400 border-emerald-900/40";
          if (type.includes("OUTPUT")) typeColor = "bg-amber-950 text-amber-400 border-amber-900/40";
          if (type.includes("PROCESS")) typeColor = "bg-cyan-950 text-cyan-400 border-cyan-800/40";
          if (type.includes("RULE")) typeColor = "bg-rose-950 text-rose-455 border-rose-900/40";
          
          elements.push(
            <div key={i} className="flex items-start gap-2.5 my-2.5 font-mono text-sm hover:translate-x-1 transition-transform duration-200">
              <span className={`inline-block border px-2 py-0.5 rounded text-xs font-bold tracking-wider shrink-0 ${typeColor}`}>
                {type}
              </span>
              <span className="text-slate-300 leading-relaxed">{desc}</span>
            </div>
          );
          continue;
        }

        // Custom step badge parser: 1. ... 2. ...
        const stepRegex = /^(\d+)\.\s+(.*)$/;
        const matchStep = stepRegex.exec(processed);
        if (matchStep) {
          const num = matchStep[1];
          const text = matchStep[2];
          elements.push(
            <div key={i} className="flex items-start gap-2.5 my-2.5 font-mono text-sm">
              <span className="inline-flex items-center justify-center bg-cyan-950 text-cyan-400 border border-cyan-800 w-5 h-5 rounded-full text-xs font-bold shrink-0 mt-0.5">
                {num}
              </span>
              <span className="text-slate-300 leading-relaxed">{text}</span>
            </div>
          );
          continue;
        }

        // Bold text replacements
        const boldRegex = /\*\*(.*?)\*\*/g;
        const parts = [];
        let lastIndex = 0;
        let match;
        while ((match = boldRegex.exec(processed)) !== null) {
          if (match.index > lastIndex) {
            parts.push(processed.substring(lastIndex, match.index));
          }
          parts.push(<strong key={match.index} className="font-bold text-slate-100">{match[1]}</strong>);
          lastIndex = boldRegex.lastIndex;
        }
        if (lastIndex < processed.length) {
          parts.push(processed.substring(lastIndex));
        }

        elements.push(
          <p key={i} className="text-sm text-slate-300 leading-relaxed my-2 font-sans pl-1">
            {parts.length > 0 ? parts : processed}
          </p>
        );
      }
    }

    if (inTable) {
      const tableElem = flushTable(lines.length);
      if (tableElem) elements.push(tableElem);
    }

    return <div className="space-y-1">{elements}</div>;
  };

  // Fetch complete markdown document on init
  useEffect(() => {
    fetch("/VYUH_Complete_Documentation.md")
      .then(res => {
        if (!res.ok) throw new Error("File not found in public folder");
        return res.arrayBuffer();
      })
      .then(buffer => {
        const decoder = new TextDecoder("utf-8");
        const text = decoder.decode(buffer);
        setDocMarkdown(text);
      })
      .catch(err => {
        console.warn("Unable to fetch complete documentation. Using simulated fallback.", err.message);
        setDocMarkdown("# VYUH Complete Documentation\n\n---\n\n# 01_Executive_Summary\n\n## 1. Concept & Vision\nVYUH is a next-generation institutional portfolio intelligence and construction engine designed specifically for the Indian Stock Options (NSE F&O) Market. Operating as a quantitative decision-support system, it deploys and manages ₹25 Crores.\n\n## 2. Core System Boundaries\nIt does not perform execution or low-level market scanning directly. It compiles proposed portfolio transitions into a standardized, signed JSON packet and pushes to Vega.");
      });

    const pnlTimer = setInterval(() => {
      setLivePnL(prev => prev + (Math.random() * 800 - 400));
      setDeltaVal(prev => parseFloat((prev + (Math.random() * 0.004 - 0.002)).toFixed(2)));
    }, 3000);

    const timeTimer = setInterval(() => {
      const now = new Date();
      setSysTime(
        now.toLocaleTimeString("en-US", { hour12: false }) + " UTC"
      );
    }, 1000);

    return () => {
      clearInterval(pnlTimer);
      clearInterval(timeTimer);
    };
  }, [fetchProposals]);

  const pnlPct = ((livePnL - 2150000) / 2150000 * 100).toFixed(2);

  // Switch tab handles logs logging
  const handleTabChange = (tab: typeof activeTab) => {
    setActiveTab(tab);
    setAuditLogs(prev => [
      { timestamp: new Date().toLocaleTimeString("en-US", { hour12: false }) + " UTC", category: "AUDIT", message: `User navigated to ${tab.toUpperCase()} cockpit tab view.` },
      ...prev
    ]);
  };

  // Filter option positions logic
  const filteredPositions = positions.filter(pos => {
    const matchesSearch = pos.symbol.toLowerCase().includes(posSearch.toLowerCase());
    const matchesFilter = posFilter === "ALL" || pos.type === posFilter;
    return matchesSearch && matchesFilter;
  });

  // Filter audit logs logic
  const filteredAuditLogs = auditLogs.filter(log => {
    return logFilter === "ALL" || log.category === logFilter;
  });

  return (
    <div className="flex-1 bg-[#040815] text-slate-100 flex flex-col min-h-screen font-sans w-full select-none antialiased">
      {/* Toast Alert */}
      {toastMessage && (
        <div className="fixed bottom-6 right-6 z-50 bg-[#0c1224]/90 border border-blue-500/50 text-blue-400 px-6 py-4 rounded-lg shadow-2xl backdrop-blur-md flex items-center gap-3">
          <span className="text-blue-500 text-lg">✔</span>
          <span className="font-mono text-base">{toastMessage}</span>
        </div>
      )}

      {/* Main Container with 100% width & clean layout matching mockup */}
      <div className="w-full flex flex-col min-h-screen">
        
        {/* Top Header matching mockup precisely */}
        <header className="border-b border-slate-900 bg-[#080d1e]/80 backdrop-blur-md px-6 py-5 flex flex-col lg:flex-row justify-between items-start lg:items-center gap-4">
          <div className="flex items-center gap-4">
            <div className="flex flex-col">
              <span className="text-white font-extrabold text-xl tracking-wider">VYUH ENGINE</span>
              <span className="text-xs text-slate-500 font-mono tracking-widest uppercase">Quant Option Fund</span>
            </div>
            
            <div className="h-8 w-px bg-slate-800 mx-4 hidden sm:block"></div>
            
            {/* Tabs List */}
            <div className="flex gap-2.5 text-sm font-mono">
              <button
                onClick={() => handleTabChange("dashboard")}
                className={`px-4 py-2 rounded transition ${
                  activeTab === "dashboard" ? "bg-blue-950/80 text-blue-455 border border-blue-900/40" : "text-slate-400 hover:text-slate-205"
                }`}
              >
                Dashboard
              </button>
              <button
                onClick={() => handleTabChange("portfolio")}
                className={`px-4 py-2 rounded transition ${
                  activeTab === "portfolio" ? "bg-blue-950/80 text-blue-455 border border-blue-900/40" : "text-slate-400 hover:text-slate-205"
                }`}
              >
                Portfolio
              </button>
              <button
                onClick={() => handleTabChange("research")}
                className={`px-4 py-2 rounded transition ${
                  activeTab === "research" ? "bg-blue-950/80 text-blue-455 border border-blue-900/40" : "text-slate-400 hover:text-slate-205"
                }`}
              >
                Research
              </button>
              <button
                onClick={() => handleTabChange("order-book")}
                className={`px-4 py-2 rounded transition ${
                  activeTab === "order-book" ? "bg-blue-950/80 text-blue-455 border border-blue-900/40" : "text-slate-400 hover:text-slate-205"
                }`}
              >
                Order Book
              </button>
              <button
                onClick={() => handleTabChange("analytics")}
                className={`px-4 py-2 rounded transition ${
                  activeTab === "analytics" ? "bg-blue-950/80 text-blue-455 border border-blue-900/40" : "text-slate-400 hover:text-slate-205"
                }`}
              >
                Analytics
              </button>
              <button
                onClick={() => handleTabChange("logs")}
                className={`px-4 py-2 rounded transition ${
                  activeTab === "logs" ? "bg-blue-950/80 text-blue-455 border border-blue-900/40" : "text-slate-400 hover:text-slate-205"
                }`}
              >
                Logs
              </button>
            </div>
          </div>

          {/* Right Status Information */}
          <div className="flex items-center gap-6 text-sm font-mono">
            <div className="flex items-center gap-2">
              <span className="text-slate-500">API Status:</span>
              <span className="bg-emerald-950 text-emerald-400 px-3 py-1 rounded text-xs font-bold">🟢 Active</span>
            </div>
            <div className="flex items-center gap-2">
              <span className="text-slate-500">Market:</span>
              <span className="text-emerald-400 font-bold">Open</span>
            </div>
            <div className="text-slate-400">
              <span className="text-slate-500 mr-2">Time:</span>
              <span>{sysTime || "10:48:32 UTC"}</span>
            </div>
          </div>
        </header>

        {/* Outer body grid with full-width rendering */}
        <main className="flex-1 p-6 w-full text-base">

          {activeTab === "dashboard" && (
            <div className="space-y-6 w-full animate-fade-in">
              
              {/* TOP SPLIT: Real-Time Portfolio Metrics vs Greeks Monitor */}
              <div className="grid grid-cols-1 xl:grid-cols-2 gap-6 w-full">
                
                {/* 1. Real-Time Portfolio Metrics Card Container */}
                <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60 relative overflow-hidden">
                  <div className="flex justify-between items-center mb-5 border-b border-slate-900 pb-3">
                    <h2 className="text-sm font-bold uppercase tracking-wider text-slate-355">Real-Time Portfolio Metrics</h2>
                    <span className="text-xs text-slate-500">•••</span>
                  </div>

                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-5">
                    {/* Stat A: Net PnL */}
                    <div className="bg-[#050813]/60 border border-slate-900/60 rounded-lg p-5 relative overflow-hidden flex flex-col justify-between">
                      <div>
                        <span className="text-xs text-slate-400 uppercase tracking-wider">Portfolio Net PnL</span>
                        <div className="mt-2 text-2xl font-bold font-mono text-emerald-400 neon-glow-green">
                          +₹{(livePnL / 1000000).toFixed(2)}M <span className="text-sm font-normal">({pnlPct}%)</span>
                        </div>
                      </div>
                      <div className="w-full h-12 mt-3">
                        <svg className="w-full h-full text-emerald-500/85 filter drop-shadow-[0_0_6px_rgba(16,185,129,0.3)]" viewBox="0 0 200 40" preserveAspectRatio="none">
                          <defs>
                            <linearGradient id="pnlGrad" x1="0" y1="0" x2="0" y2="1">
                              <stop offset="0%" stopColor="rgb(16, 185, 129)" stopOpacity="0.25"/>
                              <stop offset="100%" stopColor="rgb(16, 185, 129)" stopOpacity="0"/>
                            </linearGradient>
                          </defs>
                          <path d="M0,35 Q20,25 40,28 T80,15 T120,22 T160,8 T200,12" fill="none" stroke="currentColor" strokeWidth="2" />
                          <path d="M0,35 Q20,25 40,28 T80,15 T120,22 T160,8 T200,12 L200,40 L0,40 Z" fill="url(#pnlGrad)" />
                        </svg>
                      </div>
                    </div>

                    {/* Stat B: Margin */}
                    <div className="bg-[#050813]/60 border border-slate-900/60 rounded-lg p-5 flex flex-col justify-between">
                      <div>
                        <span className="text-xs text-slate-400 uppercase tracking-wider">Margin Utilization</span>
                        <div className="mt-2 text-2xl font-bold font-mono text-cyan-400">68%</div>
                        <div className="w-full bg-slate-900 rounded-full h-2 mt-3 overflow-hidden">
                          <div className="bg-gradient-to-r from-cyan-500 to-blue-500 h-full rounded-full" style={{ width: "68%" }}></div>
                        </div>
                      </div>
                      <span className="block mt-2 text-xs text-slate-500 font-mono text-right">1.2B / 1.8B Max</span>
                    </div>

                    {/* Stat C: Theta Decay */}
                    <div className="bg-[#050813]/60 border border-slate-900/60 rounded-lg p-5 flex flex-col justify-between">
                      <div>
                        <span className="text-xs text-slate-400 uppercase tracking-wider">Daily Theta Decay</span>
                        <div className="mt-2 text-2xl font-bold font-mono text-rose-500">-41.2K</div>
                      </div>
                      <div className="flex gap-2 items-end h-8 mt-3">
                        <div className="w-2.5 bg-rose-500/20 h-4 rounded-sm"></div>
                        <div className="w-2.5 bg-rose-500/40 h-6 rounded-sm"></div>
                        <div className="w-2.5 bg-rose-500/70 h-8 rounded-sm"></div>
                        <div className="w-2.5 bg-rose-500 h-5 rounded-sm"></div>
                        <div className="w-2.5 bg-rose-500/30 h-3 rounded-sm"></div>
                        <div className="w-2.5 bg-rose-500/80 h-7 rounded-sm"></div>
                        <div className="w-2.5 bg-rose-500/50 h-4 rounded-sm"></div>
                      </div>
                    </div>

                    {/* Stat D: Value at Risk Radial Gauge */}
                    <div className="bg-[#050813]/60 border border-slate-900/60 rounded-lg p-5 flex flex-col justify-between items-center relative overflow-hidden">
                      <span className="text-xs text-slate-400 uppercase tracking-wider self-start">Value at Risk (99% VaR)</span>
                      
                      <div className="w-28 h-12 relative mt-1">
                        <svg className="w-full h-full text-emerald-400/80 filter drop-shadow-[0_0_6px_rgba(16,185,129,0.35)]" viewBox="0 0 100 50">
                          <defs>
                            <linearGradient id="dialGrad" x1="0" y1="0" x2="1" y2="0">
                              <stop offset="0%" stopColor="#10b981" />
                              <stop offset="70%" stopColor="#eab308" />
                              <stop offset="100%" stopColor="#ef4444" />
                            </linearGradient>
                          </defs>
                          <path d="M 10 50 A 40 40 0 0 1 90 50" fill="none" stroke="#0f172a" strokeWidth="8" strokeLinecap="round" />
                          <path d="M 10 50 A 40 40 0 0 1 90 50" fill="none" stroke="url(#dialGrad)" strokeWidth="8" strokeLinecap="round" strokeDasharray="125" strokeDashoffset="45" />
                          <circle cx="50" cy="50" r="3" fill="#ffffff" />
                        </svg>
                      </div>

                      <div className="flex justify-between items-center w-full text-xs font-mono mt-2">
                        <span className="text-emerald-400 font-bold">2.45% SAFE</span>
                        <span className="text-slate-500">Max: 3.50%</span>
                      </div>
                    </div>
                  </div>
                </div>

                {/* 2. Greeks Monitor (Aggregated) */}
                <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60 relative overflow-hidden">
                  <div className="flex justify-between items-center mb-5 border-b border-slate-900 pb-3">
                    <h2 className="text-sm font-bold uppercase tracking-wider text-slate-350">Greeks Monitor (Aggregated)</h2>
                    <span className="text-xs text-slate-500">•••</span>
                  </div>

                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-5">
                    {/* Delta */}
                    <div className="bg-[#050813]/60 border border-slate-900/60 hover:border-emerald-500/35 rounded-lg p-4 flex flex-col justify-between relative overflow-hidden transition-all duration-300 group">
                      <div className="flex justify-between items-start">
                        <span className="text-xs text-slate-400 uppercase tracking-widest font-mono">Delta</span>
                        <span className="text-emerald-400 font-mono font-bold text-sm group-hover:scale-105 transition-transform">+{deltaVal}</span>
                      </div>
                      <div className="w-full h-12 mt-2">
                        <svg className="w-full h-full text-emerald-400/80 filter drop-shadow-[0_0_6px_rgba(52,211,153,0.3)]" viewBox="0 0 200 45" preserveAspectRatio="none">
                          <path d="M0,35 C15,20 25,10 40,30 C55,45 65,15 80,25 C95,35 105,5 120,20 C135,35 145,15 160,18 C175,22 185,10 200,28" fill="none" stroke="currentColor" strokeWidth="1.5" />
                        </svg>
                      </div>
                      <span className="block text-[10px] text-slate-500 uppercase tracking-wider font-mono">Volatility</span>
                    </div>

                    {/* Gamma */}
                    <div className="bg-[#050813]/60 border border-slate-900/60 hover:border-cyan-505/35 rounded-lg p-4 flex flex-col justify-between relative overflow-hidden transition-all duration-300 group">
                      <div className="flex justify-between items-start">
                        <span className="text-xs text-slate-400 uppercase tracking-widest font-mono">Gamma</span>
                        <span className="text-emerald-400 font-mono font-bold text-sm group-hover:scale-105 transition-transform">+0.04</span>
                      </div>
                      <div className="w-full h-12 mt-2">
                        <svg className="w-full h-full text-cyan-400/80 filter drop-shadow-[0_0_6px_rgba(34,211,238,0.3)]" viewBox="0 0 200 45" preserveAspectRatio="none">
                          <defs>
                            <linearGradient id="gammaGrad" x1="0" y1="0" x2="0" y2="1">
                              <stop offset="0%" stopColor="rgb(34, 211, 238)" stopOpacity="0.2"/>
                              <stop offset="100%" stopColor="rgb(34, 211, 238)" stopOpacity="0"/>
                            </linearGradient>
                          </defs>
                          <path d="M0,42 C30,42 60,42 80,35 C90,30 95,5 100,5 C105,5 110,30 120,35 C140,42 170,42 200,42" fill="none" stroke="currentColor" strokeWidth="1.5" />
                          <path d="M0,42 C30,42 60,42 80,35 C90,30 95,5 100,5 C105,5 110,30 120,35 C140,42 170,42 200,42 Z" fill="url(#gammaGrad)" />
                        </svg>
                      </div>
                      <span className="block text-[10px] text-slate-500 uppercase tracking-wider font-mono">Risk</span>
                    </div>

                    {/* Vega */}
                    <div className="bg-[#050813]/60 border border-slate-900/60 hover:border-indigo-500/35 rounded-lg p-4 flex flex-col justify-between relative overflow-hidden transition-all duration-300 group">
                      <div className="flex justify-between items-start">
                        <span className="text-xs text-slate-400 uppercase tracking-widest font-mono">Vega</span>
                        <span className="text-cyan-400 font-mono font-bold text-sm group-hover:scale-105 transition-transform">+1.15k</span>
                      </div>
                      <div className="w-full h-12 mt-2">
                        <svg className="w-full h-full text-indigo-400/60" viewBox="0 0 200 50" preserveAspectRatio="none">
                          <path d="M40,40 L100,10 L160,40 Z" fill="none" stroke="currentColor" strokeWidth="1" />
                          <path d="M50,38 L100,15 L150,38 Z" fill="none" stroke="currentColor" strokeWidth="0.8" />
                          <path d="M60,36 L100,20 L140,36 Z" fill="none" stroke="currentColor" strokeWidth="0.6" />
                          <path d="M70,34 L100,25 L130,34 Z" fill="none" stroke="currentColor" strokeWidth="0.5" />
                          <line x1="100" y1="5" x2="100" y2="45" stroke="currentColor" strokeWidth="1" />
                          <line x1="80" y1="23" x2="60" y2="36" stroke="currentColor" strokeWidth="0.5" />
                          <line x1="120" y1="23" x2="140" y2="36" stroke="currentColor" strokeWidth="0.5" />
                        </svg>
                      </div>
                      <span className="block text-[10px] text-slate-500 uppercase tracking-wider font-mono">Surface</span>
                    </div>

                    {/* Theta */}
                    <div className="bg-[#050813]/60 border border-slate-900/60 hover:border-rose-500/35 rounded-lg p-4 flex flex-col justify-between relative overflow-hidden transition-all duration-300 group">
                      <div className="flex justify-between items-start">
                        <span className="text-xs text-slate-400 uppercase tracking-widest font-mono">Theta</span>
                        <span className="text-rose-500 font-mono font-bold text-sm group-hover:scale-105 transition-transform">-38.5k</span>
                      </div>
                      <div className="w-full h-12 mt-2">
                        <svg className="w-full h-full text-rose-500/80 filter drop-shadow-[0_0_6px_rgba(239,68,68,0.3)]" viewBox="0 0 200 45" preserveAspectRatio="none">
                          <defs>
                            <linearGradient id="thetaGrad" x1="0" y1="0" x2="0" y2="1">
                              <stop offset="0%" stopColor="rgb(239, 68, 68)" stopOpacity="0.2"/>
                              <stop offset="100%" stopColor="rgb(239, 68, 68)" stopOpacity="0"/>
                            </linearGradient>
                          </defs>
                          <path d="M0,5 C30,10 70,25 120,38 C160,42 180,43 200,43" fill="none" stroke="currentColor" strokeWidth="1.5" />
                          <path d="M0,5 C30,10 70,25 120,38 C160,42 180,43 200,43 L200,45 L0,45 Z" fill="url(#thetaGrad)" />
                        </svg>
                      </div>
                      <span className="block text-[10px] text-slate-500 uppercase tracking-wider font-mono">Decay</span>
                    </div>
                  </div>
                </div>

              </div>

              {/* MIDDLE ROW: Option Positions Grid with Live Filters */}
              <section className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60 w-full mt-6">
                <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3 mb-5 border-b border-slate-900 pb-3">
                  <div className="flex items-center gap-2">
                    <h2 className="text-sm font-bold uppercase tracking-wider text-slate-300">Option Positions (Equity India)</h2>
                  </div>
                  
                  {/* Interactive Search & Filter Controls */}
                  <div className="flex flex-wrap gap-2 text-sm font-mono w-full sm:w-auto">
                    <input
                      type="text"
                      placeholder="Search Ticker..."
                      value={posSearch}
                      onChange={e => setPosSearch(e.target.value)}
                      className="bg-slate-950 border border-slate-800 rounded px-3 py-1.5 text-xs text-slate-202 focus:outline-none focus:border-blue-505 w-full sm:w-44"
                    />
                    <div className="flex bg-slate-950 border border-slate-855 p-0.5 rounded text-xs">
                      <button
                        onClick={() => setPosFilter("ALL")}
                        className={`px-3 py-1 rounded transition ${posFilter === "ALL" ? "bg-blue-900/60 text-white font-bold" : "text-slate-400"}`}
                      >
                        ALL
                      </button>
                      <button
                        onClick={() => setPosFilter("CE")}
                        className={`px-3 py-1 rounded transition ${posFilter === "CE" ? "bg-emerald-950/80 text-emerald-400 font-bold" : "text-slate-400"}`}
                      >
                        Calls
                      </button>
                      <button
                        onClick={() => setPosFilter("PE")}
                        className={`px-3 py-1 rounded transition ${posFilter === "PE" ? "bg-rose-950/80 text-rose-455 font-bold" : "text-slate-400"}`}
                      >
                        Puts
                      </button>
                    </div>
                  </div>
                </div>

                <div className="overflow-x-auto w-full">
                  <table className="w-full text-left text-sm border-collapse font-mono">
                    <thead>
                      <tr className="border-b border-slate-900 text-slate-505 text-xs uppercase">
                        <th className="pb-3">Symbol</th>
                        <th className="pb-3">Type</th>
                        <th className="pb-3">Expiry</th>
                        <th className="pb-3 text-right">Strike</th>
                        <th className="pb-3 text-right">Qty</th>
                        <th className="pb-3 text-right">Px</th>
                        <th className="pb-3 text-right">IV</th>
                        <th className="pb-3 text-right font-bold">PnL</th>
                        <th className="pb-3 text-right">Δ</th>
                        <th className="pb-3 text-right">Γ</th>
                        <th className="pb-3 text-right">Θ</th>
                        <th className="pb-3 text-center">Action</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-900 text-sm">
                      {filteredPositions.length === 0 ? (
                        <tr>
                          <td colSpan={12} className="py-8 text-center text-slate-555">No active positions matching current filters.</td>
                        </tr>
                      ) : (
                        filteredPositions.map(pos => (
                          <tr key={pos.id} className="hover:bg-[#050813]/40 transition-colors">
                            <td className="py-4 font-bold text-slate-200">{pos.symbol}</td>
                            <td className="py-4">
                              <span className={`px-3 py-1 rounded text-xs font-bold ${
                                pos.type === "CE" ? "bg-emerald-955/60 text-emerald-400 border border-emerald-900/30" : "bg-rose-955/60 text-rose-455 border border-rose-900/30"
                              }`}>
                                {pos.type}
                              </span>
                            </td>
                            <td className="py-4 text-slate-400">{pos.expiry}</td>
                            <td className="py-4 text-right text-slate-300">{pos.strike}</td>
                            <td className="py-4 text-right text-slate-300">{pos.qty.toLocaleString()}</td>
                            <td className="py-4 text-right text-slate-300">₹{pos.px.toFixed(2)}</td>
                            <td className="py-4 text-right text-slate-400">{pos.iv}</td>
                            <td className={`py-4 text-right font-bold ${pos.pnl >= 0 ? "text-emerald-450" : "text-rose-500"}`}>
                              {pos.pnl >= 0 ? "+" : ""}₹{(pos.pnl / 1000).toFixed(1)}K
                            </td>
                            <td className="py-4 text-right text-slate-400">{pos.delta}</td>
                            <td className="py-4 text-right text-slate-400">{pos.gamma}</td>
                            <td className="py-4 text-right text-rose-500">{pos.theta}</td>
                            <td className="py-4 text-center">
                              <button
                                onClick={() => {
                                  setPositions(prev => prev.filter(p => p.id !== pos.id));
                                  showToast(`Liquidation request for ${pos.symbol} sent to Gateway OMS.`);
                                }}
                                className="bg-rose-950/60 hover:bg-rose-900 border border-rose-900/40 text-rose-400 px-3 py-1.5 rounded text-xs font-bold uppercase transition"
                              >
                                Exit
                              </button>
                            </td>
                          </tr>
                        ))
                      )}
                    </tbody>
                  </table>
                </div>
              </section>

              {/* BOTTOM ROW: AI Optimization Proposals */}
              <section className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60 w-full mt-6">
                <div className="flex justify-between items-center mb-5 border-b border-slate-900 pb-2">
                  <h2 className="text-sm font-bold uppercase tracking-wider text-slate-300">AI Optimization Proposals</h2>
                  <span className="text-xs text-slate-500">•••</span>
                </div>

                <div className="overflow-x-auto w-full">
                  <table className="w-full text-left text-sm border-collapse font-mono">
                    <thead>
                      <tr className="border-b border-slate-900 text-slate-505 text-xs uppercase">
                        <th className="pb-3">Strategy</th>
                        <th className="pb-3">Symbol</th>
                        <th className="pb-3">Action</th>
                        <th className="pb-3">Details</th>
                        <th className="pb-3 text-right">Estimated PnL</th>
                        <th className="pb-3 text-right font-bold">Confidence Score</th>
                        <th className="pb-3 text-center">Status</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-900 text-sm">
                      {loadingProposals ? (
                        <tr>
                          <td colSpan={7} className="py-8 text-center text-slate-555">Syncing proposals queue...</td>
                        </tr>
                      ) : proposals.length === 0 ? (
                        <tr>
                          <td colSpan={7} className="py-8 text-center text-slate-555">All proposals reviewed.</td>
                        </tr>
                      ) : (
                        proposals.map(prop => (
                          <tr key={prop.proposalId} className="hover:bg-[#050813]/40 transition-colors">
                            <td className="py-4 font-bold text-slate-205">{prop.parameterName}</td>
                            <td className="py-4 text-slate-300 font-bold">{prop.stockId}</td>
                            <td className="py-4">
                              <span className={`px-3 py-1 rounded text-xs font-bold ${
                                prop.proposedValue === "BUY" ? "bg-emerald-955/60 text-emerald-400 border border-emerald-900/30" : "bg-rose-955/60 text-rose-455 border border-rose-900/30"
                              }`}>
                                {prop.proposedValue}
                              </span>
                            </td>
                            <td className="py-4 text-slate-400">{prop.currentValue}</td>
                            <td className="py-4 text-right text-emerald-400 font-bold">+₹1.4M</td>
                            <td className="py-4 text-right text-slate-300 font-bold">{(prop.confidenceScore * 100).toFixed(0)}%</td>
                            <td className="py-4 text-center">
                              <div className="flex gap-2.5 justify-center">
                                <button
                                  onClick={() => handleProposalResponse(prop.proposalId, true)}
                                  className="bg-blue-950/80 hover:bg-blue-900 border border-blue-455 px-4 py-1.5 rounded text-xs font-bold uppercase transition"
                                >
                                  Approve
                                </button>
                                <button
                                  onClick={() => handleProposalResponse(prop.proposalId, false)}
                                  className="bg-slate-900 hover:bg-slate-800 border border-slate-800 text-slate-400 px-3 py-1.5 rounded text-xs font-bold uppercase transition"
                                >
                                  Dismiss
                                </button>
                              </div>
                            </td>
                          </tr>
                        ))
                      )}
                    </tbody>
                  </table>
                </div>
              </section>

            </div>
          )}

          {/* TAB 2: PORTFOLIO COCKPIT & REGULATORY COMPLIANCE MONITOR */}
          {activeTab === "portfolio" && (
            <div className="space-y-6 w-full animate-fade-in font-mono text-sm">
              
              {/* Portfolio stats cards */}
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6 w-full">
                <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60">
                  <span className="text-xs text-slate-500 uppercase tracking-widest block mb-1">Total Account Equity</span>
                  <div className="text-3xl font-bold text-slate-100">₹24,50,00,000.00</div>
                  <span className="text-xs text-emerald-400 block mt-1.5">🟢 Leverage ratio: 1.48x</span>
                </div>
                <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60">
                  <span className="text-xs text-slate-500 uppercase tracking-widest block mb-1">Cash & Collateral Balance</span>
                  <div className="text-3xl font-bold text-cyan-400">₹9,50,00,000.00</div>
                  <span className="text-xs text-slate-400 block mt-1.5">Available for new option sells</span>
                </div>
                <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60">
                  <span className="text-xs text-slate-505 uppercase tracking-widest block mb-1">Aggregate Margin Committed</span>
                  <div className="text-3xl font-bold text-rose-500">₹15,00,00,000.00</div>
                  <span className="text-xs text-rose-455 block mt-1.5">⚠️ Portfolio margin utilization: 61.2%</span>
                </div>
              </div>

              {/* Main split grid: Asset allocation vs Compliance panel */}
              <div className="grid grid-cols-1 xl:grid-cols-3 gap-6 w-full mt-6">
                
                {/* Left side: Allocation & margins table */}
                <div className="xl:col-span-2 glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60">
                  <div className="flex justify-between items-center mb-4 border-b border-slate-900 pb-2">
                    <h2 className="text-sm font-bold uppercase tracking-wider text-slate-355">Asset Allocation & Margin Breakdown</h2>
                  </div>

                  <div className="overflow-x-auto w-full">
                    <table className="w-full text-left text-sm border-collapse">
                      <thead>
                        <tr className="border-b border-slate-900 text-slate-505 text-xs uppercase">
                          <th className="pb-3">Underlying Asset</th>
                          <th className="pb-3 text-right">Portfolio Allocation (%)</th>
                          <th className="pb-3 text-right">Active Delta</th>
                          <th className="pb-3 text-right">Committed Margin</th>
                          <th className="pb-3 text-right">Active Contracts</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-slate-900 text-sm">
                        <tr className="hover:bg-[#050813]/40 transition-colors">
                          <td className="py-4 font-bold text-slate-205">RELIANCE (Oil & Gas)</td>
                          <td className="py-4 text-right text-emerald-400">35%</td>
                          <td className="py-4 text-right text-emerald-455">+0.04</td>
                          <td className="py-4 text-right text-slate-305">₹8.50 Cr</td>
                          <td className="py-4 text-right">3 Contracts</td>
                        </tr>
                        <tr className="hover:bg-[#050813]/40 transition-colors">
                          <td className="py-4 font-bold text-slate-205">TCS (Information Tech)</td>
                          <td className="py-4 text-right text-emerald-400">45%</td>
                          <td className="py-4 text-right text-rose-500">+0.08</td>
                          <td className="py-4 text-right text-slate-305">₹9.80 Cr</td>
                          <td className="py-4 text-right">4 Contracts</td>
                        </tr>
                        <tr className="hover:bg-[#050813]/40 transition-colors">
                          <td className="py-4 font-bold text-slate-205">SBIN (Banking & Finance)</td>
                          <td className="py-4 text-right text-emerald-400">20%</td>
                          <td className="py-4 text-right text-rose-500">-0.02</td>
                          <td className="py-4 text-right text-slate-305">₹4.20 Cr</td>
                          <td className="py-4 text-right">2 Contracts</td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                </div>

                {/* Right side: SEBI Regulatory Compliance Monitor */}
                <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60 space-y-4">
                  <div className="border-b border-slate-900 pb-2 flex justify-between items-center">
                    <span className="text-sm font-bold uppercase tracking-wider text-cyan-400 neon-glow-cyan">📋 Regulatory Limits</span>
                    <span className="text-[10px] text-emerald-400 font-mono font-bold bg-emerald-950 px-1.5 py-0.5 rounded border border-emerald-900/40">COMPLIANT</span>
                  </div>

                  {/* Diversification HHI Score */}
                  <div className="bg-[#050813]/60 border border-slate-900/60 rounded-lg p-4 space-y-2">
                    <div className="flex justify-between text-xs">
                      <span className="text-slate-400 uppercase">Diversification Score (HHI)</span>
                      <span className="text-emerald-400 font-bold">98.75%</span>
                    </div>
                    <div className="w-full bg-slate-900 rounded-full h-1.5 overflow-hidden">
                      <div className="bg-emerald-500 h-full rounded-full" style={{ width: "98.75%" }}></div>
                    </div>
                    <span className="text-[10px] text-slate-500 block">Limits configurations require &gt; 95% overall rating.</span>
                  </div>

                  {/* Sector Margin Cap (15% limit checks) */}
                  <div className="space-y-3 pt-2">
                    <span className="text-xs text-slate-400 uppercase tracking-widest block font-bold">Sector Caps (Max 15% / ₹3.75 Cr)</span>
                    
                    {/* IT Sector */}
                    <div className="space-y-1">
                      <div className="flex justify-between text-[11px]">
                        <span className="text-slate-300">Information Tech</span>
                        <span className="text-yellow-400 font-bold">39.2% of cap (₹9.80 Cr)</span>
                      </div>
                      <div className="w-full bg-slate-950 rounded-full h-1.5 overflow-hidden">
                        <div className="bg-yellow-500 h-full rounded-full" style={{ width: "98%" }}></div>
                      </div>
                    </div>

                    {/* Oil & Gas Sector */}
                    <div className="space-y-1">
                      <div className="flex justify-between text-[11px]">
                        <span className="text-slate-300">Oil & Gas</span>
                        <span className="text-cyan-450">34.0% of cap (₹8.50 Cr)</span>
                      </div>
                      <div className="w-full bg-slate-900 rounded-full h-1.5 overflow-hidden">
                        <div className="bg-cyan-500 h-full rounded-full" style={{ width: "85%" }}></div>
                      </div>
                    </div>

                    {/* Banking */}
                    <div className="space-y-1">
                      <div className="flex justify-between text-[11px]">
                        <span className="text-slate-300">Banking & Finance</span>
                        <span className="text-emerald-455">16.8% of cap (₹4.20 Cr)</span>
                      </div>
                      <div className="w-full bg-slate-900 rounded-full h-1.5 overflow-hidden">
                        <div className="bg-emerald-500 h-full rounded-full" style={{ width: "42%" }}></div>
                      </div>
                    </div>
                  </div>

                  {/* Expiry week exits & physical settlement checker */}
                  <div className="border-t border-slate-900 pt-3 space-y-3">
                    <div className="flex justify-between items-center">
                      <span className="text-xs text-slate-400 uppercase font-bold">SEBI Physical Settlement Checks</span>
                      <span className="text-[10px] text-rose-455 font-mono font-bold bg-rose-950 px-1.5 py-0.5 rounded">EXPIRY WEEK</span>
                    </div>

                    <div className="bg-rose-955/20 border border-rose-900/30 rounded p-3 text-[11px] text-rose-455 space-y-2">
                      <div className="font-bold flex items-center gap-1">
                        <span>⚠️</span> EXPIRED-MINUS-2 DAYS REACHED (Wednesday)
                      </div>
                      <p className="text-slate-400 leading-relaxed text-[10px]">
                        SEBI requires complete exit of ITM derivatives. The following positions have been marked for immediate exit to prevent physical delivery obligations:
                      </p>
                      <div className="font-mono text-xs flex justify-between bg-rose-955/40 p-1.5 rounded border border-rose-900/30">
                        <span className="font-bold text-slate-205">RELIANCE 27 JUN 24 2800 CE</span>
                        <span className="bg-rose-900 text-white px-1.5 rounded text-[10px] font-bold">ITM - ACTION REQ</span>
                      </div>
                    </div>
                  </div>

                  {/* MWPL Ban Tracker */}
                  <div className="border-t border-slate-900 pt-3 space-y-2 text-xs">
                    <span className="text-slate-400 uppercase font-bold block">Market Wide Position Limit (MWPL)</span>
                    <div className="grid grid-cols-3 gap-2 text-center text-[10px]">
                      <div className="bg-slate-950 border border-slate-900 p-2 rounded">
                        <span className="text-slate-550 block">RELIANCE</span>
                        <span className="text-emerald-455 font-bold">45% MWPL</span>
                      </div>
                      <div className="bg-slate-950 border border-slate-900 p-2 rounded">
                        <span className="text-slate-550 block">TCS</span>
                        <span className="text-cyan-400 font-bold">62% MWPL</span>
                      </div>
                      <div className="bg-slate-950 border border-slate-900 p-2 rounded">
                        <span className="text-slate-550 block">SBIN</span>
                        <span className="text-rose-455 font-bold">81% BAN RISK</span>
                      </div>
                    </div>
                  </div>

                </div>

              </div>

            </div>
          )}

          {/* TAB 3: RESEARCH / AI RESEARCH LAB & SYSTEM REFERENCES */}
          {activeTab === "research" && (
            <div className="w-full animate-fade-in text-sm flex flex-col">
              {showFullDocReader ? (
                // Full Screen Documentation Hub (renders master VYUH_Complete_Documentation.md file beautifully)
                <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/75 flex flex-col font-mono h-[calc(100vh-210px)] overflow-hidden">
                  <div className="flex justify-between items-center border-b border-slate-900 pb-4 mb-4 shrink-0">
                    <div className="space-y-1">
                      <span className="text-xs text-slate-500">/VYUH_Complete_Documentation.md - 139.2 KB</span>
                      <h2 className="text-base font-bold uppercase tracking-wider text-cyan-400 neon-glow-cyan">
                        📖 Comprehensive System Documentation Hub
                      </h2>
                    </div>
                    <button
                      onClick={() => setShowFullDocReader(false)}
                      className="bg-blue-955 hover:bg-blue-900 border border-blue-455 text-white px-5 py-2 rounded text-xs uppercase tracking-wider transition font-bold"
                    >
                      Close Reader
                    </button>
                  </div>

                  <div className="grid grid-cols-1 lg:grid-cols-4 gap-6 flex-1 overflow-hidden">
                    {/* Left Column: Quick Navigation Index */}
                    <div className="space-y-2 lg:border-r lg:border-slate-900 lg:pr-6 text-sm font-mono overflow-y-auto h-full pb-6">
                      <span className="text-slate-505 uppercase tracking-widest block mb-2 font-bold text-sm">Table of Contents</span>
                      <button
                        onClick={() => {
                          const el = document.getElementById("doc-content-panel");
                          if (el) el.scrollTop = 0;
                        }}
                        className="w-full text-left p-2.5 rounded hover:bg-slate-950 text-slate-300 hover:text-cyan-400 transition"
                      >
                        Top of Document
                      </button>
                      <button
                        onClick={() => {
                          const el = document.getElementById("sec-01");
                          if (el) el.scrollIntoView({ behavior: "smooth" });
                        }}
                        className="w-full text-left p-2.5 rounded hover:bg-slate-950 text-slate-305 hover:text-cyan-400 transition"
                      >
                        01. Executive Summary & Vision
                      </button>
                      <button
                        onClick={() => {
                          const el = document.getElementById("sec-02");
                          if (el) el.scrollIntoView({ behavior: "smooth" });
                        }}
                        className="w-full text-left p-2.5 rounded hover:bg-slate-950 text-slate-305 hover:text-cyan-400 transition"
                      >
                        02. Business Requirements (BRD)
                      </button>
                      <button
                        onClick={() => {
                          const el = document.getElementById("sec-04");
                          if (el) el.scrollIntoView({ behavior: "smooth" });
                        }}
                        className="w-full text-left p-2.5 rounded hover:bg-slate-950 text-slate-350 hover:text-cyan-400 transition"
                      >
                        04. System Architecture
                      </button>
                      <button
                        onClick={() => {
                          const el = document.getElementById("sec-05");
                          if (el) el.scrollIntoView({ behavior: "smooth" });
                        }}
                        className="w-full text-left p-2.5 rounded hover:bg-slate-950 text-slate-350 hover:text-cyan-400 transition"
                      >
                        05. Database Schemas
                      </button>
                    </div>

                    {/* Right Column: Scrollable Beautifully Rendered Document Panel */}
                    <div
                      id="doc-content-panel"
                      className="lg:col-span-3 h-full overflow-y-auto pr-4 space-y-4 bg-slate-950/40 p-5 rounded-lg border border-slate-900 font-sans pb-12"
                    >
                      {/* Document Control Table rendered dynamically */}
                      <div className="border-b border-slate-900 pb-4 mb-4">
                        <h1 className="text-[#f97316] font-bold text-lg tracking-wider uppercase border-l-4 border-orange-500 pl-3.5 mt-2">
                          VYUH — Comprehensive Master Specification
                        </h1>
                        <div className="overflow-x-auto w-full mt-3">
                          <table className="w-full text-left text-sm border-collapse border border-slate-900 font-mono">
                            <thead>
                              <tr className="bg-slate-950 text-slate-400 font-bold border-b border-slate-900 text-sm">
                                <th className="p-2.5 border-r border-slate-900">Field</th>
                                <th className="p-2.5">Value</th>
                              </tr>
                            </thead>
                            <tbody className="divide-y divide-slate-900 text-sm text-slate-300">
                              <tr>
                                <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">Version</td>
                                <td className="p-2.5">1.0.0</td>
                              </tr>
                              <tr>
                                <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">Category</td>
                                <td className="p-2.5">Portfolio Construction & Risk Engine</td>
                              </tr>
                              <tr>
                                <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">EOS Layer</td>
                                <td className="p-2.5">Layer 3 — Business Engines</td>
                              </tr>
                              <tr>
                                <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">Communication</td>
                                <td className="p-2.5">SignalR / Kafka Event Bus</td>
                              </tr>
                              <tr>
                                <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">AI Agent</td>
                                <td className="p-2.5">VIB (VYUH Intelligence Brain)</td>
                              </tr>
                              <tr>
                                <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">Owner</td>
                                <td className="p-2.5">Solanki PM / Chief Architect</td>
                              </tr>
                              <tr>
                                <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">Last Updated</td>
                                <td className="p-2.5">2026-07-16</td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                      </div>

                      {/* Render parsed contents dynamically */}
                      {renderMarkdown(docMarkdown)}
                    </div>
                  </div>
                </div>
              ) : (
                // Standard Split Layout: AI Research Lab & Sidebar specs
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 w-full">
                  <div className="lg:col-span-2 space-y-6">
                    
                    {/* AI Research Lab console */}
                    <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60 space-y-6">
                      <div>
                        <h2 className="text-base font-bold uppercase tracking-wider text-cyan-400 flex items-center gap-2 neon-glow-cyan">
                          🧠 AI Research Lab — VIB RAG Assistant
                        </h2>
                        <p className="text-sm text-slate-400 leading-relaxed font-mono mt-1 mb-4">
                          Scan local PDF documents, broker research briefs, and historical volatility chain indices in real-time.
                        </p>
                      </div>

                      <form onSubmit={handleRAGQuerySubmit} className="flex gap-2">
                        <input
                          type="text"
                          placeholder="Ask VIB (e.g. 'Evaluate volatility crush speed post-earnings for RELIANCE')"
                          value={ragQuery}
                          onChange={e => setRagQuery(e.target.value)}
                          className="flex-1 bg-slate-950 border border-slate-800 rounded-lg px-4 py-3.5 text-sm font-mono text-slate-202 focus:outline-none focus:border-cyan-500/50"
                        />
                        <button
                          type="submit"
                          disabled={ragLoading}
                          className="bg-cyan-950 hover:bg-cyan-900 border border-cyan-800/40 text-cyan-400 font-bold px-7 rounded text-sm font-mono tracking-wider uppercase transition"
                        >
                          {ragLoading ? "Analyzing..." : "Ask VIB"}
                        </button>
                      </form>

                      {/* Clickable Quick-Query Chips */}
                      <div className="flex flex-wrap gap-2 items-center">
                        <span className="text-xs text-slate-500 font-mono uppercase tracking-wider">Suggested Queries:</span>
                        <button
                          onClick={() => {
                            setRagQuery("Reliance Volatility crush speed");
                            handleRAGQuery("Reliance Volatility crush speed");
                          }}
                          className="bg-slate-950 hover:bg-slate-900 border border-slate-805 text-slate-455 hover:text-slate-205 px-3 py-1.5 rounded text-xs font-mono transition"
                        >
                          💡 Reliance Vol Crush
                        </button>
                        <button
                          onClick={() => {
                            setRagQuery("Nifty margin limits and tail risk thresholds");
                            handleRAGQuery("Nifty margin limits and tail risk thresholds");
                          }}
                          className="bg-slate-950 hover:bg-slate-900 border border-slate-800 text-slate-405 hover:text-slate-205 px-3 py-1.5 rounded text-xs font-mono transition"
                        >
                          💡 Nifty Margin Limits
                        </button>
                        <button
                          onClick={() => {
                            setRagQuery("SEBI option writing rebalancing guidelines");
                            handleRAGQuery("SEBI option writing rebalancing guidelines");
                          }}
                          className="bg-slate-950 hover:bg-slate-900 border border-slate-800 text-slate-400 hover:text-slate-205 px-3 py-1.5 rounded text-xs font-mono transition"
                        >
                          💡 SEBI Option Rules
                        </button>
                      </div>

                      {ragAnswer && (
                        <div className="bg-slate-950/60 border border-slate-900 rounded-lg p-5 space-y-4">
                          <div className="text-sm leading-relaxed text-slate-300 font-mono">
                            <span className="text-cyan-400 font-bold uppercase text-xs">Reasoned Analysis:</span>
                            <p className="mt-2.5 leading-relaxed">{ragAnswer}</p>
                          </div>
                          {ragSources.length > 0 && (
                            <div className="pt-3 border-t border-slate-900 flex flex-wrap gap-2 items-center">
                              <span className="text-xs text-slate-500 font-mono uppercase tracking-wider">Citations:</span>
                              {ragSources.map((source, idx) => (
                                <span key={idx} className="bg-slate-900 border border-slate-800 text-slate-450 px-2.5 py-1 rounded text-xs font-mono">
                                  📁 {source}
                                </span>
                              ))}
                            </div>
                          )}
                        </div>
                      )}
                    </div>

                    {/* Chapter Viewer panel formatted exactly like screenshot */}
                    <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60 space-y-4 font-mono text-sm">
                      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-2 border-b border-slate-900 pb-3">
                        <div className="text-xs text-slate-500">
                          {activeChapter.path} - {activeChapter.size}
                        </div>
                        
                        {/* Chapter selector */}
                        <div className="flex items-center gap-2">
                          <span className="text-xs text-slate-400">Select Document:</span>
                          <select
                            value={activeChapter.id}
                            onChange={e => {
                              const chap = chapters.find(c => c.id === e.target.value);
                              if (chap) setActiveChapter(chap);
                            }}
                            className="bg-slate-950 border border-slate-800 rounded px-2 py-1 text-xs text-slate-202 focus:outline-none focus:border-blue-500"
                          >
                            {chapters.map(c => (
                              <option key={c.id} value={c.id}>{c.name}</option>
                            ))}
                          </select>
                        </div>
                      </div>

                      {/* Title & Metadata table matches mockup exactly */}
                      <h3 className="text-lg font-bold text-slate-100">{activeChapter.title}</h3>

                      <div className="overflow-x-auto w-full pt-1">
                        <table className="w-full text-left text-xs border-collapse border border-slate-900">
                          <thead>
                            <tr className="bg-slate-950 text-slate-400 text-xs font-bold border-b border-slate-900">
                              <th className="p-2.5 border-r border-slate-900 w-1/3">Field</th>
                              <th className="p-2.5">Value</th>
                            </tr>
                          </thead>
                          <tbody className="divide-y divide-slate-900 text-xs text-slate-300">
                            <tr>
                              <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">Version</td>
                              <td className="p-2.5">{activeChapter.version}</td>
                            </tr>
                            <tr>
                              <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">Category</td>
                              <td className="p-2.5">{activeChapter.category}</td>
                            </tr>
                            <tr>
                              <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">EOS Layer</td>
                              <td className="p-2.5">{activeChapter.eosLayer}</td>
                            </tr>
                            <tr>
                              <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">Communication</td>
                              <td className="p-2.5">{activeChapter.communication}</td>
                            </tr>
                            <tr>
                              <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">AI Agent</td>
                              <td className="p-2.5">{activeChapter.aiAgent}</td>
                            </tr>
                            <tr>
                              <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">Owner</td>
                              <td className="p-2.5">{activeChapter.owner}</td>
                            </tr>
                            <tr>
                              <td className="p-2.5 border-r border-slate-900 font-bold text-slate-400 bg-slate-950/20">Last Updated</td>
                              <td className="p-2.5">{activeChapter.lastUpdated}</td>
                            </tr>
                          </tbody>
                        </table>
                      </div>

                      {/* Section Content styled with orange headers exactly like screen */}
                      <div className="space-y-4 pt-2 text-slate-350 leading-relaxed text-sm">
                        <div className="space-y-2">
                          <h4 className="text-[#f97316] font-bold text-sm tracking-wider uppercase">{activeChapter.conceptTitle}</h4>
                          <p className="pl-1 text-slate-300 font-sans">{activeChapter.conceptContent}</p>
                        </div>

                        <div className="space-y-2 pt-2">
                          <h4 className="text-[#f97316] font-bold text-sm tracking-wider uppercase">{activeChapter.flowTitle}</h4>
                          <p className="pl-1 text-slate-300 font-sans">{activeChapter.flowContent}</p>
                        </div>
                      </div>

                    </div>

                  </div>

                  {/* RAG Knowledgebase References & API Console Links Panel */}
                  <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60 font-mono space-y-5">
                    <div>
                      <span className="block text-sm text-cyan-455 font-bold uppercase tracking-wider neon-glow-cyan">📘 COMPREHENSIVE DOCUMENTATION</span>
                      <div className="space-y-2 text-xs mt-2.5">
                        <button
                          onClick={() => setShowFullDocReader(true)}
                          className="w-full text-left p-2.5 rounded bg-slate-950 border border-slate-850 hover:border-blue-500/50 text-slate-300 hover:text-blue-400 transition font-bold"
                        >
                          📘 Open Integrated Document Hub
                        </button>
                      </div>
                    </div>

                    {/* Local Microservice API Swagger Consoles */}
                    <div className="pt-4 border-t border-slate-900 space-y-2">
                      <span className="block text-xs text-slate-400 uppercase tracking-widest font-bold">Local API Specifications</span>
                      <div className="space-y-1.5 text-xs">
                        <a
                          href="http://localhost:5063/scalar/v1"
                          className="flex justify-between items-center p-2.5 rounded bg-slate-950 hover:bg-slate-900 text-slate-305 hover:text-cyan-400 transition"
                          target="_blank"
                        >
                          <span>Gateway Hub API</span>
                          <span className="text-[10px] text-slate-500 font-mono">Port 5063</span>
                        </a>
                        <a
                          href="http://localhost:5206/scalar/v1"
                          className="flex justify-between items-center p-2.5 rounded bg-slate-950 hover:bg-slate-900 text-slate-305 hover:text-cyan-400 transition"
                          target="_blank"
                        >
                          <span>Optimizer API</span>
                          <span className="text-[10px] text-slate-500 font-mono">Port 5206</span>
                        </a>
                        <a
                          href="http://localhost:5084/scalar/v1"
                          className="flex justify-between items-center p-2.5 rounded bg-slate-950 hover:bg-slate-900 text-slate-305 hover:text-cyan-400 transition"
                          target="_blank"
                        >
                          <span>Risk API</span>
                          <span className="text-[10px] text-slate-500 font-mono">Port 5084</span>
                        </a>
                        <a
                          href="http://localhost:5048/scalar/v1"
                          className="flex justify-between items-center p-2.5 rounded bg-slate-950 hover:bg-slate-900 text-slate-305 hover:text-cyan-400 transition"
                          target="_blank"
                        >
                          <span>Ingestion API</span>
                          <span className="text-[10px] text-slate-500 font-mono">Port 5048</span>
                        </a>
                      </div>
                    </div>

                    {/* System technical metrics */}
                    <div className="pt-4 border-t border-slate-900 space-y-2 text-xs text-slate-500">
                      <div className="flex justify-between">
                        <span>Embeddings:</span>
                        <span className="text-slate-300">bge-m3-large (1024)</span>
                      </div>
                      <div className="flex justify-between">
                        <span>Vector DB:</span>
                        <span className="text-slate-300">PostgreSQL pgvector</span>
                      </div>
                    </div>
                  </div>
                </div>
              )}
            </div>
          )}

          {/* TAB 4: ORDER BOOK */}
          {activeTab === "order-book" && (
            <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60 w-full animate-fade-in font-mono text-sm">
              <div className="flex justify-between items-center mb-4 border-b border-slate-900 pb-2">
                <h2 className="text-sm font-bold uppercase tracking-wider text-slate-355">Order Execution Audit Trail</h2>
                <span className="text-xs text-slate-500">Gateway OMS Logs</span>
              </div>

              <div className="overflow-x-auto w-full">
                <table className="w-full text-left text-sm border-collapse">
                  <thead>
                    <tr className="border-b border-slate-900 text-slate-505 text-xs uppercase">
                      <th className="pb-3">Order ID</th>
                      <th className="pb-3">Timestamp</th>
                      <th className="pb-3">Symbol</th>
                      <th className="pb-3">Type</th>
                      <th className="pb-3">Strategy</th>
                      <th className="pb-3 text-right">Quantity</th>
                      <th className="pb-3 text-right">Limit Price</th>
                      <th className="pb-3 text-center">Status</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-900 text-sm">
                    {orders.map(order => (
                      <tr key={order.id} className="hover:bg-[#050813]/40 transition-colors">
                        <td className="py-4 font-bold text-slate-200">{order.id}</td>
                        <td className="py-4 text-slate-400">{order.timestamp}</td>
                        <td className="py-4 font-bold text-slate-205">{order.symbol}</td>
                        <td className="py-4">
                          <span className={`px-3 py-1 rounded text-xs font-bold ${
                            order.type === "CE" ? "bg-emerald-950/60 text-emerald-400 border border-emerald-900/30" : "bg-rose-950/60 text-rose-455 border border-rose-900/30"
                          }`}>
                            {order.type}
                          </span>
                        </td>
                        <td className="py-4 text-slate-400">{order.strategy}</td>
                        <td className="py-4 text-right text-slate-350">{order.qty.toLocaleString()}</td>
                        <td className="py-4 text-right text-slate-350">₹{order.px.toFixed(2)}</td>
                        <td className="py-4 text-center">
                          <span className="bg-emerald-950 text-emerald-400 border border-emerald-900/40 px-3 py-1 rounded text-xs font-bold uppercase">
                            {order.status}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}

          {/* TAB 5: OPTION ANALYTICS */}
          {activeTab === "analytics" && (
            <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60 animate-fade-in space-y-6 w-full text-sm">
              <div className="border-b border-slate-800 pb-4">
                <h2 className="text-base font-bold uppercase tracking-wider text-slate-200 flex items-center gap-2">
                  📈 Options Analytics & BSM Probability Engine
                </h2>
                <p className="text-xs text-slate-455 leading-relaxed font-mono mt-1">
                  Connects to the backend Optimizer mathematical endpoints to run standard options calculations.
                </p>
              </div>

              <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                
                {/* Inputs */}
                <div className="bg-[#050813]/60 border border-slate-900 rounded-xl p-5 space-y-4 font-mono text-xs">
                  <span className="block text-sm text-slate-300 font-bold uppercase tracking-wider mb-2">Calculation Inputs</span>
                  
                  <div className="space-y-1">
                    <label className="block text-slate-500">Spot Price (S)</label>
                    <input
                      type="number"
                      value={calcSpot}
                      onChange={e => setCalcSpot(parseFloat(e.target.value))}
                      className="w-full bg-slate-900 border border-slate-800 rounded px-3.5 py-2.5 text-sm text-slate-202 focus:outline-none focus:border-blue-500"
                    />
                  </div>

                  <div className="space-y-1">
                    <label className="block text-slate-500">Implied Volatility (IV %)</label>
                    <input
                      type="number"
                      value={calcVol}
                      onChange={e => setCalcVol(parseFloat(e.target.value))}
                      className="w-full bg-slate-900 border border-slate-800 rounded px-3.5 py-2.5 text-sm text-slate-202 focus:outline-none focus:border-blue-505"
                    />
                  </div>

                  <div className="space-y-1">
                    <label className="block text-slate-500">Days To Expiry (T)</label>
                    <input
                      type="number"
                      value={calcDays}
                      onChange={e => setCalcDays(parseInt(e.target.value))}
                      className="w-full bg-slate-900 border border-slate-800 rounded px-3.5 py-2.5 text-sm text-slate-202 focus:outline-none focus:border-blue-500"
                    />
                  </div>

                  <div className="border-t border-slate-800 my-2 pt-2 space-y-4">
                    <div className="flex items-center gap-4 text-xs">
                      <span className="text-slate-505">Contract Type:</span>
                      <label className="flex items-center gap-2 cursor-pointer">
                        <input
                          type="radio"
                          checked={calcIsCall}
                          onChange={() => setCalcIsCall(true)}
                          className="accent-blue-500"
                        />
                        <span>CE</span>
                      </label>
                      <label className="flex items-center gap-2 cursor-pointer">
                        <input
                          type="radio"
                          checked={!calcIsCall}
                          onChange={() => setCalcIsCall(false)}
                          className="accent-blue-500"
                        />
                        <span>PE</span>
                      </label>
                    </div>

                    <div className="space-y-1">
                      <label className="block text-slate-500">Single Strike Target (K)</label>
                      <input
                        type="number"
                        value={calcStrike}
                        onChange={e => setCalcStrike(parseFloat(e.target.value))}
                        className="w-full bg-slate-900 border border-slate-800 rounded px-3.5 py-2.5 text-sm text-slate-202 focus:outline-none focus:border-blue-500"
                      />
                    </div>

                    <div className="grid grid-cols-2 gap-2.5">
                      <div className="space-y-1">
                        <label className="block text-slate-500">Range Lower Strike</label>
                        <input
                          type="number"
                          value={calcLowerStrike}
                          onChange={e => setCalcLowerStrike(parseFloat(e.target.value))}
                          className="w-full bg-slate-900 border border-slate-800 rounded px-3 py-2 text-sm text-slate-202 focus:outline-none focus:border-blue-505"
                        />
                      </div>
                      <div className="space-y-1">
                        <label className="block text-slate-500">Range Upper Strike</label>
                        <input
                          type="number"
                          value={calcUpperStrike}
                          onChange={e => setCalcUpperStrike(parseFloat(e.target.value))}
                          className="w-full bg-slate-900 border border-slate-800 rounded px-3 py-2 text-sm text-slate-202 focus:outline-none focus:border-blue-500"
                        />
                      </div>
                    </div>
                  </div>

                  <button
                    onClick={calculateOptionProbabilities}
                    disabled={calcLoading}
                    className="w-full bg-blue-600 hover:bg-blue-505 text-white font-bold py-3 rounded text-xs uppercase tracking-wider transition mt-2"
                  >
                    {calcLoading ? "Running calculations..." : "Compute Statistics"}
                  </button>
                </div>

                {/* Outputs */}
                <div className="lg:col-span-2 flex flex-col justify-between space-y-4">
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-5 font-mono text-center">
                    <div className="bg-[#050813]/60 border border-slate-900 rounded-lg p-6">
                      <span className="text-xs text-slate-500 uppercase tracking-wider block mb-1">Probability ITM (BSM)</span>
                      <div className="text-4xl font-extrabold text-blue-400 neon-glow-cyan">
                        {bsmPob !== null ? `${bsmPob.toFixed(2)}%` : "--"}
                      </div>
                      <span className="block mt-2 text-xs text-slate-500">Likelihood of expiring in-the-money</span>
                    </div>

                    <div className="bg-[#050813]/60 border border-slate-900 rounded-lg p-6">
                      <span className="text-xs text-slate-500 uppercase tracking-wider block mb-1">Probability of Touch</span>
                      <div className="text-4xl font-extrabold text-cyan-400 neon-glow-cyan">
                        {touchPob !== null ? `${touchPob.toFixed(2)}%` : "--"}
                      </div>
                      <span className="block mt-2 text-xs text-slate-500">Likelihood of touching strike pre-expiry</span>
                    </div>

                    <div className="bg-[#050813]/60 border border-slate-900 rounded-lg p-6">
                      <span className="text-xs text-slate-500 uppercase tracking-wider block mb-1">Probability of Range</span>
                      <div className="text-4xl font-extrabold text-emerald-400 neon-glow-green">
                        {rangePob !== null ? `${rangePob.toFixed(2)}%` : "--"}
                      </div>
                      <span className="block mt-2 text-xs text-slate-550">Likelihood of expiring inside bounds</span>
                    </div>
                  </div>

                  <div className="bg-[#050813]/60 border border-slate-900 rounded-xl p-5 text-sm font-mono text-slate-400 space-y-3 leading-relaxed">
                    <span className="block text-sm text-slate-350 font-bold uppercase tracking-wider">Model Formulation Notes</span>
                    <p>Calculations use Standard Cumulative Normal Distribution integrations.</p>
                    <p>
                      Probability of ITM:
                      {"\\[P(\\text{ITM}) = N(d_2) \\quad \\text{where} \\quad d_2 = \\frac{\\ln(S/K) + (r - \\sigma^2/2)T}{\\sigma\\sqrt{T}}\\]"}
                    </p>
                  </div>
                </div>

              </div>
            </div>
          )}

          {/* TAB 6: AUDIT LOGS with Filters & Categories */}
          {activeTab === "logs" && (
            <div className="glass-panel rounded-xl p-6 border border-slate-900 bg-[#090d1f]/60 w-full animate-fade-in font-mono text-sm space-y-4">
              <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3 border-b border-slate-900 pb-3">
                <div className="flex items-center gap-4">
                  <h2 className="text-sm font-bold uppercase tracking-wider text-slate-355">Live Audit & Gateway Trace</h2>
                  
                  {/* Category Filtering Tabs */}
                  <div className="flex bg-slate-950 border border-slate-850 p-0.5 rounded text-[10px]">
                    <button
                      onClick={() => setLogFilter("ALL")}
                      className={`px-3 py-1 rounded transition ${logFilter === "ALL" ? "bg-blue-900/60 text-white font-bold" : "text-slate-500"}`}
                    >
                      ALL
                    </button>
                    <button
                      onClick={() => setLogFilter("SYSTEM")}
                      className={`px-3 py-1 rounded transition ${logFilter === "SYSTEM" ? "bg-cyan-950/80 text-cyan-400 font-bold" : "text-slate-500"}`}
                    >
                      System
                    </button>
                    <button
                      onClick={() => setLogFilter("RISK")}
                      className={`px-3 py-1 rounded transition ${logFilter === "RISK" ? "bg-rose-955/80 text-rose-455 font-bold" : "text-slate-500"}`}
                    >
                      Risk
                    </button>
                    <button
                      onClick={() => setLogFilter("GATEWAY")}
                      className={`px-3 py-1 rounded transition ${logFilter === "GATEWAY" ? "bg-emerald-955/80 text-emerald-400 font-bold" : "text-slate-500"}`}
                    >
                      Gateway
                    </button>
                  </div>
                </div>
                
                <button
                  onClick={() => setAuditLogs([
                    { timestamp: new Date().toLocaleTimeString("en-US", { hour12: false }) + " UTC", category: "SYSTEM", message: "Logs database manual purge/refresh triggered." },
                    ...auditLogs
                  ])}
                  className="text-xs text-blue-400 hover:underline uppercase"
                >
                  Refresh Logs
                </button>
              </div>

              <div className="bg-slate-950 p-5 rounded-lg border border-slate-900 font-mono text-sm text-slate-300 space-y-2 h-[380px] overflow-y-auto">
                {filteredAuditLogs.length === 0 ? (
                  <div className="text-slate-555 text-center py-12">No logs matching selected category level.</div>
                ) : (
                  filteredAuditLogs.map((log, idx) => (
                    <div key={idx} className="flex gap-2.5 hover:bg-slate-900/40 py-1 rounded px-2 transition-colors">
                      <span className="text-slate-550">[{log.timestamp}]</span>
                      <span className={`font-bold ${
                        log.category === "SYSTEM" ? "text-cyan-400" :
                        log.category === "AUDIT" ? "text-indigo-400" :
                        log.category === "GATEWAY" ? "text-emerald-400" : "text-rose-500"
                      }`}>
                        {log.category}:
                      </span>
                      <span className="text-slate-205">{log.message}</span>
                    </div>
                  ))
                )}
              </div>
            </div>
          )}

        </main>

        {/* Footer */}
        <footer className="border-t border-slate-900 bg-[#04060f] px-6 py-5 flex flex-col md:flex-row justify-between items-center text-slate-555 text-xs font-mono gap-2 w-full shrink-0">
          <span>© 2026 VYUH Options Trading Technologies. Private Institutional Access.</span>
          <div className="flex gap-4">
            <span className="text-slate-600 font-semibold">OMS Latency: 42ms</span>
            <span className="text-slate-650 font-semibold">Gateway Version: 1.2.0</span>
          </div>
        </footer>

      </div>
    </div>
  );
}
