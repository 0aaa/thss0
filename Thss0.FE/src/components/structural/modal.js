import { connect } from 'react-redux'
import Login from '../auth/login'
import Register from '../auth/register'
import Devices from '../views/devices'

const ModalGen = props =>
    <div id="modalGen" tabIndex="-1" className="modal fade" aria-labelledby="modalLabel" aria-hidden="true">
        <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content rounded-0">
                <div className="modal-header">
                    <h1 id="modalLabel" className="modal-title fs-5">{props.modalName}</h1>
                    <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                {{
                    'Login': <Login />
                    , 'Register': <Register />
                    , 'Devices': <Devices />
                }[props.modalName]}
            </div>
        </div>
    </div>

const mapStateToProps = state => ({modalName: state.modalName})

export default connect(mapStateToProps)(ModalGen)