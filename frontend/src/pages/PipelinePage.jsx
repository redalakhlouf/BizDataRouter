import { useState } from 'react'
import { restartPipeline, startPipeline, stopPipeline } from '../services/api'
import { formatDate } from '../utils/format'
import StatusBadge from '../components/StatusBadge'
import LoadingState from '../components/LoadingState'

export default function PipelinePage({ status, onStatusRefresh }) {
  const [pending, setPending] = useState(''); const [message, setMessage] = useState('')
  const command = async (name, action) => { setPending(name); setMessage(''); try { const response = await action(); setMessage(response.succeeded ? `${name} completed.` : `${name} was not applied.`); await onStatusRefresh() } catch (error) { setMessage(error.message) } finally { setPending('') } }
  if (!status) return <LoadingState message="Loading pipeline status…" />
  return <div className="page-stack"><section className="panel"><div className="panel-heading"><div><h2>Current status</h2><p>Use these controls to manage the pipeline.</p></div><StatusBadge active={status.isRunning} activeLabel="Running" inactiveLabel="Stopped" /></div><dl className="details-grid"><div><dt>Started at</dt><dd>{formatDate(status.startedAt)}</dd></div><div><dt>Last collection</dt><dd>{formatDate(status.lastCollectionAt)}</dd></div><div><dt>Last upload</dt><dd>{formatDate(status.lastUploadAt)}</dd></div><div><dt>PI Web API</dt><dd><StatusBadge active={status.piApiConnected} /></dd></div><div><dt>MinIO</dt><dd><StatusBadge active={status.minioConnected} /></dd></div></dl><div className="action-row"><button onClick={() => command('Start', startPipeline)} disabled={status.isRunning || Boolean(pending)}>{pending === 'Start' ? 'Starting…' : 'Start'}</button><button className="danger-button" onClick={() => command('Stop', stopPipeline)} disabled={!status.isRunning || Boolean(pending)}>{pending === 'Stop' ? 'Stopping…' : 'Stop'}</button><button className="secondary-button" onClick={() => command('Restart', restartPipeline)} disabled={!status.isRunning || Boolean(pending)}>{pending === 'Restart' ? 'Restarting…' : 'Restart'}</button></div>{message && <p className="command-message">{message}</p>}</section></div>
}
