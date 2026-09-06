import { useState } from 'react'
import { getData } from '../services/api'
import { formatDate } from '../utils/format'
import LoadingState from '../components/LoadingState'
import ErrorState from '../components/ErrorState'

export default function DataPage() {
  const [date, setDate] = useState(new Date().toISOString().slice(0, 10)); const [data, setData] = useState(null); const [loading, setLoading] = useState(false); const [error, setError] = useState('')
  const load = async () => { setLoading(true); setError(''); setData(null); try { setData(await getData(date)) } catch (e) { setError(e.message.includes('Aucune') ? 'No data available for this date.' : e.message) } finally { setLoading(false) } }
  return <div className="page-stack"><section className="panel"><div className="panel-heading"><div><h2>Data explorer</h2><p>Read CSV measurements stored in MinIO.</p></div></div><form className="filter-form" onSubmit={(event) => { event.preventDefault(); load() }}><label>Date<input type="date" value={date} onChange={(event) => setDate(event.target.value)} required /></label><button type="submit" disabled={loading}>{loading ? 'Loading…' : 'Load data'}</button></form>{loading ? <LoadingState /> : error ? <ErrorState message={error} onRetry={load} /> : data && <><div className="summary-strip"><span><strong>{data.totalFiles}</strong> CSV files</span><span><strong>{data.totalRows}</strong> records</span><span>Selected date: <strong>{data.date}</strong></span></div><div className="table-wrap"><table><thead><tr><th>Timestamp</th><th>Element</th><th>Atelier</th><th>Atelier quality</th><th>Pressure</th><th>Process temp</th><th>Random values</th></tr></thead><tbody>{data.readings.map((row, index) => <tr key={`${row.element}-${row.timestamp}-${index}`}><td>{formatDate(row.timestamp)}</td><td>{row.element}</td><td>{row.atelier}</td><td>{row.atelierQuality}</td><td>{row.pressure}</td><td>{row.processTemp}</td><td>{row.randomValues}</td></tr>)}</tbody></table></div></>}</section></div>
}
