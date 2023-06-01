import { Children } from 'react'
import { NavLink, Route, Routes, useNavigate } from 'react-router-dom'
import App from '../App'
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
import { AUTH_TOKEN } from '../config/consts'


const Navigation = (props) => {
    const isAuthenticated = sessionStorage.getItem(AUTH_TOKEN)
    const navigate = useNavigate()
    const entities = ['all', 'departments', 'clients', 'professionals', 'procedures', 'results', 'substances']
    return <ul>
        <li>
            <NavLink to="/">Home</NavLink>
        </li>
        <li>
            <NavLink to="/cat/departments">Departments</NavLink>
        </li>
        <li>
            <NavLink to="/cat/substances">Substances</NavLink>
        </li>
        <li>
            <NavLink to="/cat/users/professional">Professionals</NavLink>
        </li>
        {isAuthenticated
            ? <>
                <li>
                    <NavLink to="/cat/users/client">Clients</NavLink>
                </li>
                <li>
                    <NavLink to="/cat/procedures">Procedures</NavLink>
                </li>
                <li>
                    <NavLink to="/cat/results">Results</NavLink>
                </li>
                <li>
                    <h5>{props.username}</h5>
                </li>
                <li>
                    <NavLink to="/logout">Logout</NavLink>
                </li>
            </>
            : <>
                <li>
                    <NavLink to="/register">Register</NavLink>
                </li>
                <li>
                    <NavLink to="/login">Login</NavLink>
                </li>
            </>
        }
        <li>
            <NavLink to="/privacy">Privacy</NavLink>
        </li>
        <form onSubmit={(event) => {
            event.preventDefault()
            navigate(`/${event.target[0].value}/${event.target[1].value}`)
        }} className="w-25">
            <select className="form-select">
                {Children.toArray(entities.map(e =>
                    <option value={e}>{e.replace(/^./, e[0].toUpperCase())}</option>
                ))}
            </select>
            <input className="form-control d-inline" placeholder="Search" />
            <button type="submit" className="d-inline btn btn-outline-primary">Search</button>
        </form>
        <Routes>
            <Route exact path="/" element={App()} />
            {isAuthenticated
                ? <>
                    <Route path="/logout" element={<Logout />} />
                    <Route path="/add/:entityName" element={<AddRouter />} />
                    <Route path="/edit/:entityName/:id" element={<EditRouter />} />
                    <Route path="/delete/:entityName/:id" element={<DeleteRouter />} />
                </>
                : <>
                    <Route path="/register" element={<Register />} />
                </>
            }
            <Route path="/login" element={isAuthenticated ? App() : <Login />} />
            <Route path="/privacy" element={<Privacy />} />
            <Route path="/cat/:entityName/:toFind?/:order?/:printBy?/:page?" element={<List />} />
            <Route path="/details/:entityName/:id" element={<Details key={'details'} />} />
            <Route path="*" element={<Error404 />} />
        </Routes>
    </ul>
}
const mapStateToProps = (state) => {
    return {
        printBy: state.printBy
        , username: state.username
    }
}
export default connect(mapStateToProps)(Navigation)