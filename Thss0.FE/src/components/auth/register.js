import { getTokenAsync, makeRegister } from '../../services/auth'
import React, { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { updateAuth } from '../../actionCreator/actionCreator'
import { connect } from 'react-redux'

const Register = props =>
    <form onSubmit={e => props.HandleRegister(e)}>
        <div id="registerError" className="alert alert-danger d-none"></div>
        <div className="modal-body">
            <input id="name" className="form-control border-0 border-bottom rounded-0" placeholder="Name" />
            <input type="email" id="email" className="form-control border-0 border-bottom rounded-0 my-1" placeholder="Email" />
            <input id="phone-number" className="form-control border-0 border-bottom rounded-0" placeholder="Phone number" />
            <input type="password" id="password" className="form-control border-0 border-bottom rounded-0 my-1" placeholder="Password" />
            <input type="password" id="password-repeat" className="form-control border-0 border-bottom rounded-0" placeholder="Password repeat" />
        </div>
        <div className="modal-footer btn-group">
            <button type="button" className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0`} data-bs-dismiss="modal">Close</button>
            <input type="submit" value="Submit" className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0 mt-1`} data-bs-dismiss="modal" />
        </div>
    </form>

const mapStateToProps = state => ({ btnColor: state.btnColor })

const mapDispatchToProps = dispatch => ({
    HandleRegister: async e => {
        e.preventDefault()
        !await makeRegister(e.target) && window.location.reload()
        !await getTokenAsync(e) && window.location.reload()//
        const navigate = useNavigate()
        useEffect(() => navigate('/'))
        dispatch(updateAuth())
    }
})

export default connect(mapStateToProps, mapDispatchToProps)(Register)