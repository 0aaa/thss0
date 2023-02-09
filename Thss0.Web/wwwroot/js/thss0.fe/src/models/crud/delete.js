import React from "react"
import deleteRecord from "../../services/entity-service"

export default class Delete extends React.Component {
    constructor(props) {
        super(props)
        this.url = decodeURIComponent(props.match.params.href)
        this.state = {
            content: []
        }
    }
    handleDelete(event) {
        event.preventDefault()
        deleteRecord(this.props.match.params.entityName, event)
    }
    async componentDidMount() {
        const response = await fetch(this.url)
        const content = await response.json()
        this.setState({ content })
    }
    render() {
        return (
            <>
                <h5>Delete {Object.values(this.state.content)[0]}</h5>
                <div id="delete-error" className="alert alert-danger d-none"></div>
                <form onSubmit={this.handleDelete} className="w-50">
                    <dl>
                        {Object.keys(this.state.content).map(cntnt =>
                            <>
                                <dt>{cntnt}</dt>
                                <dd>{this.state.content[cntnt]}</dd>
                            </>
                        )}
                    </dl>
                    <input type="submit" value="Delete" className="btn btn-outline-danger" />
                </form>
                <input onClick={this.props.history.goBack()} value="Back" className="btn btn-outline-primary" />
            </>
        )
    }
}