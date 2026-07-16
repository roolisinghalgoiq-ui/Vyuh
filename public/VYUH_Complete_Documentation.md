﻿# VYUH Complete Documentation


---

# 01_Executive_Summary

# VYUH Engine - Executive Summary

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Chief Quantitative Architect | Investment Committee | Initial Specification |

---

## 2. Executive Vision
**VYUH** (Sanskrit: *à¤µà¥à¤¯à¥‚à¤¹*, meaning "strategic placement" or "battle array") is a next-generation institutional portfolio intelligence and construction engine designed specifically for the **Indian Stock Options (NSE F&O) Market**. 

The platform operates as a quantitative decision-support system, aiming to deploy and manage a target capital of **â‚¹25 Crores (INR 250,000,000)** across a diversified portfolio of **70 to 80 liquid stocks**. 

By processing real-time option chains, technical indicators, and historical volatility profiles across **200+ NSE F&O underlyings** at a **10-second tick resolution**, VYUH constructs an optimal options-selling portfolio that maximizes theta decay and probability of profit while maintaining strict risk boundaries.

---

## 3. Core System Boundaries (What VYUH is NOT)
To maintain structural integrity and high performance, VYUH enforces strict boundaries. It does **not** perform execution or low-level market scanning directly.

```mermaid
graph TD
    subgraph Market Data & Analytics (Inputs)
        G[Ganesh: Hist DB] --> V[VYUH Engine]
        L[Lakshmi: Realtime Feed] --> V
        S[Suchak: Tech Indicators] --> V
        T[TalkOptions: Greeks & Analytics] --> V
    end
    
    subgraph Core Intelligence
        V --> |Construct Portfolio| P[Execution Packet Generation]
    end
    
    subgraph Execution & OMS (Outputs)
        P --> |SignalR / Kafka| Exec[Vega: Execution & OMS]
    end
    
    style V fill:#f9f,stroke:#333,stroke-width:4px
    style Exec fill:#bbf,stroke:#333,stroke-width:2px
```

### Scope Matrix
*   **NOT an Option Scanner**: VYUH does not alert users about random option spikes or standard breakout scans. It evaluates the *entire* universe holistically.
*   **NOT a Strategy Builder**: It does not allow discretionary creation of customized multi-leg templates. It works with a fixed set of institutional strategies (Short Straddles, Strangles, Iron Condors, Iron Flies, and Ratio Spreads).
*   **NOT an Execution Engine**: It does not place orders or manage active market connections (Direct Market Access).
*   **NOT an OMS (Order Management System)**: It does not handle order queues, broker accounts, or clearing.

---

## 4. Existing Engines & Integrations
VYUH is a consumer of existing enterprise infrastructure. It interfaces with five mature engines via gRPC, Kafka, and REST APIs:

### 4.1. Ganesh (Historical Engine)
*   **Responsibilities**: Historical OHLC, Historical ATR (Average True Range), Historical Volatility (HV10, HV20, HV30, HV90), historical returns distribution, and historical analytics databases.
*   **Interface**: High-performance gRPC querying.

### 4.2. Lakshmi (Realtime Market Feed)
*   **Responsibilities**: Ultra-low latency feed handler. Spot prices, futures prices, complete options chain (Bid/Ask, Volume, Open Interest, Tick-by-Tick trade data).
*   **Interface**: Apache Kafka topic subscriptions.

### 4.3. Suchak (Technical Indicators Engine)
*   **Responsibilities**: Real-time computation of technical overlays (RSI, EMA, SMA, VWAP, ADX, MACD, Supertrend, Support and Resistance levels).
*   **Interface**: Redis state store.

### 4.5. TalkOptions API (Options Analytics Provider)
*   **Responsibilities**: Greek calculations (Delta, Gamma, Theta, Vega), Implied Volatility (IV), IV Rank (IVR), IV Percentile (IVP), Expected Move (EM), Premium Analytics, and Liquidity metrics.
*   **Interface**: REST API for bulk fetches; Redis cache for active tickers.

### 4.6. Vega (Execution Engine & OMS)
*   **Responsibilities**: Margin monitoring, order routing and placement, stop-loss and target tracking, positions monitoring, and OMS operations.
*   **Interface**: SignalR Hub connection and Kafka order event stream.

---

## 5. VYUH Functional Responsibilities
VYUH acts as the "Central Brain". Every 10 seconds, it executes the following core loop:

```
[Realtime Options & Technical Feed] 
               â”‚
               â–¼
   1. Market Intelligence  â”€â”€â”€â–º Normalizes spot, future, ATR, and IV profiles.
               â”‚
               â–¼
   2. Probability Analysis â”€â”€â”€â–º Calculates Touch, Range, and Historical probabilities.
               â”‚
               â–¼
   3. Strike Intelligence  â”€â”€â”€â–º Identifies optimal strike prices with high liquidity.
               â”‚
               â–¼
   4. Strategy Selection   â”€â”€â”€â–º Selects premium-rich strategies matching the market regime.
               â”‚
               â–¼
   5. Portfolio Builder    â”€â”€â”€â–º Assembles 70-80 stock portfolio allocating â‚¹25 Crores.
               â”‚
               â–¼
   6. Risk & Diversify     â”€â”€â”€â–º Applies sector limits, correlation caps, and VaR stress-testing.
               â”‚
               â–¼
   7. Decision & Packet    â”€â”€â”€â–º Generates signed execution packets for Vega.
```

1.  **Market Intelligence**: Reads multi-source data feeds, normalizes them, and classifies market regimes (Bullish, Bearish, Range-bound, High Volatility, Low Volatility).
2.  **Probability Analysis**: Performs multi-factor probability calculations (Historical Probability, IV-implied Touch/Range Probabilities) for every strike.
3.  **Strike Intelligence**: Filters strikes based on expected move, premium richness, delta exposure, and liquidity thresholds.
4.  **Portfolio Construction & Diversification**: Dynamically constructs a â‚¹25 Crore portfolio, allocating capital across 70–80 stock underlyings while maintaining sector caps (<15% exposure per sector) and limiting pair-wise correlations (<0.35).
5.  **Risk Analysis & Decision Making**: Simulates extreme market scenarios (e.g., Â±5% spot crash, +50% IV spike) and generates the execution packet.
6.  **Execution Packet Generation**: Compiles the proposed portfolio transitions into a standardized, cryptographically signed JSON execution packet containing entry, adjustments, and exits, and publishes it to the Vega gRPC/Kafka channel.

---

## 6. Key Institutional Parameters & Operational Constraints
*   **Target Capital**: â‚¹250,000,000 (INR 25 Crores) fully deployed.
*   **Universe Size**: ~200 NSE F&O underlyings.
*   **Active Underlyings**: 70 to 80 distinct underlyings to achieve maximum statistical diversification.
*   **Options Scope**: At-The-Money (ATM) option and Â±20 strikes.
*   **Computation Frequency**: Complete portfolio evaluation and scoring every **10 seconds**.
*   **Options-Selling Orientation**: Focus is on net-short options strategies to capture the variance risk premium (VRP).

---

## 7. Performance & High-Availability Targets
*   **End-to-End Latency**: The complete cycle (Ingestion, Scoring, Portfolio Optimization, and Execution Packet Generation) must complete in **< 1,800 milliseconds**.
*   **Availability**: 99.99% uptime during market hours (09:00 AM - 03:30 PM IST).
*   **Resiliency**: Multi-region/multi-zone hot-standby deployment. Zero state loss in case of a node failure via Redis replication and PostgreSQL WAL archiving.


---

# 02_Business_Requirements

# VYUH Engine - Business Requirements Document (BRD)

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Product Owner | Chief Investment Officer | Initial Specification |

---

## 2. Business Mandate and Investment Thesis
VYUH is designed to capture the **Variance Risk Premium (VRP)** in the Indian equity derivatives market (NSE F&O). Historical quantitative analysis shows that Implied Volatility (IV) systematically overestimates realized volatility (RV). By consistently writing options (selling volatility) across a highly diversified basket of underlyings, VYUH aims to harvest this theta decay.

### 2.1. Operational Constraints
*   **Target Capital Deployment**: â‚¹250,000,000 (INR 25 Crores).
*   **Underlying Universe**: The ~200 NSE-listed derivatives (F&O) stocks.
*   **Active Basket Size**: 70 to 80 distinct underlyings. This ensures that no single stock can dominate the portfolio risk profile.
*   **Target Monthly Yield**: 1.5% to 2.0% net of transaction costs, slippage, and taxes (equivalent to 18%-24% annualized).
*   **Max Peak-to-Trough Drawdown**: Capped at **8%** at the portfolio level.

---

## 3. Portfolio Allocation & Diversification Mandates
To manage risk without relying on directional views, VYUH enforces strict diversification mathematical constraints.

### 3.1. Allocation Framework
The â‚¹25 Crore capital is structured into three distinct tiers:

```mermaid
graph TD
    Capital["Total Capital: â‚¹25 Crores"]
    Capital --> Margin["Margin Capital<br/>60% (â‚¹15 Cr)<br/>Used to back short options"]
    Capital --> Buffer["Liquid Buffer<br/>30% (â‚¹7.5 Cr)<br/>Maintained in liquid funds"]
    Capital --> Reserve["Drawdown Reserve<br/>10% (â‚¹2.5 Cr)<br/>Overnight liquid treasury"]
```

> [!NOTE]
> * **Margin Capital** is dynamic and adjusted daily based on active contract margin requirements.
> * **Liquid Buffer** functions as a volatility cushion to prevent forced liquidations during intraday margin spikes.

1.  **Margin Capital (60% / â‚¹15 Crores)**: Deposited with the clearing member to back active short options positions.
2.  **Liquid Buffer (30% / â‚¹7.5 Crores)**: Kept in liquid mutual funds, G-Secs, or overnight funds. This buffer is automatically drawn to meet margin expansion (e.g., due to IV spikes or adverse price moves).
3.  **Drawdown Reserve (10% / â‚¹2.5 Crores)**: Maintained in cash equivalents, used only in extreme tail events.

### 3.2. Diversification Limits
The portfolio construction algorithm rejects any allocation that violates these limits:

> [!IMPORTANT]
> **Core Portfolio Limits:**
> *   **Single Stock Margin Limit**: No single underlying can consume more than **2.0%** of the total allocated margin (i.e., Max â‚¹30 Lakhs margin exposure per stock).
> *   **Sector Limit**: Total capital allocated to any single NSE sector (e.g., Nifty Bank, Nifty IT, Nifty Pharma) must not exceed **15%** of the portfolio (i.e., Max â‚¹3.75 Crores margin exposure per sector).
> *   **Industry Limit**: Total capital allocated to any single industry classification within a sector (e.g., Private Banks vs. Public Banks) must not exceed **10%** of the portfolio.
> *   **Pairwise Correlation Limit**: The system computes a daily asset correlation matrix from Ganesh historical returns. If any two underlyings show a Pearson correlation $r > 0.35$ over a 90-day window, they are treated as a single joint risk block, and their combined margin usage is capped at **3.0%**.

---

## 4. Compliance and Regulatory Rules (SEBI Constraints)
Trading stock options in India requires adherence to strict Securities and Exchange Board of India (SEBI) guidelines:

### 4.1. Physical Delivery Risk Management
*   **Mandate**: Stock options in India are physically settled in the expiry week.
*   **Business Rule**: VYUH must exit all In-The-Money (ITM) options and close all near-the-money options before the close of trade on **Expiry-Minus-3 Days (Tuesday of the expiry week)** to prevent physical delivery obligations.
*   **Enforcement**: The system will automatically mark these options for exit and refuse to roll or enter positions in the expiry week.

### 4.2. Market Wide Position Limits (MWPL)
*   **Mandate**: Underlyings are banned from F&O trading if open interest exceeds 95% of MWPL.
*   **Business Rule**: VYUH will ban entry into any underlying whose overall MWPL utilization exceeds **80%** to avoid liquidity lock-up or entry into a security under F&O Ban status.

### 4.3. Client-Level Position Limits
*   **Mandate**: The gross open position of a client across all derivative contracts of an underlying must not exceed 1% of the free float market capitalization or 5% of the open interest, whichever is lower.
*   **Business Rule**: Real-time monitoring validation before packet generation.

---

## 5. Operations and Human-in-the-Loop (HITL) Workflow
VYUH does not run autonomously without human supervision. A structured workflow ensures safety and compliance:

```mermaid
flowchart TD
    Start["08:30 AM: Pre-Market Check<br/>(Health, Margin, Manual Config)"]
    Start --> Ingest["09:08 AM: Ingest Pre-Open Data<br/>(Lakshmi feeds, VIB updates Regime)"]
    Ingest --> Loop["09:15 AM: Live Market Hours Loop<br/>(Every 10s: Market Intel -> Score -> Port Build -> Risk)"]
    Loop --> Decision{"Action Type?"}
    Decision -->|Minor Adjustment| Auto["Auto-Execution Packet"]
    Decision -->|Rebalance / Roll| HITL["HITL Gate<br/>(Requires PM Sign-off)"]
    Auto --> Send["Send to Vega Engine"]
    HITL -->|Approved| Send
    Send --> Close["03:30 PM: Market Close<br/>(Reconciliation, DB, VIB Learning)"]
    Close --> Report["05:00 PM: Daily EOD Risk Report"]
```

1.  **Pre-Market Checks (08:30 AM - 09:00 AM IST)**:
    *   System checks gRPC connections to Ganesh, TalkOptions, and SignalR links to Vega.
    *   Portfolio Manager reviews and overrides global risk settings (e.g., disabling specific stocks due to earnings announcements).
2.  **Market Hours Loop (09:15 AM - 03:30 PM IST)**:
    *   Automatic mode: System monitors existing positions and generates adjustment packets if stop-loss or profit targets are hit.
    *   Rebalancing mode: At scheduled intervals (e.g., 11:30 AM and 02:00 PM), proposed adjustments or additions are sent to the Portfolio Manager's UI dashboard.
    *   Approval Action: Portfolio Manager must click "Approve and Sign Packet" on the UI, which cryptographically signs the packet with their private key and sends it to the Vega execution client.
3.  **Post-Market Replay & Learning (03:30 PM - 05:00 PM IST)**:
    *   Slippage and transaction cost analysis are recorded.
    *   VIB (VYUH Intelligence Brain) ingests the day's trades to perform regime categorization updates and failure analyses.

---

## 6. User Persona Definitions
*   **Portfolio Manager (PM)**: Has final authority to approve portfolio rebalancing and execution packets. Sets risk parameters and reviews AI regime classification.
*   **Risk Officer (RO)**: Configures stress-test scenarios, correlation caps, and max drawdown parameters. Reviews daily Value-at-Risk (VaR) and margin breach events.
*   **Compliance Officer (CO)**: Monitors position limits (MWPL, client level) and expiry week exits.
*   **DevOps / SRE Engineer**: Monitors system health, 10-second loop latency, Kafka queue depths, and API error rates.


---

# 03_Functional_Requirements

# VYUH Engine - Functional Requirements Document (FRD)

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead Product Owner | Chief Quantitative Officer | Initial Specification |

---

## 2. Platform Functional Architecture
The VYUH Engine is composed of **15 modular sub-engines** that collaborate in a structured pipeline. The pipeline runs every 10 seconds during market hours.
```mermaid
flowchart TD
    Ingest["Ingestion & Normalization"]
    
    Ingest --> Market["Market Intelligence<br/>- Regime Classifier"]
    Ingest --> Liquidity["Liquidity Engine<br/>- Spread & OI Scorer"]
    Ingest --> Premium["Premium Valuation<br/>- Richness / Yield"]
    
    Market --> Prob["Probability Engines<br/>- Hist / IV / Touch"]
    Liquidity --> Strike["Strike Intelligence<br/>- Strike Selection"]
    Premium --> Strategy["Strategy Selection<br/>- Strategy Template"]
    
    Prob --> Optimizer["Portfolio Construction<br/>- Knapsack Optimizer"]
    Strike --> Optimizer
    Strategy --> Optimizer
    
    Optimizer --> Diversify["Diversification Engine<br/>- Sector & Correlation Caps"]
    Diversify --> Risk["Portfolio Risk Engine<br/>- Stress, VaR, Margin"]
    Risk --> Decision["Decision Engine<br/>- Hold / Adjust / Roll / Exit"]
    Decision --> Monitor["Monitoring & Reporting<br/>- SignalR Hub & EOD Reports"]
```

---

## 3. Specifications of the 15 Core Engines

