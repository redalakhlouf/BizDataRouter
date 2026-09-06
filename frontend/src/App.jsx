import { useCallback, useEffect, useState } from 'react'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import Sidebar from './components/Sidebar'
import Header from './components/Header'
import { getStatus } from './services/api'
import DashboardPage from './pages/DashboardPage'
import PipelinePage from './pages/PipelinePage'
import DataPage from './pages/DataPage'
import FilesPage from './pages/FilesPage'
import ConfigurationPage from './pages/ConfigurationPage'

function Application() {
  const [status, setStatus] = useState(null)
  const refreshStatus = useCallback(async () => { try { setStatus(await getStatus()) } catch { setStatus(null) } }, [])
  useEffect(() => { refreshStatus(); const timer = window.setInterval(refreshStatus, 4000); return () => window.clearInterval(timer) }, [refreshStatus])
  return <div className="app-shell"><Sidebar /><div className="content-shell"><Header status={status} /><main><Routes><Route path="/" element={<DashboardPage status={status} />} /><Route path="/pipeline" element={<PipelinePage status={status} onStatusRefresh={refreshStatus} />} /><Route path="/data" element={<DataPage />} /><Route path="/files" element={<FilesPage />} /><Route path="/configuration" element={<ConfigurationPage />} /></Routes></main></div></div>
}

export default function App() { return <BrowserRouter><Application /></BrowserRouter> }
