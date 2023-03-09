import React from "react"
import { useNavigate, useParams } from "react-router-dom"
import { getRecords } from "../../services/entity-service"

class Details extends React.Component {
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
        return (
            <>
                <h5>{this.state.content['name']}</h5>
                {Object.keys(this.state.content).map(cntnt =>
                    <dl key={cntnt}>
                        <dt>{cntnt}</dt>
                        <dd>{this.state.content[cntnt].length > 0 ? this.state.content[cntnt] : 'Empty'}</dd>
                    </dl>
                )}
                <button onClick={() => this.props.navigate(-1)} className="btn btn-outline-primary">Back</button>
            </>
        )
    }
}
function DetailsRouter(props) {
    return <Details {...props} params={useParams()} navigate={useNavigate()} />
}
export default DetailsRouter