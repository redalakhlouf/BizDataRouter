import { useEffect, useState } from 'react'
import { getConfig } from '../services/api'
import LoadingState from '../components/LoadingState'
import ErrorState from '../components/ErrorState'

function ConfigRow({ label, value }) { return <div className="config-row"><span>{label}</span><strong>{value}</strong></div> }

export default function ConfigurationPage() {
  const [config, setConfig] = useState(null); const [error, setError] = useState('')
  const load = async () => { setError(''); try { setConfig(await getConfig()) } catch (e) { setError(e.message) } }
  useEffect(() => { load() }, [])
  if (error) return <ErrorState message={error} onRetry={load} />
  if (!config) return <LoadingState message="Loading configuration…" />
  return <div className="config-grid"><section className="panel"><h2>PI API</h2><ConfigRow label="URL" value={config.piApiUrl} /></section><section className="panel"><h2>CSV</h2><ConfigRow label="Maximum file size" value={`${config.maximumFileSizeMb} MB`} /></section><section className="panel"><h2>MinIO</h2><ConfigRow label="Endpoint" value={config.minioEndpoint} /><ConfigRow label="Bucket" value={config.minioBucketName} /><ConfigRow label="Use SSL" value={config.minioUseSsl ? 'Enabled' : 'Disabled'} /><ConfigRow label="Access key" value={config.accessKeyConfigured ? 'Configured' : 'Not configured'} /><ConfigRow label="Secret key" value={config.secretKeyConfigured ? 'Configured' : 'Not configured'} /></section></div>
}
