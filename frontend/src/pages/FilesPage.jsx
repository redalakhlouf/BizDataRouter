import { useEffect, useState } from 'react'
import { getFile, getFiles } from '../services/api'
import { formatBytes, formatDate } from '../utils/format'
import LoadingState from '../components/LoadingState'
import ErrorState from '../components/ErrorState'

export default function FilesPage() {
  const [files, setFiles] = useState([]); const [selected, setSelected] = useState(null); const [query, setQuery] = useState(''); const [loading, setLoading] = useState(true); const [error, setError] = useState('')
  const load = async () => { setLoading(true); setError(''); try { setFiles(await getFiles()) } catch (e) { setError(e.message) } finally { setLoading(false) } }
  useEffect(() => { load() }, [])
  const choose = async (name) => { try { setSelected(await getFile(name)) } catch (e) { setError(e.message) } }
  const filtered = files.filter((file) => file.name.toLowerCase().includes(query.toLowerCase()))
  return <div className="page-stack"><section className="panel"><div className="panel-heading"><div><h2>Files in MinIO</h2><p>{files.length} object{files.length === 1 ? '' : 's'} available.</p></div><button className="secondary-button" onClick={load}>Refresh</button></div><label className="search-label">Search files<input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search files…" /></label>{loading ? <LoadingState /> : error ? <ErrorState message={error} onRetry={load} /> : <div className="file-layout"><div className="table-wrap"><table><thead><tr><th>File name</th><th>Size</th><th>Last modified</th></tr></thead><tbody>{filtered.map((file) => <tr key={file.name} className={selected?.name === file.name ? 'selected-row' : ''} onClick={() => choose(file.name)}><td>{file.name}</td><td>{formatBytes(file.sizeBytes)}</td><td>{formatDate(file.lastModified)}</td></tr>)}</tbody></table>{filtered.length === 0 && <div className="empty-state">No matching files.</div>}</div><aside className="file-detail"><h3>File details</h3>{selected ? <dl><dt>Name</dt><dd>{selected.name}</dd><dt>Size</dt><dd>{formatBytes(selected.sizeBytes)}</dd><dt>Last modified</dt><dd>{formatDate(selected.lastModified)}</dd></dl> : <p>Select a file to view its details.</p>}</aside></div>}</section></div>
}
