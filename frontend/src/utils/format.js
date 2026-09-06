export function formatBytes(bytes) {
  if (bytes === null || bytes === undefined) return '—'
  if (bytes === 0) return '0 B'
  const units = ['B', 'KB', 'MB', 'GB', 'TB']
  const index = Math.min(Math.floor(Math.log(bytes) / Math.log(1024)), units.length - 1)
  const value = bytes / 1024 ** index
  return `${value >= 10 || index === 0 ? value.toFixed(0) : value.toFixed(1)} ${units[index]}`
}

export function formatDate(value) {
  if (!value || Number.isNaN(new Date(value).getTime())) return 'Not available'
  return new Intl.DateTimeFormat('en-GB', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' }).format(new Date(value))
}

export function formatUptime(startedAt) {
  if (!startedAt) return 'Not running'
  const seconds = Math.max(0, Math.floor((Date.now() - new Date(startedAt).getTime()) / 1000))
  return `${Math.floor(seconds / 3600)}h ${Math.floor((seconds % 3600) / 60)}m ${seconds % 60}s`
}

export function dateFromFileName(name) {
  const match = name?.match(/_year=(\d{4})_month=(\d{2})_day=(\d{2})/)
  return match ? `${match[1]}-${match[2]}-${match[3]}` : null
}
