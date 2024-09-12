import { connect } from 'react-redux'
import Details from '../views/crud/details'
import AddRouter from '../views/crud/add'
import EditRouter from '../views/crud/edit'
import Delete from '../views/crud/delete'

const OffcanvasGen = props => {
    return <div id="crud" tabIndex="-1" className="offcanvas offcanvas-end w-50 text-dark fs-5" style={{ backgroundImage: `url(../../img/${props.entityName}${(Math.floor(Math.random() * 4))}.jpg)`, backgroundPosition: 'center', backgroundSize: 'cover' }}>
        <div className="offcanvas-header bg-white bg-opacity-50 mx-3">
            <h5 className="offcanvas-title">{props.offcanvasName} {props.entityName}</h5>
            <button className="btn-close" data-bs-dismiss="offcanvas"></button>
            <div id="detailsErr" className="alert alert-danger d-none"></div>
        </div>
        {{
            'Add': <AddRouter entityName={props.entityName} />
            , 'Details': <Details />
            , 'Edit': <EditRouter />
            , 'Delete': <Delete />
        }[props.offcanvasName]}
    </div>
}

const mapStateToProps = state => ({ offcanvasName: state.offcanvasName })

export default connect(mapStateToProps)(OffcanvasGen)