export default function StatusBadge({ active, activeLabel = 'Connected', inactiveLabel = 'Disconnected' }) {
  return <span className={`status-badge ${active ? 'is-active' : 'is-inactive'}`}>{active ? activeLabel : inactiveLabel}</span>
}
