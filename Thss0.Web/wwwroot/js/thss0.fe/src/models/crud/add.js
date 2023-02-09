import React from "react"
import addRecord from "../../services/entity-service"

export default class Add extends React.Component {
    handleAdd(event) {
        event.preventDefault()
        addRecord(this.props.match.params.entityName, event)
    }
    render() {
        return (
            <>
                <h5>Add new</h5>
                <form onSubmit={this.handleAdd} className="w-50">
                    <table className="table">
                        <tbody>
                            {Object.keys(this.state.content).map(cntnt =>
                                <tr className="form-group">
                                    <span id={cntnt + '-error'} className="alert alert-danger d-none" />
                                    <th>
                                        <label for={cntnt} className="col-form-label">{cntnt}</label>
                                    </th>
                                    <td>
                                        <input id={cntnt} className="form-control" />
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                    <input type="submit" value="Submit" className="btn btn-outline-primary" />
                </form>
                <input onClick={this.props.history.goBack()} value="Back" className="btn btn-outline-primary" />
            </>
        )
    }
}