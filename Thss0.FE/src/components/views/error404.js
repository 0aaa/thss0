import { connect } from 'react-redux'
import { useNavigate } from 'react-router-dom'

const Error404 = props => {
    const navigate = useNavigate()
    return <div className="d-flex flex-column" style={{ height: '50vh' }}>
            <h5 className="m-auto mb-2">Error 404</h5>
            <button onClick={() => navigate(-1)} className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0 mx-auto`}>Return</button>
        </div>    
}

const mapStateToProps = state => ({ btnColor: state.btnColor })

export default connect(mapStateToProps)(Error404)