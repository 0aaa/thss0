import { getTokenAsync, makeRegister } from '../../services/auth'
import React, { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { updateAuth } from '../../actionCreator/actionCreator'
import { connect } from 'react-redux'

class Register extends React.Component {
    render() {
        return (
            <>
                <h4>Register</h4>
                <div id="register-error" className="alert alert-danger d-none"></div>
                <form onSubmit={(event) => this.props.HandleRegister(event, this.props.effect)} className="w-25">
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
const RegisterRouter = (props) => <Register {...props} params={useParams()} />

const mapDispatchToProps = (dispatch) => {
    return {
        HandleRegister: async (event, effect) => {
            event.preventDefault()
            if (!await makeRegister(event.target)) {
                window.location.reload()
            }
            if (!await getTokenAsync(event)) {//
                window.location.reload()
            }
            const navigate = useNavigate()
            useEffect(() => navigate('/'))
            effect()
            dispatch(updateAuth())
        }
    }
}
export default connect(mapDispatchToProps)(RegisterRouter)