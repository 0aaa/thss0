import React from "react"
import { connect } from "react-redux"
import { useNavigate, useParams } from "react-router-dom"
import { updateState } from "../../actions/actions"
import { addRecord } from "../../services/entity-service"

class Add extends React.Component {
    constructor(props) {
        super(props)
        this.path = props.params.entityName
        this.state = {
            keys: []
        }
    }
    handleAdd(event) {
        event.preventDefault()
        let formCollection = {};
        [...event.target.elements].forEach(cntrl => formCollection[cntrl.id] = cntrl.value)
        delete formCollection['']
        addRecord(this.path, formCollection)
    }
    async componentDidMount() {
        const data = (await getRecords(this.path))[0]
        delete data['id']
        delete data['creationTime']                     // For the Procedure type only.
        this.setState({ keys: Object.keys(data) })
    }
    render() {
        return (
            <>
                <h5>Add new {this.props.params.entityName.replace(/.$/, '')}</h5>
                <form onSubmit={this.handleAdd} className="w-50">
                    <table className="table">
                        <tbody>
                            {this.state.keys.map(k =>
                                <tr key={k} className="form-group">
                                    <th>
                                        <label htmlFor={k} className="col-form-label">{k}</label>
                                    </th>
                                    <td>
                                        <span id={k + '-error'} className="alert alert-danger d-none"></span>
                                        <input type={k.endsWith('Time') ? 'datetime-local' : 'text'} id={k} placeholder={k} className="form-control" />
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                    <input type="submit" className="btn btn-outline-primary" />
                </form>
                <button onClick={() => this.props.navigate(-1)} className="btn btn-outline-primary">Back</button>
            </>
        )
    }
}
function AddRouter(props) {
    return <Add {...props} params={useParams()} navigate={useNavigate()} />
}

function mapStateToProps(state) {
    return state
}
function mapDispatchToProps(dispatch) {
    return {
        addRecord: (url, credentials) => dispatch(updateState(url, credentials))// Must be fixed.
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(AddRouter)