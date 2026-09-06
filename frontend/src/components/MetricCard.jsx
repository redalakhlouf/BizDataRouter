export default function MetricCard({ label, value, detail, children }) {
  return <section className="metric-card"><p className="eyebrow">{label}</p><div className="metric-value">{value}</div>{detail && <p className="metric-detail">{detail}</p>}{children}</section>
}
