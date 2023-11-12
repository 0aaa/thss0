import { NavLink, Route, Routes, useNavigate } from 'react-router-dom'
// import App from '../App'
import Logout from '../auth/logout'
import Error404 from '../views/error404'
import Privacy from '../views/privacy'
import EditRouter from '../views/crud/edit'
import { connect } from 'react-redux'
import List from '../views/crud/list'
import { AUTH_TOKEN, USERNAME } from '../../config/consts'
import 'bootstrap/dist/css/bootstrap.css'
import Schedule from '../views/schedule'
import { updateModal, updateTheme } from '../../actionCreator/actionCreator'
import ModalGen from './modal'

const Navigation = props => {
    const isAuthenticated = sessionStorage.getItem(AUTH_TOKEN)
    const navigate = useNavigate()
    return <div className="px-4">
        <style>            
            {'.form-control:focus {box-shadow: none}'}
        </style>
        <nav className="navbar navbar-expand-lg">
            <div className="container-fluid p-0">
                <button type="button" className="navbar-toggler border-0 border-bottom rounded-0" data-bs-toggle="offcanvas"
                        data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                    <img src="/favicon.ico" alt="Menu" />
                </button>
                <div id="navbarNav" tabIndex="-1" className="offcanvas offcanvas-end" aria-labelledby="navbarNavXlLabel">
                    <div className="offcanvas-header">
                        <h5 id="offcanvasNavbarLabel" className="offcanvas-title">Medsys</h5>
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
                                <NavLink to="/c/professional" className="nav-link">Professionals</NavLink>
                            </li>
                            {(isAuthenticated
                                && <>
                                    <li className="nav-item">
                                        <NavLink to="/c/client" className="nav-link">Clients</NavLink>
                                    </li>
                                    <li className="nav-item">
                                        <NavLink to="/c/results" className="nav-link">Results</NavLink>
                                    </li>
                                    <li className="nav-item">
                                        <NavLink to="/schedule" className="nav-link">Schedule</NavLink>
                                    </li>
                                    <li className="nav-item input-group ms-auto" style={{ width: '164px', height: '42px' }}>
                                        {/* <input type="search" id="search" className="form-control border-0 border-bottom border-2 rounded-0 pe-0 ps-1" aria-label="Search" placeholder="Search" />
                                        <NavLink type="submit" to="/c/search" className="nav-link p-1"><img src="/magnifier.ico" alt="Search" /></NavLink> */}
                                        <form onSubmit={event => { event.preventDefault(); navigate(`/c/search/${encodeURIComponent(event.target[0].value)}`) }}
                                                role="search" className="input-group ms-auto" style={{ width: '164px' }}>
                                            <input type="search" aria-label="Search" className="form-control border-0 border-bottom rounded-0 btn-outline-dark pe-0 ps-1" placeholder="Search" />
                                            <button type="submit" className="btn border-0 border-bottom rounded-0 py-0 px-1">
                                                <img src="/magnifier.ico" alt="Search" />
                                            </button>
                                        </form>
                                    </li>
                                    <li className="nav-item ps-1" style={{ paddingTop: '6px' }}>
                                        <h5>{sessionStorage.getItem(USERNAME)}</h5>
                                    </li>
                                    <li className="nav-item">
                                        <NavLink to="/logout" className="nav-link">Logout</NavLink>
                                    </li>
                                </>)
                                || <>
                                    <li className="nav-item">
                                        <a href="/" onClick={event => props.updateModal(event)} className="nav-link" data-bs-toggle="modal" data-bs-target="#modalGen">Register</a>
                                    </li>
                                    <li className="nav-item">
                                        <a href="/" onClick={event => props.updateModal(event)} className="nav-link" data-bs-toggle="modal" data-bs-target="#modalGen">Login</a>
                                    </li>
                                </>
                            }
                            <li className="nav-item">
                                <NavLink to="/privacy" className="nav-link">Privacy</NavLink>
                            </li>
                            <li>
                                <button onClick={event => props.updateTheme(event)} className="btn border-0 px-0">
                                    <img src={`/${props.btnColor}.ico`} alt={props.btnColor} />
                                </button>
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
                    <Route path="/edit/:entityName/:id" element={<EditRouter />} />
                </>
            }
            <Route path="/privacy" element={<Privacy />} />
            <Route path="/schedule" element={<Schedule />} />
            <Route path="/c/:entityName/:toFind?/:order?/:printBy?/:page?" element={<List />} />
            <Route path="*" element={<Error404 />} />
        </Routes>
        <ModalGen />
        <footer></footer>
    </div>
}

const mapStateToProps = state => ({
    printBy: state.printBy
    , username: state.username
    , btnColor: state.btnColor
})

const mapDispatchToProps = dispatch => ({
    updateTheme: event => {
        event.preventDefault()
        document.documentElement.setAttribute('data-bs-theme', (document.documentElement.getAttribute('data-bs-theme') === 'light' && 'dark') || 'light')
        dispatch(updateTheme())
    }
    , updateModal: event => {
        event.preventDefault()
        dispatch(updateModal(event.target.innerHTML))
    }
})

export default connect(mapStateToProps, mapDispatchToProps)(Navigation)