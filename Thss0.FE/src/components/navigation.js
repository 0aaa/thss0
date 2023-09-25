// import { Children } from 'react'
import { NavLink, Route, Routes, useNavigate } from 'react-router-dom'
// import App from '../App'
import Login from './auth/login'
import Register from './auth/register'
import Logout from './auth/logout'
import Error404 from './error-404'
import Privacy from './privacy'
import AddRouter from './crud/add'
import Details from './crud/details'
import EditRouter from './crud/edit'
import DeleteRouter from './crud/delete'
import { connect } from 'react-redux'
import List from './crud/list'
import { AUTH_TOKEN, USERNAME } from '../config/consts'
import 'bootstrap/dist/css/bootstrap.css'
import Schedule from './schedule'


const Navigation = () => {
    const isAuthenticated = sessionStorage.getItem(AUTH_TOKEN)
    const navigate = useNavigate()
    // const entities = ['departments', 'clients', 'professionals', 'procedures', 'results', 'substances']
    return <div className="px-4">
        <style>            
            {'.form-control:focus {box-shadow: none}'}
        </style>
        <nav className="navbar navbar-expand-lg">
            <div className="container-fluid p-0">
                <button className="navbar-toggler border-0 border-bottom rounded-0" type="button" data-bs-toggle="offcanvas"
                        data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                    <img src="/favicon.ico" alt="Menu" />
                </button>
                <div className="offcanvas offcanvas-end" tabIndex="-1" id="navbarNav" aria-labelledby="navbarNavXlLabel">
                    <div className="offcanvas-header">
                        <h5 className="offcanvas-title" id="offcanvasNavbarLabel">Medsys</h5>
                        <button type="button" className="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
                    </div>
                    <div className="offcanvas-body">
                        <ul className="navbar-nav w-100 nav-underline column-gap-0">
                            <li className="nav-item">
                                <NavLink to="/" className="navbar-brand me-0">
                                    <img src="/favicon.ico" alt="Home" />
                                </NavLink>
                            </li>
                            <li className="nav-item">
                                <NavLink to="/c/departments" className="nav-link">Departments</NavLink>
                            </li>
                            <li className="nav-item">
                                <NavLink to="/c/substances" className="nav-link">Substances</NavLink>
                            </li>
                            <li className="nav-item">
                                <NavLink to="/c/users/professional" className="nav-link">Professionals</NavLink>
                            </li>
                            {(isAuthenticated
                                && <>
                                    <li className="nav-item">
                                        <NavLink to="/c/users/client" className="nav-link">Clients</NavLink>
                                    </li>
                                    <li className="nav-item">
                                        <NavLink to="/c/procedures" className="nav-link">Procedures</NavLink>
                                    </li>
                                    <li className="nav-item">
                                        <NavLink to="/c/results" className="nav-link">Results</NavLink>
                                    </li>
                                    <li className="nav-item">
                                        <NavLink to="/schedule" className="nav-link">Schedule</NavLink>
                                    </li>
                                    <form onSubmit={event => {
                                                event.preventDefault()
                                                navigate(`/c/${event.target[0].value}/${encodeURIComponent(event.target[1].value)}`)
                                            }} role="search" className="input-group ms-auto" style={{ width: '164px' }}>
                                        {/* <select className="form-select btn btn-outline-dark border-0 border-bottom rounded-0 text-start pe-0 ps-1">
                                            {Children.toArray(entities.map(e =>
                                                <option value={e}>{e.replace(/^./, e[0].toUpperCase())}</option>
                                            ))}
                                        </select> */}
                                        <input type="search" aria-label="Search" className="form-control border-0 border-bottom rounded-0 btn-outline-dark pe-0 ps-1" placeholder="Search" />
                                        <button type="submit" className="btn border-0 border-bottom rounded-0 py-0 px-1">
                                            <img src="/magnifier.ico" alt="Search" />
                                        </button>
                                    </form>
                                    <li className="nav-item ps-1" style={{ paddingTop: '6px' }}>
                                        <h5>{sessionStorage.getItem(USERNAME)}</h5>
                                    </li>
                                    <li className="nav-item">
                                        <NavLink to="/logout" className="nav-link">Logout</NavLink>
                                    </li>
                                </>)
                                || <>
                                    <li className="nav-item">
                                        <a href="/" className="nav-link" data-bs-toggle="modal" data-bs-target="#registerModal">Register</a>
                                    </li>
                                    <li className="nav-item">
                                        <a href="/" className="nav-link" data-bs-toggle="modal" data-bs-target="#loginModal">Login</a>
                                    </li>
                                </>
                            }
                            <li className="nav-item">
                                <NavLink to="/privacy" className="nav-link">Privacy</NavLink>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </nav>
        <Routes>
            {/* <Route exact path="/" element={App()} /> */}
            {isAuthenticated
                && <>
                    <Route path="/logout" element={<Logout />} />
                    <Route path="/add/:entityName" element={<AddRouter />} />
                    <Route path="/edit/:entityName/:id" element={<EditRouter />} />
                    <Route path="/delete/:entityName/:id" element={<DeleteRouter />} />
                </>
            }
            <Route path="/privacy" element={<Privacy />} />
            <Route path="/schedule" element={<Schedule />} />
            <Route path="/c/:entityName/:toFind?/:order?/:printBy?/:page?" element={<List />} />
            <Route path="/details/:entityName/:id" element={<Details />} />
            <Route path="*" element={<Error404 />} />
        </Routes>
        <Register />
        <Login />
        <footer></footer>
    </div>
}
const mapStateToProps = state => {
    return {
        printBy: state.printBy
        , username: state.username
    }
}
export default connect(mapStateToProps)(Navigation)