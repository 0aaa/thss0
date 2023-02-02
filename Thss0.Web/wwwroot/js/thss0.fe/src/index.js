import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';

const NAVIGATION = (
  <Router>
    <ul>
      <li>
        <NavLink to='/'>Home</NavLink>
      </li>
      <li>
        <NavLink to='/professionals'>Professionals</NavLink>
      </li>
      <li>
        <NavLink to='/clients'>Clients</NavLink>
      </li>
      <li>
        <NavLink to='/procedures'>Procedures</NavLink>
      </li>
      <li>
        <NavLink to='/substances'>Substances</NavLink>
      </li>
      <li>
        <NavLink to='/privacy'>Privacy</NavLink>
      </li>
      <li>
        <NavLink to='/register'>Register</NavLink>
      </li>
      <li>
        <NavLink to='/login'>Login</NavLink>
      </li>
      <Switch>
        <Route exact path='/' component={App} />
        <Route path='/:entity_name' component={Entity} />
        <Route path='/register' component={Register} />
        <Route path='/login' component={Login} />
        <Route component={ERROR_404} />
      </Switch>
    </ul>
  </Router>
)

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <NAVIGATION />
    <script type='text/javascript' src='src/services/entity-service.js'/>
  </React.StrictMode>
);