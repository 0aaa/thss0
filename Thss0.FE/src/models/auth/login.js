import React from 'react'
import { connect } from 'react-redux'
import { updateAuth } from '../../actionCreator/actionCreator'
import { getTokenAsync } from '../../services/auth'

const Login = (props) => {
    return (
        <>
            <h5>Login</h5>
            <div id="400-error" className="alert alert-danger d-none"></div>
            <form onSubmit={(event) => props.UpdateAuth(event)} className="w-25">
                <input id="name" className="form-control" placeholder="Name" />
                <input type="password" id="password" className="form-control mx-1" placeholder="Password" />
                <input type="submit" value="Submit" className="btn btn-outline-primary" />
            </form>
        </>
    )
}
const mapDispatchToProps = (dispatch) => {
    return {
        UpdateAuth: async (event) => {
            event.preventDefault()
            const data = await getTokenAsync(event.target)
            if (!data) {
                window.location.reload()
            }
            dispatch(updateAuth(data.username))
        }
    }
}
export default connect(null, mapDispatchToProps)(Login)