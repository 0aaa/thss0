import React from "react"
import { useNavigate, useParams } from "react-router-dom"
import API_URL from "../../config/consts"
import { addRecord } from "../../services/entity-service"

class Add extends React.Component {
    constructor(props) {
        super(props)
        this.url = API_URL + props.params.entityName
        this.state = {
            keys: []
        }
        this.handleAdd = this.handleAdd.bind(this)
    }
    handleAdd(event) {
        event.preventDefault()
        let formCollection = {};
        [...event.target.elements].forEach(cntrl => formCollection[cntrl.id] = cntrl.value)
        delete formCollection['']
        addRecord(this.url, formCollection)
    }
    async componentDidMount() {
        const response = await fetch(this.url)
        const data = (await response.json())[0]
        delete data['id']
        // delete data['creationTime']                     // For the Procedure type only.
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
export default AddRouter