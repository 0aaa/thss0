import React from 'react'
import { connect } from 'react-redux'
import { updateAuth } from '../../actionCreator/actionCreator'
import { getTokenAsync } from '../../services/auth'
import { Modal } from 'bootstrap'

const Login = props =>
    <form onSubmit={e => props.UpdateAuth(e)}>
        <div id="400error" className="alert alert-danger d-none"></div>
        <div className="modal-body">
            <input id="login-name" className="form-control border-0 border-bottom rounded-0" placeholder="Name" defaultValue="Linda_Wilson" />
            <input type="password" id="login-password" className="form-control my-1 border-0 border-bottom rounded-0" placeholder="Password" defaultValue="*1Admin" />
        </div>
        <div className="modal-footer btn-group">
            <button className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0`} data-bs-dismiss="modal">Close</button>
            <input type="submit" value="Submit" className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0`} />
        </div>
    </form>

const mapStateToProps = state => ({ btnColor: state.btnColor })

const mapDispatchToProps = dispatch => ({
    UpdateAuth: async e => {
        e.preventDefault()
        const data = await getTokenAsync(e.target)
        if (!data) {
            return
        }
        Modal.getInstance('#modalGen').hide()
        dispatch(updateAuth(data.username))
    }
})

export default connect(mapStateToProps, mapDispatchToProps)(Login)