import React, { Children } from 'react'
import { connect } from 'react-redux'
import { useNavigate, useParams } from 'react-router-dom'
import { updateContent } from '../../actionCreator/actionCreator'
import { deleteRecord, getRecords } from '../../services/entities'

class Delete extends React.Component {
    constructor(props) {
        super(props)
        this.path = props.params.entityName + '/' + props.params.id
        this.state = {
            content: []
        }
    }
    async componentDidMount() {
        const content = await getRecords(this.path)
        this.setState({ content })
    }
    render() {
        if (!this.state.content) {
            return
        }
        return (
            <form onSubmit={(event) => this.props.handleDelete(event, this.path, this.props.params.entityName, this.props.navigate)}>
                <div id="delete-error" className="alert alert-danger d-none"></div>
                <legend className="d-flex">Delete {this.state.content['name'] ?? this.state.content['userName']}
                    <div className="btn-group w-25 ms-auto me-2">
                        <input type="submit" value="Delete" className="btn btn-outline-danger border-0 border-bottom rounded-0 col-6" />
                        <button type="button" onClick={() => this.props.navigate(-1)} className="btn btn-outline-dark border-0 border-bottom rounded-0 col-6">Back</button>
                    </div>
                </legend>
                {Children.toArray(Object.keys(this.state.content).map(key =>
                    this.state.content[key] !== '' && !key.includes('Names')
                        && <dl>
                            <dt>{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</dt>
                            <dd>
                                {this.state.content[key].length > 0
                                    ? ['department', 'user', 'procedure', 'result'].includes(key)
                                        ? Children.toArray(this.state.content[key].split('\n').filter(e => e !== '').map((_, i) =>
                                        <>
                                                {this.state.content[key + 'Names'].split('\n')[i]}
                                                <br/>
                                            </>))
                                        : this.state.content[key]
                                        : 'Empty'
                                    }
                            </dd>
                        </dl>
                ))}
            </form>
        )
    }
}
const DeleteRouter = (props) => <Delete {...props} params={useParams()} navigate={useNavigate()} />

const mapStateToProps = (state) => { return state }

const mapDispatchToProps = (dispatch) => {
    return {
        handleDelete: async (event, path, entityName, navigate) => {
            event.preventDefault()
            await deleteRecord(path)
            const data = await getRecords(entityName)
            if (data) {
                dispatch(updateContent(data.content))
            }
            navigate(-1)
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(DeleteRouter)