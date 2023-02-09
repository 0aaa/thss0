import React, { useEffect } from 'react'
import makeRegister from '../../services/auth'
import REGISTER_PATH from '../../config/consts'
import { useNavigate } from 'react-router-dom'

async function HandleRegister(event) {
    event.preventDefault()
    const navigate = useNavigate()
    if (!await makeRegister(event)) {
        window.location.reload()
    }
    useEffect(() => navigate(REGISTER_PATH))
}
class Register extends React.Component {
    render() {
        return (
            <>
                <h4>Register</h4>
                <form onSubmit={HandleRegister} className="w-50">
                    <input id="name" className="form-control" placeholder="Name" />
                    <input type="email" id="email" className="form-control mx-1" placeholder="Email" />
                    <input id="phone-number" className="form-control" placeholder="Phone number" />
                    <input type="password" id="password" className="form-control mx-1" placeholder="Password" />
                    <input type="password" id="password-repeat" className="form-control" placeholder="Password repeat" />
                    <input type="submit" value="Submit" className="btn btn-outline-primary mt-1" />
                </form>
            </>
        )
    }
}
export default Register