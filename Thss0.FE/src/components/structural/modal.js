import { connect } from 'react-redux'
import Login from '../auth/login'
import Register from '../auth/register'
import Devices from '../views/devices'

const ModalGen = props =>
    <div id="modalGen" tabIndex="-1" className="modal fade">
        <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content rounded-0">
                <div className="modal-header">
                    <h1 className="modal-title fs-5">{props.modalName}</h1>
                    <button className="btn-close" data-bs-dismiss="modal"></button>
                </div>
                {{
                    'Login': <Login />
                    , 'Register': <Register />
                    , 'Devices': <Devices />
                    , 'AI Analyse': <p className="p-2">{props.payload}</p>
                    , 'Device data': <p className="p-2">{props.payload}</p>
                }[props.modalName]}
            </div>
        </div>
    </div>

const mapStateToProps = state => ({ modalName: state.modalName, payload: state.payload })

export default connect(mapStateToProps)(ModalGen)