import { connect } from 'react-redux'
import Details from '../views/crud/details'
import AddRouter from '../views/crud/add'
import EditRouter from '../views/crud/edit'
import Delete from '../views/crud/delete'

const OffcanvasGen = props =>
    <div id="offcanvasCrud" tabIndex="-1" className="offcanvas offcanvas-end w-50 text-dark" data-bs-scroll="true" aria-labelledby="offcanvasCrudLabel" style={{ background: 'url(../../img/pob.jpg)' }}>
        <div className="offcanvas-header">
            <h5 id="offcanvasCrudLabel" className="offcanvas-title ms-2">{props.offcanvasName} {props.entityName}</h5>
            <button type="button" className="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
            <div id="detailsError" className="alert alert-danger d-none"></div>
        </div>
        {{
            'Add': <AddRouter entityName={props.entityName} />
            , 'Details': <Details />
            , 'Edit': <EditRouter />
            , 'Delete': <Delete />
        }[props.offcanvasName]}
    </div>

const mapStateToProps = state => ({ offcanvasName: state.offcanvasName })

export default connect(mapStateToProps)(OffcanvasGen)