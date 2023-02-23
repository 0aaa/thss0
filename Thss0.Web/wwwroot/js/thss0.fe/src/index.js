import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter as Router, NavLink, Route, Routes } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.css'
import App from './App';
import Register from './models/auth/register'
import Login from './models/auth/login'
import { Logout } from './services/auth'
import Error404 from './models/error-404'
import Privacy from './models/privacy';
import ListRouter from './models/crud/list';
import AddRouter from './models/crud/add';
import DeleteRouter from './models/crud/delete';
import DetailsRouter from './models/crud/details';
import EditRouter from './models/crud/edit';

const navigation = (
  <Router>
    <ul>
      <li>
        <NavLink to="/">Home</NavLink>
      </li>
      <li>
        <NavLink to="/users/1">Professionals</NavLink>
      </li>
      <li>
        <NavLink to="/users/0">Clients</NavLink>
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
        <Route path="/privacy" element={<Privacy />} />
        <Route path="/:entityName/:roleIndex?" element={<ListRouter />} />
        <Route path="/add/:entityName" element={<AddRouter />} />
        <Route path="/details/:entityName/:id" element={<DetailsRouter />} />
        <Route path="/edit/:entityName/:id" element={<EditRouter />} />
        <Route path="/delete/:entityName/:id" element={<DeleteRouter />} />
        <Route path="*" element={<Error404 />} />
      </Routes>
    </ul>
  </Router>
)

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  navigation
);