import { getTokenAsync, makeRegister } from '../../services/auth'
import React, { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { updateAuth } from '../../actionCreator/actionCreator'
import { connect } from 'react-redux'

class Register extends React.Component {
    render() {
        return (
            <>
                <div id="register-error" className="alert alert-danger d-none"></div>
                <div className="modal fade" id="registerModal" tabIndex="-1" aria-labelledby="registerModalLabel" aria-hidden="true">
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content rounded-0">
                            <div className="modal-header">
                                <h1 className="modal-title fs-5" id="registerModalLabel">Register</h1>
                                <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <form onSubmit={event => this.props.HandleRegister(event, this.props.effect)}>
                                <div className="modal-body">
                                    <input id="name" className="form-control border-0 border-bottom rounded-0" placeholder="Name" />
                                    <input type="email" id="email" className="form-control border-0 border-bottom rounded-0 my-1" placeholder="Email" />
                                    <input id="phone-number" className="form-control border-0 border-bottom rounded-0" placeholder="Phone number" />
                                    <input type="password" id="password" className="form-control border-0 border-bottom rounded-0 my-1" placeholder="Password" />
                                    <input type="password" id="password-repeat" className="form-control border-0 border-bottom rounded-0" placeholder="Password repeat" />
                                </div>
                                <div className="modal-footer btn-group">
                                    <button type="button" className="btn btn-outline-dark border-0 border-bottom rounded-0" data-bs-dismiss="modal">Close</button>
                                    <input type="submit" data-bs-dismiss="modal" value="Submit" className="btn btn-outline-dark border-0 border-bottom rounded-0 mt-1" />
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </>
        )
    }
}
const RegisterRouter = props => <Register {...props} params={useParams()} />

const mapDispatchToProps = dispatch => {
    return {
        HandleRegister: async (event, effect) => {
            event.preventDefault()
            !await makeRegister(event.target) && window.location.reload()
            !await getTokenAsync(event) && window.location.reload()//
            const navigate = useNavigate()
            useEffect(() => navigate('/'))
            effect()
            dispatch(updateAuth())
        }
    }
}
export default connect(mapDispatchToProps)(RegisterRouter)