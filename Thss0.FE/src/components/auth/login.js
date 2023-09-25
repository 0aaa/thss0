import React from 'react'
import { connect } from 'react-redux'
import { updateAuth } from '../../actionCreator/actionCreator'
import { getTokenAsync } from '../../services/auth'

const Login = props =>
    <>
        <div id="400-error" className="alert alert-danger d-none"></div>
        <div className="modal fade" id="loginModal" tabIndex="-1" aria-labelledby="loginModalLabel" aria-hidden="true">
            <div className="modal-dialog modal-dialog-centered">
                <div className="modal-content rounded-0">
                    <div className="modal-header">
                        <h1 className="modal-title fs-5" id="loginModalLabel">Login</h1>
                        <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <form onSubmit={event => props.UpdateAuth(event)}>
                        <div className="modal-body">
                            <input id="login-name" className="form-control border-0 border-bottom rounded-0" placeholder="Name" />
                            <input type="password" id="login-password" className="form-control my-1 border-0 border-bottom rounded-0" placeholder="Password" />
                        </div>
                        <div className="modal-footer btn-group">
                            <button type="button" className="btn btn-outline-dark border-0 border-bottom rounded-0" data-bs-dismiss="modal">Close</button>
                            <input type="submit" data-bs-dismiss="modal" value="Submit" className="btn btn-outline-dark border-0 border-bottom rounded-0" />
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </>
const mapDispatchToProps = dispatch => {
    return {
        UpdateAuth: async event => {
            event.preventDefault()
            const data = await getTokenAsync(event.target)
            !data && window.location.reload()
            dispatch(updateAuth(data.username))
        }
    }
}
export default connect(null, mapDispatchToProps)(Login)