### 3.1. Market Intelligence Engine
*   **Purpose**: Processes inputs from Suchak, Lakshmi, and TalkOptions to determine the volatility and trend regimes of the 200 F&O stocks.
*   **Inputs**: Spot Prices, Futures Prices, Technical Overlays (RSI, ADX, Supertrend, Support/Resistance), IV Percentile (IVP), IV Rank (IVR).
*   **Process**:
    1.  Computes directional bias (Bullish/Bearish/Neutral) using Support/Resistance and EMA/Supertrend.
    2.  Classifies Volatility State: *Low-Vol* (IVP < 25), *Normal-Vol* (25 <= IVP <= 70), *High-Vol* (IVP > 70).
    3.  Classifies Volatility Trend: *Expanding* vs. *Contracting* using IV 5-day EMA vs. 20-day EMA.
*   **Outputs**: Stock Regime State Packet (Directional Bias, Volatility State, Trend Strength).

### 3.2. Historical Price Probability Engine
*   **Purpose**: Computes the historical probability of an underlying stock closing above or below specific strike price thresholds at contract expiry.
*   **Inputs**: 5-Year Historical daily returns from Ganesh.
*   **Process**: Runs a non-parametric kernel density estimation (KDE) or historical bootstrap on the price return distributions of the underlying over the number of days remaining to expiry ($D$).
*   **Outputs**: Cumulative distribution function (CDF) values for the selected strikes.

### 3.3. IV Probability Engine
*   **Purpose**: Computes options-implied probability density functions (PDF) using the current options chain.
*   **Inputs**: Implied Volatilities, Strikes, Spot Price, Time to Expiry ($T$), Risk-Free Rate ($r$).
*   **Process**: Uses the Breeden-Litzenberger method to extract the risk-neutral probability density from the second derivative of the option price with respect to strike:
    $$f(K) = e^{rT} \frac{\partial^2 C(K)}{\partial K^2}$$
*   **Outputs**: Implied probability of closing between any two strikes at expiry.

### 3.4. Touch Probability Engine
*   **Purpose**: Estimates the probability of the underlying price touching or crossing a target strike boundary *at any point* before expiry.
*   **Inputs**: Spot Price, Drift ($\mu$), Implied Volatility ($\sigma$), Time to Expiry ($T$), target Strike ($K$).
*   **Process**: Solves the first-passage time problem for a geometric Brownian motion (GBM). Touch probability is mathematically approximated as:
    $$P_{\text{touch}}(K, T) = 2 \cdot \Phi\left( \frac{-\ln(K / S_0) + \mu T}{\sigma \sqrt{T}} \right)$$
    where $\Phi$ is the standard normal cumulative distribution.
*   **Outputs**: Probability of Touch (0.0 to 1.0) for every strike in the chain.

### 3.5. Range Probability Engine
*   **Purpose**: Calculates the probability that the underlying price will remain strictly within a specific upper and lower strike band (e.g., Short Strangle boundaries) at expiry.
*   **Inputs**: CDF values from the Historical and IV Probability Engines.
*   **Process**:
    $$P_{\text{range}}(K_{\text{lower}}, K_{\text{upper}}) = P(S_T \le K_{\text{upper}}) - P(S_T \le K_{\text{lower}})$$
*   **Outputs**: Expected Range Probability for every strike pair.

### 3.6. Liquidity Engine
*   **Purpose**: Scores and filters strikes based on trade execution feasibility to prevent slippage losses.
*   **Inputs**: Bid-Ask Spread, Bid-Ask Volume Depth, Open Interest (OI), Daily Vol Volume.
*   **Process**: Evaluates a weighted Liquidity Score (0 to 100):
    $$\text{Score} = w_1 \cdot \text{Spread\_Factor} + w_2 \cdot \text{OI\_Factor} + w_3 \cdot \text{Depth\_Factor}$$
    Strikes with Score < 40 are excluded from selection.
*   **Outputs**: List of liquid strikes.

### 3.7. Strike Intelligence Engine
*   **Purpose**: Evaluates candidate strikes to locate the optimal balance of premium collection vs. probability of being tested.
*   **Inputs**: Volatility regimes, expected move boundaries, Touch probabilities, and liquid strike list.
*   **Process**: Evaluates strikes lying near the 1-Standard-Deviation expected move limit (typically corresponding to a delta of 0.15 to 0.20 for Strangles).
*   **Outputs**: Primary Short Call Strike, Primary Short Put Strike, Hedge Call Strike, Hedge Put Strike.

### 3.8. Premium Valuation Engine
*   **Purpose**: Identifies mispriced options by comparing Implied Volatility (IV) to Historical Volatility (HV) and computing "Premium Richness".
*   **Inputs**: Market option premium, Implied Volatility ($\sigma_{\text{implied}}$), Historical Volatility ($\sigma_{\text{historical}}$).
*   **Process**: Computes the Volatility Risk Premium (VRP) ratio:
    $$\text{Richness} = \frac{\sigma_{\text{implied}}}{\sigma_{\text{historical}}}$$
    Also calculates Theta-to-Margin efficiency: $(\text{Theta} \times 365) / \text{Margin Required}$.
*   **Outputs**: Richness Score, Theta Efficiency rating.

### 3.9. Strategy Selection Engine
*   **Purpose**: Maps the Market Intelligence regime and Premium Valuation metrics to the optimal Net-Short option structure.
*   **Inputs**: Volatility state, direction bias, and premium richness.
*   **Process**:
    *   *High IVR + Neutral Bias*: Short Strangle / Short Straddle.
    *   *High IVR + Directional Bias*: Short Put Strangle (skewed) or Ratio Spread.
    *   *Low/Medium IVR + Neutral*: Iron Condor / Iron Fly (limited risk).
*   **Outputs**: Strategy Template (legs, ratios, limit structures).

### 3.10. Portfolio Construction Engine
*   **Purpose**: Aggregates the candidate strategies across the 200 stocks and constructs a diversified portfolio utilizing the â‚¹25 Crores capital.
*   **Inputs**: Strategy candidates, required margin per strategy, expected yields, and correlation matrices.
*   **Process**: Solves a multi-constraint Knapsack optimization problem, maximizing total expected portfolio theta subject to capital limits and stock allocation bounds.
*   **Outputs**: Proposed Portfolio State (active underlyings, quantities, strikes).

### 3.11. Diversification Engine
*   **Purpose**: Enforces sector, industry, and correlation constraints on the proposed portfolio.
*   **Inputs**: Proposed portfolio state, asset correlation matrix, sector mappings.
*   **Process**: Adjusts weights of underlyings to respect the 15% Sector Cap and rejects underlyings if their correlation with already selected assets exceeds 0.35.
*   **Outputs**: Diversified Portfolio Allocation.

### 13.12. Portfolio Risk Engine
*   **Purpose**: Calculates risk metrics under normal and stressed market environments.
*   **Inputs**: Live portfolio, market feed, spot beta coefficients.
*   **Process**:
    1.  Computes Portfolio Delta, Gamma, Theta, and Vega.
    2.  Calculates 99% 1-day Value-at-Risk (VaR) using Historical Simulation.
    3.  Stress tests portfolio under a -10% spot market shock and a +100% IV spike.
*   **Outputs**: VaR, CVaR, Stress Test results, Margin Breach Alerts.

### 3.13. Decision Engine
*   **Purpose**: Determines the specific action (Hold, Adjust, Roll Over, Exit) for each active position based on risk-to-reward boundaries.
*   **Inputs**: Current PnL, Touch Probability status, remaining days to expiry, and current delta exposure.
*   **Process**:
    *   *Rule 1 (Exit)*: If profit >= 50% of maximum credit, exit.
    *   *Rule 2 (Adjust)*: If one side delta reaches 0.35, roll the untested side to collect more credit.
    *   *Rule 3 (Stop-Loss)*: If touch probability of a short strike reaches 0.85, exit or roll to a wider spread.
*   **Outputs**: Action recommendations (Exit leg, Enter leg, Hold).

### 3.14. Monitoring Engine
*   **Purpose**: Runs health checks on all integrations (Ganesh, Lakshmi, Suchak, TalkOptions, Vega) and monitors the 10-second loop execution latency.
*   **Inputs**: Latency logs, system heartbeats, connection status.
*   **Outputs**: System Health Dashboard states, critical alert notifications.

### 3.15. Reporting Engine
*   **Purpose**: Generates end-of-day analytics, trade logs, performance metrics, and compliance filings.
*   **Inputs**: Daily trade executions, realized PnL, margins consumed.
*   **Outputs**: Daily PDF risk report, Kafka reporting events, performance attribution summaries.

---

## 4. System Use Cases and User Workflows

### Use Case 1: Daily Portfolio Construction
1.  System initializes at 09:15 AM.
2.  Market Intelligence normalizes the universe and defines regimes.
3.  Portfolio Construction calculates the optimal basket of 70–80 stocks.
4.  Diversification and Risk Engines validate exposure limits.
5.  Decision Engine compiles these into an execution packet.
6.  Portfolio Manager approves the packet via the UI, sending it to Vega.

### Use Case 2: Risk Breach and Position Adjustment
1.  At 11:15 AM, Stock A spot rises sharply, causing the short Call delta to hit 0.38 (breaching the 0.35 threshold).
2.  Decision Engine detects the breach, computes the adjustment (rolling up the Put side to delta 0.15).
3.  An adjustment packet is compiled and pushed to Vega for immediate execution (or PM review depending on risk settings).


---

# 04_Architecture

# VYUH Engine - System Architecture Document

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Chief Software Architect | Architecture Review Board | Initial Specification |

---

## 2. Architectural Principles & Blueprint
VYUH is designed on the principles of **Clean Architecture**, **Domain-Driven Design (DDD)**, and **Command Query Responsibility Segregation (CQRS)** inside a **Microservices** deployment model.

```
       PRESENTATION / API LAYER (.NET 9 Web API, SignalR, Next.js UI)
                                 â”‚
                                 â–¼
         APPLICATION LAYER (MediatR Handlers, CQRS, DTOs, Validators)
                                 â”‚
                                 â–¼
            DOMAIN LAYER (Entities, Value Objects, Domain Events)
                                 â–²
                                 â”‚
  INFRASTRUCTURE LAYER (PostgreSQL, Redis Cache, Kafka, gRPC, Clients)
```

*   **Clean Architecture**: Separation of concerns. The Domain layer has zero dependencies on databases, UI, or external APIs. The Infrastructure layer contains the technical details (EF Core, Kafka topics, Redis clients).
*   **Domain-Driven Design (DDD)**: Logic is isolated into Bounded Contexts. Aggregate roots manage domain entity boundaries.
*   **CQRS**: Commands (state modifications) are separated from Queries (data retrieval) via `MediatR` to optimize database read/write pathways.
*   **Event-Driven Microservices**: Communication between services is primarily asynchronous via Apache Kafka, ensuring decoupled scalability and backpressure tolerance.

---

## 3. Tech Stack Specification
*   **Frontend**: Next.js 15, React 19, TypeScript 5, Tailwind CSS v4, AG Grid Enterprise (for high-throughput tabular portfolio view), TradingView Charts (lightweight charting library for spot and options metrics).
*   **Backend**: .NET 9, ASP.NET Core, SignalR (for real-time streaming to the UI), Background Workers (IHostedService/BackgroundService for 10-second loops).
*   **Databases**: PostgreSQL 17 (relational persistence, auditing, config), Redis 7.4 (in-memory options chain, fast state retrieval).
*   **Infrastructure**: Apache Kafka (message broker), Docker (containerization), Kubernetes (orchestration), Prometheus (metrics collection), Grafana (visualization), OpenTelemetry (distributed tracing).

---

## 4. Microservice Boundaries & Bounded Contexts
The engine is split into four microservices:

```mermaid
graph TD
    UI[Next.js Frontend] <-->|SignalR / HTTP| Gateway[Web API & Gateway Service]
    Gateway <-->|gRPC| ScoreSvc[Ingestion & Scoring Service]
    Gateway <-->|gRPC| OptSvc[Portfolio Optimizer Service]
    Gateway <-->|gRPC| RiskSvc[Risk & Stress Service]
    
    ScoreSvc -->|Publish Ticks| Kafka{Kafka Broker}
    OptSvc -->|Publish Allocations| Kafka
    RiskSvc -->|Publish Stress Metrics| Kafka
    
    classDef svc fill:#cfc,stroke:#333,stroke-width:2px;
    class Gateway,ScoreSvc,OptSvc,RiskSvc svc;
```

### 4.1. Ingestion & Scoring Service
*   **Bounded Context**: `MarketIntelligence`, `ProbabilityModeling`.
*   **Responsibilities**: Ingests spot, futures, and option chain ticks via Kafka from Lakshmi. Calls Suchak and TalkOptions to calculate indicators and greeks. Runs the Probability Engines (Historical, IV, Touch, Range) every 10 seconds.
*   **Data Stores**: Redis (active ticks and option chains), PostgreSQL (scoring archives).

### 4.2. Portfolio Optimizer Service
*   **Bounded Context**: `PortfolioIntelligence`, `Diversification`.
*   **Responsibilities**: Consumes scoring data from the Ingestion Service, runs the Knapsack optimizer and diversification checks. Generates the draft portfolio.
*   **Data Stores**: Redis (temporary optimization parameters).

### 4.3. Risk & Stress Service
*   **Bounded Context**: `RiskEngine`.
*   **Responsibilities**: Stress-tests the draft portfolio against predefined market shocks. Computes VaR and CVaR. Runs margin models.
*   **Data Stores**: PostgreSQL (historical simulation returns).

### 4.4. Web API & Gateway Service
*   **Bounded Context**: `DecisionEngine`, `Reporting`.
*   **Responsibilities**: Orchestrates client UI connections, manages user authentication, hosts SignalR hubs for real-time tickers and portfolio states. Manages the execution packet sign-off state.
*   **Data Stores**: PostgreSQL (system configurations, user audit logs, position states).

---

## 5. Event-Driven Workflow Sequence
Below is the sequence of events during a single 10-second portfolio evaluation and execution cycle:

```mermaid
sequenceDiagram
    autonumber
    participant L as Lakshmi (Realtime)
    participant IS as Ingestion & Scoring Service
    participant PO as Portfolio Optimizer
    participant RS as Risk & Stress Service
    participant GW as Gateway & Web API
    participant UI as Next.js UI
    
    L->>IS: Kafka Event: raw.options.chain.tick
    IS->>IS: Run Probability & Liquidity Models
    IS->>PO: gRPC: GetOptimalBasket(stockScores)
    PO->>PO: Run Knapsack Allocation & Sector Limits
    PO->>RS: gRPC: ValidatePortfolioRisk(draftPortfolio)
    RS->>RS: Compute 99% VaR & Shock Simulation
    RS-->>PO: Return RiskMetrics (VaR, stressPnL, margin)
    PO-->>IS: Return ValidatedPortfolio
    IS->>GW: Kafka Event: vyuh.portfolio.proposed-allocation
    GW->>UI: SignalR Stream: PushProposedPortfolio
    Note over UI: PM Reviews and Clicks 'Approve'
    UI->>GW: HTTPS POST: ApprovePacket (Signed)
    GW->>IS: Kafka Event: vyuh.portfolio.confirmed-allocation
    IS->>IS: Generate Cryptographic Execution Packet
    IS->>L: (To Vega Execution Engine) Kafka: order.execution.packet
```

---

## 6. Project Structure (.NET 9 Clean Architecture Template)
For developers, the backend source repository must map to the following structure:

