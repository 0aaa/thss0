import { NavLink } from 'react-router-dom'
import 'bootstrap/dist/css/bootstrap.css'

const App = () =>
  <>
    <style>
      {`.custom-shadow {text-shadow: 1px 1px 1px  black }`}
    </style>
    <div>
      <ul className="d-flex list-unstyled row row-cols-2 g-1">
        <li className="nav-item col-12 col-md-6">
          <NavLink to="c/users/professional" className="nav-link card m-0 p-0 text-white rounded-0">
            <img src="img/professionals.jpg" alt="img/favicon.ico" className="card-img opacity-75 rounded-0" />
            <div className="card-img-overlay">
              <h3 className="card-title custom-shadow">Professionals</h3>
              <div className="card-text custom-shadow fs-5">List of the care professionals</div>
            </div>
          </NavLink>
        </li>
        <li className="nav-item col-12 col-md-6">
          <NavLink to="c/users/client" className="nav-link card text-black m-0 p-0 h-100 rounded-0">
            <img src="img/clients.jpg" alt="img/favicon.ico" className="card-img h-100 opacity-75 rounded-0" />
            <div className="card-img-overlay">
              <h3 className="card-title">Clients</h3>
              <div className="card-text fs-5">List of our clients</div>
            </div>
          </NavLink>
        </li>
        <li className="nav-item col-12 col-md-4">
          <NavLink to="c/procedures" className="nav-link card m-0 p-0 text-white h-100 rounded-0">
            <img src="img/procedureNames.jpg" alt="img/favicon.ico" className="card-img h-100 rounded-0" />
            <div className="card-img-overlay">
              <h5 className="card-title custom-shadow">Procedures</h5>
              <div className="card-text custom-shadow fw-bold">List of the procedures in a row</div>
            </div>
          </NavLink>
        </li>
        <li className="nav-item col-12 col-md-4">
          <NavLink to="c/substances" className="nav-link card m-0 p-0 text-black h-100 rounded-0">
            <img src="img/substanceNames.jpg" alt="img/favicon.ico" className="card-img opacity-50 rounded-0" />
            <div className="card-img-overlay">
              <h5 className="card-title">Substances</h5>
              <div className="card-text fw-bold">List of the allowed substances</div>
            </div>
          </NavLink>
        </li>
        <li className="nav-item col-12 col-md-4">
          <NavLink to="/privacy" className="nav-link card m-0 p-0 text-white h-100 rounded-0">
            <img src="img/privacy.jpg" alt="img/favicon.ico" className="card-img h-100 opacity-75 rounded-0" />
            <div className="card-img-overlay">
              <h5 className="card-title">Privacy</h5>
              <div className="card-text fw-bold">Learn more about protection of your personal data</div>
            </div>
          </NavLink>
        </li>
      </ul>
    </div>
  </>
export default App;