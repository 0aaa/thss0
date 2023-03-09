import React from 'react'
import { connect } from 'react-redux'
import { NavLink, useParams } from 'react-router-dom'
import { updateState } from "../../actions/actions"
import { getRecords } from '../../services/entity-service'

class List extends React.Component {
    constructor(props) {
        super(props)
        this.path = props.params.entityName + '/' + (props.params.roleIndex ?? '')
        this.state = {
            content: []
        }
    }
    async componentDidMount() {
        const content = await getRecords(this.path)
        this.setState({ content })
    }
    render() {
        return (
            <>
                <NavLink to={`/add/${this.props.params.entityName}`} className="btn btn-outline-primary">Add new</NavLink>
                <table className="table">
                    <thead>
                    </thead>
                    <tbody>
                        {this.state.content.map(cntnt =>
                            <tr id={cntnt.id} key={cntnt.id}>
                                <td>
                                    <NavLink to={`/details/${this.props.params.entityName}/${cntnt.id}`}>
                                        {cntnt.name ?? cntnt.title}
                                    </NavLink>
                                </td>
                                <td>
                                    <NavLink to={`/edit/${this.props.params.entityName}/${cntnt.id}`}>
                                        Edit
                                    </NavLink>
                                </td>
                                <td>
                                    <NavLink to={`/delete/${this.props.params.entityName}/${cntnt.id}`}>
                                        Delete
                                    </NavLink>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </>
        )
    }
}
const ListRouter = (props) => <List {...props} params={useParams()} />

function mapStateToProps(state) {
    return { content: state.content }
}
function mapDispatchToProps(dispatch) {
    return {
        updateState: async (path) => {
            const data = await getRecords(path)
            dispatch(updateState(data))
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(ListRouter)