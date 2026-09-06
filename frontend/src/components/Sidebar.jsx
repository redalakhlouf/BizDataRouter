import { NavLink } from 'react-router-dom'

const links = [['/', 'Dashboard'], ['/pipeline', 'Pipeline'], ['/data', 'Data'], ['/files', 'Files'], ['/configuration', 'Configuration']]

export default function Sidebar() {
  return <aside className="sidebar"><div className="brand"><span className="brand-mark">BD</span><div><strong>BizDataRouter</strong><small>PI to MinIO pipeline</small></div></div><nav aria-label="Main navigation">{links.map(([to, label]) => <NavLink key={to} to={to} end={to === '/'}>{label}</NavLink>)}</nav></aside>
}