```
VYUH.Engine/
â”‚
â”œâ”€â”€ src/
â”‚   â”œâ”€â”€ Services/
â”‚   â”‚   â”œâ”€â”€ IngestionScoringService/
â”‚   â”‚   â”‚   â”œâ”€â”€ VYUH.Ingestion.Api/
â”‚   â”‚   â”‚   â”œâ”€â”€ VYUH.Ingestion.Application/
â”‚   â”‚   â”‚   â”œâ”€â”€ VYUH.Ingestion.Domain/
â”‚   â”‚   â”‚   â””â”€â”€ VYUH.Ingestion.Infrastructure/
â”‚   â”‚   â”œâ”€â”€ PortfolioOptimizerService/
â”‚   â”‚   â”‚   â”œâ”€â”€ VYUH.Optimizer.Api/
â”‚   â”‚   â”‚   â”œâ”€â”€ VYUH.Optimizer.Application/
â”‚   â”‚   â”‚   â”œâ”€â”€ VYUH.Optimizer.Domain/
â”‚   â”‚   â”‚   â””â”€â”€ VYUH.Optimizer.Infrastructure/
â”‚   â”‚   â””â”€â”€ RiskService/
â”‚   â”‚       â”œâ”€â”€ VYUH.Risk.Api/
â”‚   â”‚       â”œâ”€â”€ VYUH.Risk.Application/
â”‚   â”‚       â”œâ”€â”€ VYUH.Risk.Domain/
â”‚   â”‚       â””â”€â”€ VYUH.Risk.Infrastructure/
â”‚   â””â”€â”€ Shared/
â”‚       â””â”€â”€ VYUH.Shared.Contracts/ (Kafka schemas, DTO structures, Shared Utils)
â”‚
â””â”€â”€ tests/
    â”œâ”€â”€ VYUH.Ingestion.UnitTests/
    â”œâ”€â”€ VYUH.Optimizer.UnitTests/
    â””â”€â”€ VYUH.IntegrationTests/
```
Each service enforces strict boundaries using .NET project references, with Domain having no dependencies, Application depending only on Domain, and Infrastructure and Api depending on Application.


---

# 05_Database

# VYUH Engine - Database Schema Specification

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead Database Architect | Principal Architect | Initial Specification |

---

## 2. PostgreSQL Relational Schemas
PostgreSQL is the database of record for configuration, audit trails, portfolios, and historical metrics.

```mermaid
erDiagram
    UNDERLYING_CONFIG ||--o{ STOCK_DAILY_SCORE : "has daily"
    UNDERLYING_CONFIG ||--o{ SECTOR_CAPS : "belongs to"
    PORTFOLIO_STATE ||--o{ POSITION_STATE : "contains"
    PORTFOLIO_STATE ||--o{ EXECUTION_PACKET : "generates"
    EXECUTION_PACKET ||--o{ ORDER_LOG : "triggers"
```

### 2.1. System Configuration Tables

#### 2.1.1. Underlying Configurations
```sql
CREATE TABLE vyuh_config.underlying_stocks (
    stock_id VARCHAR(20) PRIMARY KEY,          -- NSE Ticker (e.g., 'RELIANCE')
    stock_name VARCHAR(100) NOT NULL,
    sector VARCHAR(50) NOT NULL,              -- e.g., 'OIL & GAS'
    industry VARCHAR(50) NOT NULL,            -- e.g., 'REFINERIES'
    lot_size INT NOT NULL,                     -- NSE Option Lot Size (e.g., 250)
    is_active BOOLEAN DEFAULT TRUE NOT NULL,
    max_position_multiplier NUMERIC(3,2) DEFAULT 1.00 NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL
);

CREATE INDEX idx_underlyings_sector ON vyuh_config.underlying_stocks(sector);
```

#### 2.1.2. Sector Limit Configurations
```sql
CREATE TABLE vyuh_config.sector_limits (
    sector_name VARCHAR(50) PRIMARY KEY,
    max_margin_allocation_pct NUMERIC(4,2) NOT NULL, -- Sector Cap (e.g., 15.00)
    current_allocation_pct NUMERIC(4,2) DEFAULT 0.00 NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL
);
```

### 2.2. Transactional and Portfolio Tables

#### 2.2.1. Portfolio States (Master Record)
```sql
CREATE TABLE vyuh_portfolio.portfolio_states (
    portfolio_state_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    timestamp TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    total_capital NUMERIC(15,2) NOT NULL,               -- e.g., 250000000.00
    margin_deployed NUMERIC(15,2) NOT NULL,              -- e.g., 150000000.00
    liquid_buffer NUMERIC(15,2) NOT NULL,                -- e.g., 75000000.00
    drawdown_reserve NUMERIC(15,2) NOT NULL,             -- e.g., 25000000.00
    portfolio_theta NUMERIC(12,2) NOT NULL,              -- Daily decay amount
    portfolio_delta NUMERIC(12,2) NOT NULL,              -- Net delta exposure
    portfolio_gamma NUMERIC(12,2) NOT NULL,
    portfolio_vega NUMERIC(12,2) NOT NULL,
    status VARCHAR(20) DEFAULT 'ACTIVE' NOT NULL       -- 'DRAFT', 'ACTIVE', 'ARCHIVED'
);

CREATE INDEX idx_portfolio_states_timestamp ON vyuh_portfolio.portfolio_states(timestamp DESC);
```

#### 2.2.2. Position States
```sql
CREATE TABLE vyuh_portfolio.position_states (
    position_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    portfolio_state_id UUID REFERENCES vyuh_portfolio.portfolio_states(portfolio_state_id) ON DELETE CASCADE,
    stock_id VARCHAR(20) REFERENCES vyuh_config.underlying_stocks(stock_id),
    strategy_type VARCHAR(30) NOT NULL,               -- 'STRANGLE', 'STRADDLE', etc.
    option_type VARCHAR(2) NOT NULL,                   -- 'CE' (Call), 'PE' (Put)
    strike_price NUMERIC(10,2) NOT NULL,
    expiry_date DATE NOT NULL,
    quantity INT NOT NULL,                             -- Positive for Long, Negative for Short
    average_entry_price NUMERIC(8,2) NOT NULL,
    current_market_price NUMERIC(8,2) NOT NULL,
    unrealized_pnl NUMERIC(12,2) NOT NULL,
    margin_consumed NUMERIC(12,2) NOT NULL,
    delta NUMERIC(6,4) NOT NULL,
    theta NUMERIC(8,4) NOT NULL,
    touch_probability NUMERIC(4,3) NOT NULL,           -- Computed Touch Probability (0.0 to 1.0)
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL
);

CREATE INDEX idx_positions_portfolio ON vyuh_portfolio.position_states(portfolio_state_id);
CREATE INDEX idx_positions_stock ON vyuh_portfolio.position_states(stock_id);
```

#### 2.2.3. Execution Packets (For Vega OMS)
```sql
CREATE TABLE vyuh_execution.execution_packets (
    packet_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    portfolio_state_id UUID REFERENCES vyuh_portfolio.portfolio_states(portfolio_state_id),
    action_timestamp TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    raw_packet_json TEXT NOT NULL,                    -- Complete execution JSON payload
    digital_signature VARCHAR(512) NOT NULL,          -- PM Private Key Signature
    approved_by VARCHAR(50) NOT NULL,                 -- PM Username
    status VARCHAR(20) NOT NULL                       -- 'PENDING', 'SENT', 'EXECUTED', 'FAILED'
);
```

### 2.3. Historical Scoring and Analytics Partitioned Tables
Since VYUH scores 200 stocks every 10 seconds, the daily scores table accumulates data rapidly (~172,800 rows per underlying per day). We partition this table by date range.

```sql
CREATE TABLE vyuh_analytics.stock_scores_archive (
    score_id BIGSERIAL,
    stock_id VARCHAR(20) NOT NULL,
    timestamp TIMESTAMP WITH TIME ZONE NOT NULL,
    spot_price NUMERIC(10,2) NOT NULL,
    iv_percentile NUMERIC(5,2) NOT NULL,
    liquidity_score NUMERIC(5,2) NOT NULL,
    strategy_selected VARCHAR(30) NOT NULL,
    expected_yield NUMERIC(6,4) NOT NULL,
    risk_score NUMERIC(5,2) NOT NULL,
    PRIMARY KEY (score_id, timestamp)
) PARTITION BY RANGE (timestamp);

-- Example Partition Creation (Automated via pg_partman)
CREATE TABLE vyuh_analytics.stock_scores_archive_y2026m07d15 PARTITION OF vyuh_analytics.stock_scores_archive
    FOR VALUES FROM ('2026-07-15 00:00:00+05:30') TO ('2026-07-16 00:00:00+05:30');
```

---

## 3. Redis High-Throughput Data Structures
Redis is the primary cache layer for fast computations within the 10-second loop.

### 3.1. Key Structures and Mappings

#### 3.1.1. Options Chain Snapshot (Hash Structure)
*   **Key**: `vyuh:market:chain:{StockId}:{ExpiryDate}`
*   **Data Structure**: HASH
*   **Fields**:
    *   `spot_price`: Current Spot Price (string representation of float)
    *   `future_price`: Current Future Price
    *   `last_update`: ISO Timestamp
    *   `strike:{StrikePrice}:{CE/PE}:bid`: Bid Price
    *   `strike:{StrikePrice}:{CE/PE}:ask`: Ask Price
    *   `strike:{StrikePrice}:{CE/PE}:oi`: Open Interest
    *   `strike:{StrikePrice}:{CE/PE}:volume`: Daily Traded Volume
    *   `strike:{StrikePrice}:{CE/PE}:delta`: Delta (TalkOptions greek value)
    *   `strike:{StrikePrice}:{CE/PE}:theta`: Theta
    *   `strike:{StrikePrice}:{CE/PE}:iv`: Implied Volatility

#### 3.1.2. Realtime Stock Score Cache (Sorted Set Structure)
*   **Key**: `vyuh:scoring:overall_rank`
*   **Data Structure**: ZSET (Sorted Set)
*   **Score**: Overall score rating (0 to 100)
*   **Value**: Stock Ticker ID (e.g., 'TCS', 'RELIANCE')
*   **Usage**: The Portfolio Builder retrieves the top-ranked stocks by sorting values.

#### 3.1.3. Active Portfolio State Snapshot
*   **Key**: `vyuh:portfolio:active_snapshot`
*   **Data Structure**: HASH
*   **Fields**:
    *   `current_margin_utilization`: Float value of active margin.
    *   `active_positions_count`: Integer of current active stocks.
    *   `sector_allocation:{SectorName}`: Current margin allocated per sector.
    *   `stock_allocation:{StockId}`: Current margin allocated per stock.

---

## 4. Data Retention and Archival Policy
*   **Redis Real-time Tickers**: Expires every day at 04:00 PM IST (post-market close).
*   **PostgreSQL Daily Scores Partition**: Maintained live for **30 Days**. Partitions older than 30 days are automatically exported to Parquet format and stored in AWS S3 / MinIO cold storage before dropping from PostgreSQL.
*   **Portfolio and Transactional Logs**: Retained in PostgreSQL indefinitely (minimum 8 years for regulatory audit purposes).


---

# 06_APIs

# VYUH Engine - API Specification Document

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead Integration Engineer | Technical Architect | Initial Specification |

---

## 2. OpenAPI 3.0 REST Endpoints (Gateway Service)

### 2.1. Retrieve Stock Scoring Universe
*   **Endpoint**: `GET /api/v1/scoring/universe`
*   **Description**: Returns the active ranked universe of 200 stocks, sorted by overall portfolio construction suitability score.
*   **Request Headers**:
    ```http
    Authorization: Bearer <JWT_TOKEN>
    X-Client-Signature: HMAC-SHA256(<Payload>, <ClientSecret>)
    ```
*   **Response Payload (`200 OK`)**:
    ```json
    {
      "timestamp": "2026-07-15T09:40:00Z",
      "count": 200,
      "stocks": [
        {
          "rank": 1,
          "stockId": "RELIANCE",
          "spotPrice": 2450.50,
          "overallScore": 92.4,
          "liquidityScore": 98.1,
          "volatilityRegime": "HIGH_VOL_CONTRACTING",
          "ivPercentile": 78.5,
          "recommendedStrategy": "SHORT_STRANGLE",
          "expectedYield": 0.0215,
          "requiredMargin": 180000.00
        },
        {
          "rank": 2,
          "stockId": "TCS",
          "spotPrice": 3410.20,
          "overallScore": 89.1,
          "liquidityScore": 95.3,
          "volatilityRegime": "LOW_VOL_EXPANDING",
          "ivPercentile": 18.2,
          "recommendedStrategy": "IRON_CONDOR",
          "expectedYield": 0.0120,
          "requiredMargin": 120000.00
        }
      ]
    }
    ```

### 2.2. Generate Execution Packet
*   **Endpoint**: `POST /api/v1/portfolio/rebalance/generate`
*   **Description**: Forces a portfolio rebalancing calculation and returns the draft Execution Packet for Portfolio Manager approval.
*   **Response Payload (`200 OK`)**:
    ```json
    {
      "portfolioStateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "timestamp": "2026-07-15T09:40:10Z",
      "marginRequired": 145000000.00,
      "bufferAvailable": 80000000.00,
      "trades": [
        {
          "action": "SELL",
          "stockId": "RELIANCE",
          "optionType": "CE",
          "strike": 2600.00,
          "expiry": "2026-07-30",
          "quantity": 1000,
          "limitPrice": 35.50
        },
        {
          "action": "SELL",
          "stockId": "RELIANCE",
          "optionType": "PE",
          "strike": 2300.00,
          "expiry": "2026-07-30",
          "quantity": 1000,
          "limitPrice": 28.00
        }
      ],
      "signatureRequired": true
    }
    ```

### 2.3. Submit Approval Signature
*   **Endpoint**: `POST /api/v1/portfolio/rebalance/approve`
*   **Description**: Submits the cryptographic signature approving the rebalance packet. Once validated, the gateway publishes the packet directly to the Vega execution stream.
*   **Request Payload**:
    ```json
    {
      "portfolioStateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "approvedBy": "solanki_pm",
      "digitalSignature": "MEYCIQCc3vY7sS...gih8="
    }
    ```
*   **Response Payload (`202 Accepted`)**:
    ```json
    {
      "status": "APPROVED_AND_SENT_TO_OMS",
      "transactionId": "b1b229c1-5df7-4630-80ea-3e7514a60193",
      "timestamp": "2026-07-15T09:40:15Z"
    }
    ```

---

## 3. SignalR WebSockets Realtime Streaming
The Gateway Service exposes a SignalR Hub at `/hubs/portfolio-stream`. Clients subscribe to live event streams.

### 3.1. Client Subscriptions
*   `SubscribeToPortfolioMetrics()`: Streams portfolio-level statistics.
*   `SubscribeToBreaches()`: Streams risk and margin threshold alerts.

### 3.2. Outbound Hub Events

#### 3.2.1. `ReceivePortfolioUpdate`
Pushed every 10 seconds following a loop evaluation.
```json
{
  "portfolioTheta": 452100.00,      -- Daily portfolio decay
  "portfolioDelta": 12.45,          -- Net delta exposure
  "marginUsagePct": 58.00,          -- 58% of target capital used
  "unrealizedPnL": 1420500.00,
  "lastEvaluationTime": "2026-07-15T09:40:20Z"
}
```

#### 3.2.2. `ReceiveRiskBreach`
Pushed immediately upon threshold violation.
```json
{
  "breachType": "SINGLE_STOCK_MARGIN_EXCEEDED",
  "stockId": "SBIN",
  "currentMarginUsage": 3250000.00, -- Exceeds the max â‚¹30 Lakhs (2% limit)
  "limit": 3000000.00,
  "severity": "CRITICAL"
}
```

---

## 4. Apache Kafka Message Schemas

### 4.1. Raw Options Chain Tick Event
*   **Topic**: `raw.options.chain.tick`
*   **Key**: `StockId` (string)
*   **Payload Schema**:
    ```json
    {
      "stockId": "RELIANCE",
      "spotPrice": 2450.50,
      "futurePrice": 2462.10,
      "timestamp": 1784108420000, -- Epoch MS
      "options": [
        {
          "strike": 2400.00,
          "optionType": "CE",
          "bid": 85.20,
          "ask": 85.90,
          "oi": 120500,
          "volume": 2500
        },
        {
          "strike": 2400.00,
          "optionType": "PE",
          "bid": 34.50,
          "ask": 35.10,
          "oi": 89400,
          "volume": 1800
        }
      ]
    }
    ```

### 4.2. Order Execution Packet Event (To Vega Engine)
*   **Topic**: `order.execution.packet`
*   **Key**: `PortfolioStateId` (UUID)
*   **Payload Schema**:
    ```json
    {
      "packetId": "d0e12d1b-7a74-4b5c-a5b6-6d6c6e7f8a9b",
      "timestamp": 1784108435000,
      "engineSource": "VYUH",
      "signature": "MEYCIQCc3vY7sS...gih8=",
      "orders": [
        {
          "orderId": "vyuh-rel-ce-2600-s",
          "stockId": "RELIANCE",
          "segment": "DERIVATIVES",
          "productType": "NRML",
          "orderType": "LIMIT",
          "optionType": "CE",
          "strikePrice": 2600.00,
          "expiryDate": "2026-07-30",
          "quantity": 1000,
          "side": "SELL",
          "limitPrice": 35.50
        }
      ]
    }
    ```


