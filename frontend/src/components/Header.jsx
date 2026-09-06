import { useLocation } from 'react-router-dom'
import StatusBadge from './StatusBadge'

const titles = { '/': 'Dashboard', '/pipeline': 'Pipeline control', '/data': 'Data explorer', '/files': 'Files', '/configuration': 'Configuration' }

export default function Header({ status }) {
  const { pathname } = useLocation()
  return <header className="header"><div><h1>{titles[pathname] || 'BizDataRouter'}</h1><p>Operational data pipeline monitoring</p></div><StatusBadge active={status?.isRunning} activeLabel="Pipeline running" inactiveLabel="Pipeline stopped" /></header>
}
