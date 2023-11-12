import { NavLink } from 'react-router-dom'
import 'bootstrap/dist/css/bootstrap.css'

const App = () =>
  <>
    <style>
      {'.custom-shadow {text-shadow: 1px 1px 1px black}'}
    </style>
    <div>
      <ul className="d-flex list-unstyled row row-cols-2 g-1">
        <li className="nav-item col-12 col-md-4">
          <NavLink to="c/users/professional" className="nav-link card m-0 p-0 text-white rounded-0">
            <img src="img/userNames.jpg" alt="Professionals" className="card-img opacity-75 rounded-0" />
            <div className="card-img-overlay">
              <h3 className="card-title custom-shadow">Professionals</h3>
              <div className="card-text custom-shadow fs-5">List of the care professionals</div>
            </div>
          </NavLink>
        </li>
        <li className="nav-item col-12 col-md-4">
          <NavLink to="c/users/client" className="nav-link card m-0 p-0 h-100 rounded-0">
            <img src="img/clients.jpg" alt="Clients" className="card-img h-100 opacity-75 rounded-0" />
            <div className="card-img-overlay">
              <h3 className="card-title">Clients</h3>
              <div className="card-text fs-5">List of our clients</div>
            </div>
          </NavLink>
        </li>
        <li className="nav-item col-12 col-md-4">
          <NavLink to="schedule" className="nav-link card m-0 p-0 h-100 text-white rounded-0">
            <img src="img/procedureNames.jpg" alt="Schedule" className="card-img h-100 rounded-0" />
            <div className="card-img-overlay">
              <h3 className="card-title custom-shadow">Schedule</h3>
              <div className="card-text custom-shadow fs-5">List of the procedures in a row</div>
            </div>
          </NavLink>
        </li>
        <li className="nav-item col-12 col-md-4">
          <NavLink to="c/departments" className="nav-link card m-0 p-0 text-white rounded-0">
            <img src="img/departmentNames.jpg" alt="Departments" className="card-img rounded-0" />
            <div className="card-img-overlay">
              <h3 className="card-title custom-shadow">Departments</h3>
              <div className="card-text custom-shadow fs-5">List of our departments</div>
            </div>
          </NavLink>
        </li>
        <li className="nav-item col-12 col-md-4">
          <NavLink to="c/results" className="nav-link card m-0 p-0 text-white rounded-0">
            <img src="img/resultNames.jpg" alt="Results" className="card-img rounded-0" />
            <div className="card-img-overlay">
              <h3 className="card-title custom-shadow">Results</h3>
              <div className="card-text custom-shadow fs-5">List of the results</div>
            </div>
          </NavLink>
        </li>
        <li className="nav-item col-12">
          <NavLink to="/privacy" className="nav-link card m-0 p-0 h-100 text-white rounded-0">
            <img src="img/privacy.jpg" alt="Privacy" className="card-img h-100 opacity-75 rounded-0" />
            <div className="card-img-overlay">
              <h3 className="card-title">Privacy</h3>
              <div className="card-text fs-5">Learn more about protection of your personal data</div>
            </div>
          </NavLink>
        </li>
      </ul>
    </div>
  </>

export default App;