---

# 07_Mathematical_Models

# VYUH Engine - Mathematical Models Document

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead Quantitative Analyst | Head of Quantitative Research | Initial Specification |

---

## 2. Core Quantitative Models

### 2.1. Average True Range (ATR)
ATR measures market volatility by decomposing the entire range of an asset's price.

#### 2.1.1. Formula
The True Range ($TR$) for a given period $t$ is:
$$TR_t = \max\left( H_t - L_t, \, |H_t - C_{t-1}|, \, |L_t - C_{t-1}| \right)$$
where $H_t$ is the current high, $L_t$ is the current low, and $C_{t-1}$ is the previous close.

The ATR over an $n$-period window is smoothed using Wilder's technique:
$$ATR_t = \frac{ATR_{t-1} \times (n - 1) + TR_t}{n}$$

#### 2.1.2. Worked Example
For $n = 14$ and given:
*   Previous $ATR_{t-1} = 45.00$
*   Current High $H_t = 2480.00$, Current Low $L_t = 2442.00$, Previous Close $C_{t-1} = 2450.00$.

1.  Calculate $TR_t$:
    *   $H_t - L_t = 2480 - 2442 = 38.00$
    *   $|H_t - C_{t-1}| = |2480 - 2450| = 30.00$
    *   $|L_t - C_{t-1}| = |2442 - 2450| = 8.00$
    *   $TR_t = \max(38, 30, 8) = 38.00$
2.  Calculate $ATR_t$:
    $$ATR_t = \frac{(45.00 \times 13) + 38.00}{14} = \frac{585 + 38}{14} = 44.50$$

#### 2.1.3. Edge Cases & Validation
*   **Zero Volume / No-Trade Days**: $TR_t$ collapses to 0. The system uses the previous $ATR_{t-1}$ value as a fallback.
*   **First $n$ Days of History**: Initialize the first ATR value as the simple moving average (SMA) of $TR$ values over the first $n$ periods.

---

### 2.2. Expected Move (EM)
Expected Move represents the market's implied range of price fluctuation for an underlying stock before option contract expiry.

#### 2.2.1. Formula
Institutional expected move is computed using the At-The-Money (ATM) option premium:
$$EM = \text{ATM Straddle Price} \times 0.85$$
Alternative mathematical model using Implied Volatility:
$$EM = S_0 \times \sigma \times \sqrt{\frac{T}{365}}$$
where $S_0$ is the current spot price, $\sigma$ is the implied volatility (IV), and $T$ is the days to expiry.

#### 2.2.2. Worked Example
*   Spot Price $S_0 = 2500.00$
*   ATM Straddle Price (ATM Call Premium + ATM Put Premium) = $120.00 + 105.00 = 225.00$
*   $EM = 225.00 \times 0.85 = 191.25$
*   Implied upper bound = $2500 + 191.25 = 2691.25$
*   Implied lower bound = $2500 - 191.25 = 2308.75$

#### 2.2.3. Edge Cases & Validation
*   **Skew/Smile Discrepancy**: During steep skew events, the ATM straddle can underrepresent tail risk. The system adjusts the calculation by adding $0.1 \times \text{Out-Of-The-Money (OTM) Strangle premium}$.

---

### 2.3. IV Rank (IVR)
IV Rank measures where the current Implied Volatility stands relative to its 52-week high and low.

#### 2.3.1. Formula
$$IVR = \frac{IV_{\text{current}} - IV_{\text{min}}}{IV_{\text{max}} - IV_{\text{min}}} \times 100$$
where $IV_{\text{min}}$ and $IV_{\text{max}}$ are the minimum and maximum daily IV values over the past 252 trading days.

#### 2.3.2. Worked Example
*   $IV_{\text{current}} = 28\%$
*   $IV_{\text{min}} = 12\%$, $IV_{\text{max}} = 52\%$
*   $$IVR = \frac{28 - 12}{52 - 12} \times 100 = \frac{16}{40} \times 100 = 40.00\%$$

#### 2.3.3. Edge Cases & Validation
*   **Division by Zero**: If $IV_{\text{max}} = IV_{\text{min}}$ (extreme flat volatility environment), set $IVR = 0$.
*   **Volatility Spikes (Abnormal Outliers)**: A single anomalous IV day (e.g. budget day at 120%) will compress all subsequent IVR values. The system resolves this by applying a 95th percentile clamp on the 252-day history.

---

### 2.4. IV Percentile (IVP)
IV Percentile measures the percentage of days in the past 252 trading days where the implied volatility was lower than the current implied volatility.

#### 2.4.1. Formula
$$IVP = \frac{\sum_{i=1}^{N} \mathbb{I}(IV_i < IV_{\text{current}})}{N} \times 100$$
where $N = 252$ and $\mathbb{I}$ is the indicator function (returns 1 if true, 0 if false).

#### 2.4.2. Worked Example
*   Out of the last 252 trading days, the IV was below the current $IV_{\text{current}}$ on exactly 189 days.
*   $$IVP = \frac{189}{252} \times 100 = 75.00\%$$

#### 2.4.3. Edge Cases & Validation
*   Requires a minimum of 60 trading days of historical IV database records before the metric is active.

---

### 2.5. Premium Richness (PR)
Premium Richness quantifies the Volatility Risk Premium (VRP) by comparing implied option pricing volatility to historical volatility.

#### 2.5.1. Formula
$$PR = \frac{IV_{\text{current}}}{HV_{30}}$$
where $HV_{30}$ is the 30-day historical realized volatility computed from daily log returns:
$$HV_{30} = \sqrt{\frac{252}{30} \sum_{i=1}^{30} \left( \ln(S_i / S_{i-1}) - \mu \right)^2}$$

#### 2.5.2. Worked Example
*   $IV_{\text{current}} = 24.5\%$
*   $HV_{30} = 17.5\%$
*   $$PR = \frac{24.5}{17.5} = 1.40$$
    *   $PR > 1.0$: Options are rich (overpriced relative to historical volatility). Excellent selling environment.
    *   $PR < 1.0$: Options are cheap. Avoid writing naked positions.

---

### 2.6. Theta Efficiency (TE)
Theta Efficiency measures the yield per rupee of margin capital consumed by the options strategy.

#### 2.6.1. Formula
$$TE = \frac{\Theta_{\text{position}} \times 365}{\text{Margin Required}} \times 100$$
where $\Theta_{\text{position}}$ is the daily theta decay of the options position in rupees.

#### 2.6.2. Worked Example
*   Daily Theta $\Theta_{\text{position}} = â‚¹1,200.00$
*   Margin Required = â‚¹180,000.00
*   $$TE = \frac{1,200.00 \times 365}{180,000.00} \times 100 = \frac{438,000.00}{180,000.00} \times 100 = 243.33\%$$
    *   This indicates the position yields a annualized theta decay rate equivalent to 243% of its margin requirement (assuming no spot movement).
*   **Rebalancing Threshold**: Strategies with $TE < 50\%$ are flag-marked for rollover or consolidation.


---

# 08_Probability_Models

# VYUH Engine - Probability Models Document

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead Quantitative Researcher | Principal Quantitative Architect | Initial Specification |

---

## 2. Quantitative Probability Subsystems

### 2.1. Historical Price Probability (Non-Parametric Bootstrap)
Estimates the empirical probability of price closing above/below a strike based on historical return distribution, avoiding normality assumptions.

#### 2.1.1. Formula
Let $\{r_1, r_2, \dots, r_M\}$ be the historical daily log returns of the underlying from Ganesh.
To calculate the probability of the spot price at expiry $S_T$ exceeding a target strike $K$, given time to expiry $T$ (in days):
1.  Draw $B$ bootstrap samples of size $T$ from the returns dataset.
2.  For each bootstrap path $b \in \{1, \dots, B\}$, compute the terminal price:
    $$S_T^{(b)} = S_0 \cdot \exp\left( \sum_{i=1}^{T} r_i^{(b)} \right)$$
3.  The empirical probability is:
    $$P_{\text{hist}}(S_T \ge K) = \frac{1}{B} \sum_{b=1}^{B} \mathbb{I}\left(S_T^{(b)} \ge K\right)$$

#### 2.1.2. Pseudocode Implementation
```python
import numpy as np

def calculate_historical_probability(spot: float, strike: float, days_to_expiry: int, 
                                     historical_returns: np.ndarray, num_simulations: int = 10000) -> float:
    # Validate input length
    if len(historical_returns) < 252:
        raise ValueError("Insufficient historical returns history.")
        
    # Bootstrap returns
    sampled_returns = np.random.choice(historical_returns, size=(num_simulations, days_to_expiry), replace=True)
    
    # Sum log returns per path
    cumulative_returns = np.sum(sampled_returns, axis=1)
    
    # Project terminal prices
    terminal_prices = spot * np.exp(cumulative_returns)
    
    # Calculate probability of closing above strike
    successes = np.sum(terminal_prices >= strike)
    return float(successes / num_simulations)
```

---

### 2.2. IV-Implied Probability (BSM Risk-Neutral Model)
Extracts the probability of closing In-The-Money (ITM) using the Black-Scholes-Merton risk-neutral framework.

#### 2.2.1. Formula
Under risk-neutral measures, the probability that a Call option expires In-The-Money ($S_T \ge K$) is given by the dual delta, which is equal to the Standard Normal cumulative distribution of $d_2$:
$$P_{\text{implied}}(S_T \ge K) = \Phi(d_2)$$
$$d_2 = \frac{\ln(S_0 / K) + (r - \sigma^2 / 2)T}{\sigma \sqrt{T}}$$
where:
*   $S_0$ = Current Spot Price
*   $K$ = Strike Price
*   $r$ = Risk-free interest rate (annualized)
*   $\sigma$ = Implied Volatility (annualized)
*   $T$ = Time to expiry (expressed in years: $T_{\text{days}} / 365$)
*   $\Phi(x)$ = Cumulative Distribution Function (CDF) of the standard normal distribution.

For a Put option, the probability of expiring ITM ($S_T \le K$) is:
$$P_{\text{implied}}(S_T \le K) = \Phi(-d_2) = 1 - \Phi(d_2)$$

#### 2.2.2. Worked Example
*   Spot Price $S_0 = 2500.00$
*   Strike Price $K = 2600.00$ (Out-Of-The-Money Call)
*   Time to Expiry $T_{\text{days}} = 30$ days ($T = 30 / 365 = 0.0822$ years)
*   Implied Volatility $\sigma = 24.0\% = 0.24$
*   Risk-free rate $r = 6.5\% = 0.065$

1.  Compute $d_2$:
    $$\ln(S_0 / K) = \ln(2500 / 2600) = \ln(0.9615) = -0.03922$$
    $$(r - \sigma^2 / 2)T = (0.065 - 0.24^2 / 2) \times 0.0822 = (0.065 - 0.0288) \times 0.0822 = 0.0362 \times 0.0822 = 0.00298$$
    $$\sigma \sqrt{T} = 0.24 \times \sqrt{0.0822} = 0.24 \times 0.2867 = 0.06881$$
    $$d_2 = \frac{-0.03922 + 0.00298}{0.06881} = \frac{-0.03624}{0.06881} = -0.5267$$
2.  Compute $\Phi(d_2)$:
    $$P(S_T \ge 2600) = \Phi(-0.5267) \approx 0.2992 = 29.92\%$$

#### 2.2.3. Edge Cases & Validation
*   **Time to Expiry $T \to 0$**: The term $\sigma \sqrt{T}$ approaches zero, causing division by zero.
    *   *Validation Rule*: If $T_{\text{days}} < 0.5$, bypass BSM and evaluate probability as $\mathbb{I}(S_0 \ge K)$ based on live spot ticks.

---

### 2.3. Touch Probability (First-Passage Time)
Calculates the probability that the spot price will cross a specific barrier strike price *at any moment* during the lifetime of the option contract.

#### 2.3.1. Formula (Geometric Brownian Motion First-Passage)
For an upper barrier $H > S_0$, the probability of the spot price touching $H$ before time $T$ is:
$$P_{\text{touch}}(H, T) = \Phi(y_1) + \left( \frac{H}{S_0} \right)^{2\mu / \sigma^2} \Phi(y_2)$$
where:
$$y_1 = \frac{-\ln(H / S_0) + \mu T}{\sigma \sqrt{T}}, \quad y_2 = \frac{-\ln(H / S_0) - \mu T}{\sigma \sqrt{T}}$$
and $\mu = r - \sigma^2 / 2$ is the risk-neutral drift.

In practice, if drift $\mu \approx 0$ (short time horizons), this simplifies to the reflection principle approximation:
$$P_{\text{touch}}(H, T) \approx 2 \cdot P_{\text{implied}}(S_T \ge H) = 2 \cdot \Phi(d_2)$$

#### 2.3.2. Worked Example
*   $S_0 = 2500.00$
*   Target Strike Call $K = 2600.00$
*   $P_{\text{implied}}(S_T \ge 2600) = 29.92\%$ (computed above)
*   $$P_{\text{touch}}(2600, 30 \text{ days}) \approx 2 \times 0.2992 = 59.84\%$$
*   *Interpretation*: While there is only a ~30% chance the stock closes above 2600 at expiry, there is a ~60% chance it touches 2600 during the 30-day period.

---

### 2.4. Range Probability
Calculates the probability that the spot price will expire strictly within the boundaries of a multi-leg short position (e.g. short strangle).

#### 2.4.1. Formula
$$P_{\text{range}}(K_1, K_2) = P(S_T \ge K_1 \text{ and } S_T \le K_2) = \Phi(d_2(K_2)) - \Phi(d_2(K_1))$$
where $K_1$ is the lower strike (Put) and $K_2$ is the upper strike (Call).

#### 2.4.2. Worked Example
*   $S_0 = 2500.00$
*   Put Strike $K_1 = 2300.00$, Call Strike $K_2 = 2700.00$
*   Using BSM $d_2$ calculations:
    *   $\Phi(d_2(2700)) = 0.1587$ (15.87% probability of closing above 2700)
    *   $\Phi(d_2(2300)) = 0.1151$ (11.51% probability of closing below 2300)
*   The probability of closing inside the range:
    $$P_{\text{range}}(2300, 2700) = (1 - 0.1587) - 0.1151 = 0.7262 = 72.62\%$$


---

# 09_Portfolio_Intelligence

# VYUH Engine - Portfolio Intelligence Specification

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead Quantitative Architect | Investment Committee | Initial Specification |

---

## 2. Liquidity Engine Scoring
The Liquidity Engine filters out illiquid underlyings and options contracts to avoid high slippage costs when entering or adjusting positions.

### 2.1. Liquidity Score ($LS$) Formula
For any option contract (strike $K$, type $CE/PE$), the Liquidity Score $LS$ (scaled 0 to 100) is calculated as:
$$LS = 0.40 \cdot S_{\text{spread}} + 0.30 \cdot S_{\text{oi}} + 0.30 \cdot S_{\text{vol}}$$

#### 2.1.1. Bid-Ask Spread Score ($S_{\text{spread}}$)
$$S_{\text{spread}} = \max\left(0, \, 100 \cdot \left[1 - \frac{\text{Ask} - \text{Bid}}{(\text{Ask} + \text{Bid})/2 \cdot 0.01}\right]\right)$$
This measures the spread as a percentage of the mid-price. A spread of 1% or higher results in $S_{\text{spread}} = 0$.

#### 2.1.2. Open Interest Score ($S_{\text{oi}}$)
$$S_{\text{oi}} = \min\left(100, \, 100 \cdot \frac{\text{OI}_{\text{contract}}}{\text{Threshold}_{\text{oi}}}\right)$$
where $\text{Threshold}_{\text{oi}}$ is set to **50,000 contracts** for liquid NSE options.

#### 2.1.3. Volume Score ($S_{\text{vol}}$)
$$S_{\text{vol}} = \min\left(100, \, 100 \cdot \frac{\text{Volume}_{\text{contract}}}{\text{Threshold}_{\text{vol}}}\right)$$
where $\text{Threshold}_{\text{vol}}$ is set to **10,000 contracts** daily volume.

#### 2.1.4. Worked Example
An option contract has:
*   Bid = 12.00, Ask = 12.20 (Mid-price = 12.10)
*   Open Interest = 35,000 contracts
*   Daily Volume = 8,000 contracts

1.  Calculate $S_{\text{spread}}$:
    $$\text{Spread Pct} = \frac{12.20 - 12.00}{12.10} = \frac{0.20}{12.10} \approx 0.01653 = 1.653\% \text{ of 1\%}$$
    $$S_{\text{spread}} = 100 \cdot (1 - 0.01653) = 98.35$$
