import { NavLink } from 'react-router-dom'
import 'bootstrap/dist/css/bootstrap.css'

const App = () =>
    <ul className="list-unstyled row g-1 h-100">
        <style>
            {'.custom-shadow {text-shadow: 1px 1px 1px black}'}
        </style>
        <li className="nav-item col-12 col-md-2 col-sm-4">
            <NavLink to="/professional" className="nav-link card m-0 p-0 h-100 rounded-0 text-white" style={{ backgroundImage: `url(img/professional0.jpg)`, backgroundPosition: 'center', backgroundSize: 'cover' }}>
                <div className="card-body">
                    <h3 className="card-title custom-shadow">Professionals</h3>
                    <div className="card-text custom-shadow fs-5">List of the care professionals</div>
                </div>
            </NavLink>
        </li>
        <li className="nav-item col-12 col-md-2 col-sm-4">
            <NavLink to="/client" className="nav-link card m-0 p-0 h-100 rounded-0 text-white" style={{ backgroundImage: `url(img/client0.jpg)`, backgroundPosition: 'center', backgroundSize: 'cover' }}>
                <div className="card-body">
                    <h3 className="card-title custom-shadow">Clients</h3>
                    <div className="card-text custom-shadow fs-5">List of our clients</div>
                </div>
            </NavLink>
        </li>
        <li className="nav-item col-12 col-md-2 col-sm-4">
            <NavLink to="/procedure" className="nav-link card m-0 p-0 h-100 rounded-0 text-white" style={{ backgroundImage: `url(img/procedure0.jpg)`, backgroundPosition: 'center', backgroundSize: 'cover' }}>
                <div className="card-body">
                    <h3 className="card-title custom-shadow">Procedures</h3>
                    <div className="card-text custom-shadow fs-5">List of the procedures in a row</div>
                </div>
            </NavLink>
        </li>
        <li className="nav-item col-12 col-md-2 col-sm-4">
            <NavLink to="/department" className="nav-link card m-0 p-0 h-100 rounded-0 text-white" style={{ backgroundImage: `url(img/department0.jpg)`, backgroundPosition: 'center', backgroundSize: 'cover' }}>
                <div className="card-body">
                    <h3 className="card-title custom-shadow">Departments</h3>
                    <div className="card-text custom-shadow fs-5">List of our departments</div>
                </div>
            </NavLink>
        </li>
        <li className="nav-item col-12 col-md-2 col-sm-4">
            <NavLink to="/result" className="nav-link card m-0 p-0 h-100 rounded-0 text-white" style={{ backgroundImage: `url(img/result0.jpg)`, backgroundPosition: 'center', backgroundSize: 'cover' }}>
                <div className="card-body">
                    <h3 className="card-title custom-shadow">Results</h3>
                    <div className="card-text custom-shadow fs-5">List of the results</div>
                </div>
            </NavLink>
        </li>
        <li className="nav-item col-12 col-md-2 col-sm-4">
            <NavLink to="/privacy" className="nav-link card m-0 p-0 h-100 rounded-0 text-white" style={{ backgroundImage: `url(img/privacy0.jpg)`, backgroundPosition: 'center', backgroundSize: 'cover' }}>
                <div className="card-body">
                    <h3 className="card-title custom-shadow">Privacy</h3>
                    <div className="card-text custom-shadow fs-5">Learn more about protection of your personal data</div>
                </div>
            </NavLink>
        </li>
    </ul>

export default App;