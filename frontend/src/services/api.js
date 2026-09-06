const baseUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/$/, '')

async function request(path, options = {}) {
  const response = await fetch(`${baseUrl}${path}`, {
    headers: { Accept: 'application/json', ...options.headers },
    ...options,
  })

  if (!response.ok) {
    const payload = await response.json().catch(() => null)
    throw new Error(payload?.message || `Request failed (${response.status})`)
  }

  return response.json()
}

export const getHealth = () => request('/api/health')
export const getStatus = () => request('/api/status')
export const getConfig = () => request('/api/config')
export const getFiles = () => request('/api/files')
export const getFile = (fileName) => request(`/api/files/${encodeURIComponent(fileName)}`)
export const getData = (date) => request(`/api/data?date=${encodeURIComponent(date)}`)
export const startPipeline = () => request('/api/pipeline/start', { method: 'POST' })
export const stopPipeline = () => request('/api/pipeline/stop', { method: 'POST' })
export const restartPipeline = () => request('/api/pipeline/restart', { method: 'POST' })