2.  Calculate $S_{\text{oi}}$:
    $$S_{\text{oi}} = 100 \cdot \frac{35,000}{50,000} = 70.00$$
3.  Calculate $S_{\text{vol}}$:
    $$S_{\text{vol}} = 100 \cdot \frac{8,000}{10,000} = 80.00$$
4.  Calculate $LS$:
    $$LS = (0.40 \times 98.35) + (0.30 \times 70.00) + (0.30 \times 80.00) = 39.34 + 21.00 + 24.00 = 84.34$$
    *   *Decision*: $LS \ge 40$. The contract is classified as **Liquid** and is eligible for trade generation.

---

## 3. Stock Selection Scoring Model
The system ranks the ~200 NSE F&O stocks to select the optimal 70–80 candidates.

### 3.1. Overall Stock Score ($OSS$) Formula
$$OSS = 0.35 \cdot IVR + 0.25 \cdot IVP + 0.25 \cdot PR + 0.15 \cdot LS_{\text{stock}} - 0.20 \cdot \text{Haz}$$
where:
*   $IVR$ = IV Rank (0 to 100)
*   $IVP$ = IV Percentile (0 to 100)
*   $PR$ = Premium Richness normalized: $\min(100, \, 50 \cdot PR)$
*   $LS_{\text{stock}}$ = Liquidity Score of the ATM option chain
*   $\text{Haz}$ = Hazard Score (0 to 100), derived from directional trend strength. If the stock exhibits strong directional momentum (e.g. ADX > 40 with a Supertrend breakout), the Hazard Score increases, penalizing range-bound options selling.

---

## 4. Underlyings Filtering Pipeline
Before scoring, the universe is processed through five binary filters:

```
[200 NSE F&O Stocks]
         â”‚
         â–¼
  1. MWPL Filter â”€â”€â”€â”€â”€â”€â”€â”€â–º Reject if MWPL utilization > 80%
         â”‚
         â–¼
  2. Earnings Filter â”€â”€â”€â”€â–º Reject if Earnings Date is within Expiry Week
         â”‚
         â–¼
  3. Price Filter â”€â”€â”€â”€â”€â”€â”€â–º Reject if Spot Price < â‚¹150 (avoids low-value penny stocks)
         â”‚
         â–¼
  4. Spread Filter â”€â”€â”€â”€â”€â”€â–º Reject if ATM bid-ask spread > 2.0%
         â”‚
         â–¼
[Candidate Basket: Top 70-80 Ranked Stocks Selected]
```

---

## 5. Strike Selection Intelligence
Once the strategy (e.g., Short Strangle) is decided for a stock, the Strike Intelligence Engine selects the specific contract legs.

```
       STRIKE SELECTION ALGORITHM (SHORT STRANGLE)
                     
       [Support / S2]               [Spot]               [Resistance / R2]
             â”‚                        â”‚                        â”‚
  â”€â”€â”€xâ”€â”€â”€â”€â”€â”€â”€xâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€xâ”€â”€â”€â”€â”€â”€â”€â”€xâ”€â”€â”€â”€â”€â”€â”€â”€xâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€xâ”€â”€â”€â”€â”€â”€â”€xâ”€â”€â”€
     â”‚                       â”‚                 â”‚                       â”‚
  [Put Strike: K_P]     [Put Delta]       [Call Delta]            [Call Strike: K_C]
  (Delta: 0.15 - 0.18)   (~ 0.16)          (~ 0.16)               (Delta: 0.15 - 0.18)
  (K_P <= S2)                                                     (K_C >= R2)
```

1.  **Delta Target**: The baseline target is a **0.16 Delta** (1-Standard-Deviation limit) for both Call and Put legs.
2.  **Expected Move Overlay**: The strike must lie outside the 30-day Expected Move boundaries:
    $$K_{\text{Put}} \le S_0 - EM, \quad K_{\text{Call}} \ge S_0 + EM$$
3.  **Support/Resistance Check**:
    *   $K_{\text{Put}}$ must be equal to or lower than the major Support Level 2 ($S_2$) computed by Suchak.
    *   $K_{\text{Call}}$ must be equal to or higher than the major Resistance Level 2 ($R_2$) computed by Suchak.
4.  **Skew-Adjustment Rule**: If Put IV is significantly higher than Call IV (skewness $SK > 1.2$), the Put strike is adjusted downward (to 0.12 delta) while the Call strike is adjusted upward (to 0.18 delta) to balance premium credits and touch probabilities.


---

# 10_Strategy_Engine

# VYUH Engine - Strategy Selection Engine

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead Derivatives Strategist | Chief Investment Officer | Initial Specification |

---

## 2. Core Option-Selling Strategies
VYUH specializes in five net-short option strategies. These are selected based on the underlying stock's volatility regime and trend.

### Strategy Selection Matrix

| Direction / Trend | High Volatility ($IVR > 70$) | Low Volatility ($IVR < 30$) |
| :--- | :--- | :--- |
| **Neutral Trend** | Short Strangle / Short Straddle | Iron Condor / Iron Fly |
| **Bullish Direction** | Put-Skew Strangle<br>(Sell 0.20D PE, Sell 0.10D CE) | Bull Put Spread / Ratio Spread<br>(1x Long, 2x Short) |
| **Bearish Direction** | Call-Skew Strangle<br>(Sell 0.10D PE, Sell 0.20D CE) | Bear Call Spread / Ratio Spread<br>(1x Long, 2x Short) |


### 2.1. Short Strangle
*   **Structure**: Sell 1 OTM Call + Sell 1 OTM Put.
*   **Target Delta**: 0.15 to 0.18 on entry.
*   **Profit Target**: Exit at **50% of maximum credit collected**.
*   **Max Loss Limit**: Exit if losses reach **200% of maximum credit collected** (equivalent to 3x credit).
*   **Adjustment Trigger**: If one side is tested (delta exceeds 0.30), roll the opposite side (untested) closer to spot (to delta 0.20) to collect more premium.

### 2.2. Short Straddle
*   **Structure**: Sell 1 ATM Call + Sell 1 ATM Put.
*   **Target Delta**: Close to 0.00 net delta (ATM Call Delta $\approx$ 0.50, ATM Put Delta $\approx$ -0.50).
*   **Profit Target**: Exit at **25% of maximum credit**.
*   **Max Loss Limit**: Exit if losses reach **100% of maximum credit**.
*   **Usage**: Deployed only in environments of extremely high IV Rank ($IVR > 85$) where high volatility contraction is anticipated.

### 2.3. Iron Condor (Defined Risk Strangle)
*   **Structure**: Sell OTM Put + Buy further OTM Put (Put Spread) AND Sell OTM Call + Buy further OTM Call (Call Spread).
*   **Target Delta**: Short strikes at 0.16 Delta, Long strikes at 0.08 Delta (wing width based on 0.50 ATR).
*   **Profit Target**: Exit at **50% of maximum credit**.
*   **Max Loss Limit**: Defined by the width of the wings minus credit collected.
*   **Usage**: Deployed in low-to-medium IV Rank ($30 < IVR < 60$) to protect capital against sudden market gaps.

### 2.4. Iron Fly (Defined Risk Straddle)
*   **Structure**: Sell ATM Call + Sell ATM Put AND Buy OTM Call + Buy OTM Put.
*   **Profit Target**: Exit at **30% of maximum credit**.
*   **Usage**: Deployed during low volatility, range-bound consolidation periods.

### 2.5. Custom Call/Put Ratio Spreads
*   **Structure**: Buy 1 ITM/ATM Option + Sell 2 OTM Options (Net Credit).
*   **Target Delta**: Managed to be net directional-positive or directional-negative.
*   **Usage**: Deployed only when the AI (VIB) and Suchak indicate a strong structural trend (ADX > 35) with low implied volatility.

---

## 3. Detailed Worked Strangle and Iron Condor Structuring
Assume Spot Price $S_0 = 2500.00$, Expiry = 30 Days, 30-Day ATR = 120.00.

### 3.1. Short Strangle Setup
*   **Put Leg**: Strike $K_{\text{Put}} = 2300.00$ (0.16 Delta). Premium = â‚¹25.00.
*   **Call Leg**: Strike $K_{\text{Call}} = 2700.00$ (0.16 Delta). Premium = â‚¹28.00.
*   **Lot Size**: 250 contracts.
*   **Maximum Credit**:
    $$\text{Max Credit} = (25.00 + 28.00) \times 250 = â‚¹13,250.00$$
*   **Profit Target (50%)**: Exit if joint premium falls to â‚¹26.50 (Profit = â‚¹6,625.00).
*   **Stop-Loss (200% Loss)**: Exit if joint premium rises to $53.00 \times 3.0 = â‚¹159.00$ (Loss = â‚¹26,500.00).
*   **Margin Required**: Approximately â‚¹180,000.00 (under NSE SPAN + Exposure rules).

### 3.2. Iron Condor Setup
*   **Short Put**: Strike $K_{\text{Put\_Short}} = 2300.00$. Premium = â‚¹25.00.
*   **Long Put (Hedge)**: Strike $K_{\text{Put\_Long}} = 2240.00$ (width = 60). Premium = â‚¹12.00.
*   **Short Call**: Strike $K_{\text{Call\_Short}} = 2700.00$. Premium = â‚¹28.00.
*   **Long Call (Hedge)**: Strike $K_{\text{Call\_Long}} = 2760.00$ (width = 60). Premium = â‚¹14.00.
*   **Maximum Credit**:
    $$\text{Max Credit} = \left((25.00 - 12.00) + (28.00 - 14.00)\right) \times 250 = (13.00 + 14.00) \times 250 = â‚¹6,750.00$$
*   **Max Risk**:
    $$\text{Max Risk} = (\text{Spread Width} - \text{Credit}) \times \text{Lot Size} = (60.00 - 27.00) \times 250 = 33.00 \times 250 = â‚¹8,250.00$$
*   **Margin Required**: Approximately â‚¹45,000.00 (under NSE SPAN rule for defined-risk positions).
*   **Theta Efficiency (TE)**: Compared to the Strangle, the Iron Condor consumes less margin, making it capital-efficient when IV contraction is high.


---

# 11_Risk_Engine

# VYUH Engine - Portfolio Risk Engine

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead Risk Architect | Chief Risk Officer | Initial Specification |

---

## 2. Quantitative Risk Metrics

### 2.1. Value at Risk (VaR) - Historical Simulation
Estimates the maximum dollar loss the portfolio might suffer over a 1-day holding period at a 99% confidence level.

#### 2.1.1. Formula
Let $V_0$ be the current portfolio value.
Let $R_k = \{r_{1,k}, r_{2,k}, \dots, r_{M,k}\}$ be the historical daily log returns of asset $k$ over $M = 252$ days.
For each daily historical scenario $j \in \{1, \dots, M\}$:
1.  Compute the simulated price change for each position $i$:
    $$\Delta S_{i,j} = S_{i,0} \cdot \left(\exp(r_{i,j}) - 1\right)$$
2.  Compute the option valuation change using Taylor series Greek approximation:
    $$\Delta P_{i,j} \approx \Delta_i \cdot \Delta S_{i,j} + \frac{1}{2} \Gamma_i \cdot (\Delta S_{i,j})^2 + \nu_i \cdot \Delta \sigma_{i,j} + \theta_i \cdot \Delta t$$
    where $\Delta_i, \Gamma_i, \nu_i, \theta_i$ are the position's Delta, Gamma, Vega, and Theta.
3.  Compute the total portfolio PnL scenario value:
    $$\Delta \Pi_j = \sum_{i=1}^{L} \text{Qty}_i \cdot \Delta P_{i,j}$$
4.  Sort $\{\Delta \Pi_1, \dots, \Delta \Pi_M\}$ in ascending order. The 1-day 99% VaR is the 1st percentile of the sorted distribution:
    $$\text{VaR}_{99\%} = -\text{Quantile}\left(\{\Delta \Pi\}, \, 0.01\right)$$

---

### 2.2. Stress Testing and Scenario Analysis
The system shocks the market parameters to test the portfolio's resiliency against tail risk events.

#### 2.2.1. Shock Matrix

| Scenario Name | Spot Shock ($\Delta S$) | IV Shock ($\Delta \sigma$) | System Action |
| :--- | :--- | :--- | :--- |
| **Black Monday** | -15.0% | +100.0% | Automatic halt, liquidate near-tested short options, purchase ATM protection |
| **Bull Rally** | +8.0% | -20.0% | Rebalance untested put sides upward, exit tested calls |
| **Flash Crash** | -5.0% | +50.0% | Exit short naked options, convert strangle positions to Iron Condors |
| **Vol Crush** | 0.0% | -30.0% | Hold positions, capture maximized theta decay yield |

#### 2.2.2. Worked Stress Example
*   Portfolio contains 10 lots of Short Strangle on RELIANCE (Lot = 250, Qty = 2500 short options).
*   Delta = 0.15, Gamma = -0.0005, Vega = -120.00, Theta = 45.00. Spot $S_0 = 2500.00$.
*   Scenario: **Flash Crash** (-5% Spot, +50% IV spike).
    *   $\Delta S = 2500.00 \times (-0.05) = -125.00$
    *   $\Delta \sigma = +50\%$
    *   Greek PnL change per option:
        $$\Delta P \approx \text{Delta} \cdot \Delta S + \frac{1}{2} \text{Gamma} \cdot (\Delta S)^2 + \text{Vega} \cdot \Delta \sigma$$
        $$\Delta P \approx (0.15 \times -125.00) + \left(0.5 \times -0.0005 \times (-125.00)^2\right) + (-120.00 \times 0.50)$$
        $$\Delta P \approx -18.75 + ( -0.00025 \times 15625 ) - 60.00 = -18.75 - 3.906 - 60.00 = -82.656$$
    *   Portfolio PnL for RELIANCE position (remember options are short: multiply by -2500):
        $$\text{PnL} = -2500 \times -82.656 = +â‚¹206,640.00$$
        *(Wait, if spot crashes, the Put delta is actually negative, e.g. -0.15. Let's recalculate the short Put position where delta is -0.15 and option price rises. The option price rises, causing losses for a short option seller. Let's make sure our math reflects that option price increases when spot falls).*
        For a Short Put: Delta is negative (e.g. -0.15). If Spot falls ($\Delta S = -125$), Delta contribution = $-0.15 \times -125 = +18.75$ (option price increases by 18.75). Vega contribution (+50% IV) = $+120.00 \times 0.50 = +60.00$ (option price increases by 60). Option price increase = $+18.75 + 60.00 = 78.75$. Since we are short 2500 options, PnL = $-2500 \times 78.75 = -â‚¹196,875.00$.

---

### 2.3. SPAN Margin Estimation Model
Emulates the National Stock Exchange (NSE) SPAN margin framework.

#### 2.3.1. Formula
$$\text{Total Margin} = \text{SPAN Margin} + \text{Exposure Margin}$$
*   **SPAN Margin**: Estimated by computing the maximum loss of the options contract across 16 pricing scenarios defined by the exchange (combinations of Spot price $\pm 3\sigma$ shifts and IV shifts).
*   **Exposure Margin**: Typically a flat charge of **2.0% to 3.5%** of the gross notional value of the underlying contract.
    $$\text{Notional Value} = S_0 \times \text{Lot Size} \times \text{Num Lots}$$

---

### 2.4. Diversification Score ($DS$)
Measures portfolio concentration risk.

#### 2.4.1. Formula (HHI-Based)
The Herfindahl-Hirschman Index ($HHI$) for capital allocation is:
$$HHI = \sum_{i=1}^{P} w_i^2$$
where $w_i$ is the margin allocation weight of stock underlying $i$ in the portfolio, and $P$ is the number of active stocks (70 to 80).
The Diversification Score is normalized:
$$DS = 100 \cdot (1 - HHI)$$

#### 2.4.2. Worked Example
*   Assume a â‚¹25 Crore portfolio where capital is allocated equally across 80 stocks.
*   Weight per stock $w_i = 1 / 80 = 0.0125$.
*   Calculate $HHI$:
    $$HHI = \sum_{i=1}^{80} (0.0125)^2 = 80 \times 0.00015625 = 0.0125$$
*   Calculate $DS$:
    $$DS = 100 \cdot (1 - 0.0125) = 98.75$$
    *   *Interpretation*: Near 100, indicating highly diversified capital distribution. If the capital was concentrated in just 5 stocks ($w_i = 0.20$), then $HHI = 5 \times 0.04 = 0.20$, and $DS = 100 \cdot (1 - 0.20) = 80.00$ (violating risk limits).
