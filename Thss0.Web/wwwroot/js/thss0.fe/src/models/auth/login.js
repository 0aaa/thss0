import React, { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { getTokenAsync } from '../../services/auth'
import LOGIN_PATH from '../../config/consts'

async function HandleLogin(event) {
    event.preventDefault()
    const navigate = useNavigate()
    if (!await getTokenAsync(event)) {
        window.location.reload()
    }
    useEffect(() => navigate(LOGIN_PATH))
}
class Login extends React.Component {
    render() {
        return (
            <>
                <h5>Login</h5>
                <form onSubmit={HandleLogin} className="w-50">
                    <input id="name" className="form-control" placeholder="Name" />
                    <input type="password" id="password" className="form-control mx-1" placeholder="Password" />
                    <input type="submit" value="Submit" className="btn btn-outline-primary" />
                </form>
            </>
        )
    }
}
export default Login