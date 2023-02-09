import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter as Router, NavLink, Route, Routes } from 'react-router-dom';
import App from './App';
import Register from './models/auth/register'
import Login from './models/auth/login'
import Logout from './services/auth'
import Error404 from './models/error-404'
import 'bootstrap/dist/css/bootstrap.css'
import GetAllRouter from './models/crud/get-all';

const navigation = (
  <Router>
    <ul>
      <li>
        <NavLink to="/">Home</NavLink>
      </li>
      <li>
        <NavLink to="/professionals">Professionals</NavLink>
      </li>
      <li>
        <NavLink to="/clients">Clients</NavLink>
      </li>
      <li>
        <NavLink to="/procedures">Procedures</NavLink>
      </li>
      <li>
        <NavLink to="/substances">Substances</NavLink>
      </li>
      <li>
        <NavLink to="/privacy">Privacy</NavLink>
      </li>
      <li>
        <NavLink to="/register">Register</NavLink>
      </li>
      <li>
        <NavLink to="/login">Login</NavLink>
      </li>
      <li>
        <NavLink to="/logout">Logout</NavLink>
      </li>
      <Routes>
        <Route exact path="/" element={App()} />
        <Route path="/register" element={<Register />} />
        <Route path="/login" element={<Login />} />
        <Route path="/logout" element={<Logout />} />
        {/*To add.*/}
        <Route path="/privacy" element={<></>}/>
        {/**/}
        <Route path="/:entityName" element={<GetAllRouter />} />
        <Route path="*" element={<Error404 />} />
      </Routes>
    </ul>
  </Router>
)

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  navigation
);