*   **Regulatory Constraint**: Rejects portfolio configurations with $DS < 95.00$.


---

# 12_AI_Intelligence_Brain

# VYUH Engine - AI Intelligence Brain (VIB)

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead AI Research Engineer | Chief Investment Officer | Initial Specification |

---

## 2. VIB Subsystem Architecture
The **VIB (VYUH Intelligence Brain)** is a localized, offline AI subsystem. It operates as an asynchronous intelligence overlay on the core trading logic. **Critically, VIB never executes changes directly on production trading algorithms or execution engines.** It proposes parameter adjustments, strategy overrides, and regime assessments that must pass a human approval workflow.

```mermaid
graph TD
    subgraph Core Data Sources
        News[News Feeds & Earnings Reports] --> Embed[Embedding Model: BGE-M3]
        Ann[Corporate Announcements] --> Embed
    end
    
    subgraph Vector DB & Knowledge Graph
        Embed --> VectorStore[(Qdrant / pgvector)]
        KG[Knowledge Graph: Stock-Sector-Macro] --> RAG[RAG Orchestrator]
        VectorStore --> RAG
    end
    
    subgraph Local LLM & Reasoning
        RAG --> LLM[Local LLM: Llama-3-8B-Instruct]
        LLM --> Decision[Reasoned Proposal Generation]
    end
    
    subgraph Human Approval Gate
        Decision --> Dashboard[UI Approval Dashboard]
        Dashboard -->|Approve| Config[Update System Configuration]
        Dashboard -->|Reject| Archive[Log & Discard]
    end
    
    style LLM fill:#f96,stroke:#333,stroke-width:2px
    style Dashboard fill:#9f9,stroke:#333,stroke-width:3px
```

---

## 3. Core Modules of VIB

### 3.1. Stock Learning Module
*   **Purpose**: Analyzes the specific behavior of individual underlyings around corporate events (earnings, dividends, board meetings).
*   **Process**: Evaluates post-earnings volatility crush speeds. If a stock historically crushes 80% of its IV within the first 30 minutes of market open post-earnings, VIB learns this pattern and flags the stock for a "Post-Earnings Volatility Crush Straddle" entry.

### 3.2. Strategy Learning Module
*   **Purpose**: Conducts continuous evaluation of active option strategies against different market regimes to find performance decay.
*   **Process**: Analyzes slippage patterns and adjustments. If Short Strangles on high-beta underlyings systematically experience early test alerts that result in adjustment losses, VIB proposes widening the target entry delta from 0.16 to 0.12.

### 3.3. Sector Learning Module
*   **Purpose**: Identifies shifts in sector relative strength and correlations.
*   **Process**: If the rolling 10-day pairwise correlation between Banking stocks rises from 0.30 to 0.65, VIB flags this sector consolidation and proposes tightening the combined sector margin cap from 15% to 10% to prevent sector-wide concentration.

### 3.4. Market Regime Learning Module
*   **Purpose**: Classifies macroeconomic state shifts using non-price data (e.g. FII/DII net flows, USDINR volatility, crude oil movements).
*   **Process**: Uses unsupervised clustering (e.g. GMM) to determine whether the broader market has transitioned into a high-risk regime, alerting the Portfolio Builder to increase the cash buffer from 30% to 40%.

### 3.5. Failure Analysis Module
*   **Purpose**: Performs post-mortem root-cause analysis on trades that hit their stop-loss limits.
*   **Process**: When a position is stopped out, VIB queries Ganesh (historical returns) and Suchak (indicators) for the 48-hour window surrounding the trade. It generates an event timeline and determines if the failure was due to an unhedged earnings announcement, abnormal order flow, or execution slippage.

---

## 4. Local LLM and Vector Database Specifications
*   **Local LLM**: Llama-3-8B-Instruct running locally via vLLM engine, quantized to 4-bit (AWQ) for sub-100ms token generation latency.
*   **Embedding Database**: `Qdrant` or PostgreSQL `pgvector` index storing 1024-dimensional dense vectors generated by the `bge-large-en-v1.5` model.
*   **RAG Ingestion Pipeline**: Ingests SEBI filings, NSE corporate actions feeds, and earnings transcripts. Document chunks are tagged with stock tickers, sectors, and event dates.

---

## 5. VIB Prompt Library

### 5.1. Earnings Announcement Risk Profiling Prompt
```text
SYSTEM: You are the Chief Quantitative Analyst at an institutional option fund. Analyze corporate announcements to identify volatility risks.
USER: Evaluate this corporate announcement for RELIANCE:
[Insert Text: Reliance Industries Board meeting scheduled on 2026-07-20 to consider quarterly financial results and bonus shares payout.]

Identify:
1. Volatility Risk Level (Low/Medium/High/Extreme).
2. Expected Volatility Crush Profile based on historical events.
3. Recommended action for short options positions during the event week.

Output JSON format:
{
  "stock": "RELIANCE",
  "risk_level": "HIGH",
  "reasoning": "Board considering bonus shares in addition to results increases retail interest and speculative option volumes.",
  "vol_crush_speed_hours": 2,
  "action_recommendation": "EXIT_SHORTS_BEFORE_CLOSE_FRIDAY"
}
```

---

## 6. Human-in-the-Loop (HITL) Workflow
VIB outputs recommendation packets as JSON structures written to the PostgreSQL database.
1.  Recommendations appear on the **AI Research Lab** dashboard.
2.  The Portfolio Manager views the prompt chain, reasoning path, source documents retrieved by RAG, and proposed parameters (e.g. "Reduce SBIN Max Margin from 2.0% to 1.5%").
3.  The PM approves, rejects, or edits the proposed parameter changes.
4.  Only upon PM approval is the system configuration updated in the database and applied in the next 10-second loop.


---

# 13_UI_Design

# VYUH Engine - User Interface (UI) Specification

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead UI/UX Designer | Product Owner | Initial Specification |

---

## 2. Command Center Dashboard
The **Command Center** is the primary cockpit for the Portfolio Manager. It provides a real-time, consolidated view of portfolio performance, greeks, and the execution packet staging area.

### 2.1. Screen Layout
```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚  VYUH Engine  [Live Connect]  Market Hours: 09:15 - 15:30 IST                          â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â” â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â” â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â” â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â” â”‚
â”‚ â”‚ Portfolio Net PnL    â”‚ â”‚ Margin Deployed      â”‚ â”‚ Portfolio Theta      â”‚ â”‚ Risk    â”‚ â”‚
â”‚ â”‚ â‚¹14,20,500.00 (+0.5%)â”‚ â”‚ â‚¹15.00 Cr / â‚¹25.00 Crâ”‚ â”‚ +â‚¹4,52,100.00 / day  â”‚ â”‚ VaR 99% â”‚ â”‚
â”‚ â”‚ [â–² +â‚¹42,000 / 10s]   â”‚ â”‚ [60.0% Deployed]     â”‚ â”‚ [TE: 110.2%]         â”‚ â”‚ 2.45%   â”‚ â”‚
â”‚ â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜ â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜ â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜ â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜ â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â” â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â” â”‚
â”‚ â”‚ PORTFOLIO GREEKS ACTIVE MONITOR              â”‚ â”‚ EXECUTION PACKET STAGING (Kafka)  â”‚ â”‚
â”‚ â”‚                                              â”‚ â”‚ ID: d0e12d1b-7a74                â”‚ â”‚
â”‚ â”‚ Delta:  +12.45 [Neutral]                     â”‚ â”‚ Proposed Trades: 4                â”‚ â”‚
â”‚ â”‚ Gamma:  -0.0042 [Short Volatility]           â”‚ â”‚ Total Margin: â‚¹34,50,000.00       â”‚ â”‚
â”‚ â”‚ Vega:   -1,42,100 [Short Vega]               â”‚ â”‚                                   â”‚ â”‚
â”‚ â”‚                                              â”‚ â”‚ [Review Pending Trades Grid]      â”‚ â”‚
â”‚ â”‚ [TradingView Portfolio PnL Chart]            â”‚ â”‚ [Reject]   [APPROVE & SIGN]       â”‚ â”‚
â”‚ â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜ â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜ â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â” â”‚
â”‚ â”‚ ACTIVE REBALANCING POSITIONS (AG GRID ENTERPRISE)                                  â”‚ â”‚
â”‚ â”‚ Underlying | Strategy | Net Delta | Margin Consumed | Unr PnL | Actions            â”‚ â”‚
â”‚ â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤ â”‚
â”‚ â”‚ RELIANCE   â”‚ Strangle â”‚ -0.02     â”‚ â‚¹18,00,000.00   â”‚ +â‚¹42k   â”‚ [Adjust] [Close]   â”‚ â”‚
â”‚ â”‚ SBIN       â”‚ Iron Condâ”‚ +0.08     â”‚ â‚¹4,50,000.00    â”‚ -â‚¹12k   â”‚ [Roll]   [Close]   â”‚ â”‚
â”‚ â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜ â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

---

## 3. Market Universe Grid (AG Grid Enterprise)
The **Market Universe** screen displays the ~200 NSE F&O underlyings ranked by the scoring model.

### 3.1. Components
*   **Filters Panel**: Quick toggles for Sector (BFSI, IT, Autos, Energy), Volatility State (High IVR, Low IVR), and Status (Active, Banned, Near Earnings).
*   **Data Grid Column Definitions**:
    *   `Rank` (Pinned Left, Ascending)
    *   `Stock Ticker` (Link to Option Chain Analysis)
    *   `Spot Price` (Updates via live Lakshmi feed ticks)
    *   `IV Rank` (Formatted cell renderer with progress bar)
    *   `IV Percentile` (Progress bar)
    *   `Premium Richness` (Colored chips: Red > 1.3, Grey < 1.0)
    *   `Liquidity Score` (Numeric)
    *   `Selected Strategy` (Chip renderer: `SHORT STRANGLE`, `IRON CONDOR`)
    *   `Expected Yield %`

---

## 4. Strike Analysis Center
An interactive screen mapping the option chain smile, expected moves, and selected strikes.

```mermaid
graph TD
    subgraph Option Chain Panel
        C[Calls Leg Table] --- Strike[ATM Strike List] --- P[Puts Leg Table]
    end
    
    subgraph Charts Engine
        TV[TradingView Price Chart with S2/R2 Overlays]
        PDF[IV-implied PDF Probability Bell Curve Chart]
    end
    
    Strike -->|Highlight Selected Legs| TV
    Strike -->|Compute expected ranges| PDF
```

---

## 5. AI Research Lab Dashboard (VIB)
Interfaces with the local LLM and Vector Database RAG pipeline.

### 5.1. Features
1.  **Macro & News Feed Ingestion View**: Displays real-time news headlines processed by VIB, color-coded by volatility sentiment impact.
2.  **RAG Context Console**: Users can input natural language queries like *"What is the historical behavior of RELIANCE when crude prices surge past $85?"* VIB responds with citations linking to parsed PDFs, research notes, and historical return correlations.
3.  **Knowledge Graph Widget**: A visual force-directed node diagram showing connections (e.g., RELIANCE -> Energy Sector -> Crude Oil -> USDINR -> Nifty Index).
4.  **AI Parameter Proposals**:
    *   Lists parameter change requests (e.g. *"Reduce sector weight cap for Banking from 15% to 12% due to HDFC Bank earnings surprise potential"*).
    *   Includes a side-by-side comparison of the current parameter vs. proposed parameter, a confidence score, and a button to "Commit Change to Engine".


---

# 14_Testing

# VYUH Engine - Testing and Verification Specification

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | QA Lead Architect | Chief Software Architect | Initial Specification |

---

## 2. Test Architecture Overview
VYUH enforces a multi-tier testing harness to guarantee mathematical accuracy, sub-second latency performance, and capital security.

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚              TEST HARNESS LAYERS (CI/CD)               â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¬â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
                           â”‚
    â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
    â–¼                      â–¼                      â–¼
[Unit Testing]     [Calculation Validation] [Historical Replay]
Math functions,    Cross-validate BSM /     Replay 5 years of
Greeks, Indicators. KDE against Python.      market ticks.
    â”‚                      â”‚                      â”‚
    â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
                           â”‚
                           â–¼
               [Performance & Stress Testing]
                 10x tick load, -15% Spot shock.
                           â”‚
                           â–¼
               [Paper Trading Verification]
                 Realtime paper trading with Vega.
```

---

## 3. Calculation and Model Validation
All quantitative calculations computed in C# (.NET 9) must be cross-verified against established Python analytical libraries (`SciPy` / `pandas-ta`).

### 3.1. Mathematical Verification Rules
*   **IV Probability Check**: $\Phi(d_2)$ values generated in .NET must match `scipy.stats.norm.cdf(d2)` up to **6 decimal places**.
*   **ATR Check**: Wilder's smoothing output must match `pandas-ta.atr` output over a 252-day return series with a tolerance limit $\epsilon \le 10^{-7}$.
*   **First-Passage Touch Probability Check**: Touch probabilities must be validated against a 100,000-path Monte Carlo simulation. Analytical GBM touch values must deviate by no more than **$\pm 0.5\%$** from simulation statistics.

---

## 4. Historical Replay and Backtesting
The testing harness contains a **Replay Engine** that simulates historical market environments.

### 4.1. Setup and Methodology
1.  **Data Extraction**: Ganesh extracts tick-by-tick option chains for a selected historical period (e.g., March 2020 market crash).
2.  **Clock Emulation**: A mock clock replaces the system wall clock, pushing option chain snapshots to the Ingestion Service at a 10-second tick resolution.
3.  **Position Tracking**: A mock Vega execution client computes daily SPAN margins, records simulated fills, charges slippage (assumed at **1.0% of premium price**), and tracks equity curves.
4.  **Assertion**: The run must verify that the portfolio engine successfully triggered stop-loss and roll adjustments when deltas breached 0.35, and that drawdown limits were never violated without alerting the risk manager.

---

## 5. Performance and Load Testing
VYUH must process the 200-stock universe within a 1,800ms boundary.

### 5.1. Performance Benchmarks
*   **Throughput**: Ingest and process **200 options chains (each containing 41 strikes)** every 10 seconds (total 8,200 option rows).
*   **Latency Budget**:
    *   Ingestion & Greeks Normalization: **< 400ms**
    *   Probability Model Computations: **< 500ms**
    *   Portfolio Optimization (Knapsack Solver): **< 400ms**
    *   Risk & Stress Engine Simulation: **< 300ms**
    *   Total Pipeline Time: **< 1,600ms** (giving 200ms safety buffer).

### 5.2. Stress Test Scenario (10x Load)
Inject 2,000 simulated stock tickers with 82,000 options contracts. The Ingestion Service must scale out (Kubernetes replica scale) to process the load, and the total execution time must not exceed **3,000ms** under maximum load conditions.

---

## 6. Sprint Exit and Code Freeze Criteria
For any development sprint to be declared complete, it must satisfy the following exit gates:

### 6.1. Developer Checklist
*   [ ] Unit test coverage exceeds **90%** for all application and domain layer components.
*   [ ] No compilation warnings or style violations in C# codebase.
*   [ ] All gRPC schemas are compiled and backward-compatible.
*   [ ] Code reviewed by at least two Senior Quant/Software Architects.

### 6.2. QA Validation Checklist
*   [ ] 100% of mathematical models validated against python references.
*   [ ] 24-hour continuous historical replay test completed with zero service failures.
*   [ ] Performance latency tests show 99th percentile pipeline execution time $< 1,800$ milliseconds.
*   [ ] Security penetration tests confirm zero endpoints accept unauthenticated JWT or HMAC requests.
*   [ ] No open High or Critical severity bugs in the tracker.


---

# 15_Deployment

# VYUH Engine - Deployment & Observability Specification

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead DevOps Engineer | Chief Software Architect | Initial Specification |

---

## 2. Infrastructure Topology
The VYUH Engine is deployed inside an on-premise, ultra-low-latency high-performance **Kubernetes (K8s) Cluster** located in close proximity to the exchange data centers to ensure minimal transit network latency.

```mermaid
graph TD
    subgraph Kubernetes Cluster
        Ingress[NGINX Ingress Controller] --> GW[vyuh-gateway-service]
        GW --> IngestSvc[vyuh-ingestion-service]
        GW --> OptSvc[vyuh-optimizer-service]
        GW --> RiskSvc[vyuh-risk-service]
        
        IngestSvc --> Redis[(Redis Cluster)]
        IngestSvc --> Kafka{Kafka Cluster}
        OptSvc --> Redis
        RiskSvc --> PG[(PostgreSQL State)]
    end
    
    subgraph Observability Stack
        Prom[Prometheus] --> Grafana[Grafana Dashboard]
        OTel[OpenTelemetry Collector] --> Jaeger[Jaeger Tracing]
    end
    
    IngestSvc -->|Metrics| Prom
    OptSvc -->|Metrics| Prom
    RiskSvc -->|Metrics| Prom
    
    IngestSvc -->|Spans| OTel
    OptSvc -->|Spans| OTel
```

---

## 3. Containerization (Dockerfile)
Below is the optimized multi-stage `Dockerfile` used for building and packaging the .NET 9 microservices.

```dockerfile
# Stage 1: Build Image
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /source

# Copy dependency project structures first
COPY src/Services/IngestionScoringService/VYUH.Ingestion.Api/*.csproj ./src/Services/IngestionScoringService/VYUH.Ingestion.Api/
COPY src/Services/IngestionScoringService/VYUH.Ingestion.Application/*.csproj ./src/Services/IngestionScoringService/VYUH.Ingestion.Application/
COPY src/Services/IngestionScoringService/VYUH.Ingestion.Domain/*.csproj ./src/Services/IngestionScoringService/VYUH.Ingestion.Domain/
COPY src/Services/IngestionScoringService/VYUH.Ingestion.Infrastructure/*.csproj ./src/Services/IngestionScoringService/VYUH.Ingestion.Infrastructure/
COPY src/Shared/VYUH.Shared.Contracts/*.csproj ./src/Shared/VYUH.Shared.Contracts/

RUN dotnet restore ./src/Services/IngestionScoringService/VYUH.Ingestion.Api/VYUH.Ingestion.Api.csproj --runtime linux-musl-x64

# Copy entire source and compile
COPY . .
WORKDIR /source/src/Services/IngestionScoringService/VYUH.Ingestion.Api
RUN dotnet publish -c Release -o /app --no-restore --self-contained true -r linux-musl-x64 /p:PublishTrimmed=true /p:PublishReadyToRun=true

# Stage 2: Runtime Image
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS final
WORKDIR /app
COPY --from=build /app .

# Run with non-root security privileges
USER 1000
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["./VYUH.Ingestion.Api"]
```

---

## 4. Kubernetes Deployment Manifest
Example manifest for the **Ingestion & Scoring Service**.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: vyuh-ingestion-deployment
  namespace: vyuh-trading
  labels:
    app: vyuh-ingestion
spec:
  replicas: 4
  selector:
    matchLabels:
      app: vyuh-ingestion
  template:
    metadata:
      labels:
        app: vyuh-ingestion
    spec:
      containers:
      - name: ingestion-api
        image: registry.vyuh.internal/vyuh-ingestion:1.0.0
        ports:
        - containerPort: 5000
          name: grpc-port
        resources:
          limits:
            cpu: "4000m"
            memory: 8Gi
          requests:
            cpu: "2000m"
            memory: 4Gi
        env:
        - name: ConnectionStrings__PostgreSQL
          valueFrom:
            secretKeyRef:
              name: vyuh-db-secrets
              key: pg-connection
        - name: Redis__Host
          value: "vyuh-redis-cluster.vyuh-trading.svc.cluster.local:6379"
        - name: Kafka__BootstrapServers
          value: "vyuh-kafka-cluster-kafka-bootstrap.vyuh-trading.svc.cluster.local:9092"
        livenessProbe:
          httpGet:
            path: /health/liveness
            port: 5000
          initialDelaySeconds: 15
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/readiness
            port: 5000
          initialDelaySeconds: 10
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: vyuh-ingestion-service
  namespace: vyuh-trading
spec:
  type: ClusterIP
  selector:
    app: vyuh-ingestion
  ports:
  - name: grpc
    port: 80
    targetPort: grpc-port
```

---

## 5. Observability and Monitoring Configuration

### 5.1. OpenTelemetry Instrumentation (.NET 9 Startup Hook)
Microservices register OpenTelemetry tracers and metrics collectors during application startup:

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("VYUH.IngestionService")
        .AddAspNetCoreInstrumentation()
        .AddGrpcClientInstrumentation()
        .AddRedisInstrumentation()
        .AddOtlpExporter(opt => opt.Endpoint = new Uri(builder.Configuration["Otel:CollectorUri"])))
    .WithMetrics(metrics => metrics
        .AddMeter("VYUH.IngestionService.Meters")
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter());
```

### 5.2. Grafana Dashboard Panels
The dashboard provides live metrics to monitor system performance during market hours:
1.  **Pipeline Evaluation Latency**: Gauge and time-series line chart tracking execution latency. Alert triggers if execution exceeds **1,800ms**.
2.  **Kafka Queue Depth**: Monitors lag on the `raw.options.chain.tick` topic. Lag > 10 ticks triggers a warning.
3.  **Active Margin Exposure**: Visualizes â‚¹15 Crore deployment vs. available cash buffers.
4.  **SignalR Active Connections**: Tracks live PM dashboard connections.
5.  **Memory Leak Dashboard**: Tracks garbage collection (GC) stats inside K8s .NET containers.


---

# 16_Sprint_Handbook

# VYUH Engine - The 25 Stage-Gate Sprint Handbook

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead Product Owner | Chief Software Architect | Initial Specification |

---

## 2. Phase 1: Ingestion & Core Foundation (Sprints 1–5)

### Sprint 1: Project Setup & Clean Architecture Boilerplate
*   **Objective**: Initialize the multi-service codebase directory and configure dependencies.
*   **Scope & Features**: Create the 4 core microservice solution structures. Set up CI/CD workflows and Docker configurations.
*   **Database**: Set up PostgreSQL connection pooling and Redis client setups.
*   **API**: Establish basic health checks (`/health/liveness`, `/health/readiness`).
*   **Testing**: Unit test setup for testing mock databases.
*   **Acceptance Criteria**: Services compile, and Docker images successfully run locally.

### Sprint 2: Lakshmi Kafka Ingestion Client
*   **Objective**: Build a high-throughput options chain tick consumer.
*   **Scope & Features**: Ingest `raw.options.chain.tick` from Kafka, parse spot, future, and strike data.
*   **Database**: Cache incoming option chain states into Redis Hash (`vyuh:market:chain:{StockId}`).
*   **API**: Stream ticks to local buffers.
*   **Testing**: Ingest 10,000 mock options ticks in under 1 second.
*   **Acceptance Criteria**: Redis contains real-time bid/ask prices updated every 10 seconds.

### Sprint 3: Ganesh Historical Engine Client
*   **Objective**: Integrate historical price and volatility databases.
*   **Scope & Features**: Set up gRPC client to query Ganesh for daily returns and ATR.
*   **Database**: Set up historical stock table configurations in PostgreSQL.
*   **API**: `gRPC GetHistoricalReturns(stockId, periodDays)`.
*   **Testing**: Assert returns data matches expected formats.
*   **Acceptance Criteria**: Service fetches 5 years of daily log returns for any stock.

### Sprint 4: Suchak Technical Indicators Client
*   **Objective**: Ingest technical indicators.
*   **Scope & Features**: Pull RSI, EMA, Support, Resistance, and Supertrend data.
*   **Database**: Cache indicators in Redis under `vyuh:suchak:{StockId}`.
*   **API**: Read indicators periodically via background workers.
*   **Testing**: Unit tests verify support/resistance levels are correctly parsed.
*   **Acceptance Criteria**: Indicators are fetched and merged into the active loop.

### Sprint 5: TalkOptions Integration & Greeks Parser
*   **Objective**: Retrieve option Greeks and implied volatility details.
*   **Scope & Features**: Consume delta, gamma, vega, theta, and IV Percentiles.
*   **Database**: Cache options greeks into Redis option chains.
*   **API**: REST API client calls TalkOptions endpoints.
*   **Testing**: Validate options chain contains Greeks for all active strikes.
*   **Acceptance Criteria**: Greeks match theoretical option models.

---

## 3. Phase 2: Mathematical & Probability Modeling (Sprints 6–10)

### Sprint 6: Average True Range (ATR) & Expected Move (EM) Engine
*   **Objective**: Compute short-term price range boundaries.
*   **Scope & Features**: Implement ATR Wilder smoothing and ATM Straddle-based expected move formulas.
*   **API**: `GET /api/v1/analytics/expected-move/{stockId}`.
*   **Testing**: Cross-validate ATR and EM values against Python references.
*   **Acceptance Criteria**: EM values are calculated every 10 seconds for all stocks.

### Sprint 7: Historical Price Probability Engine
*   **Objective**: Estimate historical closing probabilities.
*   **Scope & Features**: Implement the non-parametric bootstrap simulation on log returns.
*   **Testing**: Compare bootstrap distribution quantiles with historical realized prices.
*   **Acceptance Criteria**: Calculates probability of exceeding any strike with $\pm 1\%$ simulation accuracy.

### Sprint 8: BSM IV Probability Engine
*   **Objective**: Calculate risk-neutral probabilities.
*   **Scope & Features**: Implement BSM dual-delta cumulative probability models ($\Phi(d_2)$).
*   **Testing**: Assert values match analytical SciPy normal distribution outputs.
*   **Acceptance Criteria**: Implied ITM probabilities computed for the complete option chain.

### Sprint 9: Touch Probability Engine
*   **Objective**: Estimate the probability of strikes being tested before expiry.
*   **Scope & Features**: Implement first-passage time approximations for GBM.
*   **Testing**: Assert Touch Probability $\approx 2 \cdot \Phi(d_2)$ for short horizons.
*   **Acceptance Criteria**: Touch probabilities generated for all strikes in the chain.

### Sprint 10: Range Probability Engine
*   **Objective**: Calculate strangle closing probabilities.
*   **Scope & Features**: Integrate Call and Put CDF values to compute range probability.
*   **Acceptance Criteria**: Returns joint range probability for any strike pair.

---

## 4. Phase 3: Liquidity, Scoring & Strategy Selection (Sprints 11–15)

### Sprint 11: Option Chain Liquidity Scoring
*   **Objective**: Score strikes by execution feasibility.
*   **Scope & Features**: Implement the Liquidity Score formula (spread, volume, OI).
*   **Database**: Log liquidity scores in PostgreSQL daily.
*   **Acceptance Criteria**: Excludes strikes with Liquidity Score < 40.

### Sprint 12: Stock Scoring Engine
*   **Objective**: Rank underlyings for portfolio eligibility.
*   **Scope & Features**: Implement the Overall Stock Score ($OSS$) algorithm.
*   **Database**: Store daily scores in `stock_scores_archive`.
*   **Acceptance Criteria**: Generates ZSET ranking (`vyuh:scoring:overall_rank`) in Redis.

### Sprint 13: Strike Selection Intelligence
*   **Objective**: Select specific short and long legs.
*   **Scope & Features**: Match delta target (0.16) and support/resistance boundaries.
*   **Acceptance Criteria**: Selects Call and Put legs for the active basket.

### Sprint 14: Strategy Mapping Engine
*   **Objective**: Select option structures based on market regimes.
*   **Scope & Features**: Map Volatility State and Trend Direction to strategy templates.
*   **Acceptance Criteria**: Matches stocks to Strangles, Straddles, Iron Condors, or Iron Flies.

### Sprint 15: Premium Valuation Engine
*   **Objective**: Identify overpriced options.
*   **Scope & Features**: Calculate Volatility Risk Premium (VRP) ratios and Theta-to-Margin efficiency.
*   **Acceptance Criteria**: Ranks strategies by annualized theta yield per rupee of margin.

---

## 5. Phase 4: Portfolio Construction & Risk (Sprints 16–20)

### Sprint 16: Portfolio Construction Optimizer
*   **Objective**: Allocate â‚¹25 Crores across stocks.
*   **Scope & Features**: Implement the Knapsack optimization solver.
*   **Acceptance Criteria**: Allocates capital to 70–80 stocks, maximizing expected theta.

### Sprint 17: Diversification Engine
*   **Objective**: Enforce exposure limits.
*   **Scope & Features**: Apply Sector caps (15%), Industry caps (10%), and Correlation matrix caps (r > 0.35).
*   **Acceptance Criteria**: Rejects portfolios that violate sector or correlation thresholds.

### Sprint 18: Portfolio Value at Risk (VaR) Engine
*   **Objective**: Estimate portfolio-level tail risk.
*   **Scope & Features**: Implement 1-day 99% historical simulation VaR.
*   **Acceptance Criteria**: Computes portfolio VaR and displays it on the monitoring stream.

### Sprint 19: Stress Testing Engine
*   **Objective**: Stress test portfolio parameters.
*   **Scope & Features**: Simulate Spot shocks ($\pm 5\%$ to $\pm 15\%$) and IV spikes ($+50\%$ to $+100\%$).
*   **Acceptance Criteria**: Generates potential loss estimates for the stress scenarios.

### Sprint 20: Decision & Adjustment Engine
*   **Objective**: Automate position management rules.
*   **Scope & Features**: Set exit rules (50% profit) and adjustment rules (delta > 0.35).
*   **Acceptance Criteria**: Generates adjustment trade recommendations.

---

## 6. Phase 5: Gateway, UI, & Operations (Sprints 21–25)

### Sprint 21: Gateway Security & REST API
*   **Objective**: Build secure gateway interfaces.
*   **Scope & Features**: Implement JWT, HMAC-SHA256 headers, and user audit logs.
*   **Acceptance Criteria**: All endpoints require authenticated access.

### Sprint 22: SignalR Realtime Hub
*   **Objective**: Stream live updates to UI.
*   **Scope & Features**: Stream portfolio greeks, active margins, and risk alerts.
*   **Acceptance Criteria**: SignalR pushes updates every 10 seconds.

### Sprint 23: VIB (AI Intelligence Brain) Integration
*   **Objective**: Integrate the AI overlay.
*   **Scope & Features**: Deploy local LLM, pgvector RAG, and the PM approval dashboard.
*   **Acceptance Criteria**: Parameter proposals are queued for PM review.

### Sprint 24: UI Frontend Implementation
*   **Objective**: Build the user dashboards.
*   **Scope & Features**: Next.js dashboards, AG Grid tables, and TradingView charts.
*   **Acceptance Criteria**: PM can monitor, filter, and approve trades on the UI.

### Sprint 25: Vega Client & End-to-End Dry Run
*   **Objective**: Validate integration and live performance.
*   **Scope & Features**: Connect gateway to Vega OMS. Run a simulated trading day.
*   **Acceptance Criteria**: Loops complete in < 1,800ms with zero errors.


---

# 17_Operations

# VYUH Engine - Operations and Runbook Manual

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Senior Infrastructure Engineer | Lead DevOps / SRE | Initial Specification |

---

## 2. Daily System Startup Runbook (08:30 AM IST)
Follow this execution sequence each trading day to prepare the system for market open:

```mermaid
flowchart TD
    T1["08:30 AM: Verify Network & APIs<br/>(Ganesh, Lakshmi, Suchak, TalkOptions)"]
    T1 --> T2["08:40 AM: Warming Cache<br/>(Pre-load active tickers in Redis)"]
    T2 --> T3["08:50 AM: Pre-flight Check<br/>(Verify margin & liquid buffers)"]
    T3 --> T4["09:00 AM: UI Live Heartbeat<br/>(SignalR hub, PM dashboard active)"]
    T4 --> T5["09:15 AM: Live Operations<br/>(10-second pipeline active)"]
```

1.  **Connectivity Check (08:30 AM)**: Run the script `scripts/ops/verify_connections.sh` to ping all sub-service endpoints. Confirm that gRPC connection states to Ganesh, Suchak, and TalkOptions show `READY`.
2.  **Cache Warming (08:40 AM)**: Run `scripts/ops/warm_redis_cache.sh`. This pulls the current list of 200 eligible NSE F&O underlyings from database configurations and writes empty hash structures to Redis, allocating the necessary space.
3.  **Pre-Flight Verification (08:50 AM)**:
    *   Verify with Vega gRPC that total client margin shows at least **â‚¹25 Crores**.
    *   Ensure the liquid buffer shows at least **â‚¹7.5 Crores** in cash equivalents.
4.  **UI Connection (09:00 AM)**: Confirm that the Portfolio Manager has logged into the web portal and that the SignalR connection shows `CONNECTED`.

---

## 3. Daily System Shutdown Runbook (03:40 PM IST)
Run this sequence post-market close:
1.  **Deactivate live processing loop (03:40 PM)**: Set the environment variable flag `SYSTEM_TRADING_ACTIVE=FALSE` via the configuration portal. This stops the 10-second rebalancing background service evaluation thread.
2.  **Post-Market Database Backup (03:50 PM)**:
    *   Trigger automated pg_dump to backup transactional tables.
    *   Reconcile PnL states between PostgreSQL position logs and the Vega OMS trade reports. Record any discrepancies for the audit desk.
3.  **Redis Clean Up (04:10 PM)**: Flush non-persistent ticker data from Redis. Maintain position and margin data.

---

## 4. Disaster Recovery & Failover Protocols

### 4.1. Ingestion Service Crash (Kafka Consumer Offset Reset)
*   **Symptom**: Ingest service replica pod crashes, causing options tick events to back up in Kafka.
*   **Resolution Protocol**:
    1.  Kubernetes automatically restarts the pod.
    2.  The consumer registers under the same consumer group.
    3.  If the group fails to recover, trigger the script `scripts/ops/reset_offsets_latest.sh` to reset the consumer offset to the latest message. This prevents processing backlog spikes (old ticks) when recovering.

### 4.2. Redis Master Node Failure
*   **Symptom**: Redis commands fail with connection timeout.
*   **Resolution Protocol**:
    *   The Redis Cluster sentinel nodes must elect a new Master within **5 seconds**.
    *   The .NET Stack uses StackExchange.Redis client configured with `abortConnect=false` to automatically retry and update master configurations. If the cluster fails to elect a master, the gateway service halts packet generation and sends an SMS/Email alert.

---

## 5. System Alerting Thresholds

| Alert Type | Source | Threshold Condition | Severity | System Action |
| :--- | :--- | :--- | :--- | :--- |
| **Pipeline Latency** | WebAPI Gateway | Total loop execution time > 1,800ms | CRITICAL | Page DevOps, log telemetry trace |
| **Kafka Ingestion Lag** | Kafka Exporter | Consumer group lag > 20 messages | WARNING | Spin up additional consumer pod |
| **Margin Breach** | Risk Service | Margin usage per stock > â‚¹30 Lakhs | CRITICAL | Trigger UI breach pop-up, block trades |
| **Vega OMS Connection** | Gateway Service | Connection lost > 5 seconds | CRITICAL | Pause all trade generation immediately |

---

## 6. Manual Overrides and Panic Button Protocol
A "Panic Button" is available on the Command Center UI and via CLI.
*   **CLI Panic Command**: `vyuhctl cluster emergency-stop`
*   **Action Triggered**:
    1.  Immediately sends a SignalR broadcast message setting system state to `HALTED`.
    2.  Bypasses the 10-second evaluation pipeline.
    3.  Compiles an emergency exit packet for all open positions and pushes it directly to the Vega OMS.
    4.  Refuses any new entry setups until the master database table `system_state.emergency_override` is manually updated to `FALSE` by the Risk Officer.


---

# 18_Future_Roadmap

# VYUH Engine - Future Development Roadmap

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Chief Technology Officer | Investment Committee | Initial Specification |

---

## 2. Multi-Asset Class Expansion
While the initial release of VYUH focuses strictly on NSE F&O equities, the underlying core engine architecture is designed to generalize to other derivative asset classes.

### 2.1. Index Options Integration (Nifty, BankNifty, Finnifty)
*   **Engineering Scope**: Expand Lakshmi to ingest high-frequency index option chains.
*   **Mathematical Updates**: Implement skewed jump-diffusion models to capture the pronounced volatility smile typically present in index contracts.
*   **Timeline**: Scheduled for **Q1 2027**.

### 2.2. Commodities & Currency Options (MCX, USDINR)
*   **Engineering Scope**: Adapt the Ingestion Service to handle MCX and currency derivatives.
*   **Timeline**: Scheduled for **Q3 2027**.

---

## 3. Global Options Capability (US & EU Markets)
Scaling the platform to deploy international capital.

### 3.1. US Equity Options (Cboe / OCC)
*   **Engineering Challenges**: US options are American-style (exercisable at any time), requiring a transition from the European-style Black-Scholes-Merton model to binomial tree pricing engines or Bjerksund-Stensland approximations to prevent early assignment losses.
*   **Data Scale**: US options markets contain over 5,000 underlyings with massive option chains. This requires scaling the Ingestion Service and shifting database processing tasks to GPU-accelerated computing nodes.
*   **Timeline**: Scheduled for **Q2 2028**.

---

## 4. Transition to Direct Algorithmic Execution (Zero HITL)
Moving from manual PM signatures to fully autonomous portfolio adjustments.

```mermaid
graph LR
    subgraph Phase 1: Current State
        V1[VYUH Engine] -->|Draft Packet| PM[PM Sign-Off]
        PM -->|Signed gRPC| Vega1[Vega OMS]
    end
    
    subgraph Phase 2: Autonomous State
        V2[VYUH Engine] -->|Automated signed packet| HardLimits[Hard Risk Engine Gate]
        HardLimits -->|Pass| Vega2[Vega Direct DMA]
    end
    
    style PM fill:#ff9,stroke:#333,stroke-width:2px
    style HardLimits fill:#9f9,stroke:#333,stroke-width:2px
```

1.  **Risk Sandbox Run**: Run the engine in autonomous mock mode alongside the PM's live session for a 3-month trial period.
2.  **Hard Risk Engine Integration**: Implement hardware-level risk checks directly on the network interface cards (NICs) to ensure that even if the AI loop malfunctions, order quantities and exposures cannot exceed hard limits.
3.  **Gradual Rollout**: Authorize autonomous rebalancing up to **â‚¹5 Crores** capital first, increasing limits as validation checks pass.

---

## 5. Deep Reinforcement Learning (DRL) Portfolio Construction
Transitioning from standard linear/integer knapsack optimization to a continuous reinforcement learning agent.

```
                  DRL ENVIROMENT LOOP LOGIC
                  
                 â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
                 â”‚        Environment        â”‚
                 â”‚   (NSE Option Universe)   â”‚
                 â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¬â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
                               â”‚
            Observation (State)â”‚   Action (Trade weights)
            [Spot, Greeks, IV] â”‚   [Enter Strangle,
                               â–¼    Adjust Put, Roll]
                 â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
                 â”‚       Quant Agent         â”‚
                 â”‚  (Actor-Critic Network)   â”‚
                 â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

*   **Model Architecture**: Multi-agent Deep Deterministic Policy Gradient (DDPG) or Proximal Policy Optimization (PPO).
*   **State Space**: Normalized Spot trends, implied volatility smiles, delta exposures, pairwise asset correlations, and remaining portfolio margin.
*   **Action Space**: Vector of target portfolio allocations across 80 stocks and strike locations.
*   **Reward Function**: Sharpe/Sortino ratio maximization over a rolling monthly window, heavily penalized by margin breach events and portfolio drawdowns.
*   **Timeline**: Pilot quantitative testing scheduled for **Q4 2028**.


---

# 19_Glossary

# VYUH Engine - Glossary and Terminology

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Technical Documentation Expert | Principal Architect | Initial Specification |

---

## 2. Business and Domain Terms
*   **ATM (At-The-Money)**: An option contract whose strike price is identical or extremely close to the current spot price of the underlying asset.
*   **Average True Range (ATR)**: A technical analysis indicator that measures market volatility by decomposing the entire range of an asset price over a specified period.
*   **Expected Move (EM)**: The expected pricing boundary range for an underlying stock based on option premium pricing. Typically calculated as Straddle Price $\times 0.85$.
*   **F&O Ban**: A regulatory state on the NSE where open interest of a stock derivative exceeds 95% of the Market Wide Position Limit. No new positions can be entered; only exits are allowed.
*   **Implied Volatility (IV)**: The volatility metric representing the market's expectation of future price movement implied by option contract premiums.
*   **IV Percentile (IVP)**: The percentage of trading days in a historical window (usually 252 days) where the implied volatility was lower than the current implied volatility.
*   **IV Rank (IVR)**: A metric scaling the current IV between the 52-week low and high IV values:
    $$\frac{IV_{\text{current}} - IV_{\text{low}}}{IV_{\text{high}} - IV_{\text{low}}} \times 100$$
*   **MWPL (Market Wide Position Limit)**: The maximum number of open option and future contracts permitted for a single underlying across all market participants on the exchange.
*   **OTM (Out-Of-The-Money)**: An option contract that has no intrinsic value (e.g., a Call with strike price above the spot price, or a Put with strike price below the spot price).
*   **Premium Richness (PR)**: The ratio of implied volatility to historical realized volatility. Used as a marker to detect option overpricing.
*   **SPAN Margin (Standard Portfolio Analysis of Risk)**: A system used by exchanges to calculate margin requirements for derivative portfolios based on risk-scenario simulations.
*   **Theta Efficiency (TE)**: The ratio of daily theta decay collected to the margin capital consumed, annualized:
    $$\frac{\Theta \times 365}{\text{Margin}} \times 100$$
*   **VRP (Variance Risk Premium)**: The yield difference representing the premium option buyers pay sellers for tail-risk insurance (historically, Implied Volatility > Realized Volatility).

---

## 3. Quantitative and Mathematical Terms
*   **Breeden-Litzenberger Method**: A mathematical formulation used to extract the risk-neutral probability density function from option chain prices by taking the second derivative of call option pricing with respect to strike.
*   **Dual Delta**: The partial derivative of the call option price with respect to the strike price ($\partial C / \partial K$). It represents the risk-neutral probability of the option expiring In-The-Money.
*   **First-Passage Probability (Touch Probability)**: The probability that a stochastic process (e.g., geometric Brownian motion representing stock price) will touch or cross a boundary barrier at least once during a specified time window.
*   **Herfindahl-Hirschman Index (HHI)**: A measure of concentration calculated by squaring the weight of each individual asset in the portfolio.
*   **Kernel Density Estimation (KDE)**: A non-parametric way to estimate the probability density function of a random variable (e.g. historical stock log returns).

---

## 4. Technical Architecture Terms
*   **Clean Architecture**: A software design pattern that segregates core domain business logic from database, UI, and external API infrastructure dependencies.
*   **CQRS (Command Query Responsibility Segregation)**: An architectural pattern that separates read queries from write commands, optimizing data access pathways.
*   **DDD (Domain-Driven Design)**: An software design philosophy centered on modeling software to match a business domain, isolating components into distinct bounded contexts.
*   **Execution Packet**: A cryptographically signed JSON file containing proposed trades, limits, and quantities.
*   **vLLM**: An offline high-throughput engine optimized for local LLM inference.
*   **RAG (Retrieval-Augmented Generation)**: A technique that optimizes LLM outputs by querying external vector databases for relevant context chunks.


---

# 20_Appendices

# VYUH Engine - Appendices

## 1. Document Control
| Version | Date | Author | Reviewer | Description |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-07-15 | Lead Software Engineer | Principal Architect | Initial Specification |

---

## 2. Appendix A: JSON Data Templates

### 2.1. System Configuration File (`vyuh_settings.json`)
```json
{
  "CapitalSettings": {
    "TotalCapital": 250000000.00,
    "MarginCapitalPct": 60.00,
    "LiquidBufferPct": 30.00,
    "DrawdownReservePct": 10.00
  },
  "RiskLimits": {
    "SingleStockMarginLimitPct": 2.00,
    "SectorLimitPct": 15.00,
    "IndustryLimitPct": 10.00,
    "PairCorrelationLimit": 0.35,
    "MaxDrawdownLimitPct": 8.00,
    "PortfolioVaR99PercentileLimitPct": 3.00
  },
  "ExecutionRules": {
    "TargetStrangleDelta": 0.16,
    "StrangleAdjustmentDeltaTrigger": 0.35,
    "ProfitTakeCreditPct": 50.00,
    "StopLossCreditPct": 200.00,
    "ExpiryWeekExitDay": "TUESDAY"
  }
}
```

### 2.2. Execution Packet Event (`order.execution.packet`)
```json
{
  "packetId": "d0e12d1b-7a74-4b5c-a5b6-6d6c6e7f8a9b",
  "portfolioStateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "timestamp": 1784108435000,
  "engineSource": "VYUH",
  "signature": "MEYCIQCc3vY7sSc4ZqM...gih8=",
  "approvedBy": "solanki_pm",
  "orders": [
    {
      "orderId": "vyuh-sbin-ce-800-s",
      "stockId": "SBIN",
      "segment": "DERIVATIVES",
      "productType": "NRML",
      "orderType": "LIMIT",
      "optionType": "CE",
      "strikePrice": 800.00,
      "expiryDate": "2026-07-30",
      "quantity": 1500,
      "side": "SELL",
      "limitPrice": 12.40
    },
    {
      "orderId": "vyuh-sbin-pe-740-s",
      "stockId": "SBIN",
      "segment": "DERIVATIVES",
      "productType": "NRML",
      "orderType": "LIMIT",
      "optionType": "PE",
      "strikePrice": 740.00,
      "expiryDate": "2026-07-30",
      "quantity": 1500,
      "side": "SELL",
      "limitPrice": 9.80
    }
  ]
}
```

---

## 3. Appendix B: Database DDL Schemas (PostgreSQL Reference)

```sql
-- Create core schemas
CREATE SCHEMA IF NOT EXISTS vyuh_config;
CREATE SCHEMA IF NOT EXISTS vyuh_portfolio;
CREATE SCHEMA IF NOT EXISTS vyuh_execution;
CREATE SCHEMA IF NOT EXISTS vyuh_analytics;

-- Underlyings Configuration
CREATE TABLE vyuh_config.underlying_stocks (
    stock_id VARCHAR(20) PRIMARY KEY,
    stock_name VARCHAR(100) NOT NULL,
    sector VARCHAR(50) NOT NULL,
    industry VARCHAR(50) NOT NULL,
    lot_size INT NOT NULL,
    is_active BOOLEAN DEFAULT TRUE NOT NULL,
    max_position_multiplier NUMERIC(3,2) DEFAULT 1.00 NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL
);

-- Indexing for performance
CREATE INDEX idx_stocks_sector ON vyuh_config.underlying_stocks(sector);

-- Portfolio State
CREATE TABLE vyuh_portfolio.portfolio_states (
    portfolio_state_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    timestamp TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    total_capital NUMERIC(15,2) NOT NULL,
    margin_deployed NUMERIC(15,2) NOT NULL,
    liquid_buffer NUMERIC(15,2) NOT NULL,
    drawdown_reserve NUMERIC(15,2) NOT NULL,
    portfolio_theta NUMERIC(12,2) NOT NULL,
    portfolio_delta NUMERIC(12,2) NOT NULL,
    portfolio_gamma NUMERIC(12,2) NOT NULL,
    portfolio_vega NUMERIC(12,2) NOT NULL,
    status VARCHAR(20) DEFAULT 'ACTIVE' NOT NULL
);

-- Indexing for state queries
CREATE INDEX idx_portfolio_states_time ON vyuh_portfolio.portfolio_states(timestamp DESC);
```

---

## 4. Appendix C: Secure REST Client Implementation (C#)
C# class used by microservices to sign and execute REST calls to the gateway.

```csharp
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class SecureGatewayClient
{
    private readonly HttpClient _httpClient;
    private readonly string _clientSecret;

    public SecureGatewayClient(HttpClient httpClient, string clientSecret)
    {
        _httpClient = httpClient;
        _clientSecret = clientSecret;
    }

    public async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string url, string jsonBody)
    {
        var request = new HttpRequestMessage(method, url);
        
        if (method == HttpMethod.Post || method == HttpMethod.Put)
        {
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        // Generate HMAC signature
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        string message = $"{method.Method}:{url}:{timestamp}:{jsonBody}";
        string hmacSignature = ComputeHmacSha256(message, _clientSecret);

        // Add security headers
        request.Headers.Add("X-Signature", hmacSignature);
        request.Headers.Add("X-Timestamp", timestamp);

        return await _httpClient.SendAsync(request);
    }

    private string ComputeHmacSha256(string message, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var messageBytes = Encoding.UTF8.GetBytes(message);
        
        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(messageBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
```


---

# vyuh-ui

<!-- BEGIN:nextjs-agent-rules -->
# This is NOT the Next.js you know

This version has breaking changes â€” APIs, conventions, and file structure may all differ from your training data. Read the relevant guide in `node_modules/next/dist/docs/` before writing any code. Heed deprecation notices.
<!-- END:nextjs-agent-rules -->

